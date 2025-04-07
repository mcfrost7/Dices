using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleActionManager : MonoBehaviour
{
    [SerializeField] private Button _endAction;

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
        _endAction.onClick.AddListener(EndActionButon);
    }

    public event System.Action ActionComplete;
    public static BattleActionManager Instance { get; private set; }
    public Button EndAction { get => _endAction; set => _endAction = value; }

    public void CalculateAvailableActions() { }

    public void EndActionButon()
    {
        ActionComplete?.Invoke();
        _endAction.gameObject.SetActive(false);
        BattleRerolls.Instance.EndRerolls.gameObject.SetActive(true);
        BattleRerolls.Instance.EndRerolls.enabled = false;
    }
}
