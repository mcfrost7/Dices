using UnityEngine;

public class LocationMapStarter : MonoBehaviour
{
    [SerializeField] private int _height;
    [SerializeField] private int _width;
    private void Start()
    {
        // Получаем компонент генератора карт на том же объекте
        LocationMapGenerator mapGenerator = GetComponent<LocationMapGenerator>();

        // Проверяем наличие генератора и локаций
        if (mapGenerator != null && mapGenerator.LocationConfigs.Count > 0)
        {
            // Генерируем первую локацию из списка
            mapGenerator.GenerateRandomLocationMap(mapGenerator.LocationConfigs[0], _width, _height);
        }
        else
        {
            Debug.LogError("Не найден генератор карт или список локаций пуст");
        }
    }
}