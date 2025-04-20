using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Loading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static CanvasMapGenerator;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _headerField;
    [SerializeField] private TextMeshProUGUI _contentField;
    [SerializeField] private LayoutElement _layoutElement;
    [SerializeField] private RectTransform _rectTransform;

    private int _characterWrapLimitl = 80;

    public void SetText(string content = "content", string header = "")
    {

        if (string.IsNullOrEmpty(header))
        {
            _headerField.gameObject.SetActive(false);
        }
        else
        {
            _headerField.gameObject.SetActive(true);
            _headerField.text = header;
        }

        _contentField.text = content;

        int _headerLength = _headerField.text.Length;
        int _contentLength = _contentField.text!=null ? _contentField.text.Length : 0 ;

        _layoutElement.enabled = (_headerLength > _characterWrapLimitl || _contentLength > _characterWrapLimitl) ? true : false;

    }
  
    private void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        float pivotX = mousePos.x > Screen.width / 2 ? 1 : 0; 
        float pivotY = mousePos.y > Screen.height / 2 ? 1 : 0; 

        _rectTransform.pivot = new Vector2(pivotX, pivotY);

        float offsetX = pivotX == 0 ? 10 : -10; 
        float offsetY = pivotY == 0 ? 10 : -10; 

        _rectTransform.position = mousePos + new Vector2(offsetX, offsetY);
    }
}
