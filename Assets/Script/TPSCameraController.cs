using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenCover.Framework.Model;
using UnityEngine;

public class TPSCameraController : MonoBehaviour
{
    [SerializeField] private PlayMoveController playerController;
    [SerializeField] private Transform cameraRoot;

    private Vector2 mouseMoveDelta = new(0, 0);
    private bool isCamLocked;

    private float camLockFindDistance = 100f;

    [SerializeField, Header("Lock On find radius")]
    private float camLockFindRadius = 5f;


    private GameObject lockedObj;

    private RaycastHit[] hits;
    private ICameraRotationCalculator camCal;
    private UnLockedCamCalculator unLockedCam;
    private LockedCamCalculator lockedCam;

    private void Start()
    {
        unLockedCam = new UnLockedCamCalculator(cameraRoot);
        lockedCam = new LockedCamCalculator(playerController.gameObject);
        camCal = unLockedCam;
    }

    void Update()
    {
        Debug.DrawRay(cameraRoot.position,
            new Vector3(cameraRoot.forward.x, 0, cameraRoot.forward.z));
        TriggerCamLock();
        ChangeCameraRotation();
    }

    private void ChangeCameraRotation()
    {
        mouseMoveDelta.x = Input.GetAxis("Mouse X");
        mouseMoveDelta.y = Input.GetAxis("Mouse Y");

        cameraRoot.rotation = camCal.SetCameraRotation(mouseMoveDelta);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void TriggerCamLock()
    {
        if (!Input.GetKeyDown(KeyCode.Alpha1)) return;
        if (isCamLocked)
        {
            isCamLocked = false;
            camCal.SetLockedObj(null);
            camCal = unLockedCam;

            playerController.CamLock(isCamLocked);
        }
        // ReSharper disable once AssignmentInConditionalExpression
        else if (lockedObj = FindObjectToLock())
        {
            isCamLocked = true;
            camCal = lockedCam;
            camCal.SetLockedObj(lockedObj);
            playerController.CamLock(isCamLocked);
        }
        else
        {
            camCal = unLockedCam;
            Debug.Log("No Enemy To Lock On");
        }
    }

    private GameObject FindObjectToLock()
    {
        // ReSharper disable once Unity.PreferNonAllocApi
        hits = Physics.SphereCastAll(cameraRoot.position, camLockFindRadius,
            new Vector3(cameraRoot.forward.x, 0, cameraRoot.forward.z),
            camLockFindDistance);
        return (from hit in hits
                where hit.transform.GetComponent<Enemy>()
                select hit.transform.GetComponent<Enemy>().transform.gameObject).FirstOrDefault();
    }
}

internal interface ICameraRotationCalculator {
    public Quaternion SetCameraRotation(Vector2 mouseMoveDelta);
    public void SetLockedObj(GameObject obj);
}

public class LockedCamCalculator : ICameraRotationCalculator {
    private GameObject lockedObj;
    private GameObject playerObj;

    public LockedCamCalculator(GameObject player) {
        playerObj = player;
    }

    public void SetLockedObj(GameObject obj) {
        lockedObj = obj;
    }

    public Quaternion SetCameraRotation(Vector2 mouseMoveDelta) {
        return Quaternion.LookRotation(lockedObj.transform.position - playerObj.transform.position);
    }
}

public class UnLockedCamCalculator : ICameraRotationCalculator {
    [SerializeField, Range(0, 10)] float sensitivity = 1f;
    [SerializeField, Range(0f, 60f)] float camAngleMaximum = 40f;
    [SerializeField, Range(270f, 361f)] float camAngleMinimum = 300f;
    private float camAngleXadjusted;

    private Transform cameraRoot;
    private Vector3 camAngle;
    
    public UnLockedCamCalculator(Transform root) {
        cameraRoot = root;
    }
    
    /// Get 'how much the mouse moved' and return 'how much the camera should move'
    /// It is executed when the Cam Locked off
    public Quaternion SetCameraRotation(Vector2 mouseMoveDelta) {
        camAngle = cameraRoot.rotation.eulerAngles;
        camAngleXadjusted = camAngle.x - mouseMoveDelta.y * sensitivity;
        camAngleXadjusted = 
            camAngleXadjusted < 180f ? Mathf.Clamp(camAngleXadjusted, -1f, camAngleMaximum) 
                : Mathf.Clamp(camAngleXadjusted, camAngleMinimum, 361f);

        return Quaternion.Euler(camAngleXadjusted, 
            camAngle.y + mouseMoveDelta.x * sensitivity, camAngle.z);
    }
    
    public void SetLockedObj(GameObject obj) { }
}
