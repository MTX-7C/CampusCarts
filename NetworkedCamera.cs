using Unity.Netcode.Components;

public class NetworkedCamera : NetworkTransform
{
    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer)
        {
            gameObject.SetActive(false);
        }
        base.OnNetworkSpawn();
    }
}
