using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSPlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform playerBody;

    Animator anim;


    void Start()
    {
        anim = playerBody.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
