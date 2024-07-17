using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyForConstantUpgradesPanel : MonoBehaviour
{
    private TextMeshProUGUI _currencyText;

    private void Awake()
    {
        _currencyText = GetComponentInChildren<TextMeshProUGUI>(true);
        SaveLoadController.OnDataChanged += SetText;
    }

    private void Start()
    {
        SetText();
    }

    private void SetText()
    {
        _currencyText.text = UserDataController.Instance.GetUserData().CurrencyForConstantUpgrades.ToString();
    }
}