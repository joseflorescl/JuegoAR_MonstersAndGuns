using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerPositionText;
    [SerializeField] private TMP_Text monsterCountText;
    [SerializeField] private TMP_Text gunIndexText;
    [SerializeField] private TMP_Text monsterDistanceText;

    int monsterCount;

    private void OnEnable()
    {
        GameManager.Instance.OnBattling += BattleHandler;
        GameManager.Instance.OnMonsterDead += MonsterDeadHandler;
        GameManager.Instance.OnGunFired += GunFiredHandler;
    }

   

    private void OnDisable()
    {
        GameManager.Instance.OnBattling -= BattleHandler;
        GameManager.Instance.OnMonsterDead -= MonsterDeadHandler;
        GameManager.Instance.OnGunFired -= GunFiredHandler;
    }

    private void GunFiredHandler(int gunIndex)
    {
        gunIndexText.text = gunIndex.ToString();
    }

    private void BattleHandler(List<MonsterController> monster, int level)
    {
        monsterCount = monster.Count;
        monsterCountText.text = monsterCount.ToString();
    }

    private void MonsterDeadHandler(MonsterController monsterDead)
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

    public void SetMonsterDistance(float distance)
    {
        monsterDistanceText.text = distance.ToString();
    }
}
