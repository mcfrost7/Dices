using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Start()
    {
        // Finds the first CanvasMapGenerator in the scene
        CanvasMapGenerator mapGenerator = FindObjectOfType<CanvasMapGenerator>();
        mapGenerator.EditorGenerateMap();
    }
}