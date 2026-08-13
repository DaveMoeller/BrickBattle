using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum GAME_LEVELS { Green, Purple, Chocolate, Blue, Silver, Gold };

public class GameUIData : MonoBehaviour
{
    public static GameUIData Instance;
    public string CurrentLevel;
    public GAME_LEVELS gameLevel;
    public GameLevelData[] gameLevelData;
    private Dictionary<string, GameLevelData> gameDictionary = new();
    private int numberOfBricks = 0;
    public GameLevelData GetGameLevelData(string level)
    {
        return gameDictionary[level];
    }
    public void AddBrick()
    {
        numberOfBricks++;
    }
    public void RemoveBrick()
    {
        numberOfBricks--;
    }
    public int GetNumberOfBricks() { return numberOfBricks; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance != null)
        {
            return;
        }
        else
        {
            if (gameLevelData.Length != 6)
            {
                Debug.LogError("There needs to be 6 gameLevelData elements defined!");
                return;
            }
            Instance = this;
            DontDestroyOnLoad(Instance);
            //Set defaults
            CurrentLevel = gameLevelData[0].name;
            gameLevel = 0;
            // set up dictionary
            for (int i = 0; i < gameLevelData.Length; i++)
            {
                if (gameDictionary.ContainsKey(gameLevelData[i].name))
                {
                    Debug.LogError($"Key is duplicated: {gameLevelData[i].name}");
                }
                else
                {
                    gameDictionary.Add(gameLevelData[i].name, gameLevelData[i]);
                }
            }
        }
    }

}
[System.Serializable]
public struct GameLevelData
{
    public string name;
    public GAME_LEVELS gameLevel;
    [Range(1, 100)]
    public int levelPoints;
    public Material levelMaterial;
    public Toggle levelToggle;
    [Range(0.0f, 1.0f)]
    public float randomRangeLow;
    [Range(0.0f, 1.0f)]
    public float randomRangeHigh;
    [Range(1.0f, 5.0f)]
    public float paddleSpeed;
    [Range(0.1f, 5.0f)]
    public float ballVelocity;
    [Range(1.0f, 5.0f)]
    public float ballVelocityMax;
    public GameLevelData(GAME_LEVELS level, Material material, Toggle toggle, int points = 1)
    {
        name = "Game Levels";
        gameLevel = level;
        levelPoints = points;
        levelMaterial = material;
        levelToggle = toggle;
        randomRangeLow = 0.0f;
        randomRangeHigh = 0.0f;
        paddleSpeed = 2.0f;
        ballVelocity = 0.6f;
        ballVelocityMax = 1.0f;
    }
}

