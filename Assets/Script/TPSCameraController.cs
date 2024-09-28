using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCameraController : MonoBehaviour
{
    [SerializeField]
    private Transform cameraRoot;
    [SerializeField, Range(0, 10)]
    float sensitivity = 1f;  // 시야 회전 감도
    [SerializeField, Range(0f, 60f)]
    private float camAngleMaximum;  //카메라가 어디까지 올라가나. 값이 60 이하일 것!
    [SerializeField, Range(270f, 361f)]
    float camAngleMinimum;  //카메라가 어디까지 내려가나.

    Vector2 mouseMoveDelta = new Vector2(0, 0);  //마우스 이동 입력
    Vector3 camAngle = new Vector3(0, 0, 0);  //카메라 최종 위치
    float camAngleX_adjusted; //카메라 수직 이동 범위 제한


    void Update()
    {
        LookAround();
    }

    private void LookAround()
    {
        //마우스 입력
        mouseMoveDelta.x = Input.GetAxis("Mouse X");
        mouseMoveDelta.y = Input.GetAxis("Mouse Y");

        //카메라 위치값 선계산
        camAngle = cameraRoot.rotation.eulerAngles;

        //카메라 이동 범위 제한
        camAngleX_adjusted = camAngle.x - mouseMoveDelta.y * sensitivity;
        if (camAngleX_adjusted < 180f) //카메라가 수평선 위
            camAngleX_adjusted = Mathf.Clamp(camAngleX_adjusted, -1f, camAngleMaximum);
        else //카메라가 수평선 아래
            camAngleX_adjusted = Mathf.Clamp(camAngleX_adjusted, camAngleMinimum, 361f);

        //카메라 위치 변경
        cameraRoot.rotation = Quaternion.Euler(
            camAngleX_adjusted,
            camAngle.y + mouseMoveDelta.x * sensitivity,
            camAngle.z);
    }
}
