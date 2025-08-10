using UnityEngine;
using Unity.Netcode;

public class InfectionSystem : NetworkBehaviour
{
    [Header("Infection")]
    public NetworkVariable<bool> Infected = new NetworkVariable<bool>(false);
    public NetworkVariable<float> SecondsSinceBite = new NetworkVariable<float>(0);
    public float graceSeconds = 300f; // 5 minutes
    public float deathSeconds = 480f; // 8 minutes

    [Header("Health")]
    public float maxHealth = 100f;
    public NetworkVariable<float> health = new NetworkVariable<float>(100f);

    private Inventory _inventory;

    private void Awake()
    {
        _inventory = GetComponent<Inventory>();
    }

    private void Update()
    {
        if (!IsServer) return;
        if (Infected.Value)
        {
            SecondsSinceBite.Value += Time.deltaTime;

            if (SecondsSinceBite.Value > graceSeconds)
            {
                // start health drain
                health.Value -= Time.deltaTime * 2f; // 2 HP per second after 5m
            }
            if (SecondsSinceBite.Value > deathSeconds || health.Value <= 0f)
            {
                // TODO: respawn, drop loot, convert to zombie if desired
                health.Value = maxHealth;
                Infected.Value = false;
                SecondsSinceBite.Value = 0f;
                // Simple respawn: teleport to origin (replace with proper spawn manager)
                Transform t = this.transform;
                t.position = Vector3.zero;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void BiteServerRpc()
    {
        if (!Infected.Value)
        {
            Infected.Value = true;
            SecondsSinceBite.Value = 0f;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UseAntidoteServerRpc()
    {
        // In real game: check inventory for antidote item before clearing
        Infected.Value = false;
        SecondsSinceBite.Value = 0f;
    }
}
