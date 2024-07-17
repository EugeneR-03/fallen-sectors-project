using UnityEngine;

public class WeaponButtonsReloader : MonoBehaviour
{
    private WeaponButtonController weaponButtonController;
    [SerializeField] private GameObject buttonOfFirstWeapon;

    private void Start()
    {
        weaponButtonController = buttonOfFirstWeapon.GetComponent<WeaponButtonController>();
        weaponButtonController.SelectButton();
        weaponButtonController.SelectPanelOfActiveWeapon();
    }
}