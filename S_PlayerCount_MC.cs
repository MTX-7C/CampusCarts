using UnityEngine;

public class S_PlayerCount_MC : MonoBehaviour
{
    public bool taken = false;

    private void OnTriggerEnter(Collider other)
    {
        taken = true;
    }
}
