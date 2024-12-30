using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    [SerializeField] private GameObject cameraObj;
    
    private float zoomAmount;
    private float zoomSpeed = 1f;
    private float zoomDistance;
    private float zoomDistanceMax = -100f;
    private float zoomDistanceMin = -10f;

    
    void Update()
    {
        Debug.Log(cameraObj.transform.forward);
        zoomCamera();
    }
    
    private void zoomCamera()
    {
        if (Input.GetAxis("Mouse ScrollWheel") == 0) return;
        //if (cameraObj.transform.forward.)

        zoomAmount = Input.GetAxis("Mouse ScrollWheel");
        zoomDistance = zoomAmount * zoomSpeed * Time.deltaTime;
        zoomDistance = Mathf.Clamp(zoomDistance, zoomDistanceMin, zoomDistanceMax);
        
        Vector3 position = cameraObj.transform.position;
        position += cameraObj.transform.forward * zoomDistance;
        
        cameraObj.transform.position = position;
    }
}
