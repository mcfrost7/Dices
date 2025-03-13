using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RouletteView : MonoBehaviour
{
    [SerializeField] private RectTransform _sectorParent;
    [SerializeField] private Image _sectorPrefab;

    public IEnumerator Spin(int spins, float finalAngle, float spinDuration, AnimationCurve spinCurve)
    {
        float elapsedTime = 0f;
        float targetRotation = 360f * spins + finalAngle;
        float startRotation = _sectorParent.eulerAngles.z;

        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / spinDuration;
            float curveValue = spinCurve.Evaluate(t);
            float currentAngle = Mathf.Lerp(startRotation, targetRotation, curveValue);
            _sectorParent.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }

        _sectorParent.rotation = Quaternion.Euler(0, 0, targetRotation);
    }

    public IEnumerator SpinToDefault()
    {
        float duration = 0.3f;
        float elapsedTime = 0f;
        float startRotation = _sectorParent.eulerAngles.z;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float currentAngle = Mathf.Lerp(startRotation, 0f, t);
            _sectorParent.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }

        _sectorParent.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void CreateSector(SectorConfig config, float currentFill)
    {
        var sectorImage = Instantiate(_sectorPrefab, _sectorParent);
        sectorImage.color = config.Color;
        sectorImage.fillAmount = config.Percent;
        sectorImage.fillOrigin = 2;
        sectorImage.fillClockwise = true;
        sectorImage.rectTransform.localRotation = Quaternion.Euler(0, 0, -currentFill * 360f);
    }
    public void ClearSectors()
    {
        foreach (Transform child in _sectorParent)
        {
            Destroy(child.gameObject);
        }
    }
}

