using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum EndMode
{
    Defeat,
    Victory,
}

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    private int _seconds;
    private int _minutes;
    private int _kills;
    private bool _gameIsPaused;
    private bool _gameIsInterrupted;
    private bool _gameIsOver;
    private GameObject[] _playerWeapons;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject[] _sidebars;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _optionsMenu;
    [SerializeField] private GameObject _upgradesMenu;
    [SerializeField] private GameObject _endMenu;
    [SerializeField] private TextMeshProUGUI _playerLevel;
    [SerializeField] private TextMeshProUGUI _timer;
    [SerializeField] private TextMeshProUGUI _killsCount;
    [SerializeField] private TemporaryUpgrade _selectedUpgrade;

    private void Awake() {
        Instance = this;
        _seconds = 0;
        _minutes = 0;
        _kills = 0;
        _gameIsPaused = false;
        _gameIsInterrupted = false;
        _gameIsOver = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerWeapons = _player.GetComponentsInChildren<WeaponController>(true)
            .Select(x => x.gameObject)
            .OrderBy(x => x.name)
            .ToArray();
        
        StartCoroutine(TimerRoutine());
        Invoke(nameof(ShowEndMenu), 15*60);

        Debug.Log("Player has kinetic weapons? " + _player.GetComponent<PlayerController>().HasKineticWeapons());
        Debug.Log("Player has energetic weapons? " + _player.GetComponent<PlayerController>().HasEnergeticWeapons());
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameIsOver || _gameIsInterrupted)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_gameIsPaused)
            {
                _gameIsPaused = false;
                HidePauseMenu();
            }
            else
            {
                _gameIsPaused = true;
                ShowPauseMenu();
            }
        }
    }

    private IEnumerator TimerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            _seconds++;
            if (_seconds == 60)
            {
                _minutes++;
                _seconds = 0;
            }
            _timer.text = _minutes.ToString("00") + ":" + _seconds.ToString("00");
        }
    }

    public void UpdatePlayerLevel(int level)
    {
        _playerLevel.text = "Ур: " + level.ToString();
    }

    public void UpdateKillsCount(int killsToAdd)
    {
        _kills += killsToAdd;
        _killsCount.text = _kills.ToString();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
    }

    private GameObject[] GetEnemies()
    {
        return GameObject.FindGameObjectsWithTag("Enemy");
    }

    private void ToggleEnemies(bool isActive)
    {
        GameObject[] enemies = GetEnemies();

        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(isActive);
        }
    }

    private void TogglePlayer(bool isActive)
    {
        _player.SetActive(isActive);
    }

    private void TogglePlayerWeapons(bool isActive)
    {
        foreach (GameObject weapon in _playerWeapons)
        {
            weapon.GetComponent<SpriteRenderer>().enabled = isActive;
        }
    }

    private void ToggleSidebars(bool isActive)
    {
        for (int i = 0; i < _sidebars.Length; i++)
        {
            _sidebars[i].SetActive(isActive);
        }
    }

    private void TogglePauseMenu(bool isActive)
    {
        if (isActive)
        {
            _gameIsPaused = true;
            _pauseMenu.SetActive(true);
            ToggleSidebars(false);
            TogglePlayerWeapons(false);
        }
        else
        {
            _gameIsPaused = false;
            _pauseMenu.SetActive(false);
            ToggleSidebars(true);
            TogglePlayerWeapons(true);
        }
    }

    private void ToggleOptionsMenu(bool isActive)
    {
        if (isActive)
        {
            _gameIsInterrupted = true;
            _optionsMenu.SetActive(true);

            // проверяем, открыто ли меню паузы, т. к. меню настроек можно открыть и из меню паузы
            if (!_gameIsPaused)
            {
                ToggleSidebars(false);
                TogglePlayerWeapons(false);
            }            
        }
        else
        {
            _gameIsInterrupted = false;
            _optionsMenu.SetActive(false);

            // проверяем, открыто ли меню паузы, т. к. меню настроек можно открыть и из меню паузы
            if (!_gameIsPaused)
            {
                ToggleSidebars(true);
                TogglePlayerWeapons(true);
            }
        }
    }

    private void ToggleUpgradesMenu(bool isActive)
    {
        if (isActive)
        {
            _gameIsInterrupted = true;
            _upgradesMenu.SetActive(true);
            ToggleSidebars(false);
            TogglePlayerWeapons(false);
        }
        else
        {
            _gameIsInterrupted = false;
            _upgradesMenu.SetActive(false);
            ToggleSidebars(true);
            TogglePlayerWeapons(true);
        }
    }

    private void ToggleEndMenu(bool isActive)
    {
        if (isActive)
        {
            _gameIsOver = true;
            _endMenu.SetActive(true);
            ToggleSidebars(false);
            TogglePlayerWeapons(false);
        }
        else
        {
            _gameIsOver = false;
            _endMenu.SetActive(false);
            ToggleSidebars(true);
            TogglePlayerWeapons(true);
        }
    }

    public TemporaryUpgrade SelectUpgradeFromUpgrades(TemporaryUpgrade[] upgrades, int upgradeNumber)
    {
        if (upgradeNumber > upgrades.Length || upgradeNumber < 1)
        {
            return null;
        }
        return upgrades[upgradeNumber-1];
    }

    public void ShowPauseMenu()
    {
        Pause();
        TogglePauseMenu(true);
    }

    public void HidePauseMenu()
    {
        TogglePauseMenu(false);
        Resume();
    }

    public void ShowOptionsMenu()
    {
        ToggleOptionsMenu(true);
    }

    public void HideOptionsMenu()
    {
        ToggleOptionsMenu(false);
    }

    public void ShowEndMenu()
    {
        Pause();
        ToggleEndMenu(true);
    }

    public void HideEndMenu()
    {
        ToggleEndMenu(false);
        Resume();
    }

    public void ShowUpgradesMenu()
    {
        Pause();
        ToggleUpgradesMenu(true);
        GameObject upgradePanel1 = _upgradesMenu.transform.GetChild(0).gameObject;
        GameObject upgradePanel2 = _upgradesMenu.transform.GetChild(1).gameObject;
        GameObject upgradePanel3 = _upgradesMenu.transform.GetChild(2).gameObject;

        TemporaryUpgrade upgrade1;
        TemporaryUpgrade upgrade2;
        TemporaryUpgrade upgrade3;

        PlayerController playerController = _player.GetComponent<PlayerController>();
        if (playerController.HasKineticWeapons() && playerController.HasEnergeticWeapons())
        {
            upgrade1 = TemporaryUpgrade.GetUpgrade();
            upgrade2 = TemporaryUpgrade.GetUpgrade();
            while (upgrade1.Variant == upgrade2.Variant)
            {
                upgrade2 = TemporaryUpgrade.GetUpgrade();
            }
            upgrade3 = TemporaryUpgrade.GetUpgrade();
            while (upgrade1.Variant == upgrade3.Variant || upgrade2.Variant == upgrade3.Variant)
            {
                upgrade3 = TemporaryUpgrade.GetUpgrade();
            }
        }
        else if (!playerController.HasKineticWeapons())
        {
            upgrade1 = TemporaryUpgrade.GetUpgradeWithoutUpgradesForKineticWeapons();
            upgrade2 = TemporaryUpgrade.GetUpgradeWithoutUpgradesForKineticWeapons();
            while (upgrade1.Variant == upgrade2.Variant)
            {
                upgrade2 = TemporaryUpgrade.GetUpgrade();
            }
            upgrade3 = TemporaryUpgrade.GetUpgradeWithoutUpgradesForKineticWeapons();
            while (upgrade1.Variant == upgrade3.Variant || upgrade2.Variant == upgrade3.Variant)
            {
                upgrade3 = TemporaryUpgrade.GetUpgrade();
            }
        }
        else
        {
            upgrade1 = TemporaryUpgrade.GetUpgradeWithoutUpgradesForEnergeticWeapons();
            upgrade2 = TemporaryUpgrade.GetUpgradeWithoutUpgradesForEnergeticWeapons();
            while (upgrade1.Variant == upgrade2.Variant)
            {
                upgrade2 = TemporaryUpgrade.GetUpgrade();
            }
            upgrade3 = TemporaryUpgrade.GetUpgradeWithoutUpgradesForEnergeticWeapons();
            while (upgrade1.Variant == upgrade3.Variant || upgrade2.Variant == upgrade3.Variant)
            {
                upgrade3 = TemporaryUpgrade.GetUpgrade();
            }
        }

        upgradePanel1.GetComponent<UpgradePanel>().SetUpgrade(upgrade1);
        upgradePanel2.GetComponent<UpgradePanel>().SetUpgrade(upgrade2);
        upgradePanel3.GetComponent<UpgradePanel>().SetUpgrade(upgrade3);
    }

    public void HideUpgradesMenu()
    {
        ToggleUpgradesMenu(false);
        Resume();
    }
}
