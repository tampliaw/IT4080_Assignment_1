using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Arena1Game : NetworkBehaviour
{
    public Player hostPrefab;
    public Player clientPrefab;
    public Camera arenaCamera;

    private NetworkedPlayers networkedPlayers;

    private int positionIndex = 0;
    private Vector3[] startPositions = new Vector3[]
    {
        new Vector3(2, 2, 0),
        new Vector3(-2, 2, 0),
        new Vector3(0, 2, 4),
        new Vector3(0, 2, -4)
    };

    // Start is called before the first frame update
    void Start()
    {
        arenaCamera.enabled = !IsClient;
        arenaCamera.GetComponent<AudioListener>().enabled = !IsClient;

        networkedPlayers = GameObject.Find("NetworkedPlayers").GetComponent<NetworkedPlayers>();

        if (IsServer)
        {
            SpawnPlayers();
        }
 
    }
        
    private Vector3 NextPosition()
    {
        Vector3 pos = startPositions[positionIndex];
        positionIndex += 1;
        if (positionIndex > startPositions.Length - 1)
        {
            positionIndex = 0;
        }
        return pos;
    }

    private void SpawnPlayers()
    {
        foreach (NetworkPlayerInfo info in networkedPlayers.allNetPlayers)
        {
            Player playerPrefabToSpawn = clientPrefab;
            //if (NetworkManager.LocalClientId == clientId)
            //{
            //    playerPrefabToSpawn = hostPrefab;
            //}

                Player playerSpawn = Instantiate(playerPrefabToSpawn, NextPosition(), Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(info.clientId);
           playerSpawn.playerColor.Value = info.color;
        }
    }
}

