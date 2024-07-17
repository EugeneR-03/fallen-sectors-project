using UnityEngine;
using UnityEngine.UI;

public class IncreaseCurrencyButtonController : MonoBehaviour
{
    private Button _button;
    private ConstantUpgradePanel _constantUpgradePanel;
    [SerializeField] private ConstantUpgradeVariant _constantUpgradeVariant;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _constantUpgradePanel = GetComponentInParent<ConstantUpgradePanel>();
    }

    public void TryIncrease()
    {
        ConstantUpgrade currentConstantUpgrade = UserDataController.Instance.GetUserData().ConstantUpgrades[_constantUpgradeVariant];
        if (currentConstantUpgrade.Level == 0)
        {
            return;
        }

        int currencyCount = UserDataController.Instance.GetUserData().CurrencyForConstantUpgrades;
        ConstantUpgrade previousLevelUpgrade = Tables.ConstantUpgradeStats[currentConstantUpgrade.Variant][currentConstantUpgrade.Level - 1];

        _constantUpgradePanel.SetUpgrade(previousLevelUpgrade);

        UserDataController.Instance.SetUserDataSpecificConstantUpgrade(previousLevelUpgrade);
        UserDataController.Instance.SetUserDataCurrencyForConstantUpgrades(currencyCount + currentConstantUpgrade.Cost);
    }
}