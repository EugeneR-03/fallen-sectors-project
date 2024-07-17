using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    private TemporaryUpgrade _upgrade;
    private PlayerController _playerController;
    [SerializeField] private GameObject _player;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;

    void Start()
    {
        _playerController = _player.GetComponent<PlayerController>();
    }

    public void SetUpgrade(TemporaryUpgrade upgrade)
    {
        _upgrade = upgrade;
        _title.text = TemporaryUpgrade.GetUpgradeName(upgrade.Variant);
        _description.text = TemporaryUpgrade.GetUpgradeDescription(upgrade.Variant);
        TemporaryUpgradeColor upgradeColor = TemporaryUpgrade.GetUpgradeColor(upgrade.Level);
        switch (upgradeColor)
        {
            case TemporaryUpgradeColor.White:
                _title.color = new Color32(255, 255, 255, 255);
                break;
            case TemporaryUpgradeColor.Blue:
                _title.color = new Color32(0, 0, 255, 255);
                break;
            case TemporaryUpgradeColor.Violet:
                _title.color = new Color32(139, 0, 139, 255);
                break;
            case TemporaryUpgradeColor.Orange:
                _title.color = new Color32(255, 127, 0, 255);
                break;
            default:
                break;
        }
    }

    public void SelectUpgrade()
    {
        _playerController.UpgradeStats(_upgrade);
    }
}