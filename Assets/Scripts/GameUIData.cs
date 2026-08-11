using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum GAME_LEVELS { Green, Purple, Chocolate, Blue, Silver, Gold };

public class GameUIData : MonoBehaviour
{
    public static GameUIData Instance;
    public string CurrentLevel = "Green";
    public GAME_LEVELS gameLevel = GAME_LEVELS.Green;
    public GameLevelData[] gameLevelData;
    private Dictionary<string, GameLevelData> gameDictionary = new();
    public GameLevelData GetGameLevelData(string level)
    {
        return gameDictionary[level];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance != null)
        {
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            // set up dictionary
            for (int i = 0; i < gameLevelData.Length; i++)
            {
                if (gameDictionary.ContainsKey(gameLevelData[i].gameLevel.ToString()))
                {
                    Debug.LogError($"Key is duplicated: {gameLevelData[i].gameLevel}");
                }
                else
                {
                    gameDictionary.Add(gameLevelData[i].gameLevel.ToString(), gameLevelData[i]);
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
    public GameLevelData(GAME_LEVELS level, Material material, Toggle toggle, int points = 1)
    {
        name = "Game Levels";
        gameLevel = level;
        levelPoints = points;
        levelMaterial = material;
        levelToggle = toggle;
        randomRangeLow = 0.0f;
        randomRangeHigh = 0.0f;
    }
}

