using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WeaponButtonController : MonoBehaviour
{
    private Image _image;
    private WeaponPanelController[] _weaponPanelControllers;
    [SerializeField] private GameObject[] _weaponPanels;
    [SerializeField] private GameObject[] _otherButtons;

    private void Awake() {
        _image = gameObject.GetComponent<Image>();
        _weaponPanelControllers = _weaponPanels
            .Select(x => x.GetComponent<WeaponPanelController>())
            .ToArray();
    }

    public void SelectButton()
    {
        Color white = new(1, 1, 1);
        Color bitOrange = new(1, 0.8f, 1);
        _image.color = bitOrange;

        foreach (GameObject button in _otherButtons)
        {
            button.GetComponent<Image>().color = white;
        }
    }

    public void SelectPanel(int panelIndex)
    {
        _weaponPanelControllers[panelIndex].SelectAndHideAllPanels();
        Debug.Log("index:" + panelIndex + " panel:" + _weaponPanels[panelIndex]);
        _weaponPanelControllers[panelIndex].SetTitleAndDescriptionText();
    }

    public void SelectPanelOfActiveWeapon()
    {
        WeaponVariant weaponVariant = PlayerSelectionController.GetActiveWeaponVariant();
        SelectPanel((int)weaponVariant-1);
    }
}