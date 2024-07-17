using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ReactorEnergyController : MonoBehaviour
{
    private PlayerController _playerController;
    private GameObject[] _weaponsEnergyPanelBarTiles;
    private GameObject[] _enginesEnergyPanelBarTiles;
    private GameObject[] _shieldsEnergyPanelBarTiles;
    [SerializeField] private GameObject _weaponsEnergyPanel;
    [SerializeField] private GameObject _enginesEnergyPanel;
    [SerializeField] private GameObject _shieldsEnergyPanel;
    private bool _isShieldsActive;

    private void Awake()
    {
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        _weaponsEnergyPanelBarTiles = _weaponsEnergyPanel.GetComponentsInChildren<Image>(true)
            .Where(x => x.gameObject.name.StartsWith("BarTile"))
            .Select(x => x.gameObject)
            .ToArray();
        _enginesEnergyPanelBarTiles = _enginesEnergyPanel.GetComponentsInChildren<Image>(true)
            .Where(x => x.gameObject.name.StartsWith("BarTile"))
            .Select(x => x.gameObject)
            .ToArray();
        _shieldsEnergyPanelBarTiles = _shieldsEnergyPanel.GetComponentsInChildren<Image>(true)
            .Where(x => x.gameObject.name.StartsWith("BarTile"))
            .Select(x => x.gameObject)
            .ToArray();
    }

    private void Start()
    {
        DistributeEnergyByDefault();
    }

    private void OnEnable()
    {
    }

    private void Update()
    {
        // на стрелочку влево добавляем энергию к оружию
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            AddEnergyUnitToWeapons();
        }
        // на стрелочку вверху добавляем энергию к двигателям
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            AddEnergyUnitToEngines();
        }
        // на стрелочку вправо добавляем энергию к щитам
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (_isShieldsActive)
            {
                AddEnergyUnitToShields();
            }
        }
        // на стрелочку вниз распределяем энергию по умолчанию
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            DistributeEnergyByDefault();
        }
    }

    public void SetActiveBarTile(GameObject[] barTiles, int tileIndex, bool isActive)
    {
        barTiles[tileIndex].SetActive(isActive);
    }

    public void FillEnergyBar(GameObject[] barTiles, int tilesCount)
    {
        for (int i = 0; i < barTiles.Length; i++)
        {
            if (i < tilesCount)
            {
                SetActiveBarTile(barTiles, i, true);
            }
            else
            {
                SetActiveBarTile(barTiles, i, false);
            }
        }
    }

    public int GetWeaponsEnergyUnitsCount()
    {
        int weaponsEnergyUnits = 0;
        for (int i = 0; i < _weaponsEnergyPanelBarTiles.Length; i++)
        {
            if (_weaponsEnergyPanelBarTiles[i].activeSelf)
            {
                weaponsEnergyUnits++;
            }
        }

        return weaponsEnergyUnits;
    }

    public int GetEnginesEnergyUnitsCount()
    {
        int enginesEnergyUnits = 0;
        for (int i = 0; i < _enginesEnergyPanelBarTiles.Length; i++)
        {
            if (_enginesEnergyPanelBarTiles[i].activeSelf)
            {
                enginesEnergyUnits++;
            }
        }

        return enginesEnergyUnits;
    }

    public int GetShieldsEnergyUnitsCount()
    {
        int shieldsEnergyUnits = 0;
        for (int i = 0; i < _shieldsEnergyPanelBarTiles.Length; i++)
        {
            if (_shieldsEnergyPanelBarTiles[i].activeSelf)
            {
                shieldsEnergyUnits++;
            }
        }

        return shieldsEnergyUnits;
    }

    public void AddEnergyUnitToWeapons()
    {
        int weaponsEnergyUnits = GetWeaponsEnergyUnitsCount();
        int enginesEnergyUnits = GetEnginesEnergyUnitsCount();
        int shieldsEnergyUnits = GetShieldsEnergyUnitsCount();
        
        if (weaponsEnergyUnits == 8)
        {
            return;
        }

        if (enginesEnergyUnits > 0)
        {
            FillEnergyBar(_weaponsEnergyPanelBarTiles, weaponsEnergyUnits+1);
            FillEnergyBar(_enginesEnergyPanelBarTiles, enginesEnergyUnits-1);
            _playerController.SetWeaponsEnergyUnits(weaponsEnergyUnits+1);
            _playerController.SetEnginesEnergyUnits(enginesEnergyUnits-1);
            return;
        }

        if (shieldsEnergyUnits > 0)
        {
            FillEnergyBar(_weaponsEnergyPanelBarTiles, weaponsEnergyUnits+1);
            FillEnergyBar(_shieldsEnergyPanelBarTiles, shieldsEnergyUnits-1);
            _playerController.SetWeaponsEnergyUnits(weaponsEnergyUnits+1);
            _playerController.SetShieldsEnergyUnits(shieldsEnergyUnits-1);
            return;
        }
    }

    public void AddEnergyUnitToEngines()
    {
        int weaponsEnergyUnits = GetWeaponsEnergyUnitsCount();
        int enginesEnergyUnits = GetEnginesEnergyUnitsCount();
        int shieldsEnergyUnits = GetShieldsEnergyUnitsCount();

        if (enginesEnergyUnits == 8)
        {
            return;
        }

        if (weaponsEnergyUnits > 0)
        {
            FillEnergyBar(_enginesEnergyPanelBarTiles, enginesEnergyUnits+1);
            FillEnergyBar(_weaponsEnergyPanelBarTiles, weaponsEnergyUnits-1);
            _playerController.SetEnginesEnergyUnits(enginesEnergyUnits+1);
            _playerController.SetWeaponsEnergyUnits(weaponsEnergyUnits-1);
            return;
        }

        if (shieldsEnergyUnits > 0)
        {
            FillEnergyBar(_enginesEnergyPanelBarTiles, enginesEnergyUnits+1);
            FillEnergyBar(_shieldsEnergyPanelBarTiles, shieldsEnergyUnits-1);
            _playerController.SetEnginesEnergyUnits(enginesEnergyUnits+1);
            _playerController.SetShieldsEnergyUnits(shieldsEnergyUnits-1);
            return;
        }
    }

    public void AddEnergyUnitToShields()
    {
        int weaponsEnergyUnits = GetWeaponsEnergyUnitsCount();
        int enginesEnergyUnits = GetEnginesEnergyUnitsCount();
        int shieldsEnergyUnits = GetShieldsEnergyUnitsCount();

        if (shieldsEnergyUnits == 8)
        {
            return;
        }

        if (weaponsEnergyUnits > 0)
        {
            FillEnergyBar(_shieldsEnergyPanelBarTiles, shieldsEnergyUnits+1);
            FillEnergyBar(_weaponsEnergyPanelBarTiles, weaponsEnergyUnits-1);
            _playerController.SetShieldsEnergyUnits(shieldsEnergyUnits+1);
            _playerController.SetWeaponsEnergyUnits(weaponsEnergyUnits-1);
            return;
        }

        if (enginesEnergyUnits > 0)
        {
            FillEnergyBar(_shieldsEnergyPanelBarTiles, shieldsEnergyUnits+1);
            FillEnergyBar(_enginesEnergyPanelBarTiles, enginesEnergyUnits-1);
            _playerController.SetShieldsEnergyUnits(shieldsEnergyUnits+1);
            _playerController.SetEnginesEnergyUnits(enginesEnergyUnits-1);
            return;
        }
    }

    public void DistributeEnergyByDefault()
    {
        FillEnergyBar(_weaponsEnergyPanelBarTiles, 5);
        _playerController.SetWeaponsEnergyUnits(5);
        FillEnergyBar(_enginesEnergyPanelBarTiles, 5);
        _playerController.SetEnginesEnergyUnits(5);

        if (_isShieldsActive)
        {
            _shieldsEnergyPanel.SetActive(true);
            FillEnergyBar(_shieldsEnergyPanelBarTiles, 0);
            _playerController.SetShieldsEnergyUnits(0);
        }
        else
        {
            _shieldsEnergyPanel.SetActive(false);
            _playerController.SetShieldsEnergyUnits(0);
        }
    }
}