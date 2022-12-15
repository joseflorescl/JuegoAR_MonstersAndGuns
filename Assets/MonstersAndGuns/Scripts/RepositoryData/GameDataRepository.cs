using UnityEngine;

public static class GameDataRepository
{
    private const int INITIAL_HISCORE = 150;
    private const int INITIAL_LEVEL = 1;

    public static GameplayData GetById(string GamePlayDataID)
    {
        if (PlayerPrefs.HasKey(GamePlayDataID))
        {
            var jsonData = PlayerPrefs.GetString(GamePlayDataID);
            GameplayData gameData = JsonUtility.FromJson<GameplayData>(jsonData);
            return gameData;
        }
        else
        {
            return new GameplayData(GamePlayDataID, INITIAL_HISCORE, INITIAL_LEVEL);
        }
    }


    public static void Update(GameplayData gameData)
    {
        var jsonData = JsonUtility.ToJson(gameData);
        PlayerPrefs.SetString(gameData.GamePlayDataID, jsonData);
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }

}