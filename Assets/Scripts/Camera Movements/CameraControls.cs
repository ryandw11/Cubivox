using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public int moveSpeed;
    public float minX = -360.0f;
    public float maxX = 360.0f;

    public float minY = -45.0f;
    public float maxY = 45.0f;

    public float sensX = 100.0f;
    public float sensY = 100.0f;


    float rotationY = 0.0f;
    float rotationX = 0.0f;
    private float zoomSpeed = 20.0f;


    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
            float scroll = Input.GetAxis("Mouse ScrollWheel"); //This gets the scroll
            transform.Translate(0, -(scroll * zoomSpeed), scroll * zoomSpeed, Space.World); //translate the camera. (Inverted so the controls feel right).

        if (Input.GetKey(KeyCode.Mouse1))
        { //if the mouse is pressed down
            rotationX += Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
            //rotationX += Input.GetAxis("Vertical") * 100F * Time.deltaTime;
            //rotationY += Input.GetAxis("Horizontal") * 100F * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, minY, maxY);
            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0); //Angles the camera.
        }

        if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime); //move forward
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);//move right (inverted)
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);//move back
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);//move left (inverted)
            }
            if (Input.GetKey(KeyCode.R)) // Resets Velocity so that when the camera is going crazy it will be fixed
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }

        //end else
    }//end void update
}
