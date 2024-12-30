using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAnimController : MonoBehaviour
{
    // 애니메이터 매개변수들
    // Animator.StringToHash()로 그 값들을 미리 가져와 갖고있음으로써, 연산 줄여줌.
    private static readonly int AnimIsMove = Animator.StringToHash("isMove");
    private static readonly int AnimMoveForward = Animator.StringToHash("moveForward");
    private static readonly int AnimMoveRight = Animator.StringToHash("moveRight");
    private static readonly int AnimIsCamLocked = Animator.StringToHash("isCamLocked");

    [SerializeField]private Animator anim;
    
    [FormerlySerializedAs("currenTDirection")] [FormerlySerializedAs("currendDirection")] [FormerlySerializedAs("oldDirection")] [SerializeField]private Vector2 currentDirection;
    [SerializeField]private Vector2 newDirection;

    private void Init() {
        currentDirection = Vector2.zero;
        newDirection = Vector2.zero;
    }
    
    void Start()
    {
        anim = transform.GetChild(1).GetComponent<Animator>();
        Init();
    }
    
    void Update()
    {
        lerpMoveDirection();
    }

    public void setMoveDirection(Vector2 moveDir) {
        newDirection = moveDir;
        anim.SetBool(AnimIsMove, moveDir != Vector2.zero);
    }

    private void lerpMoveDirection() {
        if (currentDirection == newDirection) return;
        
        if ((currentDirection - newDirection).magnitude >= 0.001f) {
            currentDirection = Vector2.Lerp(currentDirection, newDirection, 5f * Time.deltaTime);
        }
        
        anim.SetFloat(AnimMoveForward, currentDirection.y);
        anim.SetFloat(AnimMoveRight, currentDirection.x);
    }

    public void setIsCamLocked(bool isCamLocked) {
        anim.SetBool(AnimIsCamLocked, isCamLocked);
    }
}
