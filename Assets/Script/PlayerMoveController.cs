using System;
using UnityEngine;

namespace Script
{
    public class PlayerMoveController : MonoBehaviour, IValueModifierObserver {
        private static readonly int IsMove = Animator.StringToHash("isMove");
        private static readonly int MoveForward = Animator.StringToHash("moveForward");
        private static readonly int MoveRight = Animator.StringToHash("moveRight");
        private static readonly int IsCamLocked = Animator.StringToHash("isCamLocked");
        
        private float moveSpeed = 5f;
        private float runSpeed = 3f;
        private bool isRun;
        
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
            ValueModifierForDebug.Instance.AddThisSubscriber(this);
        }

        private void Update() {
            RunTrigger();
            Move();
        }
    
        private void Move() {
            moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); 
            isMoveInput = moveInput.magnitude != 0;
            anim.SetBool(IsMove, isMoveInput);
            if (!isMoveInput) return;
        
            anim.SetFloat(MoveForward, moveInput.y);
            anim.SetFloat(MoveRight, moveInput.x);

            forwardMove = new Vector3(cameraRoot.forward.x, 0f, cameraRoot.forward.z).normalized;
            sideMove = new Vector3(cameraRoot.right.x, 0f, cameraRoot.right.z).normalized;
            moveDir = (forwardMove * moveInput.y + sideMove * moveInput.x).normalized;
            var diagonalMoveCorrectedSpeed = Mathf.Min(moveDir.magnitude, 1f) * moveSpeed; 
        
            //no fast diagonal
            
            playerBody.forward = isCamLocked ? forwardMove : moveDir;
            transform.position += moveDir * (diagonalMoveCorrectedSpeed * Time.deltaTime);
        }

        public void CamLock(bool isLocked)
        {
            isCamLocked = isLocked;
            anim.SetBool(IsCamLocked, isCamLocked);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void RunTrigger()
        {
            if (!isMoveInput) return;        //only when walking, can trigger run.
            if(!Input.GetKeyDown(KeyCode.LeftShift) && !Input.GetKeyUp(KeyCode.LeftShift))  return;
            //when Left shift keyDown, or keyUp -> trigger run
        
            isRun = !isRun;
            moveSpeed = isRun ? moveSpeed + runSpeed : moveSpeed - runSpeed;
            //Debug.Log("RunTriggerd : " + isRun);
        
            //---should implement Stamina reduce
            //---should implement Animation Change
        
        }
        
        public void ValueModified_Debug() {
            moveSpeed = ValueModifierForDebug.Instance.MoveSpeed;
            runSpeed = ValueModifierForDebug.Instance.RunSpeed;
        }
    }
}