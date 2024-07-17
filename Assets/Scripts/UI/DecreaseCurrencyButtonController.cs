using UnityEngine;
using UnityEngine.UI;

public class DecreaseCurrencyButtonController : MonoBehaviour
{
    private Button _button;
    private ConstantUpgradePanel _constantUpgradePanel;
    [SerializeField] private ConstantUpgradeVariant _constantUpgradeVariant;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _constantUpgradePanel = GetComponentInParent<ConstantUpgradePanel>();
    }

    public void TryDecrease()
    {
        ConstantUpgrade currentConstantUpgrade = UserDataController.Instance.GetUserData().ConstantUpgrades[_constantUpgradeVariant];
        if (currentConstantUpgrade.Level == 8)
        {
            return;
        }

        int currencyCount = UserDataController.Instance.GetUserData().CurrencyForConstantUpgrades;
        ConstantUpgrade nextLevelUpgrade = Tables.ConstantUpgradeStats[currentConstantUpgrade.Variant][currentConstantUpgrade.Level + 1];

        if (currencyCount >= nextLevelUpgrade.Cost)
        {
            _constantUpgradePanel.SetUpgrade(nextLevelUpgrade);

            UserDataController.Instance.SetUserDataSpecificConstantUpgrade(nextLevelUpgrade);
            UserDataController.Instance.SetUserDataCurrencyForConstantUpgrades(currencyCount - nextLevelUpgrade.Cost);
        }
    }
}