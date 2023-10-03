using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkHelper : MonoBehaviour
{
    private static NetworkManager netMgr = NetworkManager.Singleton;
    private static void StartButtons()
    {
        if (GUILayout.Button("Host")) netMgr.StartHost();
        if (GUILayout.Button("Client")) netMgr.StartClient();
        if (GUILayout.Button("Server")) netMgr.StartServer();
    }

    private static void RunningControls()
    {
        string transportTypeName = netMgr.NetworkConfig.NetworkTransport.GetType().Name;
        UnityTransport transport = netMgr.GetComponent<UnityTransport>();
        string serverPort = "?";
        if (transport != null)
        {
            serverPort = $"{transport.ConnectionData.Address}:{transport.ConnectionData.Port}";
        }
        string mode = GetNetworkMode();
        if (GUILayout.Button($"Shutdown {mode}")) { netMgr.Shutdown(); }

        GUILayout.Label($"Transport: {transportTypeName} [{serverPort}]");
        GUILayout.Label("Mode: " + mode);
        if (netMgr.IsClient)
        {
            GUILayout.Label($"ClientId = {netMgr.LocalClientId}");
        }
        }
    
    public static void GUILayoutNetworkControls()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!netMgr.IsClient && !netMgr.IsServer)
        {
          StartButtons();
        } else
        {
            RunningControls();
        }
        GUILayout.EndArea();
    }

    public static string GetNetworkMode()
    {
        string type = "client";
        if (netMgr.IsServer)
        {
            if (netMgr.IsHost)
            {
                type = "Host";
            }
            else
            {
                type = "Server";
            }
        }
        return type;
    }

    public static void Log(string msg)
    {
        Debug.Log($"[{GetNetworkMode()} {netMgr.LocalClientId}]: {msg}");
    }

    public static void Log(NetworkBehaviour what, string msg)
    {
        ulong ownerId = what.GetComponent<NetworkObject>().OwnerClientId;
        Debug.Log($"[{GetNetworkMode()} {netMgr.LocalClientId}][{what.GetType()}][Owner: {ownerId}] {msg}");
        // The full line of code above was not visible in the lecture so I had to assume this is what we are supposed to do.
    }
}
