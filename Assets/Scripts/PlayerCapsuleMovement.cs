using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CubivoxCore.Events.Global;

using CubivoxClient;

public class PlayerCapsuleMovement : MonoBehaviour
{
    [SerializeField, Range(0f, 2f)]
    public float speed = 0.5f;
    public float sensitivity = 10f;
    public float jumpForce = 300;

    private Rigidbody rigidbody;

    private ClientCubivox clientCubivox;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        clientCubivox = ClientCubivox.GetClientInstance();
    }

    // Update is called once per frame
    void Update()
    {
        if (clientCubivox.CurrentState == GameState.PLAYING && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (clientCubivox.CurrentState != GameState.PLAYING && Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (clientCubivox.CurrentState != GameState.PLAYING) return;

        var previousPosition = transform.position;

        rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(new Vector3(0, Input.GetAxis("Mouse X") * sensitivity, 0)));
        rigidbody.MovePosition(transform.position + (transform.forward * Input.GetAxis("Vertical") * speed) + (transform.right * Input.GetAxis("Horizontal") * speed));
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rigidbody.AddForce(transform.up * jumpForce);
        }

        Camera.main.transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(new Vector3(-Input.GetAxis("Mouse Y") * sensitivity, 0, 0));

        PlayerMoveEvent playerMoveEvent = new PlayerMoveEvent(clientCubivox.LocalPlayer, LocationUtils.VectorToLocation(transform.position), LocationUtils.VectorToLocation(previousPosition));
        ClientCubivox.GetEventManager().TriggerEvent(playerMoveEvent);

        if ( playerMoveEvent.IsCanceled() )
        {
            // Reset position if the event fails.
            transform.position = previousPosition;
            // Zero out the velocity.
            rigidbody.velocity = Vector3.zero;
        }

        // Note: The player's location can be modified by the event.
    }

    bool IsGrounded()
    {
        // TODO: Remove this check when flying can be done for debug purposes through a debug mod.
#if UNITY_EDITOR
        return true;
#else
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
#endif
    }
}
