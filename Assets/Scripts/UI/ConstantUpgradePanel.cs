using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstantUpgradePanel : MonoBehaviour
{
    [SerializeField] private ConstantUpgradeVariant _constantUpgradeVariant;

    private ConstantUpgrade _constantUpgrade;
    private BarOfTilesController _barOfTilesController;

    private TextMeshProUGUI _increaseCostTMP;
    private TextMeshProUGUI _decreaseCostTMP;
    
    private int _tilesToShow;

    private void Awake()
    {
        _barOfTilesController = transform.Find("BarOfTiles").GetComponent<BarOfTilesController>();
        _increaseCostTMP = transform.Find("IncreaseCost").GetComponent<TextMeshProUGUI>();
        _decreaseCostTMP = transform.Find("DecreaseCost").GetComponent<TextMeshProUGUI>();
        
        Debug.Log("ConstantUpgradeVariant: " + _constantUpgradeVariant);
        Debug.Log("DecreaseCost: " + _decreaseCostTMP.text);
        Debug.Log("IncreaseCost: " + _increaseCostTMP.text);
    }

    private void Start()
    {
        ConstantUpgrade currentUpgrade = UserDataController.Instance.GetUserData().ConstantUpgrades[_constantUpgradeVariant];
        Debug.Log("CurrentUpgrade: " + currentUpgrade + " " + currentUpgrade.Level);

        SetUpgrade(currentUpgrade);
    }

    private void SetActiveTiles()
    {
        _barOfTilesController.SetActiveBarTiles(_tilesToShow);
    }

    private void SetActiveCost()
    {
        int increaseCost = _constantUpgrade.Cost;
        ConstantUpgrade nextLevelUpgrade = Tables.ConstantUpgradeStats[_constantUpgradeVariant][_constantUpgrade.Level + 1];
        int decreaseCost = nextLevelUpgrade.Cost;

        _increaseCostTMP.text = increaseCost.ToString();
        _decreaseCostTMP.text = decreaseCost.ToString();
    }

    public void SetUpgrade(ConstantUpgrade constantUpgrade)
    {
        _constantUpgrade = constantUpgrade;
        _tilesToShow = constantUpgrade.Level;

        SetActiveTiles();
        SetActiveCost();
    }
}