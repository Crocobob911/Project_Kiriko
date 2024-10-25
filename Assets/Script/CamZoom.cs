using System;
using UnityEngine;

namespace Script {
    public class CamZoom : MonoBehaviour {
        private GameObject cameraObj;
        private float mouseScrollAmount;
        
        private Vector2 zoomDistance;
        private readonly Vector2 zoomSpeed = new (-80f, 640f);
        private readonly Vector2 zoomDistanceMin = new (0, -8f);
        private readonly Vector2 zoomDistanceMax = new (1f, 0f);

        private void Awake() {
            cameraObj = GameObject.Find("Main Camera");
        }

        private void Update() {
            ZoomCamera();
        }

        private void ZoomCamera() {
            if (Input.GetAxis("Mouse ScrollWheel") == 0) return;

            mouseScrollAmount = Input.GetAxis("Mouse ScrollWheel");
            zoomDistance.x = mouseScrollAmount * zoomSpeed.x * Time.deltaTime;
            zoomDistance.y = mouseScrollAmount * zoomSpeed.y * Time.deltaTime;

            Vector3 position = cameraObj.transform.localPosition;
            position.y = Mathf.Clamp(position.y + zoomDistance.x, zoomDistanceMin.x, zoomDistanceMax.x);
            position.z = Mathf.Clamp(position.z + zoomDistance.y, zoomDistanceMin.y, zoomDistanceMax.y);
            cameraObj.transform.localPosition = position;
        }
    }
}