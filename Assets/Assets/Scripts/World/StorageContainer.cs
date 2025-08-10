using UnityEngine;
using Unity.Netcode;

public class StorageContainer : NetworkBehaviour
{
    public Inventory Inventory;

    private void Awake()
    {
        if (Inventory == null) Inventory = GetComponent<Inventory>();
    }
}
