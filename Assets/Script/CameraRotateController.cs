using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script
{
    [RequireComponent(typeof(Cinemachine3rdPersonFollow))]
    public class CameraRotateController : MonoBehaviour, IValueModifierObserver
    {

        private static CameraRotateController instance = null;

        public static CameraRotateController Instance
            => null == instance ? null : instance;


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



        private void Awake()
        {
            if (null == instance) instance = this;
            else Destroy(gameObject);

            cameraRoot = gameObject.transform;
            playerController = cameraRoot.transform.GetComponentInParent<PlayerMoveController>();
        }

        private void Start()
        {
            unLockedCam = new UnLockedCamCalculator(cameraRoot);
            lockedCam = new LockedCamCalculator(playerController.gameObject);
            camCal = unLockedCam;

#if UNITY_EDITOR
            ValueModifier.Instance.AddSubscriber(this);
            ValueModifierUpdated();
#endif
        }

        private void Update()
        {
            cameraRoot.rotation = camCal.SetCameraRotation(mouseMoveDelta);
        }

        public void MouseMoveAction(InputAction.CallbackContext context)
        {
            mouseMoveDelta = context.ReadValue<Vector2>() * mouseSensitivity * 0.1f;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [SuppressMessage("ReSharper", "AssignmentInConditionalExpression")]
        public void TriggerCamLock(InputAction.CallbackContext context)
        {
            if (!context.started) return;

            if (isCamLocked) SetCamLockOff();
            else if (lockedObj = FindObjectToLock()) SetCamLockOn();

            // Debug.Log("[CameraController] isCamLocked : " + isCamLocked);
        }

        private void SetCamLockOn()
        {
            isCamLocked = true;
            camCal = lockedCam;
            camCal.SetLockedObj(lockedObj);
            LockUpdate_forObservers(isCamLocked);
        }

        private void SetCamLockOff()
        {
            isCamLocked = false;
            camCal.SetLockedObj(null);
            camCal = unLockedCam;
            LockUpdate_forObservers(isCamLocked);
        }

        private GameObject FindObjectToLock()
        {
            // ReSharper disable once Unity.PreferNonAllocApi
            var hits = Physics.SphereCastAll(
                cameraRoot.position, camLockFindRadius,
                new Vector3(cameraRoot.forward.x, 0, cameraRoot.forward.z),
                camLockFindDistance);

            return (from hit in hits
                where hit.transform.GetComponent<Enemy>()
                select hit.transform.GetComponent<Enemy>().transform.gameObject).FirstOrDefault();
        }

        public void AddMeLockObserver(ICameraLockObserver observer)
        {
            lockObservers.Add(observer);
        }

        private void LockUpdate_forObservers(bool locked)
        {
            foreach (var obs in lockObservers)
            {
                obs.CamLockUpdate(locked);
            }
        }


#if UNITY_EDITOR
        public void ValueModifierUpdated()
        {
            mouseSensitivity = ValueModifier.Instance.CamSensitivity;
        }
#endif
    }

    // 카메라 록온, 록오프를 " 전략 패턴 " 으로 구현
    internal interface ICameraRotationCalculator
    {
        public Quaternion SetCameraRotation(Vector2 mouseMoveDelta);
        public void SetLockedObj(GameObject obj);
    }

    // 카메라가 록온되었을 때 카메라의 각도를 계산해줄 오브젝트
    public class LockedCamCalculator : ICameraRotationCalculator
    {
        private GameObject lockedObj;
        private readonly GameObject playerObj;

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


    // 카메라가 자유로울 때 카메라의 각도를 계산해줄 오브젝트
    public class UnLockedCamCalculator : ICameraRotationCalculator, IValueModifierObserver
    {
        // float sensitivity = 0.2f; // 얘를 사용자가 설정할 수 있게 해야할텐데
        private float camAngleMaximum = 40f;
        private float camAngleMinimum = 300f;
        private readonly Transform cameraRoot;
        private Vector3 camAngle;

        public UnLockedCamCalculator(Transform root)
        {
            cameraRoot = root;
        }

        // 마우스 움직임 Vector를 Input 받아서 Camera의 이동 거리 계산
        // 마우스가 자유롭게 움직일 때 사용됨
        public Quaternion SetCameraRotation(Vector2 mouseMoveDelta)
        {
            var camAngleXadjusted
                = cameraRoot.rotation.eulerAngles.x - mouseMoveDelta.y;

            // 카메라 위아래 각도 제한
            camAngleXadjusted = camAngleXadjusted < 180f
                ? Mathf.Clamp(camAngleXadjusted, -1f, camAngleMaximum)
                : Mathf.Clamp(camAngleXadjusted, camAngleMinimum, 361f);

            return Quaternion.Euler(camAngleXadjusted,
                cameraRoot.rotation.eulerAngles.y + mouseMoveDelta.x,
                cameraRoot.rotation.eulerAngles.z);
        }

#if UNITY_EDITOR
        public void ValueModifierUpdated()
        {
            camAngleMaximum = ValueModifier.Instance.CamAngleMaximum;
            camAngleMinimum = ValueModifier.Instance.CamAngleMinimum;
        }
#endif

        public void SetLockedObj(GameObject obj)
        {
        }
    }


    /// <summary>
    /// 카메라의 록온 상태 변경을 구독(Observe)하는 객체들을 위한 옵저버 패턴 인터페이스입니다.
    /// </summary>
    public interface ICameraLockObserver
    {
        public void CamLockUpdate(bool locked);
    }
}