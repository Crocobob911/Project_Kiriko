using System;
using UnityEngine;
using UnityEngine.Serialization;



namespace Script {

    
    public class PlayerAttackManager : MonoBehaviour {

        public enum AttackType {
            Normal = 0,
            Strong = 1,
            Ultimate = 2
        }
        
        /// <summary>
        /// 플레이어의 기본 공격력
        /// </summary>
        [SerializeField] private int playerAttackDamage = 1;

        [SerializeField] private int normalAttackDamage = 20;
        [SerializeField] private int strongAttackDamage = 30;
        [SerializeField] private int ultimateAttackDamage = 30;

        /// <summary>
        /// 데미지 계산식 | type배율 * (기본 공격력 + 공격력 보정값) * 아이템 증가율 * 부위별 증가율 * 데미지 추가비율
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isCritical"></param>
        /// <param name="damageRatio_item"></param>
        /// <param name="damageRatio_Additional"></param>
        /// <returns></returns>
        public int CalculateDamage(AttackType type, bool isCritical, float damageRatio_item = 1f) {
            var damageRatio_critical = isCritical ? 1.2f : 1f;
            return (int)(DamageRatio_Type(type)
                         * (playerAttackDamage + AttackPowerCorrection(playerAttackDamage))
                         * damageRatio_item
                         * damageRatio_critical);
        }

        /// <summary>
        /// 플레이어의 공격 종류에 따른 데미지 계산
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private int DamageRatio_Type(AttackType type) {
            return type switch {
                AttackType.Normal => normalAttackDamage,
                AttackType.Strong => strongAttackDamage,
                AttackType.Ultimate => ultimateAttackDamage,
                _ => normalAttackDamage
            };
        }

        /// <summary>
        /// 공격력이 과하게 커지는 것을 막아주는 보정값.
        /// 공격력이 높아질수록 보정값이 낮아짐.
        /// </summary>
        /// <param name="playerAttackDamage"></param>
        /// <returns></returns>
        private float AttackPowerCorrection(float playerAttackDamage) {
            return 3f;
        }
    }
}