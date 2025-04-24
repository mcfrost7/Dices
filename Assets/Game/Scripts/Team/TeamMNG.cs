using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TeamMNG : MonoBehaviour
{
    public static TeamMNG Instance { get; private set; }

    [SerializeField] private List<NewDiceConfig> _diceConfigLevel1;
    [SerializeField] private List<NewDiceConfig> _diceConfigLevel2;
    [SerializeField] private List<NewDiceConfig> _diceConfigLevel3;
    [SerializeField] private List<BuffConfig> _buffsConfigLevel1;
    [SerializeField] private List<BuffConfig> _buffsConfigLevel2;
    [SerializeField] private List<BuffConfig> _buffsConfigLevel3;
    [SerializeField] private int _units_count;

    private List<int> _playerUnitIDs = new List<int>(); // Хранит только ID юнитов игрока
    private Dictionary<int, NewUnitStats> _unitsCache = new Dictionary<int, NewUnitStats>(); // Кэш для быстрого доступа
    private int _nextUnitID = 1; // Начинаем с 1, т.к. 0 может использоваться как специальное значение
    private const int MAX_UPGRADE_COUNT = 2; // Максимальное количество улучшений
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void CreateUnits()
    {
        _playerUnitIDs.Clear(); // Очищаем список ID юнитов игрока
        _unitsCache.Clear(); // Очищаем кэш

        for (int i = 0; i < _units_count; i++) // Генерируем указанное количество юнитов 1-го уровня
        {
            NewUnitStats unitLevel1 = GenerateUnit(1);
            if (unitLevel1 != null)
            {
                _playerUnitIDs.Add(unitLevel1._ID); // Добавляем только ID
            }
        }
        SaveUnits(); // Сохраняем в PlayerData
    }

    private NewUnitStats GenerateUnit(int level)
    {
        if (level > 3) return null; // Ограничиваем уровень до 3

        // Получаем списки конфигураций
        List<NewDiceConfig> diceList = GetDiceListByLevel(level);
        List<BuffConfig> buffList = GetBuffListByLevel(level);

        // Проверка наличия данных
        if (diceList.Count == 0 || buffList.Count == 0)
        {
            Debug.LogWarning($"Не найдены данные для юнитов уровня {level}!");
            return null;
        }

        // Генерация случайного количества бафов (от 0 до уровня)
        List<BuffConfig> tempBuffList = GenerateBuffs(level, buffList);
        int unitID;
        NewUnitStats newUnit;
        CreateUnitStats(level, diceList, tempBuffList, out unitID, out newUnit);

        // Добавляем юнит в кэш
        _unitsCache[unitID] = newUnit;

        AddUpgradeToUnit(level, newUnit);

        return newUnit;
    }

    private void AddUpgradeToUnit(int level, NewUnitStats newUnit)
    {
        // Если уровень ниже 3, добавляем варианты улучшения
        if (level < 3)
        {
            int upgradesCount = MAX_UPGRADE_COUNT; // Ограничиваем улучшения
            for (int i = 0; i < upgradesCount; i++)
            {
                NewUnitStats upgradeUnit = GenerateUnit(level + 1);
                if (upgradeUnit != null)
                {
                    // Добавляем ID улучшенного юнита в список улучшений текущего юнита
                    newUnit._upgrade_list.Add(upgradeUnit._ID);
                }
            }
        }
    }

    private void CreateUnitStats(int level, List<NewDiceConfig> diceList, List<BuffConfig> tempBuffList, out int unitID, out NewUnitStats newUnit)
    {

        // Создаём новый кубик для юнита
        Dice dice = new Dice
        (
            diceList[Random.Range(0, diceList.Count)],
            new List<ItemInstance>()
        );

        // Сгенерировать уникальный ID
        unitID = _nextUnitID++;

        // Создаём юнита
        newUnit = new NewUnitStats(
            $"{dice._diceConfig.name}", // имя
            Random.Range(3, 6) * level,  // Случайное здоровье
            Random.Range(1, 3) * level,   // Случайная мораль
            level,
            dice,
            tempBuffList
        );

        // Устанавливаем ID юнита
        newUnit._ID = unitID;
    }

    private List<BuffConfig> GenerateBuffs(int level, List<BuffConfig> buffList)
    {
        int randomBuffsCount = Mathf.Min(Random.Range(0, level + 1), buffList.Count);
        List<BuffConfig> tempBuffList = new List<BuffConfig>();
        List<BuffConfig> availableBuffs = new List<BuffConfig>(buffList); // Копия списка, чтобы удалять использованные бафы

        for (int i = 0; i < randomBuffsCount; i++)
        {
            if (availableBuffs.Count == 0) break; // Если бафов не осталось, выходим

            int randomIndex = Random.Range(0, availableBuffs.Count);
            tempBuffList.Add(availableBuffs[randomIndex]);
            availableBuffs.RemoveAt(randomIndex); // Удаляем выбранный бафф
        }

        return tempBuffList;
    }

    // Получение списка кубиков по уровню
    private List<NewDiceConfig> GetDiceListByLevel(int level)
    {
        return level switch
        {
            1 => _diceConfigLevel1,
            2 => _diceConfigLevel2,
            3 => _diceConfigLevel3,
            _ => new List<NewDiceConfig>()
        };
    }

    // Получение списка баффов по уровню
    private List<BuffConfig> GetBuffListByLevel(int level)
    {
        return level switch
        {
            1 => _buffsConfigLevel1,
            2 => _buffsConfigLevel2,
            3 => _buffsConfigLevel3,
            _ => new List<BuffConfig>()
        };
    }

    // Сохранение юнитов в PlayerData
    public void SaveUnits()
    {
        // Сохраняем текущие юниты игрока (только активные юниты)
        GameDataMNG.Instance.PlayerData.PlayerUnits = _playerUnitIDs
            .Select(id => _unitsCache[id])
            .ToList();

        // Сохраняем все юниты в хранилище
        GameDataMNG.Instance.PlayerData.UnitsStorage = _unitsCache.Values.ToList();

        Debug.Log($"Юниты сохранены. Активных юнитов: {_playerUnitIDs.Count}, Всего юнитов: {_unitsCache.Count}");
    }

    // Загрузка юнитов из PlayerData
    public void LoadUnits(PlayerData playerData)
    {
        // Очищаем текущие данные
        _unitsCache.Clear();
        _playerUnitIDs.Clear();

        // Загружаем все юниты из хранилища в кэш
        if (playerData.UnitsStorage != null && playerData.UnitsStorage.Count > 0)
        {
            foreach (var unit in playerData.UnitsStorage)
            {
                _unitsCache[unit._ID] = unit;
                // Обновляем nextUnitID, чтобы новые ID не конфликтовали
                if (unit._ID >= _nextUnitID)
                {
                    _nextUnitID = unit._ID + 1;
                }
            }

            // Загружаем ID активных юнитов игрока
            if (playerData.PlayerUnits != null && playerData.PlayerUnits.Count > 0)
            {
                _playerUnitIDs = playerData.PlayerUnits.Select(u => u._ID).ToList();
                Debug.Log($"Юниты загружены из сохранения. Активных юнитов: {_playerUnitIDs.Count}, Всего юнитов: {_unitsCache.Count}");
            }
            else
            {
                Debug.Log("Активные юниты не найдены, но есть юниты в хранилище.");
                // Берем юниты из хранилища, если активных юнитов нет
                SelectInitialUnits();
            }
        }
        else
        {
            Debug.Log("Сохраненные юниты не найдены, создаем новых.");
            CreateUnits();
        }
    }

    public void GenerateAndAddUnit(int level)
    {
        NewUnitStats newUnit = GenerateUnit(level);
        if (newUnit != null)
        {
            _playerUnitIDs.Add(newUnit._ID); // Добавляем ID нового юнита в список активных
            _unitsCache[newUnit._ID] = newUnit; // Добавляем в кэш
            SaveUnits(); // Сохраняем изменения
            Debug.Log($"Добавлен новый юнит ID: {newUnit._ID}, Уровень: {level}");
            UnitsPanelUI.Instance.OnMenuLoad();
            GameDataMNG.Instance.SaveGame();
        }
    }

    // Выбор начальных юнитов из хранилища (например, при первом запуске)
    private void SelectInitialUnits()
    {
        // Выбираем юнитов 1 уровня из хранилища, если они есть
        var level1Units = _unitsCache.Values
            .Where(u => u._level == 1)
            .Take(_units_count)
            .ToList();

        if (level1Units.Count >= _units_count)
        {
            // Если достаточно юнитов 1 уровня, берем их
            _playerUnitIDs = level1Units.Select(u => u._ID).ToList();
        }
        else
        {
            // Создаем новых юнитов
            CreateUnits();
        }
    }

    // Начало новой игры - сброс и создание новых юнитов
    public void NewGame()
    {
        Debug.Log("Начало новой игры: удаление старых юнитов и создание новых.");
        _unitsCache.Clear();
        _playerUnitIDs.Clear();
        _nextUnitID = 1; // Сбрасываем счетчик ID
        CreateUnits(); // Создаем новых юнитов и сохраняем их
    }

    // Получение юнита по ID
    public NewUnitStats GetUnitByID(int id)
    {
        if (_unitsCache.TryGetValue(id, out NewUnitStats unit))
        {
            return unit;
        }
        return null;
    }

    // Получение всех активных юнитов игрока
    public List<NewUnitStats> GetPlayerUnits()
    {
        SaveUnits();
        return _playerUnitIDs
            .Where(id => _unitsCache.ContainsKey(id))
            .Select(id => _unitsCache[id])
            .ToList();
    }

    // Добавление юнита к активным юнитам игрока
    public void AddUnitToPlayer(int unitID)
    {
        if (_unitsCache.ContainsKey(unitID) && !_playerUnitIDs.Contains(unitID))
        {
            _playerUnitIDs.Add(unitID);
            SaveUnits();
        }
    }

    // Удаление юнита из активных юнитов игрока
    public void RemoveUnitFromPlayer(int unitID)
    {
        if (_playerUnitIDs.Contains(unitID))
        {
            _playerUnitIDs.Remove(unitID);
            SaveUnits();
        }
    }

    // Заменить юнит игрока на улучшенную версию
    public void UpgradePlayerUnit(int oldUnitID, int newUnitID)
    {
        if (_playerUnitIDs.Contains(oldUnitID) && _unitsCache.ContainsKey(newUnitID))
        {
            ItemMNG.Instance.ReplaceItemsOnUpgrade(GetUnitByID(oldUnitID));
            int index = _playerUnitIDs.IndexOf(oldUnitID);
            if (index >= 0)
            {
                _playerUnitIDs[index] = newUnitID;
                SaveUnits();
                UnitsPanelUI.Instance.OnMenuLoad();
                GameDataMNG.Instance.SaveGame();
            }
        }
    }
}