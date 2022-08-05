using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    private float m_horizontalInput;
    private float m_verticalInput;
    private float m_steeringAngle;

    public WheelCollider frontDriverW, frontPassengerW;
    public WheelCollider rearDriverW, rearPassengerW;
    public Transform frontDriverT, frontPassengerT;
    public Transform rearDriverT, rearPassengerT;
    public float maxSteerAngle = 30;
    public float motorForce = 50;

    public Vector3 com;
    public Rigidbody rb;

    public float xRotationLimit = 20f;
    public float yRotationLimit = 20f;
    public float zRotationLimit = 20f;

    public float brakeT = 600f;

    public void Start()
    {
        rb.centerOfMass = com;
    }

    public void GetInput()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Accelerate");
    }

    private void Steer()
    {
        m_steeringAngle = maxSteerAngle * m_horizontalInput;
        frontDriverW.steerAngle = m_steeringAngle;
        frontPassengerW.steerAngle = m_steeringAngle;
    }

    private void Accelerate()
    {
        rb.AddTorque(Vector3.forward * m_verticalInput * motorForce);
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontDriverW, frontDriverT);
        UpdateWheelPose(frontPassengerW, frontPassengerT);
        UpdateWheelPose(rearDriverW, rearDriverT);
        UpdateWheelPose(rearPassengerW, rearPassengerT);
    }

    private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
    {
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;

        _collider.GetWorldPose(out _pos, out _quat);

        _transform.position = _pos;
        _transform.rotation = _quat;
    }

    private void HandBrake()
    {
        if (Input.GetAxis("Brake") >= .5f)
        {
            frontDriverW.brakeTorque = brakeT;
            frontPassengerW.brakeTorque = brakeT;
            rearDriverW.brakeTorque = brakeT;
            rearPassengerW.brakeTorque = brakeT;
        }
        else
        {
            frontDriverW.brakeTorque = 0f;
            frontPassengerW.brakeTorque = 0f;
            rearDriverW.brakeTorque = 0f;
            rearPassengerW.brakeTorque = 0f;
        }

    }

    private void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        UpdateWheelPoses();
        HandBrake();

        rb.AddForce(-transform.up * rb.velocity.magnitude);
    }
}
