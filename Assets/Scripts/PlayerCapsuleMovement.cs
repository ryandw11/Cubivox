using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCapsuleMovement : MonoBehaviour
{
    [SerializeField, Range(0f, 2f)]
    public float speed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed;
        Vector3 displacement = velocity * Time.deltaTime;
        transform.localPosition += velocity;
    }
}
