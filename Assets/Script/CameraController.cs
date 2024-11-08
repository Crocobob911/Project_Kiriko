using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace Script {
    [RequireComponent(typeof(Cinemachine3rdPersonFollow))]
    public class CameraController : MonoBehaviour, IValueModifierObserver {
        // -------- Mouse Move -> Rotate 관련 -----
        private PlayerMoveController playerController;
        private Transform cameraRoot;
        private ICameraRotationCalculator camCal;
        private UnLockedCamCalculator unLockedCam;
        private LockedCamCalculator lockedCam;

        private Vector2 mouseMoveDelta = new(0, 0);
        private bool isCamLocked;
        private GameObject lockedObj;

        private readonly float camLockFindDistance = 100f;
        [SerializeField] private float camLockFindRadius = 5f; // 록온 찾는 원기둥 지름
        
        // -------- Zoom 관련 ----------
        [SerializeField] private CinemachineVirtualCamera vcam;
        private Cinemachine3rdPersonFollow componentBase;
        private float mouseScrollAmount;
        private float zoomDistance;
        [SerializeField] private float zoomMin;
        [SerializeField] private float zoomMax;
        [SerializeField] private float zoomSpeed;
        

        private void Awake() {
            cameraRoot = gameObject.transform;
            componentBase = vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            playerController = cameraRoot.transform.GetComponentInParent<PlayerMoveController>();
        }

        private void Start() {
            unLockedCam = new UnLockedCamCalculator(cameraRoot);
            lockedCam = new LockedCamCalculator(playerController.gameObject);
            camCal = unLockedCam;
#if UNITY_EDITOR
            ValueModifier.Instance.AddSubscriber(this);
            ValueModifierUpdated();
#endif
        }

        private void Update() {
            Debug.DrawRay(cameraRoot.position,
                new Vector3(cameraRoot.forward.x, 0, cameraRoot.forward.z));
            TriggerCamLock();
            ChangeCameraRotation();
            ZoomCamera();
        }

        private void ChangeCameraRotation() {
            mouseMoveDelta.x = Input.GetAxis("Mouse X");
            mouseMoveDelta.y = Input.GetAxis("Mouse Y");

            cameraRoot.rotation = camCal.SetCameraRotation(mouseMoveDelta);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void TriggerCamLock() {
            if (!Input.GetKeyDown(KeyCode.Alpha1)) return;
            if (isCamLocked) {
                isCamLocked = false;
                camCal.SetLockedObj(null);
                camCal = unLockedCam;

                playerController.CamLock(isCamLocked);
            }
            // ReSharper disable once AssignmentInConditionalExpression
            else if (lockedObj = FindObjectToLock()) {
                isCamLocked = true;
                camCal = lockedCam;
                camCal.SetLockedObj(lockedObj);
                playerController.CamLock(isCamLocked);
            }
            else {
                camCal = unLockedCam;
                Debug.Log("No Enemy To Lock On");
            }
        }

        // RaySphere로 바라보고 있는 곳에 있는 <Enemy> GameObject를 찾아 반홤.
        private GameObject FindObjectToLock() {
            // ReSharper disable once Unity.PreferNonAllocApi
            var hits = Physics.SphereCastAll(cameraRoot.position, camLockFindRadius,
                new Vector3(cameraRoot.forward.x, 0, cameraRoot.forward.z),
                camLockFindDistance);
            return (from hit in hits
                where hit.transform.GetComponent<Enemy>()
                select hit.transform.GetComponent<Enemy>().transform.gameObject).FirstOrDefault();
        }

        // 스크롤 드르륵 -> 카메라 줌인 줌아웃
        private void ZoomCamera() {
            if (Input.GetAxis("Mouse ScrollWheel") == 0) return;
            mouseScrollAmount = Input.GetAxis("Mouse ScrollWheel");

            zoomDistance = componentBase.CameraDistance - mouseScrollAmount * zoomSpeed * Time.deltaTime;
            zoomDistance = Mathf.Clamp(zoomDistance, zoomMin, zoomMax);
            componentBase.CameraDistance = zoomDistance;
        }


#if UNITY_EDITOR
        public void ValueModifierUpdated() {
            zoomSpeed = ValueModifier.Instance.ZoomSpeed;
            zoomMin = ValueModifier.Instance.ZoomMin;
            zoomMax = ValueModifier.Instance.ZoomMax;
        }
#endif
    }
    
    // 카메라 록온, 록오프를 " 전략 패턴 " 으로 구현
    internal interface ICameraRotationCalculator {
        public Quaternion SetCameraRotation(Vector2 mouseMoveDelta);
        public void SetLockedObj(GameObject obj);
    }

    // 카메라가 록온되었을 때 카메라의 각도를 계산해줄 오브젝트
    public class LockedCamCalculator : ICameraRotationCalculator {
        private GameObject lockedObj;
        private readonly GameObject playerObj;

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


    // 카메라가 자유로울 때 카메라의 각도를 계산해줄 오브젝트
    public class UnLockedCamCalculator : ICameraRotationCalculator, IValueModifierObserver {
        float sensitivity = 1f;
        float camAngleMaximum = 40f;
        float camAngleMinimum = 300f;
        private float camAngleXadjusted;
        private readonly Transform cameraRoot;
        private Vector3 camAngle;

        public UnLockedCamCalculator(Transform root) {
            cameraRoot = root;
        }

        // 마우스 움직임 Vector를 Input 받아서 Camera의 이동 거리 계산
        // 마우스가 자유롭게 움직일 때 사용됨
        public Quaternion SetCameraRotation(Vector2 mouseMoveDelta) {
            camAngle = cameraRoot.rotation.eulerAngles;
            camAngleXadjusted = camAngle.x - mouseMoveDelta.y * sensitivity;
            camAngleXadjusted =
                camAngleXadjusted < 180f
                    ? Mathf.Clamp(camAngleXadjusted, -1f, camAngleMaximum)
                    : Mathf.Clamp(camAngleXadjusted, camAngleMinimum, 361f);

            return Quaternion.Euler(camAngleXadjusted,
                camAngle.y + mouseMoveDelta.x * sensitivity, camAngle.z);
        }
#if UNITY_EDITOR
        public void ValueModifierUpdated() {
            sensitivity = ValueModifier.Instance.CamSensitivity;
            camAngleMaximum = ValueModifier.Instance.CamAngleMaximum;
            camAngleMinimum = ValueModifier.Instance.CamAngleMinimum;
        }
#endif

        public void SetLockedObj(GameObject obj) { }
    }
}