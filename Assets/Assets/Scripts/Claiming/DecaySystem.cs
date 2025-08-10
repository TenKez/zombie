using UnityEngine;
using Unity.Netcode;

public class DecayingObject : NetworkBehaviour
{
    public float hitpoints = 100f;
    public float decayPerMinute = 1f; // when no blood in claim chest

    public void ApplyDamage(float dmg)
    {
        if (!IsServer) return;
        hitpoints -= dmg;
        if (hitpoints <= 0) NetworkObject.Despawn();
    }
}

public class DecaySystem : NetworkBehaviour
{
    public float tickInterval = 5f;
    private float _timer;

    private void Update()
    {
        if (!IsServer) return;
        _timer += Time.deltaTime;
        if (_timer >= tickInterval)
        {
            _timer = 0f;
            Tick();
        }
    }

    private void Tick()
    {
        // Very simple: iterate all chests and decay objects in their area if missing blood
        foreach (var chest in FindObjectsOfType<ClaimMainChest>())
        {
            bool hasBlood = chest.HasBlood();
            foreach (var obj in FindObjectsOfType<DecayingObject>())
            {
                if (chest.area && chest.area.Contains(obj.transform.position))
                {
                    if (!hasBlood)
                    {
                        float perSecond = obj.decayPerMinute / 60f;
                        obj.ApplyDamage(perSecond * tickInterval);
                    }
                }
            }
        }
    }
}
