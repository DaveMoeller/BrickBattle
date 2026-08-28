using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    private Vector3 dir;
    float minX;
    float maxX;
    float minY;
    float maxY;
    [Tooltip("Enemy Speed")]
    [Range(.1f, 2.0f)]
    public float speed = .1f;
    Rigidbody rb;
    public int pointsToAddToBrickRangeLow = 1;
    public int pointsToAddToBrickRangeHigh = 100;
    public int pointsToAddToScoreRangeLow = 1;
    public int pointsToAddToScoreRangeHigh = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get starting position
        minX = MainManager.Instance.GetMinX();
        maxX = MainManager.Instance.GetMaxX();
        minY = MainManager.Instance.GetMinY();
        maxY = MainManager.Instance.GetMaxY();
        float startX = Random.Range(minX, maxX);
        float startY = Random.Range(minY, maxY);
        dir = new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 0.0f).normalized;
        //Debug.Log($"dir: {dir}");
        //rb = GetComponent<Rigidbody>();
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(speed * dir, ForceMode.VelocityChange);
    }
    // Update is called once per frame
    void Update()
    {
        //Move
        //transform.position += speed * Time.deltaTime * dir;
    }
    private void FixedUpdate()
    {
        //Move
        //Debug.Log($"Enemy Position: {transform.position}");
        Vector3 newVelocity = speed * Time.deltaTime * dir;
        rb.AddForce(newVelocity, ForceMode.VelocityChange);

    }
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"OnCollisionEnter: Enemy Collision with {collision.gameObject.name}, tag: {collision.gameObject.tag}");
        //If border tag...
        //Change direction
        //If paddle
        //If ball
    }
    public void OnCollisionExit(Collision collision)
    {
        Debug.Log($"OnCollisionExit: Enemy Collision with {collision.gameObject.name}, tag: {collision.gameObject.tag}");
        //If border tag...
        //Change direction
        //If paddle
        //If ball
    }
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter: Enemy Trigger Collision with {other.gameObject.name}, tag: {other.gameObject.tag}");
        //If border tag...
        if (other.gameObject.CompareTag("Border"))
        {
            //Change direction
            float xDirMax = 1.0f;
            float yDirMax = 1.0f;
            float xDirMin = 0.0f;
            float yDirMin = 0.0f;

            if (transform.position.x > maxX)
            {
                xDirMin = -1.0f;
                xDirMax = 0.0f;
            }
            if (transform.position.x < minX)
            {
                xDirMin = 0.0f;
                xDirMax = 1.0f;
            }
            if (transform.position.y > maxY)
            {
                yDirMin = -1.0f;
                yDirMax = 0.0f;
            }
            if (transform.position.y < minY)
            {
                yDirMin = 0.0f;
                yDirMax = 1.0f;
            }
            // Stop and adjust direction and velocity
            dir = new Vector3(Random.Range(xDirMin, xDirMax), Random.Range(yDirMin, yDirMax), 0.0f).normalized;
            Debug.Log($"dir (changed): {dir}");
            rb.linearVelocity = Vector3.zero;
        }
        // If paddle add negative score
        if (other.gameObject.CompareTag("Paddle"))
        {
            if (other.TryGetComponent<Paddle>(out Paddle hitPaddle))
            {
                int pointsToAdd = - Random.Range(pointsToAddToScoreRangeLow, pointsToAddToScoreRangeHigh);
                Debug.Log("Hit a Paddle! Score value is: " + MainManager.Instance.GetScore());
                Debug.Log($"Adding {pointsToAdd} points to score");
                MainManager.Instance.AddPoint(pointsToAdd);
                Debug.Log("New Score value is: " + MainManager.Instance.GetScore());
            }

        }
        //ToDo: If ball add ring around it and make rigid body/collision area bigger
        // If Brick
        if (other.gameObject.CompareTag("Brick"))
        {
            if (other.TryGetComponent<Brick>(out Brick hitBrick))
            {
                int pointsToAdd = Random.Range(pointsToAddToBrickRangeLow, pointsToAddToBrickRangeHigh);
                Debug.Log("Hit a brick! Its point value is: " + hitBrick.GetPoints());
                Debug.Log($"Adding {pointsToAdd} points to brick");
                hitBrick.AddPoints(pointsToAdd);
            }

        }

    }
    public void StopEnemy()
    {
        rb.linearVelocity = Vector3.zero;
    }
}
