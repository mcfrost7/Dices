using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Roulette
{
    private readonly RouletteView _view;
    private readonly RouletteConfig _config;

    private List<float> _sectorAngles = new();
    private float _currentRotation;
    private float _winningAngle;

    public Roulette(RouletteView view, RouletteConfig config)
    {
        _view = view;
        _config = config;
    }

    public void GenerateRoulette()
    {
        _sectorAngles = new List<float>();
        float currentFill = 0;

        foreach (var sector in _config.Sectors)
        {
            _view.CreateSector(sector, currentFill);
            float sectorAngle = 360f * sector.Percent;
            _sectorAngles.Add(currentFill * 360f + sectorAngle);
            currentFill += sector.Percent;
        }
    }

    public IEnumerator SpinRoulette()
    {
        _winningAngle = UnityEngine.Random.Range(0, 360);
        yield return _view.Spin(UnityEngine.Random.Range(_config.MinSpinCount, _config.MaxSpinCount), _winningAngle, _config.SpinDuration, _config.SpinCurve);
        //yield return new WaitForSeconds(_config.ShowDuration);
        //yield return _view.SpinToDefault();
    }

    public SectorConfig GetConfigByAngle()
    {
        float normalizedAngle = _winningAngle % 360f;

        for (int i = 0; i < _sectorAngles.Count; i++)
        {
            float startAngle = i == 0 ? 0 : _sectorAngles[i - 1];
            float endAngle = _sectorAngles[i];

            if (normalizedAngle >= startAngle && normalizedAngle < endAngle)
            {
                return _config.Sectors[i];
            }
        }
        return _config.Sectors.Last();
    }

    public void ClearRoulette()
    {
        _sectorAngles.Clear();
        _view.ClearSectors();
    }
}
