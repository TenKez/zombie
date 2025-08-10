using UnityEngine;
using Unity.Netcode;

public class PowerConsumer : NetworkBehaviour
{
    public float watts = 50f;
    public bool powered;

    private PowerGrid _grid;

    private void OnEnable()
    {
        _grid = FindObjectOfType<PowerGrid>();
        if (_grid) _grid.RegisterConsumer(this);
    }
    private void OnDisable()
    {
        if (_grid) _grid.UnregisterConsumer(this);
    }

    public void SetPowered(bool on)
    {
        powered = on;
        // Toggle lights, machines, etc.
        var light = GetComponentInChildren<Light>();
        if (light) light.enabled = on;
    }
}
