using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Serialization;

namespace Script
{
    public class PlayerMoveController : MonoBehaviour, IValueModifierObserver {

        [SerializeField] private float moveSpeed = 5f;
        private float sprintSpeed = 3f;
        private bool isSprint;
        
        private bool isJumping = false;
        private bool isMoveInputUnable = false;
        
        [SerializeField] private PlayerAnimController animController;
 
        [SerializeField] private Transform playerBody;
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private Rigidbody playerRigidbody;
        
        private bool isMoveInput;
        private Vector2 moveInputVector;
        private Vector3 cameraForward;
        private Vector3 cameraSide;

        private Vector2 moveDir;
        private Vector3 currentMovingDir;
        
        /**
         * 플레이어가 얼마나 빨리 도느냐
         * 얼마나 민감하게 input에 반응하느냐
         */
        [SerializeField] private float turnSpeed = 10f;
        
        [SerializeField] private float jumpForce = 5f;
        // Value Modifier

        private bool isCamLocked;

        private void Awake() {
            cameraRoot = gameObject.transform.GetChild(0).GetComponent<Transform>();
            playerBody = gameObject.transform.GetChild(1).GetComponent<Transform>();
            playerRigidbody = gameObject.transform.GetComponent<Rigidbody>();
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

        /**
         * WASD 인풋이 바뀔 때마다 호출. Player의 direction을 바꿔줌.
         */
        public void MoveInput(InputAction.CallbackContext context) {
            moveInputVector = context.ReadValue<Vector2>();
            
            if (isMoveInputUnable) return;
            SetMoveDirection(moveInputVector);
        }

        private void SetMoveDirection(Vector2 inputVector) {
            moveDir = inputVector;
            isMoveInput = moveInputVector != Vector2.zero;
            animController.SetMoveAnimDirection(moveDir);
        }

        public void Jump(InputAction.CallbackContext context) {
            if (!context.started || isJumping) return;

            isJumping = true;
            playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            
            animController.StartJump();
            isMoveInputUnable = true;
            
            // 도약 모션 들어가야함.
            // 체공 모션이 들어가야함.
            
            // 1. 공중에 있는 동안 Input 제한되어야.
            // 2. 땅에 착지하면 Input 다시 가능한 로직 구현해야함.
        }

        public void Land() {
            isJumping = false;
            isMoveInputUnable = false;
            animController.Land();
            SetMoveDirection(moveInputVector);
        }

        /// <summary>
        /// WASD 인풋이 눌리고 있는 동안 Player를 움직이게 해줌.
        /// Update 함수에서 호출
        /// </summary>
        private void Move() {
            if(!isMoveInput) return;
            currentMovingDir = LerpMoveDirection(
                        ChangeMoveVectorWithCamera(moveDir), currentMovingDir);

            // 카메라 잠금 상태에 따라 player 객체의 전방 변경
            // 잠김 = 록온 대상을 향해 | 안 잠김 = 이동하는 방향을 향해
            playerBody.forward = isCamLocked ? cameraForward : currentMovingDir;
            transform.position += currentMovingDir * (moveSpeed * Time.deltaTime); // 이동
        }

        /**
         * 카메라의 방향을 플레이어의 이동 방향에 반영해줌
         */
        private Vector3 ChangeMoveVectorWithCamera(Vector2 moveInput) {
            if (isMoveInputUnable) return new Vector3(moveInput.x, 0, moveInput.y);
            
            cameraForward = new Vector3(cameraRoot.forward.x, 0f, cameraRoot.forward.z);
            cameraSide = new Vector3(cameraRoot.right.x, 0f, cameraRoot.right.z);
            return cameraForward * moveInput.y + cameraSide * moveInput.x;
        }
        
        private Vector3 LerpMoveDirection(Vector3 newMoveDir, Vector3 currentMoveDir) {
            // 플레이어 이동 Lerp
            if ((newMoveDir - currentMoveDir).magnitude >= 0.001f) {
                return Vector3.Lerp(currentMoveDir, newMoveDir, turnSpeed * Time.deltaTime);
            }
            return newMoveDir;
        }

        public void SetCamLock(bool isLocked)
        {
            isCamLocked = isLocked;
            animController.SetIsCamLocked(isLocked);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void SprintTrigger(InputAction.CallbackContext context)       // 좌 Shift를 누를 때 , 뗄 때 => 달리기 Trigger
        {
            Debug.Log("Sprint Trigger Called");
            //걷고있을 때에만 달리기 트리거
            if (!isMoveInput) return;
        
            isSprint = !isSprint;
            moveSpeed = isSprint ? moveSpeed + sprintSpeed : moveSpeed - sprintSpeed;
            Debug.Log("Sprint Trigger : " + isSprint);
        
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