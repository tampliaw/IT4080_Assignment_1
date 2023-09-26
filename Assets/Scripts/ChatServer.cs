using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ChatServer : NetworkBehaviour
{
    public ChatUi chatUi;
    const ulong SYSTEM_ID = ulong.MaxValue;
    private ulong[] dmClientIds = new ulong[2];

    void Start()
    {
        chatUi.printEnteredText = false;
        chatUi.MessageEntered += OnChatUiMessageEntered;

        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback += ServerOnClientConnected;
            NetworkManager.OnClientDisconnectCallback += ServerOnClientDisconnected;
            if (IsHost)
            {
                //Changed greeting message for host
                DisplayMessageLocally(NetworkManager.LocalClientId, "Welcome, you are the host");
            }
            else
            {
                DisplayMessageLocally(SYSTEM_ID, "Welcome, you are the server");
            }
        }
        else
        {
            DisplayMessageLocally(SYSTEM_ID, $"You are a client {NetworkManager.LocalClientId}");
        }
    }

    private void ServerOnClientConnected(ulong clientId)
    {
        //Notify all clients on connection
        ServerSendDirectMessage($"Player {clientId} has connected.", SYSTEM_ID, clientId);
    }

    private void ServerOnClientDisconnected(ulong clientId) 
    {
        //Notify all clients on disconnection
        ServerSendDirectMessage($"Player {clientId} has disconnected.", SYSTEM_ID, clientId);
    }

    private void DisplayMessageLocally(ulong from, string message)
    {
        string fromStr = $"Player {from}";
        Color textColor = chatUi.defaultTextColor;

        if(from == NetworkManager.LocalClientId)
        {
            fromStr = "You";
            textColor= Color.magenta;
        }
        else if(from == SYSTEM_ID)
        {
            fromStr = "SYS";
            textColor = Color.blue;
        }
        chatUi.addEntry(fromStr, message, textColor);
    }

    private void OnChatUiMessageEntered(string message)
    {
        SendChatMessageServerRpc(message);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendChatMessageServerRpc(string message, ServerRpcParams serverRpcParams = default)
    {
        if (message.StartsWith("@"))
        {
            string[] parts = message.Split(" ");
            string clientIdStr = parts[0].Replace("@", "");
            if (ulong.TryParse(clientIdStr, out ulong toClientId))
            {
                if (NetworkManager.Singleton.ConnectedClients.TryGetValue(toClientId, out NetworkClient client))
                {
                    // Send message to target client
                    ReceiveChatMessageClientRpc(message, toClientId);
                }
                else
                {
                    // Notify sender message could not be sent
                    ReceiveChatMessageClientRpc($"User with ID {toClientId} not found.", serverRpcParams.Receive.SenderClientId);
                }
            }
            else
            {
                // Notify sender message could not be sent
                ReceiveChatMessageClientRpc($"Invalid user ID: {clientIdStr}", serverRpcParams.Receive.SenderClientId);
            }
        }
        else
        {
            // Send message to all clients
            ReceiveChatMessageClientRpc(message, serverRpcParams.Receive.SenderClientId);
        }
    }

    [ClientRpc]
    public void ReceiveChatMessageClientRpc(string message, ulong from, ClientRpcParams clientRpcParams = default)
    {
        DisplayMessageLocally(from, message);
    }


    private void ServerSendDirectMessage(string message, ulong from, ulong to)
    {
        dmClientIds[0] = from;
        dmClientIds[1] = to;
        ClientRpcParams rpcParams = default;
        rpcParams.Send.TargetClientIds= dmClientIds;

        ReceiveChatMessageClientRpc($"<whisper> {message}", from, rpcParams);

    }
}