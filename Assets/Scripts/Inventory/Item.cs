using System;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections; // ← FixedString

public enum ItemType
{
    None,
    Food,
    Water,
    Medical,
    Tool,
    Weapon,
    Ammo,
    Material,
    KeyItem,
    Resource
}

[CreateAssetMenu(menuName = "BoD/Item", fileName = "Item_")]
public class Item : ScriptableObject
{
    public string Id;
    public string DisplayName;
    public ItemType Type;
    public Sprite Icon;
    public GameObject WorldPrefab;

    [Header("Stacking")]
    public bool Stackable = true;
    public int MaxStack = 10;

    [Header("Stats (optional)")]
    public float WeightKg = 0.5f;
    public float VolumeM2 = 0.02f; // simplified "area" usage for vehicles
    public float Nutrition = 0f;
    public float Hydration = 0f;
    public float Heal = 0f;

    [Header("Weapon/Tool Tags")]
    public bool IsCrowbar;
    public bool IsAxe;
    public bool IsLockpick;
    public bool IsExplosive;
    public bool IsAntidote;
    public bool IsWalkieTalkie;
}

[Serializable]
public struct ItemStack : INetworkSerializable, IEquatable<ItemStack>
{
    public FixedString64Bytes ItemId; // ← value type, replaces string
    public int Count;

    public ItemStack(string id, int count)
    {
        ItemId = new FixedString64Bytes(id ?? "");
        Count = count;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ItemId);
        serializer.SerializeValue(ref Count);
    }

    public bool Equals(ItemStack other) => ItemId.Equals(other.ItemId) && Count == other.Count;
    public override bool Equals(object obj) => obj is ItemStack other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = ItemId.GetHashCode();
            hash = (hash * 397) ^ Count.GetHashCode();
            return hash;
        }
    }

    public override string ToString() => $"{ItemId} x{Count}";
}
