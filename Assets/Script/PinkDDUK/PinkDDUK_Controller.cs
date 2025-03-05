using UnityEngine;
using UnityEngine.InputSystem;

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
