using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    [SerializeField] private Image circle;
    [SerializeField] private GameObject monsterMinimapPrefab;
    [SerializeField] private int maxMonstersMinimap = 50;
    [SerializeField] private float delayUpdateMinimap = 0.1f;
    [SerializeField] private float worldRadiusDimension = 5f;

    GameObject[] monstersMinimap;
    WaitForSeconds waitUpdateMinimap;

    private void Awake()
    {
        monstersMinimap = new GameObject[maxMonstersMinimap];

        for (int i = 0; i < monstersMinimap.Length; i++)
        {
            var m = Instantiate(monsterMinimapPrefab, circle.transform);
            m.SetActive(false);
            monstersMinimap[i] = m;
        }

        waitUpdateMinimap = new WaitForSeconds(delayUpdateMinimap);
    }

    private void OnEnable()
    {
        GameManager.Instance.OnBattling += BattlingHanlder;
    }

    

    private void OnDisable()
    {
        GameManager.Instance.OnBattling -= BattlingHanlder;
    }

    private void BattlingHanlder(List<MonsterController> arg1, int arg2)
    {        
        

        //TODO: la corutina deber�a empezar en un evento de GM, y detenerse en otro evento del GM, ahora esto es solo TEST
        StartCoroutine(MinimapRoutine());

        //TODO: falta el StopCoroutine
    }

    IEnumerator MinimapRoutine()
    {
        float sizeImage = circle.rectTransform.rect.size.x;
        float diameterWorld = worldRadiusDimension * 2f;        
        float scaleRatio = sizeImage / diameterWorld;

        var monsters = GameManager.Instance.Monsters;
        var player = GameManager.Instance.Player;
        while (true)
        {
            DeactivateMinimapMonsters();
            
            for (int i = 0; i < monsters.Count; i++)
            {
                var monster = monsters[i];
                var monsterPositionWorldSpace = monster.transform.position;


                //Ahora hay que transformar esa posici�n de mundo, c/r al player:
                var positionRelativeToPlayer = player.InverseTransformPoint(monsterPositionWorldSpace);
                positionRelativeToPlayer.y = 0;

                //TODO: falta validar que si la posici�n de mundo est� muy fuera del radio max, se debe hacer un clamp
                //  para que igual se dibuje el monster, pero en el borde del minimap

                var minimapPosition = positionRelativeToPlayer * scaleRatio;
                minimapPosition.y = minimapPosition.z;
                minimapPosition.z = 0;


                //print("Posici�n de ui minimap = " + minimapPosition);

                //TODO: cambiar el tipo de dato de la var para no tener que hacer el GetComponent
                monstersMinimap[i].GetComponent<RectTransform>().anchoredPosition = minimapPosition;
                monstersMinimap[i].SetActive(true);

                //TODO: setear el color del monsterminimap al color del monster, al menos si esta en patrol o en attack

                
            }

            yield return waitUpdateMinimap;
        }
    }

    void DeactivateMinimapMonsters()
    {
        for (int i = 0; i < monstersMinimap.Length; i++)
        {
            monstersMinimap[i].SetActive(false);
        }
    }

}
