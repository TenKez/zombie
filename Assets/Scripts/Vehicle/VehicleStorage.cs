using UnityEngine;
using Unity.Netcode;

public class VehicleStorage : NetworkBehaviour
{
    public float capacityM2 = 6f;
    public float currentM2;

    public bool CanStore(Item item, int count)
    {
        float need = item.VolumeM2 * count;
        return currentM2 + need <= capacityM2;
    }

    [ServerRpc(RequireOwnership = false)]
    public void StoreServerRpc(string itemId, float volumePer, int count)
    {
        float need = volumePer * count;
        currentM2 += need;
    }
}
