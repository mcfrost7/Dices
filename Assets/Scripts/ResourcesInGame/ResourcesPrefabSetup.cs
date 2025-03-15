using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesPrefabSetup : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _amount;

    public void Setup(Sprite resourceImage, string resourceName, int resourceNumber)
    {
        _image.sprite = resourceImage;
        _name.text = resourceName;
        _amount.text = resourceNumber.ToString();
    }

    public void UpdateAmount(int newAmount)
    {
        _amount.text = newAmount.ToString();
    }

}
