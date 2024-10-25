using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class ValueModifierForDebug : MonoBehaviour {
    private PlayerMoveController playerMoveController;
    private CamZoom camZoom;
    private UnLockedCamCalculator unLockedCamCalculator;

    [Header("Player Move Speed")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 3f;
    
    [Space(5f), Header("Camera")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField, Range(0, 10)] private float camSensitivity = 1f;
    [SerializeField, Range(0f, 60f)] float camAngleMaximum = 40f;
    [SerializeField, Range(270f, 361f)] float camAngleMinimum = 300f;
    
    private void Awake() {
        playerMoveController = GameObject.Find("Player").GetComponent<PlayerMoveController>();
        camZoom= GameObject.Find("CameraRoot").GetComponent<CamZoom>();
        unLockedCamCalculator = 
            GameObject.Find("CameraRoot").GetComponent<CameraController>().GetUnLockedCam();
    }
}
