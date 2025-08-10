using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using System.Collections;

public enum ZombieClass { Walker, Crawler, Runner, Brute, Mega }

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : NetworkBehaviour
{
    public ZombieClass zombieClass = ZombieClass.Walker;
    public float visionRange = 20f;
    public float hearingRange = 25f;
    public float lightBias = 0.5f;
    public float attackRange = 1.8f;
    public float attackDamage = 20f;
    public float health = 150f;

    private NavMeshAgent _agent;
    private Vector3 _home;
    private float _stateTimer;
    private enum State { Idle, Wander, Investigate, Chase, Attack }
    private State _state;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) enabled = false;
        _agent = GetComponent<NavMeshAgent>();
        _home = transform.position;
        ConfigureClass(zombieClass);
        _state = State.Wander;
        _stateTimer = Random.Range(3f, 6f);
    }

    private void ConfigureClass(ZombieClass cls)
    {
        switch (cls)
        {
            case ZombieClass.Crawler:
                _agent.speed = 1.2f; attackDamage = 10f; health = 80f; break;
            case ZombieClass.Runner:
                _agent.speed = 5.5f; attackDamage = 15f; health = 120f; break;
            case ZombieClass.Brute:
                _agent.speed = 2.5f; attackDamage = 40f; health = 300f; break;
            case ZombieClass.Mega:
                _agent.speed = 6.0f; attackDamage = 50f; health = 500f; break;
            default:
                _agent.speed = 3.0f; attackDamage = 20f; health = 150f; break;
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        // Noise reactions
        foreach (var ev in NoiseSystem.Instance?.GetSnapshot() ?? System.Array.Empty<NoiseEvent>())
        {
            float dist = Vector3.Distance(transform.position, ev.Position);
            if (dist < hearingRange * Mathf.Lerp(0.5f, 1.5f, ev.Strength))
            {
                _state = State.Investigate;
                _agent.SetDestination(ev.Position);
                _stateTimer = 120f; // 2 minutes localized wandering
            }
        }

        // Look for players
        var target = FindClosestVisiblePlayer();
        if (target != null)
        {
            float d = Vector3.Distance(transform.position, target.position);
            if (d <= attackRange)
            {
                _state = State.Attack;
                StartCoroutine(AttackRoutine(target));
            }
            else
            {
                _state = State.Chase;
                _agent.SetDestination(target.position);
            }
            return;
        }

        // Scent following if nothing else
        var scent = ScentTrailEmitter.GetRecentDirection(transform.position, 60f);
        if (scent.HasValue)
        {
            _state = State.Investigate;
            _agent.SetDestination(scent.Value);
            _stateTimer = 300f; // up to 5 minutes broader wander
        }
        else
        {
            // Wander logic
            _stateTimer -= Time.deltaTime;
            if (_stateTimer <= 0f || _agent.remainingDistance < 1f)
            {
                _stateTimer = Random.Range(3f, 8f);
                Vector3 rand = _home + Random.insideUnitSphere * 10f;
                rand.y = transform.position.y;
                _agent.SetDestination(rand);
                _state = State.Wander;
            }
        }
    }

    private Transform FindClosestVisiblePlayer()
    {
        float best = float.MaxValue;
        Transform bestT = null;
        foreach (var netObj in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList)
        {
            if (!netObj || !netObj.IsPlayerObject) continue;
            var t = netObj.transform;
            float d = Vector3.Distance(transform.position, t.position);
            if (d < visionRange && HasLineOfSight(t))
            {
                float score = d;
                var le = t.GetComponent<LightEmitter>();
                if (le) score *= Mathf.Lerp(1f, 1f - lightBias, le.brightness);
                if (score < best) { best = score; bestT = t; }
            }
        }
        return bestT;
    }

    private bool HasLineOfSight(Transform t)
    {
        Vector3 dir = (t.position + Vector3.up) - (transform.position + Vector3.up);
        if (Physics.Raycast(transform.position + Vector3.up, dir.normalized, out RaycastHit hit, visionRange))
        {
            return hit.transform == t;
        }
        return false;
    }

    private IEnumerator AttackRoutine(Transform target)
    {
        _state = State.Attack;
        while (true)
        {
            if (!target) yield break;
            float d = Vector3.Distance(transform.position, target.position);
            if (d > attackRange * 1.2f) yield break;

            var inf = target.GetComponent<InfectionSystem>();
            if (inf)
            {
                inf.health.Value -= attackDamage;
                // Chance to infect on hit
                if (Random.value < 0.25f) inf.BiteServerRpc();
            }
            yield return new WaitForSeconds(1.2f);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float dmg)
    {
        health -= dmg;
        if (health <= 0) { NetworkObject.Despawn(); }
    }
}
