using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesPrefabSetup : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _amount;

    public Image Image { get => _image; set => _image = value; }
    public TextMeshProUGUI Name { get => _name; set => _name = value; }
    public TextMeshProUGUI Amount { get => _amount; set => _amount = value; }

    public void Setup(Sprite resourceImage, string resourceName, int resourceNumber)
    {
        Image.sprite = resourceImage;
        Name.text = resourceName;
        Amount.text = resourceNumber.ToString();
    }

    public void UpdateAmount(int newAmount)
    {
        Amount.text = newAmount.ToString();
    }

    public string GetResourceName()
    {
        return Name.text;
    }
}
