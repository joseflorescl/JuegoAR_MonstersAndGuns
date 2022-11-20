using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerPositionText;


    private void Update()
    {
        var pos = GameManager.Instance.PlayerPosition();
        playerPositionText.text = pos.ToString();   
    }
}
