using System.Collections;
using System.Collections.Generic;
using Script;
using Unity.VisualScripting;
using UnityEngine;

public class LandingColliderForPlayer : MonoBehaviour {
    [SerializeField] private PlayerMoveController player;

    private void OnTriggerEnter(Collider other){
        if (!other.transform.CompareTag("Ground")) {
            player.Jump_end();
        }
    }
}
