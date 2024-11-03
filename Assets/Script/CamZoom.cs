using System;
using Cinemachine;
using UnityEngine;

namespace Script {
    public class CamZoom : MonoBehaviour, IValueModifierObserver {
        [SerializeField] private CinemachineVirtualCamera vcam;
        [SerializeField] private Cinemachine3rdPersonFollow componentBase;
        private float mouseScrollAmount;
        
        private float zoomDistance;
        private float zoomSpeed = 8f;
        private readonly Vector2 zoomDistanceMin = new (0, -8f);
        private readonly Vector2 zoomDistanceMax = new (1f, 0f);

        private void Start() {
            //vcam = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
            componentBase = vcam.GetComponent<Cinemachine3rdPersonFollow>();
        }
        

        private void Update() {
            ZoomCamera();
        }

        private void ZoomCamera() {
            if (Input.GetAxis("Mouse ScrollWheel") == 0) return;

            mouseScrollAmount = Input.GetAxis("Mouse ScrollWheel");
            
            zoomDistance = mouseScrollAmount * zoomSpeed * Time.deltaTime;
            componentBase.CameraDistance = zoomDistance;
        }
        
        #if UNITY_EDITOR
        public void ValueModifierUpdated() {
            zoomSpeed = ValueModifier.Instance.ZoomSpeed;
        }
        #endif
    }
}