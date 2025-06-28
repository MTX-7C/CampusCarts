using System.Collections;
using TMPro;
using UnityEngine;

public class DisplayRanking : MonoBehaviour
{
    public Rankings rankings;
    public TMP_Text[] places;
    public GameObject menu, ranking;

    private void Update()
    {
        if (ranking.activeInHierarchy && !menu.activeInHierarchy && Input.anyKeyDown)
        {
            menu.SetActive(true);
        }
    }

    public void DisplayRankings(string p1, string p2, string p3, string p4)
    {
        places[0].text = p1;
        places[1].text = p2;
        places[2].text = p3;
        places[3].text = p4;
        //StartCoroutine(ShowMenuScreen());
    }

    IEnumerator ShowMenuScreen()
    {
        yield return new WaitForSeconds(10);
        menu.SetActive(true);
    }
}
