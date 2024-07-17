using UnityEngine;

public sealed class UserDataSaveLoader : ISaveLoader
{
    public void LoadData()
    {
        UserData userData = UserDataRepository.GetData();
        Debug.Log("Loaded data: " + userData.CurrencyForConstantUpgrades);
        UserDataController.Instance.SetUserData(userData);
    }

    public void SaveData()
    {
        UserData userData = UserDataController.Instance.GetUserData();
        UserDataRepository.SetData(userData);
    }
}