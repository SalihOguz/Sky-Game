using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public Transform ship;

    private bool isEnabled = false;
    
    public void StartFlying()
    {
        isEnabled = true;
    }

    private void Update()
    {
        if (!isEnabled)
        {
            return;
        }
        
        ship.position = Vector3.Lerp(ship.position, ship.position + new Vector3(Input.GetAxis("Fire1"), Input.GetAxis("Vertical"), -Input.GetAxis("Horizontal")), Time.deltaTime * 5f);
    }
}
