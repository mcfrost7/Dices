using NUnit.Framework;
using UnityEngine;

public class UnitStatsTests
{
    private TypesInfo.Type testType;
    private UnitStats unitStats;

    [SetUp]
    public void SetUp()
    {
        // Создание тестовой текстуры для спрайта
        Texture2D texture = new Texture2D(64, 64);  // Размер текстуры 64x64 пикселя
        texture.SetPixels(new Color[64 * 64]);  // Инициализация текстуры пикселями

        // Применяем текстуру к спрайту
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), Vector2.zero);

        // Создаем тестовый тип
        testType = new TypesInfo.Type
        {
            TypeName = "TestType",
            Sprite = sprite,
            Level = 1,
            Dice = ScriptableObject.CreateInstance<DiceConfig>()
        };

        // Создаем объект UnitStats с начальными параметрами
        unitStats = new UnitStats(100, 50, testType);
    }

    [TearDown]
    public void TearDown()
    {
        // Очистка ресурсов после теста
        Object.DestroyImmediate(testType.Sprite.texture);  // Уничтожение текстуры
        Object.DestroyImmediate(testType.Dice);  // Уничтожение DiceConfig
    }

    [Test]
    public void UnitStats_Initialization_CorrectValues()
    {
        // Проверка инициализации
        Assert.AreEqual(100, unitStats.Health);
        Assert.AreEqual(50, unitStats.Moral);
        Assert.AreEqual(100, unitStats.CurrentHealth);
        Assert.AreEqual(testType.Sprite, unitStats.Sprite);
        Assert.IsTrue(unitStats.IsPlayerUnit);
    }

    [Test]
    public void UnitStats_UpdateHealth_IncreasesHealth()
    {
        // Проверяем, что здоровье увеличивается
        unitStats.UpdateHealth(20);
        Assert.AreEqual(100, unitStats.CurrentHealth);  // Здоровье не должно превышать максимум

        unitStats.UpdateHealth(-50);
        Assert.AreEqual(50, unitStats.CurrentHealth);  // Здоровье не может быть меньше нуля

        unitStats.UpdateHealth(-100);
        Assert.AreEqual(0, unitStats.CurrentHealth);  // Здоровье не может быть меньше нуля
    }

    [Test]
    public void UnitStats_UpdateMoral_IncreasesMoral()
    {
        // Проверка обновления морали
        unitStats.UpdateMoral(20);
        Assert.AreEqual(70, unitStats.Moral);  // Мораль должна увеличиться на 20

        unitStats.UpdateMoral(-60);
        Assert.AreEqual(10, unitStats.Moral);  // Мораль не может быть меньше 0

        unitStats.UpdateMoral(200);
        Assert.AreEqual(100, unitStats.Moral);  // Мораль не может быть больше 100
    }

    [Test]
    public void UnitStats_HealthCannotExceedMax()
    {
        // Проверка, что здоровье не может превысить максимум
        unitStats.UpdateHealth(50);  // Здоровье становится 150, но должно быть 100
        Assert.AreEqual(100, unitStats.CurrentHealth);
    }

    [Test]
    public void UnitStats_MoralCannotExceedMax()
    {
        // Проверка, что мораль не может превышать 100
        unitStats.UpdateMoral(100);  // Мораль становится 150, но должна быть ограничена 100
        Assert.AreEqual(100, unitStats.Moral);
    }

    [Test]
    public void UnitStats_MoralCannotGoBelowZero()
    {
        // Проверка, что мораль не может быть меньше 0
        unitStats.UpdateMoral(-20);  // Мораль становится -10, но должна быть ограничена 0
        Assert.AreEqual(30, unitStats.Moral);
    }
}
