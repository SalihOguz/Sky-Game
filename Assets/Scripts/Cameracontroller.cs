using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameracontroller : MonoBehaviour
{
    [SerializeField] private GameObject fpCam;
    [SerializeField] private GameObject tpCam;

    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            fpCam.SetActive(true);
            tpCam.SetActive(false);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            fpCam.SetActive(false);
            tpCam.SetActive(true);
        }
    }
}
