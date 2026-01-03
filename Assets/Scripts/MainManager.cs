// Data Persistence
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainManager : MonoBehaviour
{
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
    public static MainManager Instance;
    public int bestScore = 0;
    public string playerWithBestScore = "";
    // Start is called before the first frame update
    void Start()
    {
        Setup();
        if (MenuManager.Instance != null)
        {
            playerName = MenuManager.Instance.playerName;
        }
        else
        {
            playerName = "Player 01";
        }
    }
    private void Reset()
    {
        Setup();
    }
    void Setup()
    {
        const float step = 0.6f;
         int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 2, 3, 4, 5, 6 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
        LoadAllData();
        if (MenuManager.Instance != null)
        {
            playerName = MenuManager.Instance.playerName;
        }
        ScoreText.text = playerName + $" Score : {m_Points}";
        bestScoreText.text = "Best Score: " + playerWithBestScore + " : " + bestScore;
    }
    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Ball.transform.SetParent(ballParentTransform);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                LoadAllData();
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
            //PlayerPrefs.SetString("playerName", playerName);
            PlayerPrefs.SetString("playerWithBestScore", playerWithBestScore);
            PlayerPrefs.SetInt("bestScore", bestScore);
        }
        else
        {
            SaveData data = new SaveData();
            data.playerName = playerName;
            data.bestScore = bestScore;
            data.playerWithBestScore = playerWithBestScore;

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
            playerWithBestScore = PlayerPrefs.GetString("playerWithBestScore");
            bestScore = PlayerPrefs.GetInt("bestScore");
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
        Debug.Log("Loading scene 0");
        SceneManager.LoadScene(0);
    }

}
