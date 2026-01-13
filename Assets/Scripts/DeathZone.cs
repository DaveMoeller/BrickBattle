using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public MainManager Manager;

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("DeathZone: other.gameObject.name: " + other.gameObject.name);
        Destroy(other.gameObject);
        //Ball ball = (Ball)gameObject;
        //(Ball)other.gameObject.StopBall();
        Manager.GameOver();
    }
}
