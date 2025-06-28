using System.Collections;
using UnityEngine;

public class TrackRespawns : MonoBehaviour
{
    public Transform[] respawns;

    private void Start()
    {
        Queue queue = new Queue();
        respawns = GetComponentsInChildren<Transform>();
        for(int i = 0; i < respawns.Length - 1; i++)
        {
            respawns[i] =  respawns[i + 1];
        }
    }
}
