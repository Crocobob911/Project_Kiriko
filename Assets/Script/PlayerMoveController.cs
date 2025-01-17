using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Serialization;

namespace Script
{
    public class PlayerMoveController : MonoBehaviour, IValueModifierObserver {
        
        [SerializeField] private float defaultMoveSpeed = 5f;
        private float moveSpeed;
        private float sprintSpeed = 3f;
        private bool isSprint = false;
        private bool isSprintInput = false;
        
        private bool isJumping = false;
        // private bool isMoveInputUnable = false;
        
        [SerializeField] private PlayerAnimController animController;
 
        [SerializeField] private Transform playerBody;
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private Rigidbody playerRigidbody;
        
        private bool isMoveInput;
        private Vector2 moveInputVector;
        private Vector3 cameraForward;

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
            ChangeDelegate_Idle();

#if UNITY_EDITOR
            ValueModifier.Instance.AddSubscriber(this);
            ValueModifierUpdated();
#endif
        }

        private void Update() {
            Move();
        }
        
#region Delegates Switch
        
        private void ChangeDelegate_Idle() {
            dl_moveApply = MoveApply_NotJump;
            dl_changeVectorWithCamera = ChangeMoveVectorWithCamera;
            dl_sprintApply = SprintApply_NotJump;
        }

        private void ChangeDelegate_Jump() {
            dl_moveApply = vector => { };
            dl_changeVectorWithCamera = DontChangeMoveVectorWithCamera;
            dl_sprintApply = input => { };
        }

#endregion

        /**
         * WASD 인풋이 바뀔 때마다 호출. Player의 direction을 바꿔줌.
         */
        public void MoveInput(InputAction.CallbackContext context) {
            moveInputVector = context.ReadValue<Vector2>();
            
            dl_moveApply(moveInputVector);
        }
        
#region MoveApply Delegates
        private delegate void MoveApply(Vector2 inputVector);
        private MoveApply dl_moveApply;
        
        
        private void MoveApply_NotJump(Vector2 inputVector) {
            moveDir = inputVector;
            isMoveInput = moveInputVector != Vector2.zero;
            animController.SetMoveAnimDirection(moveDir);
        }
#endregion

#region Change Vector With Camera Delegates
        
        /// <summary>
        /// 움직이는 동안, 카메라 방향에 따라 WASD의 방향이 바뀐다.
        /// 점프 중엔 적용되지 않는다.
        /// </summary>
        private delegate Vector3 AdjustMoveDir(Vector2 moveDirection);
        private AdjustMoveDir dl_changeVectorWithCamera;
                
        /**
         * 카메라의 방향을 플레이어의 이동 방향에 반영해줌
         */
        private Vector3 ChangeMoveVectorWithCamera(Vector2 moveDirection) { 
            cameraForward = new Vector3(cameraRoot.forward.x, 0f, cameraRoot.forward.z); 
            var cameraSide = new Vector3(cameraRoot.right.x, 0f, cameraRoot.right.z);
            return cameraForward * moveDirection.y + cameraSide * moveDirection.x;
        }

        private Vector3 DontChangeMoveVectorWithCamera(Vector2 moveDirection) {
            var playerForward = new Vector3(playerBody.forward.x, 0f, playerBody.forward.z);
            var playerSide = new Vector3(playerBody.right.x, 0f, playerBody.right.z); 
            return playerForward * moveDirection.y + playerSide * moveDirection.x;
        }
#endregion

        /// <summary>
        /// WASD 인풋이 눌리고 있는 동안 Player를 움직이게 해줌.
        /// Update 함수에서 호출
        /// 점프 중에는 WASD 인풋에 의한 움직임을 제한함.
        /// </summary>
        private void Move() {
            if(!isMoveInput) return;
            
            currentMovingDir = LerpMoveDirection(
                dl_changeVectorWithCamera(moveDir), currentMovingDir);
            
            // 카메라 잠금 상태에 따라 player 객체의 전방 변경
            // 잠김 = 록온 대상을 향해 | 안 잠김 = 이동하는 방향을 향해
            playerBody.forward = isCamLocked ? cameraForward : currentMovingDir;
            
            transform.position += currentMovingDir * (moveSpeed * Time.deltaTime); // 이동
        }


        public void Jump(InputAction.CallbackContext context) {
            if (!context.started || isJumping) return;

            isJumping = true;
            playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            
            animController.StartJump();
            
            ChangeDelegate_Jump();

            // 도약 모션 들어가야함.
            // 체공 모션이 들어가야함.
        }

        public void Jump_end() {
            isJumping = false;
            
            ChangeDelegate_Idle();
            
            animController.Land();
            
            dl_moveApply(moveInputVector);
            dl_sprintApply(isSprintInput);
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
        public void SprintInput(InputAction.CallbackContext context)       // 좌 Shift를 누를 때 , 뗄 때 => 달리기 Trigger
        {
            if(!isMoveInput) return;        // 걷고있을 때에만 달리기 트리거
            
            if(context.started) isSprintInput = true;
            else if (context.canceled) isSprintInput = false;
            else return;
            
            Debug.Log("SprintInput : " + isSprintInput);
            
            dl_sprintApply(isSprintInput);
        }

        
#region Sprint Apply Delegates
        
        private delegate void SprintApply(bool input);
        private SprintApply dl_sprintApply;
        
        private void SprintApply_NotJump(bool input) {
            if (isSprint == input) return;
            
            isSprint = input;
            moveSpeed = isSprint ? defaultMoveSpeed + sprintSpeed : defaultMoveSpeed;
            Debug.Log("Sprint Trigger : " + isSprint + " && moveSpeed : " + moveSpeed);
            
            //---나중에 스태미너 감소 구현해야함
            //---나중에 달리기 애니메이션 넣어야함
        }
        
#endregion


        #if UNITY_EDITOR
        public void ValueModifierUpdated() {
            moveSpeed = ValueModifier.Instance.MoveSpeed;
            sprintSpeed = ValueModifier.Instance.SprintSpeed;
        }
        #endif
    }
}