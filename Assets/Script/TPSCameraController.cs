using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCameraController : MonoBehaviour
{
    [SerializeField]
    TPSPlayerController playerController;
    [SerializeField]
    private Transform cameraRoot;
    [SerializeField]
    GameObject playerEyeTrack;

    [SerializeField, Range(0, 10)]
    float sensitivity = 1f;  // �þ� ȸ�� ����
    [SerializeField, Range(0f, 60f)]
    private float camAngleMaximum;  //ī�޶� ������ �ö󰡳�. ���� 60 ������ ��!
    [SerializeField, Range(270f, 361f)]
    float camAngleMinimum;  //ī�޶� ������ ��������.

    Vector2 mouseMoveDelta = new Vector2(0, 0);  //���콺 �̵� �Է�
    Vector3 camAngle = new Vector3(0, 0, 0);  //ī�޶� ���� ��ġ
    float camAngleX_adjusted; //ī�޶� ���� �̵� ���� ����
    bool isCamLocked = false;


    void Update()
    {
        LookAround();
        CamLockOn();
    }

    private void LookAround()
    {
        //���콺 �Է�
        mouseMoveDelta.x = Input.GetAxis("Mouse X");
        mouseMoveDelta.y = Input.GetAxis("Mouse Y");

        //ī�޶� ��ġ�� �����
        camAngle = cameraRoot.rotation.eulerAngles;

        //ī�޶� �̵� ���� ����
        camAngleX_adjusted = camAngle.x - mouseMoveDelta.y * sensitivity;
        if (camAngleX_adjusted < 180f) //ī�޶� ���� ��
            camAngleX_adjusted = Mathf.Clamp(camAngleX_adjusted, -1f, camAngleMaximum);
        else //ī�޶� ���� �Ʒ�
            camAngleX_adjusted = Mathf.Clamp(camAngleX_adjusted, camAngleMinimum, 361f);

        //ī�޶� ��ġ ����
        cameraRoot.rotation = Quaternion.Euler(
            camAngleX_adjusted,
            camAngle.y + mouseMoveDelta.x * sensitivity,
            camAngle.z);
    }


    private void CamLockOn()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isCamLocked = !isCamLocked;
            playerController.IsCamLocked = isCamLocked;
            Debug.Log("isCamLockOn = " + isCamLocked);
            if (isCamLocked)
            {
                playerEyeTrack.SetActive(true);
                //Debug.Log()
                playerEyeTrack.SetActive(false);
            }
        }
    }
}
