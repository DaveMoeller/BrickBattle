#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Overlays;
#endif
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
public class MenuManager : MonoBehaviour
{
    public string playerName;
    public string saveFile = "/savefile.json";
    public TMP_InputField nameInputField;
    public static MenuManager Instance;
    public static GameObject MenuCanvas;
    //public MenuManager Instance;
    public List<string> previousPlayers = new List<string>();
    public TMP_Dropdown previousPlayersDropdown;
    [UnityEngine.Range(1, 30)]
    public int maxSavedPreviousPlayers = 20;
    public bool usePlayerPreferences = true;
    public string CurrentLevel = "Green";
    public ToggleGroup levelToggleGroup;
    //public TMPro.TMP_Text nameInputField;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{

    //}
    //public void Start()
    //{
    //    nameInputField = GetComponent<TMP_InputField>();
    //    Debug.Log("nameInputField: " +  nameInputField.name);
    //}
    public void Awake()
    {
        Debug.Log("gameObject.name: " + gameObject.name);

        //start of new code
        if (Instance != null)
        {
            Destroy(gameObject);
            MenuCanvas.SetActive(true);
            LoadAllData();
            return;
        }
        //end of new code

        Instance = this;
        MenuCanvas = this.gameObject;

        //DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(Instance); // same as GameObject

        LoadAllData();

        if (nameInputField != null)
        {
            nameInputField.text = playerName;
            //Assign previous players to dropdown
            previousPlayersDropdown.ClearOptions();
            previousPlayersDropdown.AddOptions(previousPlayers);
        }
        else
        {
            Debug.Log("Awake nameInputField is null");
        }
        //levelToggleGroup = GetParent<ToggleGroup>();
        if (levelToggleGroup == null)
        {
            Debug.LogError("ToggleGroup is not assigned!");
        }
    }
    public void AssignPreviousPlayer()
    {
        playerName = previousPlayers.ElementAt(previousPlayersDropdown.value);
        Debug.Log("Chosen Name:" + playerName);
        nameInputField.text = playerName;
    }
    public void SavePlayer()
    {
        if (nameInputField != null && nameInputField.text.Length > 0)
        {
            Debug.Log("Saving Player: " + nameInputField.text);
            previousPlayers.Insert(0, nameInputField.text);
            previousPlayersDropdown.ClearOptions();
            previousPlayersDropdown.AddOptions(previousPlayers);

            if (previousPlayersDropdown.options.Count > maxSavedPreviousPlayers)
            {
                previousPlayersDropdown.options.RemoveRange(maxSavedPreviousPlayers, previousPlayersDropdown.options.Count - maxSavedPreviousPlayers);
            }
        }
    }
    public void StartNew()
    {
        if (nameInputField != null)
        {
            playerName = nameInputField.text;
            SaveAllData();
        }
        else
        {
            Debug.Log("nameInputField is null");
        }

        //Test
        gameObject.SetActive(false);

        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
    public void Exit()
    {
        //if (MenuManager.Instance == null)
        //{
        //    Debug.Log("MenuManager.Instance=null");
        //}
        //else
        //{
        //    Debug.Log("MenuManager.Instance=" + MenuManager.Instance);
        //}

        MenuManager.Instance.SaveAllData();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
    class SaveData
    {
        public string playerName;
        public List<string> previousPlayers;
    }

    public void SaveAllData()
    {
        if (usePlayerPreferences)
        {
            SaveAllPreferences();
        }
        else
        {
            SaveData data = new SaveData();
            string filePath = Application.persistentDataPath + saveFile;
            data.playerName = playerName;
            data.previousPlayers = previousPlayers;

            string json = JsonUtility.ToJson(data);
            Debug.Log($"Save File Path: {filePath}\nSaveData:\n{json}");
            File.WriteAllText(filePath, json);
            //    loadButton.SetActive(true);

        }
    }
    public bool SaveFileExists()
    {
        string path = Application.persistentDataPath + saveFile;
        return File.Exists(path);
    }
    public void LoadAllData()
    {
        if (usePlayerPreferences)
        {
            LoadAllPreferences();
        }
        else
        {
            string path = Application.persistentDataPath + saveFile;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                playerName = data.playerName;
                previousPlayers = data.previousPlayers;
                //previousPlayersDropdown.ClearOptions();
                //previousPlayersDropdown.AddOptions(data.previousPlayers);
            }
        }
    }
    public void LoadAllPreferences()
    {
        //Load Preferences (cookie)
        playerName = PlayerPrefs.GetString("playerName");
        previousPlayers = PlayerPrefs.GetString("previousPlayers").Split(";").ToList();
    }
    public void SaveAllPreferences()
    {
        //Save Preferences (cookie)
        PlayerPrefs.SetString("playerName", playerName);
        PlayerPrefs.SetString("previousPlayers", string.Join(";", previousPlayers));
    }
    public void SetCurrentLevel()
    {
        Toggle toggle = levelToggleGroup.ActiveToggles().FirstOrDefault();
        if (toggle.isOn)
        {
            //string level;
            CurrentLevel = toggle.GetComponentInChildren<Text>().text.Trim();
            Debug.Log($"Setting current level to: {CurrentLevel}\n");

        }
    }
}
