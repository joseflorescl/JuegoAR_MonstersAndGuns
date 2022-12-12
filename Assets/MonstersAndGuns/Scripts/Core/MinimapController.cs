using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    [SerializeField] private Image circle;
    [SerializeField] private RectTransform monsterMinimapPrefab;
    [SerializeField] private RectTransform bossMonsterMinimapPrefab;
    [SerializeField] private int maxMonstersMinimap = 50;
    [SerializeField] private float delayUpdateMinimap = 0.1f;
    [SerializeField] private float worldRadiusDimension = 5f;

    RectTransform[] monstersMinimap;
    RectTransform bossMonsterMinimap;
    WaitForSeconds waitUpdateMinimap;
    Transform player;

    private void Awake()
    {
        monstersMinimap = new RectTransform[maxMonstersMinimap];

        for (int i = 0; i < monstersMinimap.Length; i++)
        {
            var m = Instantiate(monsterMinimapPrefab, circle.transform);
            m.gameObject.SetActive(false);
            monstersMinimap[i] = m;
        }

        bossMonsterMinimap = Instantiate(bossMonsterMinimapPrefab, circle.transform);
        bossMonsterMinimap.gameObject.SetActive(false);

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
        var monsters = GameManager.Instance.Monsters;
        player = GameManager.Instance.Player;
        

        while (true)
        {
            DeactivateMinimapIcons();
            
            for (int i = 0; i < monsters.Count; i++)
            {
                var monster = monsters[i];
                var monsterPositionWorldSpace = monster.transform.position;

                var monsterPositionRelativeToPlayer = GetPositionRelativeToPlayer(monsterPositionWorldSpace, worldRadiusDimension);
                var minimapPosition = GetMinimapPosition(monsterPositionRelativeToPlayer);
                ActivateMinimapIcon(monstersMinimap[i], minimapPosition, monster.CurrentColor);                
                // La rotación de la imagen c/r a la rotación del monster es imperceptible, por lo que no vale la pena hacerlo
            }

            var bossMonster = GameManager.Instance.BossMonster;
            if (bossMonster)
            {
                var bossMonsterPositionWorldSpace = bossMonster.transform.position;
                var bossMonsterPositionRelativeToPlayer = GetPositionRelativeToPlayer(bossMonsterPositionWorldSpace, worldRadiusDimension);
                var minimapPosition = GetMinimapPosition(bossMonsterPositionRelativeToPlayer);
                ActivateMinimapIcon(bossMonsterMinimap, minimapPosition, bossMonster.CurrentColor);
            }

            // y para los misiles también!
            //TODO: usar otra imagen de circle con el borde más delgado


            yield return waitUpdateMinimap;
        }
    }

    void DeactivateMinimapIcons()
    {
        for (int i = 0; i < monstersMinimap.Length; i++)
        {
            monstersMinimap[i].gameObject.SetActive(false);
        }

        bossMonsterMinimap.gameObject.SetActive(false);
    }

    Vector3 GetPositionRelativeToPlayer(Vector3 position, float maxLength)
    {
        var positionRelativeToPlayer = player.InverseTransformPoint(position);
        positionRelativeToPlayer.y = 0;
        // Si esa posición está muy fuera del radio max, se debe hacer un clamp
        //  para que igual se dibuje el monster, pero en el borde del minimap
        positionRelativeToPlayer = Vector3.ClampMagnitude(positionRelativeToPlayer, maxLength);
        return positionRelativeToPlayer;
    }

    Vector2 GetMinimapPosition(Vector3 localPosition)
    {
        float sizeImage = circle.rectTransform.rect.size.x;
        float diameterWorld = worldRadiusDimension * 2f;
        float scaleRatio = sizeImage / diameterWorld;

        var minimapPosition = localPosition * scaleRatio;
        minimapPosition.y = minimapPosition.z;
        minimapPosition.z = 0;

        return minimapPosition;
    }

    void ActivateMinimapIcon(RectTransform minimapIcon, Vector2 position, Color color)
    {
        minimapIcon.anchoredPosition = position;
        minimapIcon.gameObject.SetActive(true);
        minimapIcon.GetComponent<Image>().color = color;
    }

}
