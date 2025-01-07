using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAnimController : MonoBehaviour
{
    // 애니메이터 매개변수들
    // Animator.StringToHash()로 그 값들을 미리 가져와 갖고있음으로써, 연산 줄여줌.
    private readonly int animIsMove = Animator.StringToHash("isMove");
    private readonly int animMoveForward = Animator.StringToHash("moveForward");
    private readonly int animMoveRight = Animator.StringToHash("moveRight");
    private readonly int animIsCamLocked = Animator.StringToHash("isCamLocked");

    [SerializeField]private Animator animator;
    
    [SerializeField]private Vector2 currentDirection;
    [SerializeField]private Vector2 newDirection;

    private void Init() {
        currentDirection = Vector2.zero;
        newDirection = Vector2.zero;
    }
    
    void Start() {
        animator = transform.GetChild(1).GetComponent<Animator>();
        
        Init();
    }
    
    void Update() {
        SetPlayerAnim(LerpMoveAnimDirection(newDirection, currentDirection));
    }
    
    public void SetMoveAnimDirection(Vector2 moveDir) {
        newDirection = moveDir;
        animator.SetBool(animIsMove, moveDir != Vector2.zero);
    }

    private Vector2 LerpMoveAnimDirection(Vector2 newDir, Vector2 currentDir) {
        if ((currentDir - newDir).magnitude >= 0.001f) {
            currentDir = Vector2.Lerp(currentDir, newDir, 5f * Time.deltaTime);
        }
        else currentDir = newDir;
        
        return currentDir;
    }

    private void SetPlayerAnim(Vector2 currentDir) {
        animator.SetFloat(animMoveForward, currentDir.y);
        animator.SetFloat(animMoveRight, currentDir.x);
    }

    public void SetIsCamLocked(bool isCamLocked) {
        animator.SetBool(animIsCamLocked, isCamLocked);
    }
}
