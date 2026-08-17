// Data Persistence
using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
    public int pointMutiplier = 1;
    //Colors
    public Vector3 BallInitialTransform;
    public Color titleColor = Color.green;
    private static PlayerControls controls; // Reference to the generated class
    public PlayerControls PlayerControlsShared { get { return controls; } }
    private GameObject goTextPrefab;
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
        //ToDo: Instantiate the paddle
        var paddle = Instantiate(paddlePrefab);
        //ToDo: Instantiate the ball
        var ball = Instantiate(ballPrefab);
        ballRB = ball.GetComponent<Rigidbody>();
        //
        //pointMutiplier = GameLevelData.Inst
        //Set the point multiplier
        GameLevelData levelData = GameUIData.Instance.GetGameLevelData(GameUIData.Instance.CurrentLevel);
        //Debug.Log($"GameUIData.Instance.CurrentLevel: {GameUIData.Instance.CurrentLevel}");
        pointMutiplier = levelData.levelPoints;
        //Debug.Log($"pointMultiplier: {pointMutiplier}");
        randomRangeLow = levelData.randomRangeLow;
        randomRangeHigh = levelData.randomRangeHigh;
        titleColor = levelData.levelMaterial.color;
        //Set score title color
        bestScoreText.color = titleColor;
        ScoreText.color = titleColor;
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
                brickScript.PointValue = brickScript.row * pointMutiplier;
                brickScript.onDestroyed.AddListener(AddPoint);
                GameUIData.Instance.AddBrick();
            }
        }

    }
    private void Update()
    {
        if (!m_Started)
        {
            bool isPressed = controls.Gameplay.GameStart.IsPressed();
            if (isPressed && (ballRB != null))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new(randomDirection, 1, 0);
                forceDir.Normalize();
                ballRB.transform.SetParent(null);
                ballRB.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
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
        if (GameUIData.Instance.GetNumberOfBricks() == 0)
        {
            GameOver();
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = playerName + $" Score : {m_Points}";
        if (m_Points > bestScore)
        {
            bestScore = m_Points;
            playerWithBestScore = playerName;
            //Best Score : Player Name : 0
            bestScoreText.text = "Best Score: " + playerWithBestScore + " : " + bestScore;
        }
    }

    public void GameOver()
    {
        //ToDo: Display Win Message
        //ToDo: goText is null sometimes
        Ball.Instance.StopBall();
        //stop the ball
        SaveAllData();
        //ToDo: need to attach to canvas?
        //goTextPrefab = Instantiate(gameOverTextPrefab);
        Text goText = gameOverText.GetComponent<Text>();
        if (goText != null)
        {
            goText.text = "Game Over " + playerName+ "!";
            if (GameUIData.Instance.GetNumberOfBricks() == 0)
            {
                goText.text += " You Won!";
            }
            //ToDo: Create textfield on GUI for this
            goText.text += "\n\nPress Space to Restart";
            gameOverText.SetActive(true);
        }
        m_GameOver = true;
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
