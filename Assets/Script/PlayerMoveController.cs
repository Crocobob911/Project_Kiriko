using System;
using UnityEngine;

namespace Script
{
    public class PlayerMoveController : MonoBehaviour, IValueModifierObserver {
        // 애니메이터 매개변수들
        // Animator.StringToHash()로 그 값들을 미리 가져와 갖고있음으로써, 연산 줄여줌.
        private static readonly int AnimIsMove = Animator.StringToHash("isMove");
        private static readonly int AnimMoveForward = Animator.StringToHash("moveForward");
        private static readonly int AnimMoveRight = Animator.StringToHash("moveRight");
        private static readonly int AnimIsCamLocked = Animator.StringToHash("isCamLocked");
        
        private float moveSpeed = 5f;
        private float sprintSpeed = 3f;
        private bool isSprint;
        
        private Animator anim;
        private Transform playerBody;
        private Transform cameraRoot;
        
        private bool isMoveInput;
        private Vector2 moveInput;
        private Vector3 forwardMove;
        private Vector3 sideMove;
        private Vector3 moveDir;

        private bool isCamLocked;

        private void Awake() {
            cameraRoot = gameObject.transform.GetChild(0).GetComponent<Transform>();
            playerBody = gameObject.transform.GetChild(1).GetComponent<Transform>();
            anim = playerBody.GetComponent<Animator>();
        }
        
        private void Start() {
            ValueModifier.Instance.AddSubscriber(this);
        }

        private void Update() {
            SprintTrigger();
            Move();
        }
    
        private void Move() {
            moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); 
            isMoveInput = moveInput.magnitude != 0;
            anim.SetBool(AnimIsMove, isMoveInput);
            if (!isMoveInput) return;
        
            anim.SetFloat(AnimMoveForward, moveInput.y);
            anim.SetFloat(AnimMoveRight, moveInput.x);

            forwardMove = new Vector3(cameraRoot.forward.x, 0f, cameraRoot.forward.z).normalized;
            sideMove = new Vector3(cameraRoot.right.x, 0f, cameraRoot.right.z).normalized;
            moveDir = (forwardMove * moveInput.y + sideMove * moveInput.x).normalized;
            var diagonalMoveCorrectedSpeed = Mathf.Min(moveDir.magnitude, 1f) * moveSpeed; 
            //대각선 빨라짐 제한
            
            playerBody.forward = isCamLocked ? forwardMove : moveDir;
            transform.position += moveDir * (diagonalMoveCorrectedSpeed * Time.deltaTime);
        }

        public void CamLock(bool isLocked)
        {
            isCamLocked = isLocked;
            anim.SetBool(AnimIsCamLocked, isCamLocked);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void SprintTrigger()
        {
            if (!isMoveInput) return;        //걷고있을 때에만 달리기 트리거
            if(!Input.GetKeyDown(KeyCode.LeftShift) && !Input.GetKeyUp(KeyCode.LeftShift))  return;
            // 좌 Shift를 누를 때 , 뗄 때 => 달리기 Trigger
        
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