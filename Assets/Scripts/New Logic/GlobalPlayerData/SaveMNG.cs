using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class SaveLoadMNG : MonoBehaviour
{
    private static SaveLoadMNG _instance;
    public static SaveLoadMNG Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SaveLoadMNG");
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
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, jsonData);
            Debug.Log($"Данные успешно сохранены в {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка при сохранении данных: {e.Message}");
        }
    }

    public PlayerData LoadData()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                string jsonData = File.ReadAllText(SavePath);
                PlayerData data = JsonUtility.FromJson<PlayerData>(jsonData);
                Debug.Log("Данные успешно загружены");
                return data;
            }
            else
            {
                Debug.Log("Файл сохранения не найден. Создаём новые данные.");
                return CreateNewPlayerData();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка при загрузке данных: {e.Message}");
            return CreateNewPlayerData();
        }
    }

    private PlayerData CreateNewPlayerData()
    {
        return new PlayerData();
    }

    // Метод для быстрого доступа к сохранению
    public static void Save(PlayerData data)
    {
        Instance.SaveData(data);
    }

    // Метод для быстрого доступа к загрузке
    public static PlayerData Load()
    {
        return Instance.LoadData();
    }

    // Метод для проверки наличия сохранения
    public static bool SaveExists()
    {
        return File.Exists(Instance.SavePath);
    }

    // Метод для удаления сохранения
    public static void DeleteSave()
    {
        if (SaveExists())
        {
            File.Delete(Instance.SavePath);
            Debug.Log("Сохранение удалено");
        }
    }
}