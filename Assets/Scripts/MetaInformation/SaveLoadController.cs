using System;
using UnityEngine;

public interface ISaveLoader
{
    void LoadData();
    void SaveData();
}

public class SaveLoadController : MonoBehaviour
{
    private static readonly ISaveLoader[] s_saveLoaders = {
        new UserDataSaveLoader(),
    };

    public static SaveLoadController Instance { get; private set; }
    public static event Action OnDataChanged;

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        UserDataController.Instance.OnDataChanged += () => OnDataChanged?.Invoke();
        OnDataChanged += () => SaveGame();
        LoadGame();
    }

    [ContextMenu("Load Game")]
    public void LoadGame()
    {
        UserDataRepository.LoadData();

        foreach (var saveLoader in s_saveLoaders)
        {
            saveLoader.LoadData();
        }
    }

    [ContextMenu("Save Game")]
    public void SaveGame()
    {
        foreach (var saveLoader in s_saveLoaders)
        {
            saveLoader.SaveData();
        }

        UserDataRepository.SaveData();
    }
}