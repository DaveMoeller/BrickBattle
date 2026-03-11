// Data Persistence
using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    //ToDo: Stop Game when all bricks gone and display "You Won, <name>!"
    public static MainManager Instance;
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;
    public Text ScoreText;
    public Text bestScoreText;
    public GameObject GameOverText;
    private bool m_Started = false;
    private int m_Points;
    private bool m_GameOver = false;
    public string playerName; // new variable declared
    public string saveFile = "/savefile_game.json";
    public int bestScore = 0;
    public string playerWithBestScore = "";
    [Range(1, 10)]
    public int pointMutiplier = 1;
    public int pointMutiplierGreen = 1;
    public int pointMutiplierPurple = 2;
    public int pointMutiplierChocolate = 3;
    public int pointMutiplierBlue = 4;
    public int pointMutiplierSilver = 5;
    public int pointMutiplierGold = 6;
    //Colors
    public Material MaterialGreen;
    public Material MaterialPurple;
    public Material MaterialChocolate;
    public Material MaterialBlue;
    public Material MaterialSilver;
    public Material MaterialGold;
    public Vector3 BallInitialTransform;
    public Color titleColor = Color.green;
    private static PlayerControls controls; // Reference to the generated class
    public PlayerControls PlayerControlsShared {  get { return controls; } }
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
        if (MenuManager.Instance != null)
        {
            playerName = MenuManager.Instance.playerName;
        }
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
        //Set the point multiplier
        switch (MenuManager.Instance.CurrentLevel)
        {
            case
                "Green":
                {
                    pointMutiplier = pointMutiplierGreen;
                    randomRangeLow = 0.0f;
                    randomRangeHigh = 0.0f;
                    titleColor = Color.green;
                    break;
                }
            case
                "Purple":
                {
                    pointMutiplier = pointMutiplierPurple;
                    randomRangeLow = 0.5f;
                    randomRangeHigh = 1.0f;
                    titleColor = Color.purple;
                    break;
                }
            case
                 "Chocolate":
                {
                    pointMutiplier = pointMutiplierChocolate;
                    randomRangeLow = 0.0f;
                    randomRangeHigh = 0.5f;
                    titleColor = Color.chocolate;
                    break;
                }
            case
                 "Blue":
                {
                    pointMutiplier = pointMutiplierBlue;
                    randomRangeLow = 0.25f;
                    randomRangeHigh = 0.75f;
                    titleColor = Color.blue;
                    break;
                }
            case
                 "Silver":
                {
                    pointMutiplier = pointMutiplierSilver;
                    randomRangeLow = 0.15f;
                    randomRangeHigh = 0.85f;
                    titleColor = Color.silver;
                    break;
                }
            case
                 "Gold":
                {
                    pointMutiplier = pointMutiplierGold;
                    randomRangeLow = 0.0f;
                    randomRangeHigh = 1.0f;
                    titleColor = Color.gold;
                    break;
                }
            default:
                pointMutiplier = pointMutiplierGreen;
                randomRangeLow = 0.0f;
                randomRangeHigh = 0.0f;
                titleColor = Color.white;
                break;
        }
        bestScoreText.color = titleColor;
        //int[] pointCountArray = new[] { 1, 2, 3, 4, 5, 6 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                //Rotate based on level
                //Quaternion qRotation = Quaternion.identity
                Quaternion qRotation = Quaternion.Euler(0f, 0f, 90f * Random.Range(randomRangeLow, randomRangeHigh));
                var brick = Instantiate(BrickPrefab, position, qRotation);
                brick.row = i + 1;
                brick.PointValue = brick.row * pointMutiplier;
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }
    private void Update()
    {
        if (!m_Started)
        {
            //Direct read from keyboard
            //bool isPressed = Keyboard.current[Key.Space].isPressed;
            bool isPressed = controls.Gameplay.GameStart.IsPressed();
            if (isPressed)
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();
                if (Ball != null)
                {
                    Ball.transform.SetParent(null);

                    Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);

                }
                else { Debug.Log("Ball is null"); }
            }
        }
        else if (m_GameOver)
        {
            //Direct read from keyboard
            //bool isPressed = Keyboard.current[Key.Space].isPressed;
            bool isPressed = controls.Gameplay.GameStart.IsPressed();
            if (isPressed)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                Reset();
            }
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
        m_GameOver = true;
        //stop the ball
        SaveAllData();
        Text goText = GameOverText.GetComponent<Text>();
        goText.text = "Game Over " + playerName;
        GameOverText.SetActive(true);
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
            PlayerPrefs.SetString("playerWithBestScore" + "_" + MenuManager.Instance.CurrentLevel, playerWithBestScore);
            PlayerPrefs.SetInt("bestScore" + "_" + MenuManager.Instance.CurrentLevel, bestScore);
        }
        else
        {
            SaveData data = new SaveData
            {
                playerName = playerName,
                bestScore = bestScore,
                playerWithBestScore = playerWithBestScore
            };

            string json = JsonUtility.ToJson(data);

            File.WriteAllText(Application.persistentDataPath + saveFile, json);
            //    loadButton.SetActive(true);
        }
    }
    public void LoadAllData()
    {
        if (MenuManager.Instance.usePlayerPreferences)
        {
            playerName = PlayerPrefs.GetString("playerName");
            playerWithBestScore = PlayerPrefs.GetString("playerWithBestScore" + "_" + MenuManager.Instance.CurrentLevel);
            bestScore = PlayerPrefs.GetInt("bestScore" + "_" + MenuManager.Instance.CurrentLevel);
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
