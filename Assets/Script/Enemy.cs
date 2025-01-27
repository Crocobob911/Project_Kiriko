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
        }
        
    }

    public interface IAttackableObject {
        public void Attacked(int damage);
    }
}