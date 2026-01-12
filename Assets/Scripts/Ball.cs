using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody m_Rigidbody;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionExit(Collision other)
    {
        var velocity = m_Rigidbody.linearVelocity;
        //ToDo: Count bricks and stop ball if no more
        GameObject[] foundObjects;
        foundObjects = GameObject.FindGameObjectsWithTag("Brick");
        if (foundObjects.Length == 0)
        {
            velocity = new Vector3 (0, 0, 0);
        }
        else
        {

            //after a collision we accelerate a bit
            velocity += velocity.normalized * 0.01f;

            //check if we are not going totally vertically as this would lead to being stuck, we add a little vertical force
            if (Vector3.Dot(velocity.normalized, Vector3.up) < 0.1f)
            {
                velocity += velocity.y > 0 ? Vector3.up * 0.6f : Vector3.down * 0.6f;
            }

            //max velocity
            if (velocity.magnitude > 3.0f)
            {
                velocity = velocity.normalized * 3.0f;
            }
        }

        m_Rigidbody.linearVelocity = velocity;
        //ToDo: Count bricks and stop ball if no more
        //ToDo: Display "You Won <name>"
    }
}
