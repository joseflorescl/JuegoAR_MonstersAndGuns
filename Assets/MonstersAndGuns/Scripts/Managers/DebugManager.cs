using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerPositionText;
    [SerializeField] private TMP_Text monsterCountText;

    int monsterCount;

    private void OnEnable()
    {
        GameManager.Instance.OnBattling += OnBattleHandler;
        GameManager.Instance.OnEnemyDead += OnEnemyDeadHandler;

    }

   

    private void OnDisable()
    {
        GameManager.Instance.OnBattling -= OnBattleHandler;
        GameManager.Instance.OnEnemyDead -= OnEnemyDeadHandler;
    }


    private void OnBattleHandler(List<MonsterController> enemies, int level)
    {
        monsterCount = enemies.Count;
        monsterCountText.text = monsterCount.ToString();
    }

    private void OnEnemyDeadHandler()
    {
        monsterCount--;
        monsterCountText.text = monsterCount.ToString();
    }

    private void Start()
    {
        monsterCountText.text = "NoData";
    }

    private void Update()
    {
        var pos = GameManager.Instance.PlayerPosition();
        playerPositionText.text = pos.ToString();
    }
}
