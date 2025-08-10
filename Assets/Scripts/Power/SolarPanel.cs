using UnityEngine;
using Unity.Netcode;

public class SolarPanel : NetworkBehaviour, IPowerSource
{
    public float peakWatts = 300f;

    public float GetAvailablePower()
    {
        // Simple day-night cycle approximation: daylight between 6 and 18 hours
        float hour = (Time.time / 60f) % 24f; // 1 min per in-game day hour
        float sun = Mathf.Clamp01(1f - Mathf.Abs(hour - 12f) / 6f);
        return peakWatts * sun;
    }

    private void OnEnable()
    {
        var grid = FindObjectOfType<PowerGrid>();
        if (grid) grid.RegisterSource(this);
    }
    private void OnDisable()
    {
        var grid = FindObjectOfType<PowerGrid>();
        if (grid) grid.UnregisterSource(this);
    }
}
