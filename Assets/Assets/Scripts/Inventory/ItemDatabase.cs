using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BoD/ItemDatabase", fileName = "ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> Items = new List<Item>();

    private Dictionary<string, Item> _map;

    public void BuildIndex()
    {
        _map = new Dictionary<string, Item>();
        foreach (var it in Items)
        {
            if (string.IsNullOrEmpty(it.Id))
                it.Id = it.name;
            _map[it.Id] = it;
        }
    }

    public Item Get(string id)
    {
        if (_map == null) BuildIndex();
        return _map.TryGetValue(id, out var it) ? it : null;
    }
}
