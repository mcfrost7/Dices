using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveLoadMNG : MonoBehaviour
{
    private static SaveLoadMNG _instance;
    public static SaveLoadMNG Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SaveLoadManager");
                _instance = go.AddComponent<SaveLoadMNG>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private string SavePath => Path.Combine(Application.persistentDataPath, "savedata.json");

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveData(PlayerData data)
    {
        try
        {
            if (data.ResourcesData != null && data.ResourcesData.Count > 0)
            {
                data.ResourcesData = new List<ResourceData>(data.ResourcesData);
            }
            SerializablePlayerData serializableData = new SerializablePlayerData(data);
            string jsonData = JsonUtility.ToJson(serializableData, true);
            File.WriteAllText(SavePath, jsonData);
            Debug.Log($"Данные успешно сохранены в {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка при сохранении данных: {e.Message}");
            Debug.LogException(e);
        }
    }

    public PlayerData LoadData()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                string jsonData = File.ReadAllText(SavePath);
                SerializablePlayerData serializableData = JsonUtility.FromJson<SerializablePlayerData>(jsonData);
                PlayerData data = serializableData.ToPlayerData();
                Debug.Log("Данные успешно загружены");

                return data;
            }
            else
            {
                Debug.Log("Файл сохранения не найден. Создаются новые данные.");
                return CreateNewPlayerData();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка при загрузке данных: {e.Message}");
            Debug.LogException(e);
            return CreateNewPlayerData();
        }
    }

    private PlayerData CreateNewPlayerData()
    {
        return new PlayerData();
    }

    // Static methods for quick access
    public static void Save(PlayerData data)
    {
        Instance.SaveData(data);
    }

    public static PlayerData Load()
    {
        return Instance.LoadData();
    }

    public static bool SaveExists()
    {
        return File.Exists(Instance.SavePath);
    }

    public static void DeleteSave()
    {
        if (SaveExists())
        {
            File.Delete(Instance.SavePath);
            Debug.Log("Save file deleted");
        }
    }
}