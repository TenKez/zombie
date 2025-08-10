using System;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class Inventory : NetworkBehaviour
{
    [SerializeField] private int slots = 24;
    [SerializeField] private ItemDatabase database;

    public NetworkList<ItemStack> Items;

    public event Action OnInventoryChanged;

    private void Awake()
    {
        Items = new NetworkList<ItemStack>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            for (int i = 0; i < slots; i++) Items.Add(new ItemStack("", 0));
        }

        Items.OnListChanged += _ => OnInventoryChanged?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddItemServerRpc(string itemId, int count)
    {
        var id = new FixedString64Bytes(itemId ?? "");
        for (int i = 0; i < Items.Count; i++)
        {
            var st = Items[i];
            if (st.ItemId.Length == 0)
            {
                Items[i] = new ItemStack(itemId, count);
                return;
            }
            if (st.ItemId.Equals(id))
            {
                st.Count += count;
                Items[i] = st;
                return;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveItemServerRpc(string itemId, int count)
    {
        var id = new FixedString64Bytes(itemId ?? "");
        for (int i = 0; i < Items.Count; i++)
        {
            var st = Items[i];
            if (st.ItemId.Equals(id))
            {
                st.Count -= count;
                if (st.Count <= 0) st = new ItemStack("", 0);
                Items[i] = st;
                return;
            }
        }
    }

    public bool HasItem(string itemId, int count)
    {
        var id = new FixedString64Bytes(itemId ?? "");
        int total = 0;
        foreach (var st in Items)
            if (st.ItemId.Equals(id)) total += st.Count;
        return total >= count;
    }
}
