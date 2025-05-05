using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewBuffConfig", menuName = "Buff/Buff Config")]
public class BuffConfig : ScriptableObject
{
    public ActionType buffType;
    public int buffPower = 0;
    public string buffName;
    public Sprite buffSprite;

    [SerializeField] private string configName;

    public string ConfigName { get => configName; set => configName = value; }

    private void OnValidate()
    {
        // Автоматически обновляет configName при изменении в инспекторе
        if (string.IsNullOrEmpty(configName) || name != configName)
        {
            configName = this.name;
#if UNITY_EDITOR
            EditorUtility.SetDirty(this); // Помечает объект как "грязный" для сохранения
#endif
        }
    }
}
