using UnityEngine;

namespace Script {
    public class PlayerAnimController : MonoBehaviour
    {
        // 애니메이터 매개변수들
        // Animator.StringToHash()로 그 값들을 미리 가져와 갖고있음으로써, 연산 줄여줌.
        private readonly int animIsMove = Animator.StringToHash("isMove");
        private readonly int animMoveForward = Animator.StringToHash("moveForward");
        private readonly int animMoveRight = Animator.StringToHash("moveRight");
        private readonly int animIsCamLocked = Animator.StringToHash("isCamLocked");
        private readonly int animIsOnAir = Animator.StringToHash("isOnAir");
        private readonly int animAvoid = Animator.StringToHash("avoid");
        private readonly int animKnockBack = Animator.StringToHash("knockBack");

        [SerializeField]private Animator animator;
    
        [SerializeField]private Vector2 currentDirection;
        [SerializeField]private Vector2 newDirection;

        private void Init() {
            currentDirection = Vector2.zero;
            newDirection = Vector2.zero;
        }
    
        private void Start() {
            animator = transform.GetChild(1).GetComponent<Animator>();
        
            Init();
        }
    
        private void Update() {
            SetPlayerAnim();
        }
    
        private void SetPlayerAnim() {
            if(newDirection == currentDirection) return;
        
            LerpMoveAnimDirection();
        
            animator.SetFloat(animMoveForward, currentDirection.y);
            animator.SetFloat(animMoveRight, currentDirection.x);
        }
    
        public void SetMoveAnimDirection(Vector2 moveDir) {
            newDirection = moveDir;
            animator.SetBool(animIsMove, moveDir != Vector2.zero);
        }

        private void LerpMoveAnimDirection() {
            currentDirection = (currentDirection - newDirection).magnitude >= 0.01f 
                ? Vector2.Lerp(currentDirection, newDirection, 5f * Time.deltaTime) 
                : newDirection;
        }

        public void JumpAnim_Start() {
            animator.SetBool(animIsOnAir, true);
            SetMoveAnimDirection(Vector2.zero);
        
            // 도약 모션 재생 들어가야함.
            // 그 뒤에 체공 모션으로 자연스레 넘어갸야함.
            
            // 카메라 락온, 락온 아님에 따라 다른 애니 출력해줘야함.
        }

        public void JumpAnim_End() {
            animator.SetBool(animIsOnAir, false);
            // 착지 모션 재생 들어가야함.
        }
        
        public void KnockBackAnim_Start() {
            animator.SetTrigger(animKnockBack);
        }
        
        
        public void Avoid_Start() {
            animator.SetTrigger(animAvoid);
        }
        
        
        public void SetIsCamLocked(bool isCamLocked) {
            animator.SetBool(animIsCamLocked, isCamLocked);
        }
    }
}
