using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script {
    [RequireComponent(typeof(Cinemachine3rdPersonFollow))]
    public class CameraController : MonoBehaviour, IValueModifierObserver {

        private static CameraController instance = null;

        public static CameraController Instance 
            => null == instance ? null : instance;


        // -------- Mouse Move -> Rotate 관련 -----
        private PlayerMoveController playerController;
        private Transform cameraRoot;
        private ICameraRotationCalculator camCal;
        private UnLockedCamCalculator unLockedCam;
        private LockedCamCalculator lockedCam;
        
        private List<ICameraLockObserver> lockObservers = new List<ICameraLockObserver>();
        
        [SerializeField] private float mouseSensitivity = 1f; // 나중에 0.1f가 추가로 곱해짐.

        private Vector2 mouseMoveDelta;
        private bool isCamLocked;
        private GameObject lockedObj;

        private readonly float camLockFindDistance = 100f;
        [SerializeField] private float camLockFindRadius = 5f; // 록온 찾는 원기둥 지름

        // -------- Zoom 관련 ----------
        [SerializeField] private CinemachineVirtualCamera vcam;
        private Cinemachine3rdPersonFollow componentBase;
        
        [SerializeField] private float zoomMin;
        [SerializeField] private float zoomMax;
        [SerializeField] private float zoomSpeed;
        

        private void Awake() {
            if (null == instance) instance = this;
            else Destroy(gameObject);
            
            
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
            cameraRoot.rotation = camCal.SetCameraRotation(mouseMoveDelta);
        }

        public void MouseMoveAction(InputAction.CallbackContext context) {
            mouseMoveDelta = context.ReadValue<Vector2>() * mouseSensitivity * 0.1f;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [SuppressMessage("ReSharper", "AssignmentInConditionalExpression")]
        public void TriggerCamLock(InputAction.CallbackContext context) {
            if (!context.started) return;
            
            if (isCamLocked) SetCamLockOff();
            else if (lockedObj = FindObjectToLock()) SetCamLockOn();
            
            // Debug.Log("[CameraController] isCamLocked : " + isCamLocked);
        }

        private void SetCamLockOn() {
            isCamLocked = true;
            camCal = lockedCam;
            camCal.SetLockedObj(lockedObj);
            LockUpdate_forObservers(isCamLocked);
        }

        private void SetCamLockOff() {
            isCamLocked = false;
            camCal.SetLockedObj(null);
            camCal = unLockedCam;
            LockUpdate_forObservers(isCamLocked);
        }

        // RaySphere로 바라보고 있는 곳에 있는 <Enemy> GameObject를 찾아 반홤.
        private GameObject FindObjectToLock() {
            // ReSharper disable once Unity.PreferNonAllocApi
            var hits = Physics.SphereCastAll(
                    cameraRoot.position, camLockFindRadius,
                    new Vector3(cameraRoot.forward.x, 0, cameraRoot.forward.z),
                    camLockFindDistance);
            
            return (from hit in hits
                where hit.transform.GetComponent<Enemy>()
                select hit.transform.GetComponent<Enemy>().transform.gameObject).FirstOrDefault();
        }

        public void AddMeLockObserver(ICameraLockObserver observer) {
            lockObservers.Add(observer);
        }

        private void LockUpdate_forObservers(bool locked) {
            foreach (var obs in lockObservers) {
                obs.CamLockUpdate(locked);
            }
        }

        // 스크롤 드르륵 -> 카메라 줌인 줌아웃
        public void ZoomCamera(InputAction.CallbackContext context) {
            componentBase.CameraDistance 
                = Mathf.Clamp(componentBase.CameraDistance - context.ReadValue<float>() * zoomSpeed * Time.deltaTime, 
                    zoomMin, zoomMax);
        }


#if UNITY_EDITOR
        public void ValueModifierUpdated() {
            mouseSensitivity = ValueModifier.Instance.CamSensitivity;
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
        // float sensitivity = 0.2f; // 얘를 사용자가 설정할 수 있게 해야할텐데
        private float camAngleMaximum = 40f;
        private float camAngleMinimum = 300f;
        private readonly Transform cameraRoot;
        private Vector3 camAngle;

        public UnLockedCamCalculator(Transform root) {
            cameraRoot = root;
        }

        // 마우스 움직임 Vector를 Input 받아서 Camera의 이동 거리 계산
        // 마우스가 자유롭게 움직일 때 사용됨
        public Quaternion SetCameraRotation(Vector2 mouseMoveDelta) {
            var camAngleXadjusted 
                = cameraRoot.rotation.eulerAngles.x - mouseMoveDelta.y;
            
            // 카메라 위아래 각도 제한
            camAngleXadjusted = camAngleXadjusted < 180f ? 
                    Mathf.Clamp(camAngleXadjusted, -1f, camAngleMaximum)
                    : Mathf.Clamp(camAngleXadjusted, camAngleMinimum, 361f);

            return Quaternion.Euler(camAngleXadjusted,
                cameraRoot.rotation.eulerAngles.y + mouseMoveDelta.x,
                cameraRoot.rotation.eulerAngles.z);
        }
        
#if UNITY_EDITOR
        public void ValueModifierUpdated() {
            camAngleMaximum = ValueModifier.Instance.CamAngleMaximum;
            camAngleMinimum = ValueModifier.Instance.CamAngleMinimum;
        }
#endif

        public void SetLockedObj(GameObject obj) {}
    }


    public interface ICameraLockObserver {
        public void CamLockUpdate(bool locked);
    }
}