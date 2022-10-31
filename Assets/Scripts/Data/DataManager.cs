using UnityEngine;
using System.IO;

/// <summary>
/// high-level manager script for saving and loading game data; should be present in every scene where game data is modified
/// </summary>
public class DataManager : MonoBehaviour
{
    //singleton instance
    public static DataManager Instance;
    // name of json file to load and save data from and to
    private string filename = "game_data";
    // object that stores data to be saved in a serialized format
    [HideInInspector] public GameData gameData = new GameData();
    // path where data is stored as a json
    private static string path;

    private void Awake()
    {
        // Managing DataManager persistence between scenes
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Loading data
        path = Application.persistentDataPath + "/" + filename + ".json";
        // Load();
        
        Debug.Log("LOADING DATA:");
        Debug.Log(string.Format("game data:\n{0}", gameData.ToString()));
        Debug.Log(string.Format("game data path:\n{0}", path));
    }

    public void EndDay(int currentDay, int money, ItemData[] inventory, StructureData[] structures)
    {
        gameData.day = currentDay + 1;
        gameData.money = money;
        gameData.inventory = inventory;
        gameData.structures = structures;
    }


    // Save data to a JSON file as filename.json. Will overwrite an existing file
    public void Save()
    {
        Debug.Log("SAVING DATA:");
        Debug.Log(string.Format("game data:\n{0}", gameData.ToString()));
        string json = JsonUtility.ToJson(gameData);
        System.IO.File.WriteAllText(path, json);
    }

    // Load data from a JSON file filename.json and return as a GameData object
    public void Load()
    {
        if (File.Exists(path))
            gameData = JsonUtility.FromJson<GameData>(File.ReadAllText(path));
    }
}