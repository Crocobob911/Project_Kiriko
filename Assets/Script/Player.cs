using UnityEngine;

namespace Script {
    public class Player : MonoBehaviour, IAttackableObject {

        [SerializeField] private PlayerAnimController animController;
        [SerializeField] private PlayerMoveController moveController;
        
        
        private int maxHp = 300;
        public int MaxHp {
            get => maxHp;
            set {
                if(value < 0) maxHp = 0;
                else maxHp = value;
            }
        }

        private int hp;
        public int Hp {
            get => hp;
            set {
                if (value > maxHp) hp = maxHp;
                else if (value < 0) hp = 0;
                else hp = value;
            }
        }

        private void Start() {
            hp = maxHp;
        }

        public void Attacked(int damage) { 
            Hp -= damage;
            Debug.Log("Player Attacked. Hp : " + Hp);
            moveController.Attacked();
            animController.AttackedAnim_Start();
        }
    }
}
