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
    bool isCamLocked = false;

    readonly float camLockFindDistance = 100f;
    [SerializeField, Header("Lock On 인식 너비, *부채꼴 아님.")]     
    float camLockFindRadius = 5f;  //정확하게 부채꼴 범위로 찾을 순 없을까?

    GameObject lockedObj; //록온한 오브젝트

    RaycastHit[] hits; //시점 방향의 모든 오브젝트를 다 담음   
    ICameraRotationCalculator camCal;
    UnLockedCamCalculator unLockedCam;
    LockedCamCalculator lockedCam;

    private void Start()
    {
        unLockedCam = new UnLockedCamCalculator(cameraRoot);
        lockedCam = new LockedCamCalculator(playerController.gameObject);
        camCal = unLockedCam;
        Debug.Log(camCal);
    }

    void Update()
    {
        Debug.DrawRay(cameraRoot.position, 
            new Vector3(cameraRoot.forward.x, 0, cameraRoot.forward.z));
        TriggerCamLock();
        ChangeCameraRotation();
    }

    private void ChangeCameraRotation() //Camera Lock On?? ??? ??   **** ???? ?????? if ???????...??? ?????
    {
        //???²J ???
        mouseMoveDelta.x = Input.GetAxis("Mouse X");
        mouseMoveDelta.y = Input.GetAxis("Mouse Y");

        cameraRoot.rotation = camCal.SetCameraRotation(mouseMoveDelta);
    }

    private void TriggerCamLock()  //1 ?????? Cam lock ?????
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (isCamLocked)  //Cam Lock On ????
            {
                isCamLocked = false;
                camCal.SetLockedObj(null);
                camCal = unLockedCam;

                playerController.CamLock(isCamLocked);
            }
            else if (lockedObj = FindObjectToLock()) //Enemy?? ?????????? ???
            {
                isCamLocked = true;
                camCal = lockedCam;
                camCal.SetLockedObj(lockedObj);
                playerController.CamLock(isCamLocked);
            }
            else //Enemy ?????
            {
                camCal = unLockedCam;
                Debug.Log("No Enemy To Lock On");
            }
        }
    }

    private GameObject FindObjectToLock()  //RaySphere?? ????, ???? ??? ?????? Enemy?? ???
    {
        hits = Physics.SphereCastAll(cameraRoot.position, camLockFindRadius, 
            new Vector3(cameraRoot.forward.x, 0, cameraRoot.forward.z), 
                camLockFindDistance);
            foreach (RaycastHit hit in hits) { 
            if(hit.transform.GetComponent<Enemy>()) //?? ????? Enemy?? ???? ??????? ????? ?????. ???? ?????? ??????? Enemy?? ??? ??? ??????? ???.
            {
                return hit.transform.GetComponent<Enemy>().transform.gameObject;
            }
        }
        return null;
    }
}

// ??? or ????? ??, ???? ???? ???? ??????? ???? ??? ???
interface ICameraRotationCalculator
{
    abstract public Quaternion SetCameraRotation(Vector2 mouseMoveDelta);
    abstract public void SetLockedObj(GameObject obj);
}

public class LockedCamCalculator : ICameraRotationCalculator
{
    private GameObject lockedObj;
    private GameObject playerObj;

    public LockedCamCalculator(GameObject player)
    {
        playerObj = player;
    }

    public void SetLockedObj(GameObject obj)
    {
        lockedObj = obj;
    }

    public Quaternion SetCameraRotation(Vector2 mouseMoveDelta)
    {
        return Quaternion.LookRotation(lockedObj.transform.position - playerObj.transform.position);
    }
}

public class UnLockedCamCalculator : ICameraRotationCalculator
{
    [SerializeField, Range(0, 10)] float sensitivity = 1f;  // ??­? ??? ????
    [SerializeField, Range(0f, 60f)] float camAngleMaximum = 40f;  //???? ?????? ????. ???? 60 ?????? ??!
    [SerializeField, Range(270f, 361f)] float camAngleMinimum = 300f;  //???? ?????? ????????.
    private float camAngleX_adjusted; //???? ???? ??? ???? ????

    private Transform cameraRoot;
    private Vector3 camAngle;

    public UnLockedCamCalculator(Transform root)
    {
        cameraRoot = root;
    }

    public Quaternion SetCameraRotation(Vector2 mouseMoveDelta)
    {
        camAngle = cameraRoot.rotation.eulerAngles;         //???? ????? ?????
        camAngleX_adjusted = camAngle.x - mouseMoveDelta.y * sensitivity;
        if (camAngleX_adjusted < 180f) //???? ???? ??
        {
            camAngleX_adjusted = 
            Mathf.Clamp(camAngleX_adjusted, -1f, camAngleMaximum);
        }
        else //???? ???? ???
        {
            camAngleX_adjusted = 
            Mathf.Clamp(camAngleX_adjusted, camAngleMinimum, 361f);
        }

        return Quaternion.Euler(camAngleX_adjusted, 
            camAngle.y + mouseMoveDelta.x * sensitivity, camAngle.z);
    }

    public void SetLockedObj(GameObject obj) { }  //??? ???????
}
