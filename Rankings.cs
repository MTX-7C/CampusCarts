using Unity.Netcode;
using UnityEngine;

public class Rankings : NetworkBehaviour
{
    
    public string[] playerFinishOrder = new string[4];
    public NetworkVariable<int> playersFinished = new NetworkVariable<int>();
    bool p1 = false, p2 = false, p3 = false, p4 = false;
    public GameObject rankingScreen;

    private void Start()
    {
        playersFinished.Value = 0;
    }

    [Rpc(SendTo.Authority)]
    public void AddPlayerToRankingRpc(string playerToRank, string id)
    {
        Debug.Log("Got " + playerToRank + " from " + id);
        if (!p1 && id == "Player1")
        {
            p1 = true;
            AddPlayer(playerToRank);
        }
        else if (!p2 && id == "Player2")
        {
            p2 = true;
            AddPlayer(playerToRank);
        }
        else if(!p3 && id == "Player3")
        {
            p3 = true;
            AddPlayer(playerToRank);
        }
        else if (!p4 && id == "Player4")
        {
            p4 = true;
            AddPlayer(playerToRank);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            for (int i = 0; i < playerFinishOrder.Length; i++)
            {
                print(playerFinishOrder[i]);
            }
        }
        if (playerFinishOrder[3] != "")
        {
            ShowRankingsRpc(playerFinishOrder[0], playerFinishOrder[1], playerFinishOrder[2], playerFinishOrder[3]);
        }
    }

    void AddPlayer(string player)
    {
        playerFinishOrder[playersFinished.Value] = player;
        playersFinished.Value++;
    }

    [Rpc(SendTo.Everyone)]
    public void ShowRankingsRpc(string p1, string p2, string p3, string p4)
    {
        rankingScreen.SetActive(true);
        rankingScreen.GetComponent<DisplayRanking>().DisplayRankings(p1, p2, p3, p4);
    }
}
