using UnityEngine;
using Unity.Netcode;

public class ClaimMainChest : NetworkBehaviour
{
    public ClaimArea area;
    public StorageContainer chestStorage;
    public string zombieBloodItemId = "ZombieBlood";
    public int requiredBlood = 1;

    public override void OnNetworkSpawn()
    {
        if (area == null) area = GetComponentInParent<ClaimArea>();
        if (chestStorage == null) chestStorage = GetComponent<StorageContainer>();
    }

    public bool HasBlood()
    {
        if (chestStorage?.Inventory == null) return false;
        int total = 0;
        foreach (var st in chestStorage.Inventory.Items)
            if (st.ItemId.Equals(zombieBloodItemId)) total += st.Count; // ← .Equals handles FixedString64Bytes
        return total >= requiredBlood;
    }
}
