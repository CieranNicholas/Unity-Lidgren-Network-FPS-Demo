using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictionClient : Prediction
{
    public static PredictionClient Instance;

    private StatePayload[] stateBuffer;
    private InputPayload[] inputBuffer;
    private StatePayload latestServerState;
    private StatePayload lastProcessedState;
    private float horizontalInput;
    private float verticalInput;

    private ClientBehaviour clientBehaviour;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("PredictionClient: instance already exists, destroying game object");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            clientBehaviour = GetComponent<ClientBehaviour>();
            clientBehaviour.Client.StatePayload += OnServerMovementState;
        }
    }

    private void Start()
    {
        minTimeBetweenTicks = 1f / SERVER_TICK_RATE;

        stateBuffer = new StatePayload[BUFFER_SIZE];
        inputBuffer = new InputPayload[BUFFER_SIZE];
    }

    private void Update()
    {
        horizontalInput = InputManager.Instance.MovementInput.x;
        verticalInput = InputManager.Instance.MovementInput.y;

        timer += Time.deltaTime;

        while (timer >= minTimeBetweenTicks)
        {
            timer -= minTimeBetweenTicks;
            HandleTick();
            currentTick++;
        }
    }

    public void OnServerMovementState(StatePayloadPacket packet)
    {
        StatePayload serverState = new()
        {
            player = packet.Player,
            position = new Vector3(packet.X, packet.Y, packet.Z),
            tick = packet.Tick
        };
        latestServerState = serverState;
    }

    private void HandleTick()
    {
        if (!latestServerState.Equals(default(StatePayload)) &&
            (lastProcessedState.Equals(default(StatePayload)) ||
            !latestServerState.Equals(lastProcessedState)))
        {
            HandleServerReconciliation();
        }

        int bufferIndex = currentTick % BUFFER_SIZE;

        // Add payload to inputBuffer
        //InputPayload inputPayload = new InputPayload();
        //inputPayload.tick = currentTick;
        //inputPayload.inputVector = new Vector3(horizontalInput, 0, verticalInput);
        InputPayload inputPayload = new()
        {
            inputVector = new Vector3(horizontalInput, 0, verticalInput),
            tick = bufferIndex,
            player = NetworkClientInfo.Instance.LocalPlayerID
        };
        inputBuffer[bufferIndex] = inputPayload;

        // Add payload to stateBuffer
        stateBuffer[bufferIndex] = ProcessMovement(inputPayload);

        // Send input to server
        StartCoroutine(SendToServer(inputPayload));
    }

    IEnumerator SendToServer(InputPayload inputPayload)
    {
        yield return new WaitForSeconds(0.02f);

        NetOutgoingMessage message = clientBehaviour.Client.CreateMessage();
        new InputPayloadPacket()
        {
            Player = NetworkClientInfo.Instance.LocalPlayerID,
            Tick = inputPayload.tick,
            X = inputPayload.inputVector.x,
            Y = inputPayload.inputVector.y,
            Z = inputPayload.inputVector.z,
        }.PacketToNetOutGoingMessage(message);

        clientBehaviour.Client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
    }

    StatePayload ProcessMovement(InputPayload input)
    {
        // Should always be in sync with same function on Server
        //transform.position += input.inputVector * 5f * minTimeBetweenTicks;

        clientBehaviour.localPlayerObject.transform.position += input.inputVector * 5f * minTimeBetweenTicks;

        return new StatePayload()
        {
            tick = input.tick,
            position = transform.position,
            player = NetworkClientInfo.Instance.LocalPlayerID
        };
    }

    private void HandleServerReconciliation()
    {
        lastProcessedState = latestServerState;

        int serverStateBufferIndex = latestServerState.tick % BUFFER_SIZE;
        float positionError = Vector3.Distance(latestServerState.position, stateBuffer[serverStateBufferIndex].position);
        Debug.Log(stateBuffer[serverStateBufferIndex].position);
        Debug.Log(latestServerState.position);
        if (positionError > 0.001f)
        {
            Debug.Log("We have to reconcile bro");
            // Rewind & Replay
            //transform.position = latestServerState.position;
            clientBehaviour.localPlayerObject.transform.position = latestServerState.position;


            // Update buffer at index of latest server state
            stateBuffer[serverStateBufferIndex] = latestServerState;

            // Now re-simulate the rest of the ticks up to the current tick on the client
            int tickToProcess = latestServerState.tick + 1;

            while (tickToProcess < currentTick)
            {
                int bufferIndex = tickToProcess % BUFFER_SIZE;

                // Process new movement with reconciled state
                StatePayload statePayload = ProcessMovement(inputBuffer[bufferIndex]);

                // Update buffer with recalculated state
                stateBuffer[bufferIndex] = statePayload;

                tickToProcess++;
            }
        }
    }
}
