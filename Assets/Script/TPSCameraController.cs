using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCameraController : MonoBehaviour
{
    [SerializeField]
    private Transform cameraRoot;
    [SerializeField, Range(0, 10)]
    float sensitivity = 1f;  // �þ� ȸ�� ����

    Vector2 mouseMoveDelta = new Vector2(0, 0);  //���콺 �̵� �Է�
    float camAngleX_adjusted; //ī�޶� ���� �̵� ���� ����
    Vector3 camAngle = new Vector3(0, 0, 0);  //ī�޶� ���� ��ġ

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        LookAround();
    }

    private void LookAround()
    {
        mouseMoveDelta.x = Input.GetAxis("Mouse X");
        mouseMoveDelta.y = Input.GetAxis("Mouse Y");

        camAngle = cameraRoot.rotation.eulerAngles;

        camAngleX_adjusted = camAngle.x - mouseMoveDelta.y * sensitivity;
        if (camAngleX_adjusted < 180f)
        {
            camAngleX_adjusted = Mathf.Clamp(camAngleX_adjusted, -1f, 30f);
        }
        else
        {
            camAngleX_adjusted = Mathf.Clamp(camAngleX_adjusted, 225f, 361f);
        }

        cameraRoot.rotation = Quaternion.Euler(
            camAngleX_adjusted,
            camAngle.y + mouseMoveDelta.x * sensitivity,
            camAngle.z);
    }
}
