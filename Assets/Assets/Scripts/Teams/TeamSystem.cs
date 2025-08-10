using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class TeamSystem : NetworkBehaviour
{
    private Dictionary<int, HashSet<ulong>> _teams = new Dictionary<int, HashSet<ulong>>();

    [ServerRpc(RequireOwnership = false)]
    public void JoinTeamServerRpc(int teamId, ulong clientId)
    {
        if (!_teams.TryGetValue(teamId, out var set))
        {
            set = new HashSet<ulong>();
            _teams[teamId] = set;
        }
        set.Add(clientId);
        var player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerNetwork>();
        player.SetTeamServerRpc(teamId);
    }
}
