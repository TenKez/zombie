using UnityEngine;
using Unity.Netcode;

public class BuildPiece : NetworkBehaviour
{
    public float structuralStrength = 1f;
    public Vector3 size = new Vector3(2, 2, 0.2f);
    public DecayingObject decaying;

    private void Awake()
    {
        decaying = GetComponent<DecayingObject>();
        if (!decaying) decaying = gameObject.AddComponent<DecayingObject>();
    }
}
