using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesTypeSetup : MonoBehaviour
{
    [SerializeField] private Image image_resource;
    [SerializeField] private TextMeshProUGUI name_resource;
    [SerializeField] private TextMeshProUGUI number_resource;

    public void Setup(Sprite resourceImage, string resourceName, int resourceNumber)
    {
        if (image_resource != null)
        {
            image_resource.sprite = resourceImage;
        }

        if (name_resource != null)
        {
            name_resource.text = resourceName;
        }

        if (number_resource != null)
        {
            number_resource.text = resourceNumber.ToString();
        }
    }
}
