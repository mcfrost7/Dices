using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class UnitNameTranslator 
{
    private static readonly Dictionary<string, string> _nameMappings = new Dictionary<string, string>
    {
        // Первый конфиг
        {"Acolyte", "Послушник"},
        {"Aspirant", "Аспирант"},
        {"Cadet", "Кадет"},
        {"Initiate", "Посвященный"},
        {"Neophyte", "Неофит"},
        {"Novice", "Новичок"},
        {"Recruit", "Рекрут"},
        {"Scout", "Разведчик"},

        // Второй конфиг
        {"Apothecary", "Аптекарь"},
        {"Bastion", "Бастион"},
        {"Captain", "Капитан"},
        {"Casual", "Воин"},
        {"Lieutenant", "Лейтенант"},
        {"Razvedchick", "Разведчик"},
        {"Sergant", "Сержант"},
        {"Shturmovik", "Штурмовик"},

        // Третий конфиг
        {"Centurion", "Центурион"},
        {"Chaplain", "Капеллан"},
        {"Librarion", "Либератор"},
        {"Primarch", "Примарх"},
        {"Reiver", "Рейвер"},
        {"Techmarine", "Технический Десантник"},
        {"Terminator", "Терминатор"},
        {"Vanguard", "Авангардец"},

        {"T1_Gretchins", "Гретчинсы"},
        {"T1_Gretchins_suicide", "Гретчинсы-камикадзе"},
        {"T1_Rubaki", "Рубаки"},
        {"T1_Boyz", "Бойзы"},
        {"T2_Boyz", "Испорченные бойзы"},
        {"T2_Gretchins", "Испорченные гретчины"},
        {"T2_Rubaki", "Испорченные рубаки"},
        {"T1_Boss","Шута-бой" },
        {"T2_Boss","Шута-бой" }
    };

    public static string GetRussianName(string englishName)
    {
        if (_nameMappings.TryGetValue(englishName, out string russianName))
        {
            return russianName;
        }

        // Возвращаем оригинальное имя, если перевод не найден
        return englishName;
    }
}