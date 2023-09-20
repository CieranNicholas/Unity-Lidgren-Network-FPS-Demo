using Lidgren.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using UnityEngine;

public class PredictionServer : Prediction
{
    public static PredictionServer Instance;

    public StatePayload[] stateBuffer;
    public Queue<InputPayload> inputQueue;

    private ServerBehaviour serverBehaviour;
    private void Awake()
    {
        if(Instance != null)
        {
            Debug.Log("PredictionServer: instance already exists, destroying game object");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            serverBehaviour = GetComponent<ServerBehaviour>();
        }
    }

    private void Start()
    {
        minTimeBetweenTicks = 1f / SERVER_TICK_RATE;

        stateBuffer = new StatePayload[BUFFER_SIZE];
        inputQueue = new Queue<InputPayload>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        while (timer >= minTimeBetweenTicks)
        {
            timer -= minTimeBetweenTicks;
            HandleTick();
            currentTick++;
        }
    }

    IEnumerator SendToClient(StatePayload statePayload)
    {
        yield return new WaitForSeconds(0.02f);
        //Client.Instance.OnServerMovementState(statePayload);
        NetOutgoingMessage message = serverBehaviour.Server.CreateMessage();
        new StatePayloadPacket()
        {
            Player = statePayload.player,
            Tick = statePayload.tick,
            X = statePayload.position.x,
            Y = statePayload.position.y,
            Z = statePayload.position.z,
        }.PacketToNetOutGoingMessage(message);
        serverBehaviour.Server.SendMessage(
            message, 
            serverBehaviour.Server.PlayerConnections[statePayload.player], 
            NetDeliveryMethod.ReliableOrdered
        );
    }

    private void HandleTick()
    {
        // Process the input queue
        int bufferIndex = -1;
        while (inputQueue.Count > 0)
        {
            InputPayload inputPayload = inputQueue.Dequeue();

            bufferIndex = inputPayload.tick % BUFFER_SIZE;

            StatePayload statePayload = ProcessMovement(inputPayload);
            stateBuffer[bufferIndex] = statePayload;
        }

        if (bufferIndex != -1)
        {
            StartCoroutine(SendToClient(stateBuffer[bufferIndex]));
        }
    }

    StatePayload ProcessMovement(InputPayload input)
    {
        // Should always be in sync with same function on Client
        // I guess this needs to change?

        //transform.position += input.inputVector * 5f * minTimeBetweenTicks;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        Transform playerTransform;

        foreach (GameObject player in players)
        {
            if (player.transform.name == input.player)
            {
                playerTransform = player.transform;
                playerTransform.position += input.inputVector * 5f * minTimeBetweenTicks;
                return new StatePayload()
                {
                    tick = input.tick,
                    position = playerTransform.position,
                    player = input.player,
                };
            }
        }

        return new StatePayload()
        {
            tick = input.tick,
            position = new Vector3(),
            player = input.player,
        };
    }
}
