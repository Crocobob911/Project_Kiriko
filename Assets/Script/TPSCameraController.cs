using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCameraController : MonoBehaviour
{
    [SerializeField]
    TPSPlayerController playerController;
    [SerializeField]
    private Transform cameraRoot;

    Vector2 mouseMoveDelta = new Vector2(0, 0);  //���콺 �̵� �Է�
    Vector3 camAngle = new Vector3(0, 0, 0);  //ī�޶� ���� ��ġ
    bool isCamLocked = false;

    readonly float camLockFindDistance = 100f;
    [SerializeField, Header("Lock On �ν� �ʺ�, *��ä�� �ƴ�.")]     
    float camLockFindRadius = 5f;  //��Ȯ�ϰ� ��ä�� ������ ã�� �� ������?

    GameObject lockedObj; //�Ͽ��� ������Ʈ
    Quaternion rotToLockedObj;

    RaycastHit[] hits; //���� ������ ��� ������Ʈ�� �� ����
    ICameraLock cameraLocker;
    UnLockedCam unLockedCam;
    LockedCam lockedCam;

    private void Start()
    {
        unLockedCam = new UnLockedCam();
        lockedCam = new LockedCam();
        cameraLocker = unLockedCam;
    }

    void Update()
    {
        Debug.DrawRay(cameraRoot.position, 
            new Vector3(cameraRoot.forward.x, 0, cameraRoot.forward.z));
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
        cameraRoot.rotation = cameraLocker.SetCameraRotation(
            camAngle, mouseMoveDelta, lockedObj.transform.position, 
            playerController.gameObject.transform.position);
    }

    private void TriggerCamLock()  //1 ������ Cam lock Ʈ����
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (isCamLocked)  //Cam Lock On ����
            {
                isCamLocked = false;
                cameraLocker = unLockedCam;
                Debug.Log("Cam Lock Off : " + lockedObj);
                playerController.CamLock(isCamLocked);
            }
            else if (lockedObj = FindObjectToLock()) //Enemy�� ���������� ã��
            {
                isCamLocked = true;
                cameraLocker = lockedCam;
                Debug.Log("Cam Lock : " + lockedObj);
                playerController.CamLock(isCamLocked);
            }
            else //Enemy ��ã��
            {
                cameraLocker = unLockedCam;
                Debug.Log("No Enemy To Lock On");
            }
        }
    }

    private GameObject FindObjectToLock()  //RaySphere�� ����, �ٶ󺸰� �ִ� ���⿡�� Enemy�� ã��
    {
        hits = Physics.SphereCastAll(cameraRoot.position, camLockFindRadius, 
            new Vector3(cameraRoot.forward.x, 0, cameraRoot.forward.z), 
                camLockFindDistance);
            foreach (RaycastHit hit in hits) { 
            if(hit.transform.GetComponent<Enemy>()) //�� �߿��� Enemy�� ���� ������Ʈ �ϳ��� ���Ѵ�. ���� �Ϲ�ȭ�� �ʿ��ұ�? Enemy�� �ƴ� �ٸ� Ŭ������ ã��.
            {
                return hit.transform.GetComponent<Enemy>().transform.gameObject;
            }
        }
        return null;
    }
}

// �Ͽ� or �Ͽ��� ��, ī�޶� ���� ���� ������� ���� �ٸ� ��ü
interface ICameraLock
{
    abstract public Quaternion SetCameraRotation(
        Vector3 camAngle, Vector2 mouseMoveDelta, 
        Vector3 lockedObjPos, Vector3 playerObjPos);
}

public class LockedCam : ICameraLock
{
    public Quaternion SetCameraRotation(
        Vector3 camAngle, Vector2 mouseMoveDelta, 
        Vector3 lockedObjPos, Vector3 playerObjPos)
    {
        return Quaternion.LookRotation(lockedObjPos - playerObjPos);
    }
}

public class UnLockedCam : ICameraLock
{
    [SerializeField, Range(0, 10)] float sensitivity = 1f;  // �þ� ȸ�� ����
    [SerializeField, Range(0f, 60f)] float camAngleMaximum = 40f;  //ī�޶� ������ �ö󰡳�. ���� 60 ������ ��!
    [SerializeField, Range(270f, 361f)] float camAngleMinimum = 300f;  //ī�޶� ������ ��������.
    private float camAngleX_adjusted; //ī�޶� ���� �̵� ���� ����

    public Quaternion SetCameraRotation(
        Vector3 camAngle, Vector2 mouseMoveDelta, 
        Vector3 lockedObjPos, Vector3 playerObjPos)
    {
        camAngleX_adjusted = camAngle.x - mouseMoveDelta.y * sensitivity;
        if (camAngleX_adjusted < 180f) //ī�޶� ���� ��
        {
            camAngleX_adjusted = 
            Mathf.Clamp(camAngleX_adjusted, -1f, camAngleMaximum);
        }
        else //ī�޶� ���� �Ʒ�
        {
            camAngleX_adjusted = 
            Mathf.Clamp(camAngleX_adjusted, camAngleMinimum, 361f);
        }

        return Quaternion.Euler(camAngleX_adjusted, 
            camAngle.y + mouseMoveDelta.x * sensitivity, camAngle.z);
    }

}
