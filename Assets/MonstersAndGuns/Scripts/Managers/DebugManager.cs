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
    [SerializeField] private TMP_Text fpsText;

    int monsterCount;

    private void Awake()
    {
        monsterCount = 0;
        monsterCountText.text = "NoData";
    }


    private void Start()
    {
        StartCoroutine(FPSRoutine());
    }

    private void OnEnable()
    {
        GameManager.Instance.OnMonsterCreated += MonsterCreatedHandler;
        GameManager.Instance.OnMonsterDead += MonsterDeadHandler;
        GameManager.Instance.OnPlayerFired += PlayerFiredHandler;
        GameManager.Instance.OnRestart += RestartHandler;

    }

    

    private void OnDisable()
    {
        GameManager.Instance.OnMonsterCreated -= MonsterCreatedHandler;
        GameManager.Instance.OnMonsterDead -= MonsterDeadHandler;
        GameManager.Instance.OnPlayerFired -= PlayerFiredHandler;
        GameManager.Instance.OnRestart -= RestartHandler;
    }

    private void RestartHandler()
    {
        UpdateMonsterCount(0);
    }

    private void MonsterCreatedHandler()
    {
        UpdateMonsterCount(monsterCount + 1);
    }

    private void PlayerFiredHandler(int gunIndex)
    {
        gunIndexText.text = gunIndex.ToString();
    }   

    private void MonsterDeadHandler(BaseMonsterController monsterDead)
    {
        UpdateMonsterCount(monsterCount - 1);
    }

    void UpdateMonsterCount(int count)
    {
        monsterCount = count;
        monsterCountText.text = monsterCount.ToString();
    }



    private void Update()
    {
        var pos = GameManager.Instance.PlayerPosition;
        playerPositionText.text = pos.ToString();

        
    }

    IEnumerator FPSRoutine()
    {
        while (true)
        {
            float fps = 1f / Time.smoothDeltaTime;
            fpsText.text = Mathf.Round(fps).ToString();
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void SetMonsterDistance(float distance)
    {
        monsterDistanceText.text = distance.ToString();
    }

    public void TestDebug()
    {
        //FindObjectOfType<UIManager>().PlayerDamageHandler(0);
    }
}
