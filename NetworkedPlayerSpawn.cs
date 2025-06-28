using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkedPlayerSpawn : NetworkBehaviour
{
    public ClientConnectionHandler clientConnectionHandler;

    int connectedPlayers = 0;

    public GameObject[] playerCarts;
    public Transform[] startPoints;
    public GameObject[] tracks;

    public NetworkVariable<int> track = new NetworkVariable<int>();

    public NetworkPrefabsList playerList;
    MenuToGame mTG;

    public int playerID;

    public PlayerManager playerManager;

    public NetworkVariable<ulong> whatPlayersSpawned = new NetworkVariable<ulong>();

    public void Spawn(ulong id, FixedString128Bytes pID, NetworkManager nM)
    {
        mTG = GameObject.Find("MTG").GetComponent<MenuToGame>();
        playerID = mTG.cart;
        if (track.Value == 0)
        {
            track.Value = mTG.track + 1;
        }
        tracks[track.Value - 1].SetActive(true);
        startPoints[0] = tracks[track.Value - 1].transform.Find("P1");
        startPoints[1] = tracks[track.Value - 1].transform.Find("P2");
        startPoints[2] = tracks[track.Value - 1].transform.Find("P3");
        startPoints[3] = tracks[track.Value - 1].transform.Find("P4");
        if (whatPlayersSpawned.Value == 0) { whatPlayersSpawned.Value = 1; }
        connectedPlayers = playerManager.WhatPlayer(pID, id);
        if (connectedPlayers >= 0 && connectedPlayers <= 3)
        {
            SpawnPlayer(id, connectedPlayers);
        }
    }

    void SpawnPlayer(ulong id, int loc)
    {
        GameObject player = Instantiate(playerCarts[playerID], startPoints[loc].transform);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);
        print(id + "    " + whatPlayersSpawned.Value);
        if (id == whatPlayersSpawned.Value)
        {
            player.GetComponent<OnTrack>().isPlayer = true;
            PlayerSpawnedRpc();
        }
    }

    [Rpc(SendTo.Authority)]
    void PlayerSpawnedRpc()
    {
        whatPlayersSpawned.Value++;
        print("Spawned");
    }
}
