using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShipController : MonoBehaviour
{
    [Header("Ship Parts")]
    [SerializeField] private Transform shipModel;
    [SerializeField] private Transform[] backPropellers;
    [SerializeField] private Transform[] wings;
    [SerializeField] private Transform[] rudders;
    [SerializeField] private Transform wheel;
    
    [Header("Forward Movement")]
    [SerializeField] private float forwardSpeedPerGear = 5f;
    [SerializeField] private float forwardAcceleration = 2f;
    [SerializeField] private float maxGear = 3;
    
    [Header("Vertical Movement")]
    [SerializeField] private float liftSpeed = 3f;
    [SerializeField] private float liftAcceleration = 5f;
    
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 30;
    [SerializeField] private float rollAngle = 15f;
    [SerializeField] private float pitchAngle = 5f;
    [SerializeField] private float rotationAcceleration = 10f;
    
    private Rigidbody _rb;
    private Vector3 _movement;
    private Vector3 _rotation;
    
    private float _currentGear = 0;
    private float _currentForwardSpeed = 0;
    private float _currentLiftSpeed = 0;
    private float _currentRotationSpeed = 0;

    private void Start()
    {
        _rb = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _currentGear = Mathf.Min(_currentGear + 1, maxGear);
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            _currentGear = Mathf.Max(_currentGear - 1, 0);
        }
        
        // Forward
        _currentForwardSpeed = Mathf.Lerp(_currentForwardSpeed, _currentGear * forwardSpeedPerGear, Time.deltaTime * forwardAcceleration);
        _movement = transform.forward * _currentForwardSpeed;

        foreach (var backPropeller in backPropellers)
        {
            backPropeller.localEulerAngles -= _currentForwardSpeed * Vector3.up;
        }
        
        // Lift
        _currentLiftSpeed = Mathf.Lerp(_currentLiftSpeed, Input.GetAxis("Vertical") * liftSpeed, Time.deltaTime * liftAcceleration);
        _movement += Vector3.up * _currentLiftSpeed;
        
        foreach (var wing in wings)
        {
            wing.localEulerAngles = Vector3.left * (_currentLiftSpeed * 10f);
        }
        
        // Shake
        // TODO make ship shake from turbulence 
        
        
        // Rotation
        Vector3 eulerAngles = transform.eulerAngles;
        float angleX = eulerAngles.x;
        angleX = (angleX > 180) ? angleX - 360 : angleX;
        
        float angleZ = eulerAngles.z;
        angleZ = (angleZ > 180) ? angleZ - 360 : angleZ;

        _currentRotationSpeed = Mathf.Lerp(_currentRotationSpeed, Input.GetAxis("Horizontal") * rotationSpeed, Time.deltaTime * rotationAcceleration);
        _rotation = new Vector3(
            (-Input.GetAxis("Vertical") * pitchAngle - angleX), 
            _currentRotationSpeed, 
            (-Input.GetAxis("Horizontal") * rollAngle - angleZ));

        foreach (var rudder in rudders)
        {
            rudder.localEulerAngles = Vector3.forward * (_currentRotationSpeed * 1.5f);
        }

        wheel.localEulerAngles = Vector3.down * (_currentRotationSpeed * 8f);
    }

    private void FixedUpdate()
    {
        MoveZeppelin(_movement, _rotation);
    }

    private void MoveZeppelin(Vector3 direction, Vector3 rotation)
    {
        _rb.MovePosition(_rb.position + direction * Time.deltaTime);
        
        Quaternion deltaRotation = Quaternion.Euler(rotation * Time.deltaTime);
        _rb.MoveRotation(_rb.rotation * deltaRotation);
        
    }
}
