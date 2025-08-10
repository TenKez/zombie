using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VehicleController : MonoBehaviour
{
    public float enginePower = 8000f;
    public float steerPower = 60f;
    public float fuel = 100f;
    public float baseFuelUsePerMinute = 1f;
    public VehicleStorage storage;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (!storage) storage = GetComponent<VehicleStorage>();
    }

    private void FixedUpdate()
    {
        if (fuel <= 0f) return;

        float fwd = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        float weightPenalty = storage ? Mathf.Clamp01(storage.currentM2 / Mathf.Max(1f, storage.capacityM2)) : 0f;
        float sluggish = 1f - 0.3f * weightPenalty;

        _rb.AddForce(transform.forward * fwd * enginePower * sluggish * Time.fixedDeltaTime);
        _rb.AddTorque(Vector3.up * turn * steerPower * sluggish * Time.fixedDeltaTime);

        fuel -= baseFuelUsePerMinute / 60f * (1f + 0.2f * weightPenalty);
        fuel = Mathf.Max(0f, fuel);
    }
}
