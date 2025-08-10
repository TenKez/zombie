using UnityEngine;

public class LightEmitter : MonoBehaviour
{
    [Range(0f, 1f)]
    public float brightness = 0.5f;

    private void OnEnable()
    {
        // Hook into your light system if needed, for now just a marker for zombies.
    }
}
