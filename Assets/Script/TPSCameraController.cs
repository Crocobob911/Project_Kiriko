using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCameraController : MonoBehaviour
{
    [SerializeField]
    TPSPlayerController playerController;
    [SerializeField]
    private Transform cameraRoot;

    [SerializeField, Range(0, 10)]  float sensitivity = 1f;  // �þ� ȸ�� ����
    [SerializeField, Range(0f, 60f)]    float camAngleMaximum;  //ī�޶� ������ �ö󰡳�. ���� 60 ������ ��!
    [SerializeField, Range(270f, 361f)]     float camAngleMinimum;  //ī�޶� ������ ��������.

    Vector2 mouseMoveDelta = new Vector2(0, 0);  //���콺 �̵� �Է�
    Vector3 camAngle = new Vector3(0, 0, 0);  //ī�޶� ���� ��ġ
    float camAngleX_adjusted; //ī�޶� ���� �̵� ���� ����

    readonly float camLockFindDistance = 100f;
    [SerializeField, Header("Lock On �ν� �ʺ�, *��ä�� �ƴ�.")]     float camLockFindRadius = 5f;  //��Ȯ�ϰ� ��ä�� ������ ã�� �� ������?

    bool isCamLocked = false;
    GameObject lockedObj; //�Ͽ��� ������Ʈ
    Quaternion rotToLockedObj;

    RaycastHit[] hits; //���� ������ ��� ������Ʈ�� �� ����

    void Update()
    {
        Debug.DrawRay(cameraRoot.position, new Vector3(cameraRoot.forward.x, 0, cameraRoot.forward.z));
        TriggerCamLock();
        ChangeCameraRotation();
    }

    private void ChangeCameraRotation() //Camera Lock On�� �ƴ� ��   **** ���� ������ if �����ѵ�...��� ����?
    {
        //���콺 �Է�
        mouseMoveDelta.x = Input.GetAxis("Mouse X");
        mouseMoveDelta.y = Input.GetAxis("Mouse Y");

        //ī�޶� ��ġ�� �����
        camAngle = cameraRoot.rotation.eulerAngles;
        if (!isCamLocked)        //Lock Off�� �� ī�޶� ������
        {  
           //ī�޶� �̵� ���� ����
            camAngleX_adjusted = camAngle.x - mouseMoveDelta.y * sensitivity;
            if (camAngleX_adjusted < 180f) //ī�޶� ���� ��
            {
                camAngleX_adjusted = Mathf.Clamp(camAngleX_adjusted, -1f, camAngleMaximum);
            }
            else //ī�޶� ���� �Ʒ�
            {
                camAngleX_adjusted = Mathf.Clamp(camAngleX_adjusted, camAngleMinimum, 361f);
            }
            cameraRoot.rotation = Quaternion.Euler(
            camAngleX_adjusted,
            camAngle.y + mouseMoveDelta.x * sensitivity,
            camAngle.z);
        }
        else
        {  //Lock On�� �� ī�޶� ������       => ���Ʒ� ������ �����ο����� ���ڴµ�?
            rotToLockedObj =
            Quaternion.LookRotation(lockedObj.transform.position - playerController.gameObject.transform.position);
            cameraRoot.rotation = rotToLockedObj;
        }    
    }

    private void TriggerCamLock()  //1 ������ Cam lock Ʈ����
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (isCamLocked)  //Cam Lock On ����
            {
                isCamLocked = false;
                Debug.Log("Cam Lock Off : " + lockedObj);
                playerController.CamLock(isCamLocked);
            }
            else if (lockedObj = FindObjectToLock()) //Enemy�� ���������� ã��
            {
                isCamLocked = !isCamLocked;
                Debug.Log("Cam Lock to : " + lockedObj);
                playerController.CamLock(isCamLocked);
            }
            else //Enemy ��ã��
            {
                Debug.Log("No Enemy To Lock On");
            }
        }
    }

    private GameObject FindObjectToLock()  //RaySphere�� ����, �ٶ󺸰� �ִ� ���⿡�� Enemy�� ã��
    {
        hits = Physics.SphereCastAll(cameraRoot.position, camLockFindRadius, 
            new Vector3(cameraRoot.forward.x, 0, cameraRoot.forward.z), camLockFindDistance);
            foreach (RaycastHit hit in hits) { 
            if(hit.transform.GetComponent<Enemy>()) //�� �߿��� Enemy�� ���� ������Ʈ �ϳ��� ���Ѵ�. ���� �Ϲ�ȭ�� �ʿ��ұ�? Enemy�� �ƴ� �ٸ� Ŭ������ ã��.
            {
                return hit.transform.GetComponent<Enemy>().transform.gameObject;
            }
        }
        return null;
    }
}
