using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    private Rigidbody m_Rigidbody;
    public static Ball Instance;
    private float ballVelocity = 0.6f;
    private float ballVelocityMax = 3.0f;
    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        //Set values based on level
        GameLevelData levelData = GameUIData.Instance.GetGameLevelData(GameUIData.Instance.CurrentLevel);
        ballVelocity = levelData.ballVelocity;
        ballVelocityMax = levelData.ballVelocityMax;
    }

    private void OnCollisionExit(Collision other)
    {
        var velocity = m_Rigidbody.linearVelocity;
        // Count bricks and stop ball if no more
        GameObject[] foundObjects;
        foundObjects = GameObject.FindGameObjectsWithTag("Brick");
        if (foundObjects.Length == 0)
        {
            StopBall();
            // Display "You Won <name>"
            Debug.Log("You Won!");
        }
        else
        {

            //after a collision we accelerate a bit
            //velocity += velocity.normalized * 0.01f;

            //check if we are not going totally vertically as this would lead to being stuck, we add a little vertical force
            //velocity.normalized
            float dot = Vector2.Dot(velocity.normalized, Vector2.up);
            //Debug.Log($"velocity.normalized:{velocity.normalized}");
            //Debug.Log($"dot:{dot}");
            if ((dot>.99f)||(dot < -0.99f))
            {
                velocity.x = 0.1f;
                //velocity = velocity;
                Debug.Log($"new velocity:{velocity}");
            }

            //max velocity
            if (velocity.magnitude > ballVelocityMax)
            {
                velocity = velocity.normalized * ballVelocityMax;
            }
            m_Rigidbody.linearVelocity = velocity;
        }

    }
    public void StopBall()
    {
        // StopBall
        var velocity = new Vector3(0, 0, 0);
        m_Rigidbody.linearVelocity = velocity;

    }
}
