using System;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;

public class PlayerManager : NetworkBehaviour
{
    public NetworkVariable<FixedString128Bytes> player1UniqueID = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString128Bytes> player2UniqueID = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString128Bytes> player3UniqueID = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString128Bytes> player4UniqueID = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<ulong> player1ID = new NetworkVariable<ulong>();
    public NetworkVariable<ulong> player2ID = new NetworkVariable<ulong>();
    public NetworkVariable<ulong> player3ID = new NetworkVariable<ulong>();
    public NetworkVariable<ulong> player4ID = new NetworkVariable<ulong>();

    public string[] players = new string[4];

    public override void OnNetworkSpawn()
    {
        player1UniqueID.OnValueChanged += OnP1StringChanged;
        // Log the current value of the text string when the client connected
        Debug.Log($"Client-{NetworkManager.LocalClientId}'s TextString = {player1UniqueID.Value}");
    }

    private void OnP1StringChanged(FixedString128Bytes previous, FixedString128Bytes current)
    {
        // Just log a notification when m_TextString changes
        Debug.Log($"Client-{NetworkManager.LocalClientId}'s TextString = {player1UniqueID.Value}");
    }


    public int WhatPlayer(FixedString128Bytes pID, ulong id)
    {
        GetComponent<NetworkObject>().ChangeOwnership(id);
        if (player1UniqueID.Value == "" || player1UniqueID.Value == pID)
        {
            player1UniqueID.Value = pID;
            player1ID.Value = id;
            return 0;
        }
        else if (player2UniqueID.Value == "" || player2UniqueID.Value == pID)
        {
            player2UniqueID.Value = pID;
            player2ID.Value = id;
            return 1;
        }
        else if (player3UniqueID.Value == "" || player3UniqueID.Value == pID)
        {
            player3UniqueID.Value = pID;
            player3ID.Value = id;
            return 2;
        }
        else if (player4UniqueID.Value == "" || player4UniqueID.Value == pID)
        {
            player4UniqueID.Value = pID;
            player4ID.Value = id;
            return 3;
        }
        else
        {
            return 4;
        }
    }

    public void RemovePlayer(FixedString128Bytes pID, ulong id)
    {
        GetComponent<NetworkObject>().ChangeOwnership(id);
        print(pID == player2UniqueID.Value);
        if (player1UniqueID.Value == pID)
        {
            player1UniqueID.Value = "";
            TransferOwnership(0);
        }
        else if (player2UniqueID.Value == pID)
        {
            player2UniqueID.Value = "";
            TransferOwnership(1);
        }
        else if (player3UniqueID.Value == pID)
        {
            player3UniqueID.Value = "";
            TransferOwnership(2);
        }
        else if (player4UniqueID.Value == pID)
        {
            player4UniqueID.Value = "";
            TransferOwnership(3);
        }
        else
        {
            TransferOwnership(4);
        }
    }

    void TransferOwnership(int num)
    {
        print("transfering");
        if (player1UniqueID.Value != "" && num != 0)
        {
            print("changing to " + player1ID.Value);
            GetComponent<NetworkObject>().ChangeOwnership(player1ID.Value);
        }
        else if (player2UniqueID.Value != "" && num != 1)
        {
            print("changing to " + player2ID.Value);
            GetComponent<NetworkObject>().ChangeOwnership(player2ID.Value);
        }
        else if (player3UniqueID.Value != "" && num != 2)
        {
            GetComponent<NetworkObject>().ChangeOwnership(player3ID.Value);
        }
        else if (player4UniqueID.Value != "" && num != 3)
        {
            GetComponent<NetworkObject>().ChangeOwnership(player4ID.Value);
        }
        else
        {
        }
    }
}
