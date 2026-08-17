using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    private Rigidbody m_Rigidbody;
    public static Ball Instance;
    private float ballVelocity = 0.6f;
    private float ballVelocityMax = 3.0f;

    public Rigidbody GetRigidBody()
    {
        return m_Rigidbody;
    }
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
        //GameObject[] foundObjects;
        //foundObjects = GameObject.FindGameObjectsWithTag("Brick");
        if (GameUIData.Instance.GetNumberOfBricks() <= 0)
        {
            MainManager.Instance.GameOver();
            //StopBall();
            // Display "You Won <name>"
            //Debug.Log("You Won!");
            //MainManager.Instance.GameOver();
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
                velocity.x = UnityEngine.Random.Range(-.5f,.5f);
                //velocity = velocity;
                //Debug.Log($"new x velocity:{velocity}");
            }
            if ((dot > -.01f) && (dot <0.01f))
            {
                velocity.y = UnityEngine.Random.Range(-.5f, .5f);
                //velocity = velocity;
                //Debug.Log($"new y velocity:{velocity}");
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
