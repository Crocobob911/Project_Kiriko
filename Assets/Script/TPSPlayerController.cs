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
    private Vector2 moveInput = new Vector2(0,0);
    private Vector3 playerLookAt = new Vector3(0,0,0);

    Animator anim;


    void Start()
    {
        anim = playerBody.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isMoveInput = moveInput.magnitude != 0;
        anim.SetBool("isMove", isMoveInput);
        if (isMoveInput) {
            Vector3 lookForward = new Vector3(cameraRoot.forward.x, 0f, cameraRoot.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraRoot.right.x, 0f, cameraRoot.forward.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            playerBody.forward = lookForward;
            transform.position += moveDir * moveSpeed * Time.deltaTime ;
        }
        playerLookAt = new Vector3(cameraRoot.forward.x, 0f, cameraRoot.forward.z);
    }
}
