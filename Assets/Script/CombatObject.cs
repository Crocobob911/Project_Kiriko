using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 전투에 참여하는 모든 객체(플레이어, 적 등)가 공통으로 가질 속성을 정의하는 부모 클래스
/// </summary>
/// <summary>
/// 전투에 참여하는 모든 객체(플레이어, 적 등)가 공통으로 가질 속성을 정의하는 부모 클래스입니다.
/// </summary>
public class CombatObject : MonoBehaviour
{
    // 스태미나
    private int stamina;

    protected int Stamina
    {
        get => stamina;
        set
        {
            if (value > maxStamina) stamina = maxStamina;
            else if (value < 0) stamina = 0;
            stamina = value;
            Debug.Log("[" + name + "] Stamina : " + stamina);
        }
    }

    private int maxStamina = 100;
    public int MaxStamina {
        get => maxStamina;
        set => maxStamina = (value < 0) ? 0 : value; 
    }
        
    // 체력
    private int maxHp = 300;
    public int MaxHp {
        get => maxHp;
        set => maxHp = (value <= 0) ? 0 : value; 
    }

    private int hp;
    protected int Hp {
        get => hp;
        set {
            if (value >= maxHp) hp = maxHp;
            else if (value < 0) hp = 0;
            else hp = value;
                
            Debug.Log("[" + name + "] HP : " + hp);
        }
    }
}
