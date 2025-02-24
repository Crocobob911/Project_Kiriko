using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script {
    public class PlayerAttackInput : MonoBehaviour {
        [SerializeField] private Player player;

        [SerializeField] private float strongAttackTime = 1f;

        private bool isAttackInput = false;
        private float inputTime = 0f;

        public void InputAction(InputAction.CallbackContext context) {
            if (context.performed) InputStart();
            else if(context.canceled) InputCancel();
        }

        private void InputStart() {
            if(isAttackInput) return;
            Debug.Log("Attack Input Start.");
            
            isAttackInput = true;
            StartCoroutine(CountInputTime());
        }

        private void InputCancel() {
            if (inputTime <= 0f || inputTime >= strongAttackTime) return;
            Debug.Log("Attack Input Cancel.");
            
            StopCoroutine(CountInputTime());
            Attack_Normal();
        }
    

        private IEnumerator CountInputTime() {
            if (!isAttackInput) yield break;
            inputTime = 0f;

            while (inputTime < strongAttackTime) {
                inputTime += Time.deltaTime;
                yield return null;
            }
        
            inputTime = 0f;
            Attack_Strong();
        }

    
        private void Attack_Normal() {
            if (!isAttackInput) return;

            isAttackInput = false;
            player.NormalAttack_Start();
        }

        private void Attack_Strong() {
            if (!isAttackInput) return;

            isAttackInput = false;
            player.StrongAttack_Start();
        }
    }
}
