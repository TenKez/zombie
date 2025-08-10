using UnityEngine;
using Unity.Netcode;

public class LootContainer : NetworkBehaviour
{
    public Inventory Inventory;
    public float openNoise = 0.3f;

    private void Awake()
    {
        if (Inventory == null) Inventory = GetComponent<Inventory>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void OpenServerRpc(ulong opener)
    {
        NoiseSystem.Emit(transform.position, openNoise);
        // In a full game, you would validate distance/LOS; here we just emit noise
    }
}
