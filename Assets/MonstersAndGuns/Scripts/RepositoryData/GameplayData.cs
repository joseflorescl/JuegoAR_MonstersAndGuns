using UnityEngine;

public class GameplayData
{
    [SerializeField] private string gamePlayDataID; // Este ID es el que va a parar como llave del PlayerPrefs
    [SerializeField] private int score;
    [SerializeField] private int hiScore;
    [SerializeField] private int level;

    public string GamePlayDataID => gamePlayDataID;

    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            if (score > hiScore)
                hiScore = score;
        }
    }

    public int HiScore => hiScore;

    public int Level
    {
        get { return level; }
        set { level = value; }
    }


    public GameplayData(string gamePlayDataID, int initialHiScore, int level)
    {
        this.gamePlayDataID = gamePlayDataID;
        this.score = 0;
        hiScore = initialHiScore;
        this.level = level;
    }
}
