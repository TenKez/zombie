using UnityEngine;
using Unity.Netcode;

public class MapTable : NetworkBehaviour
{
    public FogOfWarSystem fog;

    private void Awake()
    {
        if (!fog) fog = FindObjectOfType<FogOfWarSystem>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ShareMapServerRpc(int teamId, ulong fromClientId)
    {
        fog.ShareWithTeamServerRpc(teamId, fromClientId);
    }
}
