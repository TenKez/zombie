using UnityEngine;
using Unity.Netcode;
using System.Collections;

public enum DoorType { Wood, Metal }
public class DoorLock : NetworkBehaviour
{
    public DoorType doorType = DoorType.Wood;
    public bool isLocked = true;

    [Header("Timers (seconds)")]
    public float axeTime = 4f;
    public float crowbarTime = 6f;
    public float lockpickTime = 8f;
    public float explosiveTime = 2f;

    [Header("Noise")]
    public float axeNoise = 1.0f;
    public float crowbarNoise = 1.2f;
    public float lockpickNoise = 0.2f;
    public float explosiveNoise = 3.0f;

    private bool _busy;

    [ServerRpc(RequireOwnership = false)]
    public void TryForceOpenServerRpc(bool hasAxe, bool hasCrowbar)
    {
        if (_busy || !isLocked) return;
        if (doorType == DoorType.Wood && (hasAxe || hasCrowbar))
        {
            StartCoroutine(ForceOpenRoutine(hasAxe ? axeTime : crowbarTime, hasAxe ? axeNoise : crowbarNoise));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TryLockpickServerRpc(bool hasLockpick)
    {
        if (_busy || !isLocked || !hasLockpick) return;
        if (doorType == DoorType.Metal)
        {
            StartCoroutine(ForceOpenRoutine(lockpickTime, lockpickNoise));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TryExplosiveServerRpc(bool hasExplosive)
    {
        if (_busy || !isLocked || !hasExplosive) return;
        StartCoroutine(ForceOpenRoutine(explosiveTime, explosiveNoise));
    }

    private IEnumerator ForceOpenRoutine(float time, float noise)
    {
        _busy = true;
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            if (Random.value < 0.15f) // periodic hits make noise
                NoiseSystem.Emit(transform.position, noise);
            yield return null;
        }
        isLocked = false;
        _busy = false;
        NoiseSystem.Emit(transform.position, noise * 1.25f);
    }
}
