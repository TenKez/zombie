using UnityEngine;
using Unity.Netcode;

public class Alarm : NetworkBehaviour
{
    public float noiseStrength = 2.0f;
    public PowerConsumer consumer;

    private float _timer;

    private void Awake()
    {
        if (!consumer) consumer = GetComponent<PowerConsumer>();
    }

    private void Update()
    {
        if (!IsServer) return;
        if (consumer && consumer.powered)
        {
            _timer += Time.deltaTime;
            if (_timer >= 1f)
            {
                _timer = 0f;
                NoiseSystem.Emit(transform.position, noiseStrength);
            }
        }
    }
}
