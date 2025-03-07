using System.Collections.Generic;
using UnityEngine;

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

    private List<NewUnitStats> _newUnits = new List<NewUnitStats>();

    public void CreateUnits()
    {
        _newUnits.Clear(); // Очищаем список перед генерацией

        for (int i = 0; i < _units_count; i++) // Генерируем 5 юнита 1-го уровня
        {
            NewUnitStats unitLevel1 = GenerateUnit(1);
            _newUnits.Add(unitLevel1);
        }
        SaveUnits();
        DebugUnits();
    } 

    private NewUnitStats GenerateUnit(int level)
    {
        if (level > 3) return null; // Ограничиваем уровень до 3

        // Получаем случайные данные в зависимости от уровня
        List<NewDiceConfig> diceList = GetDiceListByLevel(level);
        List<BuffConfig> buffList = GetBuffListByLevel(level);

        if (diceList.Count == 0 || buffList.Count == 0)
        {
            Debug.LogWarning($"Не найдены данные для юнитов уровня {level}!");
            return null;
        }

        // Создаем юнита
        NewUnitStats newUnit = new NewUnitStats(
            $"Unit_Level{level}_{Random.Range(1000, 9999)}", // Генерация случайного имени
            Random.Range(3, 5) * level,  // Случайное здоровье
            Random.Range(1, 3) * level,   // Случайная мораль
            level,
            diceList[Random.Range(0, diceList.Count)],
            buffList[Random.Range(0, buffList.Count)]
        );

        // Если уровень ниже 3, добавляем 2 варианта улучшения
        if (level < 3)
        {
            for (int i = 0; i < 2; i++)
            {
                NewUnitStats upgradeUnit = GenerateUnit(level + 1);
                if (upgradeUnit != null)
                {
                    newUnit._upgrade_list.Add(upgradeUnit);
                }
            }
        }

        return newUnit;
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

    // Вывести в лог созданных юнитов
    public void DebugUnits()
    {
        foreach (var unit in _newUnits)
        {
            Debug.Log(GenerateUnitDebugInfo(unit, 0));
        }
    }

    // Рекурсивный метод для форматирования информации о юните
    private string GenerateUnitDebugInfo(NewUnitStats unit, int depth)
    {
        string indent = new string('-', depth * 2); // Визуальный отступ для дерева улучшений

        string unitInfo = $"{indent}Юнит: {unit._name}\n" +
                          $"{indent}  - Уровень: {unit._level}\n" +
                          $"{indent}  - Здоровье: {unit._health}\n" +
                          $"{indent}  - Мораль: {unit._moral}\n" +
                          $"{indent}  - Текущий опыт: {unit._current_exp}\n" +
                          $"{indent}  - Кубик: {(unit._dice != null ? unit._dice.name : "Нет")}\n" +
                          $"{indent}  - Бафф: {(unit._buff != null ? unit._buff.name : "Нет")}\n";

        // Если есть улучшения, добавляем их
        if (unit._upgrade_list.Count > 0)
        {
            unitInfo += $"{indent}  - Возможные улучшения:\n";
            foreach (var upgrade in unit._upgrade_list)
            {
                unitInfo += GenerateUnitDebugInfo(upgrade, depth + 1);
            }
        }
        else
        {
            unitInfo += $"{indent}  - Нет доступных улучшений\n";
        }

        return unitInfo;
    }

    public void SaveUnits()
    {
        PlayerData playerData = SaveLoadMNG.Load();
        playerData.PlayerUnits = _newUnits;
        SaveLoadMNG.Save(playerData);
        Debug.Log("Юниты сохранены.");
    }

    public void LoadUnits()
    {
        PlayerData playerData = SaveLoadMNG.Load();
        _newUnits = playerData.PlayerUnits ?? new List<NewUnitStats>();

        if (_newUnits.Count == 0)
        {
            Debug.Log("Сохраненные юниты не найдены, создаем новых.");
            CreateUnits();
        }
        else
        {
            Debug.Log("Юниты загружены из сохранения.");
        }
    }

    public void NewGame()
    {
        Debug.Log("Начало новой игры: удаление старых юнитов и создание новых.");

        _newUnits.Clear(); // Удаляем старых юнитов
        CreateUnits(); // Создаем новых юнитов и сохраняем их
    }
}
