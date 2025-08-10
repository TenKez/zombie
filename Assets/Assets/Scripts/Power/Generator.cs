using UnityEngine;
using Unity.Netcode;

public class Generator : NetworkBehaviour, IPowerSource
{
    public float maxWatts = 1500f;
    public float fuel = 100f; // liters arbitrary
    public float fuelUsePerMinute = 1f;
    public bool running;

    private PowerGrid _grid;

    private void OnEnable()
    {
        _grid = FindObjectOfType<PowerGrid>();
        if (_grid) _grid.RegisterSource(this);
    }

    private void OnDisable()
    {
        if (_grid) _grid.UnregisterSource(this);
    }

    private void Update()
    {
        if (!IsServer) return;
        if (running && fuel > 0f)
        {
            fuel -= fuelUsePerMinute / 60f * Time.deltaTime * 60f;
            if (fuel <= 0f) { fuel = 0f; running = false; }
        }
    }

    public float GetAvailablePower() => running && fuel > 0f ? maxWatts : 0f;

    [ServerRpc(RequireOwnership = false)]
    public void StartStopServerRpc(bool on) { running = on; }
}
