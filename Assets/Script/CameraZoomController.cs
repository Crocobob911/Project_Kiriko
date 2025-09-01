using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Script;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 카메라의 '줌' 기능을 담당합니다.
/// 마우스 휠 스크롤에 따라 카메라와 플레이어 사이의 거리를 조절합니다.
/// </summary>
public class CameraZoomController : MonoBehaviour, IValueModifierObserver
{
    
    [SerializeField] private CinemachineVirtualCamera vcam;
    private Cinemachine3rdPersonFollow componentBase;
        
    [SerializeField] private float zoomMin;
    [SerializeField] private float zoomMax;
    [SerializeField] private float zoomSpeed;

    private void Awake() {
        vcam = GetComponent<CinemachineVirtualCamera>();
        componentBase = vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
    }

    private void Start() {
#if UNITY_EDITOR
        ValueModifier.Instance.AddSubscriber(this);
        ValueModifierUpdated();
#endif
    }

    public void ZoomCamera(InputAction.CallbackContext context) {
        componentBase.CameraDistance 
            = Mathf.Clamp(componentBase.CameraDistance - context.ReadValue<float>() * zoomSpeed * Time.deltaTime, 
                zoomMin, zoomMax);
    }
    
    
#if UNITY_EDITOR
    public void ValueModifierUpdated() {
        zoomSpeed = ValueModifier.Instance.ZoomSpeed;
        zoomMin = ValueModifier.Instance.ZoomMin;
        zoomMax = ValueModifier.Instance.ZoomMax;
    }
#endif
}
