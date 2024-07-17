using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPanelController : MonoBehaviour
{
    private GameObject _activePanel;
    private GameObject _decreaseCost;
    private GameObject _maskPanel;
    private bool _isWeaponBought;

    [SerializeField] private GameObject[] _panelsToHide;
    [SerializeField] private TextMeshProUGUI _titleBlock;
    [SerializeField] private TextMeshProUGUI _textBlock;
    [SerializeField] private WeaponVariant _weaponVariant;
    [SerializeField] private string _titleToSet;
    [TextArea]
    [SerializeField] private string _textToSet;
    
    private void Start()
    {
        _activePanel = gameObject;

        var decreaseCost = transform.GetComponentInChildren<TextMeshProUGUI>(true);
        if (decreaseCost != null)
        {
            _decreaseCost = decreaseCost.gameObject;
        }
        
        var maskPanel = transform.GetComponentsInChildren<Image>(true)
                                    .Where(x => x.gameObject.name == "MaskPanel")
                                    .FirstOrDefault();
        if (maskPanel != null)
        {
            _maskPanel = maskPanel.gameObject;
        }

        _isWeaponBought = UserDataController.Instance.GetUserData().WeaponsUnlocked[_weaponVariant];

        if (_isWeaponBought)
        {
            SetActiveMaskPanelAndDecreaseCost(false);
        }
        else
        {
            SetActiveMaskPanelAndDecreaseCost(true);
        }
    }

    private void Select()
    {
        _activePanel.transform.Find("SelectionPointer").gameObject.SetActive(true);
    }

    public void SelectAndHideAllPanels()
    {
        if (!_isWeaponBought)
        {
            return;
        }

        _activePanel.transform.Find("SelectionPointer").gameObject.SetActive(true);
        HideAllPanelsSelection();
    }

    private void HideAllPanelsSelection()
    {
        foreach (GameObject panel in _panelsToHide)
        {
            panel.transform.Find("SelectionPointer").gameObject.SetActive(false);
        }
    }

    private void SetTitleText()
    {
        _titleBlock.text = _titleToSet;
    }

    private void SetDescriptionText()
    {
        _textBlock.text = _textToSet;
    }

    public void SetTitleAndDescriptionText()
    {
        if (!_isWeaponBought)
        {
            return;
        }

        SetTitleText();
        SetDescriptionText();
    }

    public void SetActiveMaskPanelAndDecreaseCost(bool active)
    {
        if (_maskPanel != null)
        {
            _maskPanel.SetActive(active);
        }
        if (_decreaseCost != null)
        {
            _decreaseCost.SetActive(active);
        }
    }

    public void TryBuyWeapon()
    {
        if (_isWeaponBought)
        {
            return;
        }

        UserDataController.Instance.TryBuyWeapon(_weaponVariant);
        if (UserDataController.Instance.GetUserData().WeaponsUnlocked[_weaponVariant])
        {
            _isWeaponBought = true;
            SetActiveMaskPanelAndDecreaseCost(false);
        }
    }
}
