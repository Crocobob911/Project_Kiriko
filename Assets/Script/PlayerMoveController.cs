using UnityEngine.InputSystem;
using UnityEngine;

namespace Script
{
    public class PlayerMoveController : MonoBehaviour, IValueModifierObserver {

        private float moveSpeed = 5f;
        private float sprintSpeed = 3f;
        private bool isSprint;
        
        [SerializeField] private PlayerAnimController animController;
 
        [SerializeField] private Transform playerBody;
        [SerializeField] private Transform cameraRoot;
        
        private bool isMoveInput;
        private Vector2 moveInput;
        private Vector3 cameraFront;
        private Vector3 cameraSide;
        
        private Vector3 moveDir;
        private Vector3 newMoveDir; // lerp용
        
        [SerializeField] private float turnSpeed = 10f;
        // 플레이어의 방향을 바꾸면 얼마나 빨리 도느냐

        private bool isCamLocked;

        private void Awake() {
            cameraRoot = gameObject.transform.GetChild(0).GetComponent<Transform>();
            playerBody = gameObject.transform.GetChild(1).GetComponent<Transform>();
            animController = transform.GetComponent<PlayerAnimController>();
        }
        
        private void Start() {
#if UNITY_EDITOR
            ValueModifier.Instance.AddSubscriber(this);
            ValueModifierUpdated();
#endif
        }

        private void Update() {
            Move();
        }
    
        /// <summary>
        /// WASD 인풋이 바뀔 때마다 호출. Player의 direction을 바꿔줌.
        /// </summary>
        /// <param name="context"></param>
        public void MoveTrigger(InputAction.CallbackContext context) {
            moveInput = context.ReadValue<Vector2>();
            isMoveInput = !(moveInput == Vector2.zero);
            animController.setMoveDirection(moveInput);
        }

        /// <summary>
        /// WASD 인풋이 눌리고 있는 동안 Player를 움직이게 해줌.
        /// Update 함수에서 호출
        /// </summary>
        private void Move() {
            if(!isMoveInput) return;
            
            // 카메라의 방향을 플레이어의 이동 방향에 반영해줌
            cameraFront = new Vector3(cameraRoot.forward.x, 0f, cameraRoot.forward.z);
            cameraSide = new Vector3(cameraRoot.right.x, 0f, cameraRoot.right.z);
            newMoveDir = cameraFront * moveInput.y + cameraSide * moveInput.x;
            
            
            // 플레이어 이동 Lerp
            if ((newMoveDir - moveDir).magnitude >= 0.001f) {
                moveDir = Vector3.Lerp(moveDir, newMoveDir, turnSpeed * Time.deltaTime);
            }
            
            // 카메라 잠금 상태에 따라
            playerBody.forward = isCamLocked ? cameraFront : moveDir;
            
            // 이동
            transform.position += moveDir * (moveSpeed * Time.deltaTime);
        }

        public void SetCamLock(bool isLocked)
        {
            isCamLocked = isLocked;
            animController.setIsCamLocked(isLocked);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void StartTrigger()       // 좌 Shift를 누를 때 , 뗄 때 => 달리기 Trigger
        {
            //걷고있을 때에만 달리기 트리거
            if (!isMoveInput) return;
        
            isSprint = !isSprint;
            moveSpeed = isSprint ? moveSpeed + sprintSpeed : moveSpeed - sprintSpeed;
        
            //---나중에 스태미너 감소 구현해야함
            //---나중에 달리기 애니메이션 넣어야함
        }
        #if UNITY_EDITOR
        public void ValueModifierUpdated() {
            moveSpeed = ValueModifier.Instance.MoveSpeed;
            sprintSpeed = ValueModifier.Instance.SprintSpeed;
        }
        #endif
    }
}