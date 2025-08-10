using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class PlayerNetwork : NetworkBehaviour
{
    public NetworkVariable<float> WeightKg = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> TeamId = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public Transform itemHoldPoint;
    private CharacterController _cc;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Set default camera target, UI hookup, etc.
        }
    }

    [ServerRpc]
    public void SetTeamServerRpc(int team) { TeamId.Value = team; }

    public float GetSpeedMultiplier()
    {
        // Simple movement penalty for weight: up to -50% at 50kg
        float penalty = Mathf.Clamp01(WeightKg.Value / 50f) * 0.5f;
        return 1f - penalty;
    }
}
