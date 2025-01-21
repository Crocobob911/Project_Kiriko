using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Serialization;

namespace Script {
    public class PlayerMoveController : MonoBehaviour, IValueModifierObserver {

        #region Fields

        // private IPlayerMoveState[] states;

        [SerializeField] private PlayerAnimController animController;
        [SerializeField] private Transform playerBody;
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private Rigidbody playerRigidbody;

        [SerializeField] private float defaultMoveSpeed = 5f;
        private float moveSpeed;
        private bool isMoveInput;
        private Vector2 inputMoveVector;
        private Vector2 moveDir;
        private Vector3 currentMovingDir;
        [SerializeField] private float turnSpeed = 10f;
        
        [SerializeField] private float avoidSpeed;
        // value Modifier
        [SerializeField] private float avoidInputUnableTime = 0.6f;
        // value Modifier
        
        [SerializeField] private float knockBackInputUnableTime = 0.6f;
        
        
        [SerializeField] private float jumpForce = 5f;
        // value modifier

        private float sprintSpeed = 3f;
        private bool isSprint = false;
        private bool isSprintInput = false;

        private bool isCamLocked;

        #endregion

        private void Awake() {
            cameraRoot = gameObject.transform.GetChild(0).GetComponent<Transform>();
            playerBody = gameObject.transform.GetChild(1).GetComponent<Transform>();
            playerRigidbody = gameObject.transform.GetComponent<Rigidbody>();
            animController = transform.GetComponent<PlayerAnimController>();
        }

        private void Start() {
            // Debug.Log("Start");
            ChangeDelegate_Inputable();

#if UNITY_EDITOR
            ValueModifier.Instance.AddSubscriber(this);
            ValueModifierUpdated();
#endif
        }

        private void Update() {
            dl_move();
        }

        #region Delegates Switch

        //==============================================================
        private void ChangeDelegate_Inputable() {
            // Debug.Log("Inputable");
            dl_moveApply = MoveApply_Inputable;
            dl_move = Move_Idle;
            dl_CalculMoveDir = ChangeMoveVectorWithCamera;
            dl_sprintApply = SprintApply_Inputable;
            
            dl_moveApply(inputMoveVector);
        }

        private void ChangeDelegate_InputUnable() {
            // Debug.Log("Input Unable");
            dl_moveApply = vector => { };
            dl_CalculMoveDir = vector => currentMovingDir;
            dl_sprintApply = input => { };
        }

        //==============================================================

        #endregion

        #region Move
        //==============================================================
        /**
         * WASD 인풋이 바뀔 때마다 호출. Player의 direction을 바꿔줌.
         */
        public void MoveInput(InputAction.CallbackContext context) {
            inputMoveVector = context.ReadValue<Vector2>();
            dl_moveApply(inputMoveVector);
        }

        #region MoveApply Delegates

        //--------------------------------------------------------------
        private delegate void MoveApply(Vector2 inputVector);

        private MoveApply dl_moveApply;

        private void MoveApply_Inputable(Vector2 inputVector) {
            moveDir = inputVector;
            isMoveInput = inputVector != Vector2.zero;
            animController.SetMoveAnimDirection(moveDir);
        }

        //--------------------------------------------------------------

        #endregion

        #region Move
        //--------------------------------------------------------------
        private delegate void Move();

        private Move dl_move;

        /// <summary>
        /// WASD 인풋이 눌리고 있는 동안 Player를 움직이게 해줌.
        /// Update 함수에서 호출.
        /// 점프 중에는 WASD 인풋에 의한 움직임을 제한함.
        /// </summary>
        private void Move_Idle() {
            if (!isMoveInput) return;

            currentMovingDir = dl_CalculMoveDir(moveDir);

            // 카메라 잠금 상태에 따라 player 객체의 전방 변경
            // 잠김 = 록온 대상을 향해 | 안 잠김 = 이동하는 방향을 향해
            playerBody.forward = isCamLocked ? 
                new Vector3(cameraRoot.forward.x, 0f, cameraRoot.forward.z) 
                : currentMovingDir;

            transform.position += currentMovingDir * (moveSpeed * Time.deltaTime); // 이동
        }

        private void Move_KnockBack() {
            // 와 개 조잡한데 이거 맞나
            transform.position -= currentMovingDir * ((moveSpeed - 2f) * Time.deltaTime);
        }
        
        private void Move_Avoid() {
            transform.position += currentMovingDir.normalized * (avoidSpeed * Time.deltaTime);
        }

        private void Move_Avoid_Backward() {
            // Debug.Log("Move_Avoid_Backward");
            transform.position -= currentMovingDir.normalized * (avoidSpeed * Time.deltaTime);
        }
        
        //--------------------------------------------------------------

        #endregion

        #region Change Vector With Camera Delegates
        //--------------------------------------------------------------

        /// <summary>
        /// 움직이는 동안, 카메라 방향에 따라 WASD의 방향이 바뀐다.
        /// 점프 중엔 적용되지 않는다.
        /// </summary>
        private delegate Vector3 CalculateMoveDir(Vector2 moveDirection);

        private CalculateMoveDir dl_CalculMoveDir;

        /**
         * 카메라의 방향을 플레이어의 이동 방향에 반영해줌
         */
        private Vector3 ChangeMoveVectorWithCamera(Vector2 moveDirection) {
            var dir = 
                new Vector3(cameraRoot.forward.x, 0f, cameraRoot.forward.z) * moveDirection.y +
                new Vector3(cameraRoot.right.x, 0f, cameraRoot.right.z) * moveDirection.x;

            return LerpMoveDirection(dir, currentMovingDir);
        }

        private Vector3 LerpMoveDirection(Vector3 newMoveDir, Vector3 currentMoveDir) {
            // 플레이어 이동 Lerp
            return (newMoveDir - currentMoveDir).magnitude >= 0.001f
                ? Vector3.Lerp(currentMoveDir, newMoveDir, turnSpeed * Time.deltaTime)
                : newMoveDir;
        }

        //--------------------------------------------------------------

        #endregion

        //==============================================================

        #endregion

        #region Jump
        //==============================================================
        public void Jump() {
            playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

            ChangeDelegate_InputUnable();
            // 도약 모션 들어가야함.
            // 체공 모션이 들어가야함.
        }

        public void Jump_End() {
            Debug.Log("Jump End");
            ChangeDelegate_Inputable();
            dl_moveApply(inputMoveVector);
            dl_sprintApply(isSprintInput);
        }

        //==============================================================

        #endregion

        #region Sprint

        //==============================================================
        // ReSharper disable Unity.PerformanceAnalysis
        public void SprintInput(InputAction.CallbackContext context) // 좌 Shift를 누를 때 , 뗄 때 => 달리기 Trigger
        {
            if (!isMoveInput) return; // 걷고있을 때에만 달리기 트리거

            if (context.started) isSprintInput = true;
            else if (context.canceled) isSprintInput = false;
            else return;

            Debug.Log("SprintInput : " + isSprintInput);

            dl_sprintApply(isSprintInput);
        }

        #region Sprint Apply Delegates

        //--------------------------------------------------------------
        private delegate void SprintApply(bool input);

        private SprintApply dl_sprintApply;
        

        private void SprintApply_Inputable(bool input) {
            if (isSprint == input) return;

            isSprint = input;
            moveSpeed = isSprint ? defaultMoveSpeed + sprintSpeed : defaultMoveSpeed;
            Debug.Log("Sprint Trigger : " + isSprint + " && moveSpeed : " + moveSpeed);

            //---나중에 스태미너 감소 구현해야함
            //---나중에 달리기 애니메이션 넣어야함
        }

        //--------------------------------------------------------------

        #endregion

        //==============================================================

        #endregion

        #region KnockBack
        public void KnockBack_Start() {
            ChangeDelegate_InputUnable();
            dl_move = Move_KnockBack;

            Invoke(nameof(KnockBack_End), knockBackInputUnableTime);

            // 넉백 중엔 무적 판정이 있어야하진 않을까?
        }

        private void KnockBack_End() {
            Debug.Log("KnockBack End");
            ChangeDelegate_Inputable();
        }
        #endregion
        
        #region Avoid
        
        public void Avoid_Start() {
            ChangeDelegate_InputUnable();
            
            if (inputMoveVector != Vector2.zero) dl_move = Move_Avoid;
            else dl_move = Move_Avoid_Backward;
            
            Invoke(nameof(Avoid_End), avoidInputUnableTime);
        }

        public void Avoid_End() {
            ChangeDelegate_Inputable();
            
            dl_moveApply(inputMoveVector);
            dl_sprintApply(isSprintInput);
        }
        

        #endregion

        public void SetCamLock(bool isLocked) {
            isCamLocked = isLocked;
            animController.SetIsCamLocked(isLocked);
        }


#if UNITY_EDITOR
        public void ValueModifierUpdated() {
            moveSpeed = ValueModifier.Instance.MoveSpeed;
            sprintSpeed = ValueModifier.Instance.SprintSpeed;
        }
#endif
    }
}