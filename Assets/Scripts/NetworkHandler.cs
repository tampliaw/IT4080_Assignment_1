using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Netcode;


public class NetworkHandler : NetworkBehaviour
{
    void Start()
    {
        NetworkManager.OnClientStarted += OnClientStarted;
        NetworkManager.OnServerStarted += OnServerStarted;
    }


    private void PrintMe()
    {
        if (IsServer)
        {
            NetworkHelper.Log($"I am a Server! {NetworkManager.ServerClientId}");
        }
        if (IsHost)
        {
            NetworkHelper.Log($"I am a Host! {NetworkManager.ServerClientId}/{NetworkManager.LocalClientId}");
        }
        if (IsClient)
        {
            NetworkHelper.Log($"I am a Client! {NetworkManager.LocalClientId}");
        }
        if (!IsServer && !IsClient)
        {
            NetworkHelper.Log("I am Nothing yet");
        }

    }


    //Client Actions
     
    private void OnClientStarted()
    {
        NetworkHelper.Log("!! Client Started !!");
        NetworkManager.OnClientConnectedCallback += ClientOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ClientOnClientDisconnected;
        NetworkManager.OnClientStopped += ClientOnClientStopped;
        PrintMe();
    }

    private void ClientOnClientStopped(bool indicator)
    {
        NetworkHelper.Log("!! Client Stopped !!");
        NetworkManager.OnClientConnectedCallback += ClientOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ClientOnClientDisconnected;
        NetworkManager.OnServerStopped += ClientOnClientStopped;
        PrintMe();
    }

    private void ClientOnClientConnected(ulong clientId) {
        NetworkHelper.Log($"I have connected {clientId}");
    }

    private void ClientOnClientDisconnected(ulong clientId) {
        NetworkHelper.Log($"I have disconnected {clientId}");
    }


    //Server Actions

    private void OnServerStarted()
    {
        NetworkHelper.Log("!! Server Started !!");
        NetworkManager.OnClientConnectedCallback += ServerOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ServerOnClientDisconnected;
        NetworkManager.OnServerStopped += ServerOnServerStopped;
        PrintMe();
    }
    private void ServerOnServerStopped(bool indicator)
    {
        NetworkHelper.Log("!! Server Stopped !!");
        NetworkManager.OnClientConnectedCallback -= ServerOnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= ServerOnClientDisconnected;
        NetworkManager.OnServerStopped -= ServerOnServerStopped;
        PrintMe();
    }

    private void ServerOnClientConnected(ulong clientId)
    {
        NetworkHelper.Log($"Client {clientId} connected to server");
    }

    private void ServerOnClientDisconnected(ulong clientId)
    {
        NetworkHelper.Log($"Client {clientId} disconnected from the server");
    }

}


