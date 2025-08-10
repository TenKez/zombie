using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(PlayerNetwork))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    public float baseSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float gravity = -9.81f;
    public Camera playerCamera;

    private PlayerNetwork _playerNet;
    private CharacterController _cc;
    private Vector3 _velocity;

    private void Awake()
    {
        _playerNet = GetComponent<PlayerNetwork>();
        _cc = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (!IsOwner && playerCamera != null)
        {
            playerCamera.enabled = false;
            var audio = playerCamera.GetComponent<AudioListener>();
            if (audio) audio.enabled = false;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool sprint = Input.GetKey(KeyCode.LeftShift);

        Vector3 input = new Vector3(h, 0, v);
        input = transform.TransformDirection(input);
        float speed = baseSpeed * _playerNet.GetSpeedMultiplier() * (sprint ? sprintMultiplier : 1f);

        _cc.Move(input * speed * Time.deltaTime);

        if (_cc.isGrounded && _velocity.y < 0) _velocity.y = -2f;
        _velocity.y += gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);

        // Emit noise while running / voice etc. (voice TODO in VoiceChatInterface)
        if (input.sqrMagnitude > 0.2f)
        {
            NoiseSystem.Emit(transform.position, sprint ? 1.5f : 0.8f);
        }
    }
}
