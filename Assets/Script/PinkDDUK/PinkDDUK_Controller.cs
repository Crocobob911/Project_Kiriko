using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 'PinkDDUK' 모델의 애니메이션을 테스트하기 위한 컨트롤러입니다.
/// </summary>
public class PinkDDUK_Controller : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    private static readonly int Idle = Animator.StringToHash("idle");
    private static readonly int Walk = Animator.StringToHash("walk");
        
    public void IdleAnimStart(InputAction.CallbackContext context) {
        if (!context.started) return;
        
        animator.SetTrigger(Idle);
    }
    
    public void WalkAnimStart(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        
        animator.SetTrigger(Walk);
    }
}
