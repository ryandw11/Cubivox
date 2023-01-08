using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCapsuleMovement : MonoBehaviour
{
    [SerializeField, Range(0f, 2f)]
    public float speed = 0.5f;
    public float sensitivity = 10f;
    public float jumpForce = 250;

    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        /*Vector3 velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed;
        Vector3 displacement = velocity * Time.deltaTime;
        transform.localPosition += velocity;

        currentRotation.x += Input.GetAxis("Mouse X") * sensitivity;
        currentRotation.y -= Input.GetAxis("Mouse Y") * sensitivity;
        currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -10, 80);
        //Camera.main.transform.rotation = Quaternion.Euler(currentRotation.y, 0, 0);

        transform.rotation = Quaternion.Euler(0, currentRotation.x, 0);*/

        rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(new Vector3(0, Input.GetAxis("Mouse X") * sensitivity, 0)));
        rigidbody.MovePosition(transform.position + (transform.forward * Input.GetAxis("Vertical") * speed) + (transform.right * Input.GetAxis("Horizontal") * speed));
        if (Input.GetKeyDown("space"))
            rigidbody.AddForce(transform.up * jumpForce);

        Camera.main.transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(new Vector3(-Input.GetAxis("Mouse Y") * sensitivity, 0, 0));
    }
}
