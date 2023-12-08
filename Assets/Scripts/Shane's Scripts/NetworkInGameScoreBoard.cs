using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkInGameScoreBoard : NetworkBehaviour
{
    [SerializeField]
    Transform playerScoreHolder;
    [SerializeField]
    GameObject playerScoreTemplate;

    private List<ulong> clients;

    private void OnEnable()
    {
        NetworkPlayer.OnPlayerSpawn += OnPlayerSpawned;
    }
    private void OnDisable()
    {
        NetworkPlayer.OnPlayerSpawn -= OnPlayerSpawned;
    }
    private void OnPlayerSpawned(GameObject player)
    {
        GameObject playerUI = Instantiate(playerScoreTemplate, playerScoreHolder);
        playerUI.GetComponent<NetworkPlayerScore>().TrackPlayer(player);
    }

   

}
