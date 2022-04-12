using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWheel : MonoBehaviour
{

    public float angle;

    public GameObject wheel;

    public Quaternion target;

    public float rotSpeed;

    public float sensitivity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        angle += Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
        angle = Mathf.Clamp(angle, -70.0f, 70.0f);

        if (angle < 0)
        {
            angle += sensitivity * Time.deltaTime;
        }

        if (angle > 0)
        {
            angle -= sensitivity * Time.deltaTime;
        }
        transform.localRotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }
}
