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
            // Create a copy of data to avoid modifying the original
            PlayerData dataCopy = PrepareDataForSave(data);

            string jsonData = JsonUtility.ToJson(dataCopy, true);
            File.WriteAllText(SavePath, jsonData);
            Debug.Log($"Data successfully saved to {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving data: {e.Message}");
            Debug.LogException(e);
        }
    }

    // Prepare data for serialization
    private PlayerData PrepareDataForSave(PlayerData data)
    {
        PlayerData dataCopy = new PlayerData();

        // Copy map nodes
        if (data.MapNodes != null)
        {
            dataCopy.MapNodes = new List<MapNodeData>(data.MapNodes);
        }

        // Process player units
        if (data.PlayerUnits != null)
        {
            dataCopy.PlayerUnits = data.PlayerUnits.Select(unit => CloneUnitWithoutCircularReferences(unit)).ToList();
        }

        // Process storage units
        if (data.UnitsStorage != null)
        {
            dataCopy.UnitsStorage = data.UnitsStorage.Select(unit => CloneUnitWithoutCircularReferences(unit)).ToList();
        }

        // Convert resources to serializable format
        if (data.Resources != null)
        {
            dataCopy.ResourcesData = data.Resources.Select(r => r.ToSerializable()).ToList();
        }

        // Copy items
        if (data.Items != null)
        {
            dataCopy.Items = new List<ItemConfig>(data.Items);
        }

        return dataCopy;
    }

    // Create a clone of unit without circular references
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

        // Copy ID and experience
        clone._ID = unit._ID;
        clone._current_exp = unit._current_exp;

        // Copy upgrade ID list
        if (unit._upgrade_list != null)
        {
            clone._upgrade_list = new List<int>(unit._upgrade_list);
        }

        return clone;
    }

    // Clone dice object
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

    // Clone buff list
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
                Debug.Log("Data successfully loaded");

                // Restore object references after loading
                RestoreReferencesAfterLoad(data);

                return data;
            }
            else
            {
                Debug.Log("Save file not found. Creating new data.");
                return CreateNewPlayerData();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading data: {e.Message}");
            Debug.LogException(e);
            return CreateNewPlayerData();
        }
    }

    // Restore object references after loading
    private void RestoreReferencesAfterLoad(PlayerData data)
    {
        // Create dictionary for quick access to units by ID
        Dictionary<int, NewUnitStats> unitsById = new Dictionary<int, NewUnitStats>();

        // First add all units from storage to dictionary
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

        // Then add player units if they're not already in the dictionary
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

        // Restore resources from serialized data
        if (data.ResourcesData != null && data.ResourcesData.Count > 0)
        {
            data.Resources = new List<ResourceData>();

            // Find all resource configs in the project
            ResourceConfig[] allConfigs = Resources.FindObjectsOfTypeAll<ResourceConfig>();

            foreach (var serResource in data.ResourcesData)
            {
                // Find matching config by name
                ResourceConfig matchingConfig = allConfigs.FirstOrDefault(c => c.ResourceName == serResource.ConfigID);

                if (matchingConfig != null)
                {
                    data.Resources.Add(new ResourceData(matchingConfig, serResource.Count));
                }
                else
                {
                    Debug.LogWarning($"Could not find resource config with name {serResource.ConfigID}");
                }
            }
        }

        // Clear ResourcesData after it's been processed
        data.ResourcesData = null;
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