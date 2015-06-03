/// <summary>
/// Класс с описанием уровня, префабы для спавна, спавнпоинты, бонусы и проч.
/// </summary>

using System;
using UnityEngine;
using System.Collections;

public class LevelSettings : MonoBehaviour
{
    public string LevelName = "none";

    public GameObject LevelBasePrefab;

    public Transform[] SpawnPoints;

    public bool HasTeams = false;

    public bool Bots = false;

    public float RoundTime = 120f;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // Конфигурируем тут будущую сцену и сохраняем настройки для загрузки уровня

    }

    public void LoadLevel()
    {
        LevelBasePrefab = Instantiate(Resources.Load(LevelName), Vector3.zero, Quaternion.identity) as GameObject;

        GameObject spawnRoot = GameObject.Find("SpawnRoot");
        SpawnPoints = spawnRoot.GetComponentsInChildren<Transform>();

    }

   // [RPC]
   // public void SyncSettings

}
