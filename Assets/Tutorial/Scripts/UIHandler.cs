using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public Text healthText;
    public Text timeText;
    public Text nextSpawnText;
    public void Update()
    {
        healthText.text = "HEALTH: " + PlayerController.Instance.health.ToString("0");
        timeText.text = Mathf.Floor(Time.time / 60).ToString("0") + ":" + (Time.time - Mathf.Floor(Time.time / 60) * 60).ToString("00");
        nextSpawnText.text = "NEXT SPAWN IN: " + (GameHandler.Instance.spawnInterval - GameHandler.Instance.elapsed).ToString("0.0");
    }
}
