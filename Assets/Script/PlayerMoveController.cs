using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour {
    private static readonly int IsMove = Animator.StringToHash("isMove");
    private static readonly int MoveForward = Animator.StringToHash("moveForward");
    private static readonly int MoveRight = Animator.StringToHash("moveRight");
    private static readonly int IsCamLocked = Animator.StringToHash("isCamLocked");

    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int runSpeed;

    private bool isMoveInput;
    private Vector2 moveInput;
    private Vector3 forwardMove;
    private Vector3 sideMove;
    private Vector3 moveDir;

    private bool isCamLocked;
    private bool isRun;

    private Animator anim;

    private void Start() {
        anim = playerBody.GetComponent<Animator>();
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
        Debug.Log("RunTriggerd : " + isRun);
        
        //---should implement Stamina reduce
        //---should implement Animation Change
        
    }
    
}