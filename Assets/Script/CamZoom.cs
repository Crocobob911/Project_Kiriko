using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CamZoom : MonoBehaviour
{
    [SerializeField] private GameObject cameraObj;
    private float zoomAmount;
    private const float zoomSpeed_Y = -80f;
    private float zoomDistance_Y;
    private const float zoomDistanceMin_Y = 0f;
    private const float zoomDistanceMax_Y = 1f;
    
    private const float zoomSpeed_Z = zoomSpeed_Y * -8f;
    private float zoomDistance_Z;
    private const float zoomDistanceMin_Z = -8f;
    private const float zoomDistanceMax_Z = 0f;


    private void Update()
    {
        ZoomCamera();
    }

    private void ZoomCamera()
    {
        if (Input.GetAxis("Mouse ScrollWheel") == 0) return;

        zoomAmount = Input.GetAxis("Mouse ScrollWheel");
        zoomDistance_Y = zoomAmount * zoomSpeed_Y * Time.deltaTime;
        zoomDistance_Z = zoomAmount * zoomSpeed_Z * Time.deltaTime;

        Vector3 position = cameraObj.transform.localPosition;
        position.y = Mathf.Clamp(position.y + zoomDistance_Y, zoomDistanceMin_Y, zoomDistanceMax_Y);
        position.z = Mathf.Clamp(position.z + zoomDistance_Z, zoomDistanceMin_Z, zoomDistanceMax_Z);
        cameraObj.transform.localPosition = position;
    }
}