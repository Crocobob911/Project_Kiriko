using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEyeTrack : MonoBehaviour
{
    private GameObject foundObject;

    private void OnEnable()
    {
        foundObject = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        foundObject = other.gameObject;
        Debug.Log("Collision : " + other.gameObject);
    }


    public GameObject GetLockObject()
    {
        return foundObject; 
    }
}
