using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEyeTrack : MonoBehaviour
{
    /*private void OnTriggerEnter(Collision collision)
    {
        WhatAreYouLookingAt(collision);
    }*/

    public void WhatAreYouLookingAt(Collision coll)
    {
        Debug.Log("Looking At : " + coll);
    }
}
