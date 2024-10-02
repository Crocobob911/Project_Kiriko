using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSPlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform playerBody;
    [SerializeField]
    private Transform cameraRoot;
    [SerializeField]
    float moveSpeed;

    bool isMoveInput = false;
    private Vector2 moveInput = new Vector2(0,0); //WASD �Է�
    private Vector3 playerLookAt = new Vector3(0,0,0);
    private Vector3 forwardMove;
    private Vector3 sideMove;
    private Vector3 moveDir;

    private bool isCamLocked;

    Animator anim;

    void Start()
    {
        anim = playerBody.GetComponent<Animator>();
    }

    void Update()
    {
        Move();
    }

    private void Move()  
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); 
        isMoveInput = moveInput.magnitude != 0;
        anim.SetBool("isMove", isMoveInput);  //*****�ٵ� ������Ʈ ������ �긦 ȣ���ϴ� �� �³�?

        if (isMoveInput) { 
            anim.SetFloat("moveForward", moveInput.y);
            anim.SetFloat("moveRight", moveInput.x);

            forwardMove = new Vector3(cameraRoot.forward.x, 0f, cameraRoot.forward.z).normalized;
            sideMove = new Vector3(cameraRoot.right.x, 0f, cameraRoot.right.z).normalized;
            moveDir = forwardMove * moveInput.y + sideMove * moveInput.x;

            if (isCamLocked) //ī�޶� LockOn �Ǵ�
                playerBody.forward = forwardMove; // <= ĳ���Ͱ� ī�޶� ������ ��� �ٶ󺸸� ������
            else
                playerBody.forward = moveDir;   //<= ĳ���Ͱ� ī�޶�� ������� ������

            transform.position += moveDir * moveSpeed * Time.deltaTime ;
        }
        playerLookAt = new Vector3(cameraRoot.forward.x, 0f, cameraRoot.forward.z);
    }

    public void CamLock(bool isLocked)
    {
        isCamLocked = isLocked; 
        anim.SetBool("isCamLocked", isCamLocked);
    }
}