using UnityEngine;

[CreateAssetMenu(fileName = "New Game Manager Data", menuName = "Monsters n Guns/Game Manager Data")]
public class GameManagerData : ScriptableObject
{
    public string[] scenesToLoad;
    public float waitBeforeInitBattle = 2f;
    public float waitBeforeShowGameOver = 1f;
}
