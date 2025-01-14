using System.Collections;
using System.Collections.Generic;
using Script;
using Unity.VisualScripting;
using UnityEngine;

public class LandingColliderForPlayer : MonoBehaviour {
    [SerializeField] private PlayerMoveController player;

    private void OnCollisionEnter(Collision collision) {
        Debug.Log(collision.gameObject.name);
        if (!collision.transform.CompareTag(null)) {
            Debug.Log("Landing collider Entered");
            player.Land();
        }
    }
}
