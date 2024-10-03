using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCameraController : MonoBehaviour
{
    [SerializeField]
    TPSPlayerController playerController;
    [SerializeField]
    private Transform cameraRoot;

    Vector2 mouseMoveDelta = new Vector2(0, 0);  //마우스 이동 입력
    Vector3 camAngle = new Vector3(0, 0, 0);  //카메라 최종 위치
    bool isCamLocked = false;

    readonly float camLockFindDistance = 100f;
    [SerializeField, Header("Lock On 인식 너비, *부채꼴 아님.")]     
    float camLockFindRadius = 5f;  //정확하게 부채꼴 범위로 찾을 순 없을까?

    GameObject lockedObj; //록온한 오브젝트
    Quaternion rotToLockedObj;

    RaycastHit[] hits; //시점 방향의 모든 오브젝트를 다 담음
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

    private void ChangeCameraRotation() //Camera Lock On이 아닐 때   **** 여기 무지성 if 불편한데...방법 없나?
    {
        //마우스 입력
        mouseMoveDelta.x = Input.GetAxis("Mouse X");
        mouseMoveDelta.y = Input.GetAxis("Mouse Y");

        //카메라 위치값 선계산
        camAngle = cameraRoot.rotation.eulerAngles;
        cameraRoot.rotation = cameraLocker.SetCameraRotation(
            camAngle, mouseMoveDelta, lockedObj.transform.position, 
            playerController.gameObject.transform.position);
    }

    private void TriggerCamLock()  //1 누르면 Cam lock 트리거
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (isCamLocked)  //Cam Lock On 해제
            {
                isCamLocked = false;
                cameraLocker = unLockedCam;
                Debug.Log("Cam Lock Off : " + lockedObj);
                playerController.CamLock(isCamLocked);
            }
            else if (lockedObj = FindObjectToLock()) //Enemy를 성공적으로 찾음
            {
                isCamLocked = true;
                cameraLocker = lockedCam;
                Debug.Log("Cam Lock : " + lockedObj);
                playerController.CamLock(isCamLocked);
            }
            else //Enemy 못찾음
            {
                cameraLocker = unLockedCam;
                Debug.Log("No Enemy To Lock On");
            }
        }
    }

    private GameObject FindObjectToLock()  //RaySphere를 쏴서, 바라보고 있는 방향에서 Enemy를 찾는
    {
        hits = Physics.SphereCastAll(cameraRoot.position, camLockFindRadius, 
            new Vector3(cameraRoot.forward.x, 0, cameraRoot.forward.z), 
                camLockFindDistance);
            foreach (RaycastHit hit in hits) { 
            if(hit.transform.GetComponent<Enemy>()) //이 중에서 Enemy가 붙은 오브젝트 하나만 취한다. 추후 일반화가 필요할까? Enemy가 아닌 다른 클래스를 찾기.
            {
                return hit.transform.GetComponent<Enemy>().transform.gameObject;
            }
        }
        return null;
    }
}

// 록온 or 록오프 시, 카메라 방향 값을 계산해줄 서로 다른 객체
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
    [SerializeField, Range(0, 10)] float sensitivity = 1f;  // 시야 회전 감도
    [SerializeField, Range(0f, 60f)] float camAngleMaximum = 40f;  //카메라가 어디까지 올라가나. 값이 60 이하일 것!
    [SerializeField, Range(270f, 361f)] float camAngleMinimum = 300f;  //카메라가 어디까지 내려가나.
    private float camAngleX_adjusted; //카메라 수직 이동 범위 제한

    public Quaternion SetCameraRotation(
        Vector3 camAngle, Vector2 mouseMoveDelta, 
        Vector3 lockedObjPos, Vector3 playerObjPos)
    {
        camAngleX_adjusted = camAngle.x - mouseMoveDelta.y * sensitivity;
        if (camAngleX_adjusted < 180f) //카메라가 수평선 위
        {
            camAngleX_adjusted = 
            Mathf.Clamp(camAngleX_adjusted, -1f, camAngleMaximum);
        }
        else //카메라가 수평선 아래
        {
            camAngleX_adjusted = 
            Mathf.Clamp(camAngleX_adjusted, camAngleMinimum, 361f);
        }

        return Quaternion.Euler(camAngleX_adjusted, 
            camAngle.y + mouseMoveDelta.x * sensitivity, camAngle.z);
    }

}
