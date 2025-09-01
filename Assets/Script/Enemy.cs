using System;
using UnityEngine;

namespace Script
{
    public class Enemy : CombatObject, IAttackableObject {

        [SerializeField] private int enemyMaxHp;
        [SerializeField] private int enemyMaxStamina;


        private void Start() {
            Init();
        }

        private void Init() {
            MaxHp = enemyMaxHp;
            MaxStamina = enemyMaxStamina;
            
            Hp = MaxHp;
            Stamina = MaxStamina;
        }

        public void Attacked(int damage) {
            Hp -= damage;
            // 복잡한 데미지 계산 필요함.
        }
        
    }

    /// <summary>
/// 공격 가능한 모든 객체를 위한 인터페이스입니다.
/// </summary>
public interface IAttackableObject {
        public void Attacked(int damage);
    }
}