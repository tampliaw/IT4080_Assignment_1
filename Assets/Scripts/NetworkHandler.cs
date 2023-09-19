using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Runtime.CompilerServices;
//using UnityEditor.PackageManager;

public class NetworkHandler : NetworkBehaviour
{
    void Start()
    {
        NetworkManager.OnClientStarted += OnClientStarted;
        NetworkManager.OnServerStarted += OnServerStarted;
    }

    private bool hasPrinted = false;
    private void PrintMe()
    {
        if (hasPrinted)
        {
            return;
        }
        Debug.Log("I am");
        hasPrinted = true;
        if (IsServer)
        {
            Debug.Log($"  The Server! {NetworkManager.ServerClientId}");
        }
        if (IsHost)
        {
            Debug.Log($"  The Host! {NetworkManager.ServerClientId}/{NetworkManager.LocalClientId}");
        }
        if (IsClient)
        {
            Debug.Log($"  The Client! {NetworkManager.LocalClientId}");
        }
        if (!IsServer && !IsClient)
        {
            Debug.Log("  Nothing yet");
            hasPrinted = false;
        }

    }

    private void OnClientStarted()
    {
        Debug.Log("!! Client Started !!");
        NetworkManager.OnClientConnectedCallback += ClientOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ClientOnClientDisconnected;
        NetworkManager.OnClientStopped += ClientOnClientStopped;
        PrintMe();
    }

    //Client Actions

    private void ClientOnClientConnected(ulong clientID) {
        PrintMe();
        if (IsHost && IsClient)
        {
            Debug.Log($"I {clientID} have connected to server on the host");
        }
        else if (IsClient)
        {
            Debug.Log($"I {clientID} have connected to server");
        }

    }

    private void ClientOnClientDisconnected(ulong clientID) {
        PrintMe();
        if (IsHost && IsClient)
        { 
            Debug.Log($"I {clientID} have disconnected from the server on the host");
        }
        else if (IsClient)
        {
            Debug.Log($"I {clientID} have diconnected from the server");
        }
        
    }

        private void ClientOnClientStopped(bool indicator) 
    {
        Debug.Log("!! Client Stopped !!");
        hasPrinted = false;
        NetworkManager.OnClientConnectedCallback += ClientOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ClientOnClientDisconnected;
        NetworkManager.OnServerStopped += ClientOnClientStopped;
    }
    




    //Server Actions

    private void OnServerStarted()
    {
        Debug.Log("!! Server Started !!");
        NetworkManager.OnClientConnectedCallback += ServerOnClientConnected;
        NetworkManager.OnClientDisconnectCallback += ServerOnClientDisconnected;
        NetworkManager.OnServerStopped += ServerOnServerStopped;
        PrintMe();
    }


    private void ServerOnClientConnected(ulong clientID)
    {
        Debug.Log($"Client {clientID} connected to server");
    }

    private void ServerOnClientDisconnected(ulong clientID)
    {
        Debug.Log($"Client {clientID} disconnected from the server");
    }

    private void ServerOnServerStopped(bool indicator)
    {
        Debug.Log("!! Server Stopped !!");
        hasPrinted = false;
        NetworkManager.OnClientConnectedCallback -= ServerOnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= ServerOnClientDisconnected;
        NetworkManager.OnServerStopped -= ServerOnServerStopped;
    }
   

}


