using UnityEngine;
using Unity.Netcode;

public class SecurityCamera : NetworkBehaviour
{
    public PowerConsumer consumer;

    private void Awake()
    {
        if (!consumer) consumer = GetComponent<PowerConsumer>();
    }
}
