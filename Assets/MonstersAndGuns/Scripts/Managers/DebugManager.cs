using System.Collections;
using UnityEngine;
using TMPro;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerPositionText;
    [SerializeField] private TMP_Text monsterCountText;
    [SerializeField] private TMP_Text gunIndexText;
    [SerializeField] private TMP_Text monsterDistanceText;
    [SerializeField] private TMP_Text fpsText;
    [SerializeField] private TMP_Text fovText;
    [SerializeField] private AudioSource audioSource;


    int monsterCount;
    Camera arCamera;

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
        GameManager.Instance.OnMainMenuActivating += MainMenuActivatingHandler;
    }

    

    private void OnDisable()
    {
        GameManager.Instance.OnMonsterCreated -= MonsterCreatedHandler;
        GameManager.Instance.OnMonsterDead -= MonsterDeadHandler;
        GameManager.Instance.OnPlayerFired -= PlayerFiredHandler;
        GameManager.Instance.OnRestart -= RestartHandler;
        GameManager.Instance.OnMainMenuActivating -= MainMenuActivatingHandler;
    }

    private void MainMenuActivatingHandler()
    {
        arCamera = GameManager.Instance.ARCamera;
        fovText.text = "FOV: " + ((int)(arCamera.fieldOfView)).ToString();
    }

    public void FOVIncrement()
    {
        //arCamera.fieldOfView = (int)arCamera.fieldOfView + 1;
        arCamera.fieldOfView = 90f;
        //fovText.text = "FOV: " + ((int)(arCamera.fieldOfView)).ToString();
        audioSource.Play();
    }
    public void FOVDecrement()
    {
        //arCamera.fieldOfView = (int)arCamera.fieldOfView - 1;
        arCamera.fieldOfView = 60f;
        //fovText.text = "FOV: " + ((int)(arCamera.fieldOfView)).ToString();
        audioSource.Play();
    }

    private void RestartHandler()
    {
        UpdateMonsterCount(0);
    }

    private void MonsterCreatedHandler(BaseMonsterController monster)
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

        // Notar que en el mobil, el FOV de la cámara lo setea Unity, por lo que no lo podemos controlar nosotros.
        if (arCamera)
            fovText.text = "FOV: " + ((int)(arCamera.fieldOfView)).ToString();
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
