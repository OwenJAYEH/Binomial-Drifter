using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
 public Rigidbody rb;

    public float speed = 12f;
    public float rotateSpeed = 30f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    // Update is called once per frame
    void Update()
    {

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        //print(x);
        //print(z);

        Vector3 move = transform.forward * z;
        Vector3 rotate = transform.up * x;

        rb.AddForce(move * speed * Time.deltaTime);
        rb.AddTorque(rotate * rotateSpeed  * Time.deltaTime);
        //controller.transform.Rotate(rotate * Time.deltaTime);
    }
}
