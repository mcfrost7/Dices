using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Linq;

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
            // Создаем копию данных, чтобы не модифицировать оригинал
            PlayerData dataCopy = PrepareDataForSave(data);

            string jsonData = JsonUtility.ToJson(dataCopy, true);
            File.WriteAllText(SavePath, jsonData);
            Debug.Log($"Данные успешно сохранены в {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка при сохранении данных: {e.Message}");
            Debug.LogException(e);
        }
    }

    // Подготовка данных к сериализации
    private PlayerData PrepareDataForSave(PlayerData data)
    {
        // Создаем копию объекта PlayerData
        PlayerData dataCopy = new PlayerData();

        // Копируем ноды карты
        if (data.MapNodes != null)
        {
            dataCopy.MapNodes = new List<MapNodeData>(data.MapNodes);
        }

        // Обрабатываем юниты игрока
        if (data.PlayerUnits != null)
        {
            dataCopy.PlayerUnits = data.PlayerUnits.Select(unit => CloneUnitWithoutCircularReferences(unit)).ToList();
        }

        // Обрабатываем хранилище юнитов
        if (data.UnitsStorage != null)
        {
            dataCopy.UnitsStorage = data.UnitsStorage.Select(unit => CloneUnitWithoutCircularReferences(unit)).ToList();
        }

        return dataCopy;
    }

    // Создание клона юнита без циклических ссылок
    private NewUnitStats CloneUnitWithoutCircularReferences(NewUnitStats unit)
    {
        if (unit == null) return null;

        NewUnitStats clone = new NewUnitStats(
            unit._name,
            unit._health,
            unit._moral,
            unit._level,
            CloneDice(unit._dice),
            CloneBuffList(unit._buffs)
        );

        // Копируем ID
        clone._ID = unit._ID;
        clone._current_exp = unit._current_exp;

        // Копируем список ID улучшений
        if (unit._upgrade_list != null)
        {
            clone._upgrade_list = new List<int>(unit._upgrade_list);
        }

        return clone;
    }

    // Клонирование кубика
    private Dice CloneDice(Dice dice)
    {
        if (dice == null) return null;

        Dice clone = new Dice
        {
            diceConfig = dice.diceConfig
        };

        if (dice.items != null)
        {
            clone.items = new List<ItemConfig>(dice.items);
        }

        return clone;
    }

    // Клонирование списка баффов
    private List<BuffConfig> CloneBuffList(List<BuffConfig> buffs)
    {
        if (buffs == null) return null;
        return new List<BuffConfig>(buffs);
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

                // Восстанавливаем ссылки между объектами после загрузки
                RestoreReferencesAfterLoad(data);

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
            Debug.LogException(e);
            return CreateNewPlayerData();
        }
    }

    // Восстановление ссылок между объектами после загрузки
    private void RestoreReferencesAfterLoad(PlayerData data)
    {
        // Создаем словарь для быстрого доступа к юнитам по ID
        Dictionary<int, NewUnitStats> unitsById = new Dictionary<int, NewUnitStats>();

        // Сначала добавляем все юниты из хранилища в словарь
        if (data.UnitsStorage != null)
        {
            foreach (var unit in data.UnitsStorage)
            {
                if (unit != null && !unitsById.ContainsKey(unit._ID))
                {
                    unitsById[unit._ID] = unit;
                }
            }
        }

        // Затем добавляем юниты игрока, если они еще не в словаре
        if (data.PlayerUnits != null)
        {
            foreach (var unit in data.PlayerUnits)
            {
                if (unit != null && !unitsById.ContainsKey(unit._ID))
                {
                    unitsById[unit._ID] = unit;
                }
            }
        }

        // Теперь, когда у нас есть словарь всех юнитов, мы можем восстановить ссылки
        // между юнитами, используя их ID. Эта часть может быть адаптирована 
        // в зависимости от того, как вы планируете использовать ID в структуре данных.
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