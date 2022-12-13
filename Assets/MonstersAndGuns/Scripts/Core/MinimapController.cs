using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    [SerializeField] private Image circle;
    [SerializeField] private RectTransform monsterMinimapPrefab;
    [SerializeField] private RectTransform bossMonsterMinimapPrefab;
    [SerializeField] private RectTransform missileMinimapPrefab;
    [SerializeField] private int maxMonstersMinimap = 50;
    [SerializeField] private int maxMissilesMinimap = 5;
    [SerializeField] private float delayUpdateMinimap = 0.1f;
    [SerializeField] private float worldRadiusDimension = 5f;

    RectTransform[] monstersMinimap;
    RectTransform bossMonsterMinimap;
    RectTransform[] missilesMinimap;
    WaitForSeconds waitUpdateMinimap;
    Transform player;

    private void Awake()
    {
        monstersMinimap = new RectTransform[maxMonstersMinimap];
        missilesMinimap = new RectTransform[maxMissilesMinimap];

        bossMonsterMinimap = Instantiate(bossMonsterMinimapPrefab, circle.transform);
        bossMonsterMinimap.gameObject.SetActive(false);
        InstantiateMinimapIcons(monstersMinimap, monsterMinimapPrefab);
        InstantiateMinimapIcons(missilesMinimap, missileMinimapPrefab);

        waitUpdateMinimap = new WaitForSeconds(delayUpdateMinimap);
    }

    void InstantiateMinimapIcons(RectTransform[] minimapIcons, RectTransform prefab)
    {
        for (int i = 0; i < minimapIcons.Length; i++)
        {
            var m = Instantiate(prefab, circle.transform);
            m.gameObject.SetActive(false);
            minimapIcons[i] = m;
        }
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
        var missiles = GameManager.Instance.Missiles;
        player = GameManager.Instance.Player;

        while (true)
        {
            DeactivateMinimapIcons();
            
            for (int i = 0; i < monsters.Count; i++)
            {
                var monster = monsters[i];                
                ShowMinimapIconFromWorldPosition(monster.transform, monstersMinimap[i], monster.CurrentColor);                
            }

            for (int i = 0; i < missiles.Count; i++)
            {
                var missile = missiles[i];                
                ShowMinimapIconFromWorldPosition(missile.transform, missilesMinimap[i], missile.CurrentColor, true);                
            }

            var bossMonster = GameManager.Instance.BossMonster;
            if (bossMonster)
            {
                ShowMinimapIconFromWorldPosition(bossMonster.transform, bossMonsterMinimap, bossMonster.CurrentColor);
            }

            //TODO: usar otra imagen de circle con el borde más delgado
            yield return waitUpdateMinimap;
        }
    }

    void ShowMinimapIconFromWorldPosition(Transform entity, RectTransform icon, Color color, bool rotate = false)
    {
        Vector3 position = entity.position;
        var positionRelativeToPlayer = GetPositionRelativeToPlayer(position, worldRadiusDimension);
        var minimapPosition = GetMinimapPosition(positionRelativeToPlayer);
        ActivateMinimapIcon(icon, minimapPosition, color);

        if (rotate)
        {
            var forward = entity.forward;
            forward.y = 0;
            var direction = player.InverseTransformDirection(forward);
            float angle = Vector3.SignedAngle(direction, Vector3.forward, Vector3.up);
            icon.rotation = Quaternion.Euler(0f, 0f, angle);
        }

    }

    void DeactivateMinimapIcons()
    {
        for (int i = 0; i < monstersMinimap.Length; i++)
        {
            monstersMinimap[i].gameObject.SetActive(false);
        }

        bossMonsterMinimap.gameObject.SetActive(false);

        for (int i = 0; i < missilesMinimap.Length; i++)
        {
            missilesMinimap[i].gameObject.SetActive(false);
        }
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
