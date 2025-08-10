using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PowerGrid : NetworkBehaviour
{
    private List<PowerConsumer> consumers = new List<PowerConsumer>();
    private List<IPowerSource> sources = new List<IPowerSource>();

    public void RegisterConsumer(PowerConsumer pc) { if (!consumers.Contains(pc)) consumers.Add(pc); }
    public void UnregisterConsumer(PowerConsumer pc) { consumers.Remove(pc); }
    public void RegisterSource(IPowerSource src) { if (!sources.Contains(src)) sources.Add(src); }
    public void UnregisterSource(IPowerSource src) { sources.Remove(src); }

    private void Update()
    {
        if (!IsServer) return;
        float available = 0f;
        foreach (var s in sources) available += s.GetAvailablePower();

        foreach (var c in consumers)
        {
            if (available >= c.watts) { c.SetPowered(true); available -= c.watts; }
            else c.SetPowered(false);
        }
    }
}

public interface IPowerSource { float GetAvailablePower(); }
