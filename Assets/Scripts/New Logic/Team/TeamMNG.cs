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
    }

    private const int MAX_UPGRADE_COUNT = 2; // Максимальное количество улучшений

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

        // Создаём новый кубик для юнита
        Dice dice = new Dice
        {
            items = new List<ItemConfig>(),
            diceConfig = diceList[Random.Range(0, diceList.Count)]
        };

        // Создаём юнита
        NewUnitStats newUnit = new NewUnitStats(
            $"Unit_Level{level}_{Random.Range(1000, 9999)}", // Случайное имя
            Random.Range(3, 5) * level,  // Случайное здоровье
            Random.Range(1, 3) * level,   // Случайная мораль
            level,
            dice,
            tempBuffList
        );

        // Если уровень ниже 3, добавляем 2 варианта улучшения (не _units_count)
        if (level < 3)
        {
            int upgradesCount = Mathf.Min(MAX_UPGRADE_COUNT, _units_count); // Ограничиваем улучшения
            for (int i = 0; i < upgradesCount; i++)
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

    public void SaveUnits()
    {
        GameDataMNG.Instance.PlayerData.PlayerUnits = _newUnits;
        Debug.Log("Юниты сохранены.");
    }

    public void LoadUnits()
    {
        _newUnits = GameDataMNG.Instance.PlayerData.PlayerUnits ?? new List<NewUnitStats>();

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
