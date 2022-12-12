using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    [SerializeField] private Image circle;
    [SerializeField] private RectTransform monsterMinimapPrefab;
    [SerializeField] private int maxMonstersMinimap = 50;
    [SerializeField] private float delayUpdateMinimap = 0.1f;
    [SerializeField] private float worldRadiusDimension = 5f;

    RectTransform[] monstersMinimap;
    WaitForSeconds waitUpdateMinimap;

    private void Awake()
    {
        monstersMinimap = new RectTransform[maxMonstersMinimap];

        for (int i = 0; i < monstersMinimap.Length; i++)
        {
            var m = Instantiate(monsterMinimapPrefab, circle.transform);
            m.gameObject.SetActive(false);
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
        StartCoroutine(MinimapRoutine());
        // Notar que el minimap nunca más se desactivará de la UI, por eso no es necesario llamar al StopAllCoroutine.
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

                //Ahora hay que transformar esa posición de mundo, c/r al player:
                var monsterPositionRelativeToPlayer = player.InverseTransformPoint(monsterPositionWorldSpace);
                monsterPositionRelativeToPlayer.y = 0;

                //TODO: falta validar que si la posición de mundo está muy fuera del radio max, se debe hacer un clamp
                //  para que igual se dibuje el monster, pero en el borde del minimap

                var minimapPosition = monsterPositionRelativeToPlayer * scaleRatio;
                minimapPosition.y = minimapPosition.z;
                minimapPosition.z = 0;


                //TODO: cambiar el tipo de dato de la var para no tener que hacer el GetComponent
                monstersMinimap[i].anchoredPosition = minimapPosition;
                monstersMinimap[i].gameObject.SetActive(true);

                //TODO: setear el color del monsterminimap al color del monster, al menos si esta en patrol o en attack

                
            }

            //TODO: también hay que usar un ícono de minimap para el boss monster
            //TODO: usar otra imagen de circle con el borde más delgado

            yield return waitUpdateMinimap;
        }
    }

    void DeactivateMinimapMonsters()
    {
        for (int i = 0; i < monstersMinimap.Length; i++)
        {
            monstersMinimap[i].gameObject.SetActive(false);
        }
    }

}
