using UnityEngine;

namespace Script
{
    public class Enemy : MonoBehaviour, IAttackableObject {

        public void Attacked(int damage) {}
        
    }

    public interface IAttackableObject {
        public void Attacked(int damage);
    }
}