// Data Persistence
using System.IO;
//using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    [Tooltip("The Brick Prefab")]
    public GameObject brickPrefab;
    [Tooltip("The Paddle Prefab")]
    public GameObject paddlePrefab;
    [Tooltip("The Ball Prefab")]
    public GameObject ballPrefab;

    public int LineCount = 6;
    private Rigidbody ballRB;
    public Text ScoreText;
    public Text bestScoreText;
    public GameObject gameOverText;
    public GameObject gameOverTextPrefab;
    [Tooltip("Canvas for GUI display elments.")]
    //public GameObject mainCanvas;
    private bool m_Started = false;
    private int m_Points;
    private bool m_GameOver = false;
    public string playerName; // new variable declared
    public string saveFile = "/savefile_game.json";
    public int bestScore = 0;
    public string playerWithBestScore = "";
    [Range(1, 10)]
    //Colors
    public Vector3 BallInitialTransform;
    public Color titleColor = Color.green;
    [Tooltip("Object to constrain min X movement.")]
    public GameObject borderLeft;
    private float minX;
    [Tooltip("Object to constrain max X movement.")]
    private float maxX;
    public GameObject borderRight;
    [Tooltip("Object to constrain min Y movement (DeathZone).")]
    public GameObject borderBottom; //deathZone
    private float minY;
    [Tooltip("Object to constrain max Y movement.")]
    private float maxY;
    public GameObject borderTop;
    private readonly float yBuffer = 0.2f;
    private static PlayerControls controls; // Reference to the generated class
    public PlayerControls PlayerControlsShared { get { return controls; } }
    private bool gameOverCalled = false;
    public GameObject enemyPrefab;
    void OnEnable()
    {
        controls.Enable(); // Actions must be enabled
    }

    void OnDisable()
    {
        controls.Disable(); // Actions should be disabled when not in use
    }

    public void Awake()
    {
        //Debug.Log("MainManager gameObject.name: " + gameObject.name);
        if (Instance != null)
        {
            gameOverCalled = false;
            return;
        }
        else
        {
            Instance = this;
            controls = new PlayerControls();
            DontDestroyOnLoad(Instance); // same as GameObject

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // If starting in game go to menu. Game data is null
        if (GameUIData.Instance == null)
        {
            //Debug.LogError("Start the game from the menu scene!");

            SceneManager.LoadScene(0);
            Destroy(Instance);
            return;
        }
        Setup();
    }
    private void Reset()
    {
        Setup();
    }
    void Setup()
    {
        m_Started = false;
        m_GameOver = false;
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        float randomRangeLow = 0.0f;
        float randomRangeHigh = 1.0f;
        gameOverCalled = false;
        LoadAllData();
        ScoreText.text = playerName + $" Score : {m_Points}";
        bestScoreText.text = "Best Score: " + playerWithBestScore + " : " + bestScore;
        if (MenuManager.Instance != null)
        {
            playerName = MenuManager.Instance.playerName;
        }
        else
        {
            playerName = "Player 01";
        }
        // Instantiate the paddle
        var paddle = Instantiate(paddlePrefab);
        // Instantiate the ball
        var ball = Instantiate(ballPrefab);
        ballRB = ball.GetComponent<Rigidbody>();
        //
        //Set the point multiplier
        GameLevelData levelData = GameUIData.Instance.GetGameLevelData(GameUIData.Instance.CurrentLevel);
        //Debug.Log($"GameUIData.Instance.CurrentLevel: {GameUIData.Instance.CurrentLevel}");
        //Debug.Log($"pointMultiplier: {levelData.levelPoints}");
        randomRangeLow = levelData.randomRangeLow;
        randomRangeHigh = levelData.randomRangeHigh;
        titleColor = levelData.levelMaterial.color;
        //Set score title color
        bestScoreText.color = titleColor;
        ScoreText.color = titleColor;
        GameUIData.Instance.ResetBrickCount();
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                //Rotate based on level
                Quaternion qRotation = Quaternion.Euler(0f, 0f, 90f * Random.Range(randomRangeLow, randomRangeHigh));
                var brick = Instantiate(brickPrefab, position, qRotation);
                MeshRenderer meshRenderer;
                meshRenderer = brick.GetComponent<MeshRenderer>();
                //Debug.Log("Current Material: " + meshRenderer.material.name);
                meshRenderer.material = GameUIData.Instance.gameLevelData[i].levelMaterial;
                //first row i = 0 so add 1
                Brick brickScript = brick.GetComponent<Brick>();
                brickScript.row = i + 1;
                brickScript.PointValue = brickScript.row * levelData.levelPoints;
                brickScript.onDestroyed.AddListener(AddPoint);
                GameUIData.Instance.AddBrick();
                //ToDo: Use method in Brick
                //ToDo: Set text to show point value
                //Get Canvas
                //Get Text
                Transform myCanvas = brick.transform.Find("Canvas");
                if (myCanvas != null)
                {
                    Transform myText = myCanvas.transform.Find("Text");
                    if (myText != null)
                    {
                        if (myText.TryGetComponent<TMPro.TextMeshProUGUI>(out var tmpText))
                        {
                            tmpText.text = $"{brickScript.PointValue:000}";
                        }
                    }
                    //ToDo: Set text color to be same as brick
                }
            }
        }
        //Calculate Min and Max X
        CalculateConstraints();
        //Create enemies based on level

        for (int i = 0; i < levelData.numberOfEnemies; i++)
        {
            //Get random x and y
            //UnityEngine.Vector3 loc = new UnityEngine.Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
            //_ = Instantiate(enemyPrefab, loc,UnityEngine.Quaternion.identity);
            _ = Instantiate(enemyPrefab);
        }
    }
    private void CalculateConstraints()
    {
        minX = (borderLeft.transform.position.x + (borderLeft.transform.localScale.x / 2))
            + (paddlePrefab.transform.localScale.x / 2);
        //Debug.Log($"minX= {minX}");
        maxX = (borderRight.transform.position.x - (borderRight.transform.localScale.x / 2))
             - (paddlePrefab.transform.localScale.x / 2);
        //Debug.Log($"maxX= {maxX}");
        minY = (borderBottom.transform.position.y + (borderBottom.transform.localScale.y / 2))
         + yBuffer;
        //Debug.Log($"minX= {minX}");
        maxY = (borderTop.transform.position.y - (borderTop.transform.localScale.y / 2))
             - yBuffer;
    }
    public float GetMinX() { return minX; }
    public float GetMaxX() { return maxX; }
    public float GetMinY() { return minY; }
    public float GetMaxY() { return maxY; }
    private void Update()
    {
        if (!m_Started)
        {
            bool isPressed = controls.Gameplay.GameStart.WasPerformedThisFrame();
            //&& (ballRB != null)
            if (isPressed)
            {
                m_Started = true;
                if (ballRB != null)
                {
                    float randomDirection = Random.Range(-1.0f, 1.0f);
                    Vector3 forceDir = new(randomDirection, 1, 0);
                    forceDir.Normalize();
                    ballRB.transform.SetParent(null);
                    ballRB.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);

                }
                else
                {
                    //ballRB is null until the scene is fully rebuilt. Ignore issue
                    //Debug.LogError("ballRB is null!");
                }
            }
        }
        else if (m_GameOver)
        {
            bool isPressed = controls.Gameplay.GameStart.WasPerformedThisFrame();
            if (isPressed)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                Reset();
            }
        }
    }

    public void AddPoint(int point)
    {
        m_Points += point;
        // In case negative points
        if (m_Points < 0) m_Points = 0;
        if (ScoreText != null)
        {
            ScoreText.text = playerName + $" Score : {m_Points}";
            if (m_Points > bestScore)
            {
                bestScore = m_Points;
                playerWithBestScore = playerName;
                //Best Score : Player Name : 0
                bestScoreText.text = "Best Score: " + playerWithBestScore + " : " + bestScore;
            }
        }
    }

    public int GetScore()
    {
        return m_Points;
    }

    public void GameOver()
    {
        if (gameOverCalled) return;
        gameOverCalled = true;
        if (Ball.Instance != null)
        {
            Ball.Instance.StopBall();
        }
        EnemyDeath[] enemyDeath = GameObject.FindObjectsByType<EnemyDeath>();
        foreach (EnemyDeath enemy in enemyDeath)
        {
            Destroy(enemy.gameObject);
        }

        SaveAllData();
        gameOverText = GameObject.Find("GameoverText");
        gameOverText.SetActive(true);
        if (gameOverText != null)
        {
            if (gameOverText.TryGetComponent<Text>(out Text goText))
            {
                if (goText != null)
                {
                    goText.text = "Game Over " + playerName + "!";
                    if (GameUIData.Instance.GetNumberOfBricks() == 0)
                    {
                        goText.text += " You Won!";
                    }
                    goText.text += "\n\nPress Space to Restart!";
                    goText.enabled = true;
                }
            }
        }
        else
        {
            Debug.LogError("gameOverText is null!");
        }
        m_GameOver = true;
        //m_Started = false;
        GameUIData.Instance.ResetBrickCount();
    }
    [System.Serializable]
    class SaveData
    {
        public string playerName;
        public int bestScore = 0;
        public string playerWithBestScore = "";
    }
    public void SaveAllData()
    {
        if (MenuManager.Instance.usePlayerPreferences)
        {
            // Add level color MenuManager.Instance.CurrentLevel
            PlayerPrefs.SetString("playerWithBestScore" + "_" + GameUIData.Instance.CurrentLevel, playerWithBestScore);
            PlayerPrefs.SetInt("bestScore" + "_" + GameUIData.Instance.CurrentLevel, bestScore);
        }
        else
        {
            SaveData data = new()
            {
                playerName = playerName,
                bestScore = bestScore,
                playerWithBestScore = playerWithBestScore
            };

            string json = JsonUtility.ToJson(data);

            File.WriteAllText(Application.persistentDataPath + saveFile, json);
        }
    }
    public void LoadAllData()
    {
        if (MenuManager.Instance.usePlayerPreferences)
        {
            playerName = PlayerPrefs.GetString("playerName");
            playerWithBestScore = PlayerPrefs.GetString("playerWithBestScore" + "_" + GameUIData.Instance.CurrentLevel);
            bestScore = PlayerPrefs.GetInt("bestScore" + "_" + GameUIData.Instance.CurrentLevel);
        }
        else
        {

            string path = Application.persistentDataPath + saveFile;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                //playerName = data.playerName;
                bestScore = data.bestScore;
                playerWithBestScore = data.playerWithBestScore;
                bestScoreText.text = "Best Score: " + playerWithBestScore + " : " + bestScore;
            }
        }
    }
    public bool SaveFileExists()
    {
        string path = Application.persistentDataPath + saveFile;
        return File.Exists(path);
    }
    public void ExitToMenu()
    {
        SaveAllData();
        //Debug.Log("Loading scene 0");
        SceneManager.LoadScene(0);
    }

}
