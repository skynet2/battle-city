using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Code;
using Code.Objects.Common;
using Code.Objects.Maps;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class Player
{
    public TankUser Tank { get; set; }
    public GameObject GameObject { get; set; }
}

public class GameController : MonoBehaviour
{
    private readonly Color _friendlyColor = Color.green;
    private readonly Color _enemyColor = Color.red;

    [Header("References")] public GameObject TankPrefab;
    public GameObject BorderPrefab;
    public GameObject LevelObjects;
    public GameObject WoodPrefab;
    public GameObject StonePrefab;
    public GameObject BasePrefab;

    public Camera Camera;

    public bool canSelfDamage = true;


    private Player _selfPlayer;
    private readonly List<Player> _bots = new List<Player>();
    private readonly List<Vector3> _enemySpawnPoins = new List<Vector3>();

    // Use this for initialization
    public void Start()
    {
        Camera = FindObjectOfType<Camera>();
        var xLeftOffset = -17.25f;
        var halfHeight = Camera.orthographicSize;
        var xRightOffset = 15.75f;
        print(Camera.aspect);

        var offset = 0.75f;

        for (float i = xLeftOffset; i < xRightOffset + 1; i += 1.125f)
        {
            print("HalfHeigt - " + (-halfHeight + offset));
            ////  Instantiate(BorderPrefab, new Vector3(i, (-halfHeight + offset - 1.7f)), Quaternion.identity);
            //   Instantiate(BorderPrefab, new Vector3(i, halfHeight - offset), Quaternion.identity);
        }
        for (float i = -halfHeight; i < halfHeight; i += 1.125f)
        {
            print("HalfWidth - " + (-xRightOffset + offset));
            // Instantiate(BorderPrefab, new Vector3((-xRightOffset + offset), i), Quaternion.identity);
            //   Instantiate(BorderPrefab, new Vector3(xRightOffset - offset + 1.7f, i), Quaternion.identity);
        }

        var mapText = File.ReadAllText(@"C:\ExtraSSD\Git\Github\battle-city\Assets\Code\Map\map1.json");
        var mapData =
            JsonUtility.FromJson<Map>(mapText); // TODO 

        var xOffset = xLeftOffset;
        var yOffset = 8.8f;

        foreach (var row in mapData.MapRow)
        {
            foreach (var item in row.MapData)
            {
                GameObject prefab = null;

                switch (item.Type)
                {
                    case MapBlockType.None:
                        prefab = null;
                        break;
                    case MapBlockType.Wood:
                        prefab = WoodPrefab;
                        break;
                    case MapBlockType.Stone:
                        prefab = StonePrefab;
                        break;
                    case MapBlockType.Border:
                        break;
                    case MapBlockType.Base:
                        prefab = BasePrefab;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                if (prefab != null)
                {
                    item.ReferenceGameObject = Instantiate(prefab, new Vector3(xOffset, yOffset), Quaternion.identity,
                        LevelObjects.transform);
                    item.ReferenceGameObject.transform.localPosition = new Vector3(xOffset, yOffset);
                }

                if (item.IsSpawn && item.TeamId == 0) // bot
                {
                    _enemySpawnPoins.Add(new Vector3(xOffset, yOffset));
                }

                xOffset += 1.5f;

                if (xOffset > xRightOffset)
                {
                    xOffset = xLeftOffset;
                    yOffset -= 1.6f;
                }
            }
        }

        //var selfTankObject = Instantiate(TankPrefab, spawnPoints[0].transform.position, Quaternion.identity);

//        _selfPlayer = new Player()
//        {
//            GameObject = selfTankObject,
//            Tank = selfTankObject.GetComponent<TankUser>()
//        };
//
//        _selfPlayer.Tank.SpriteRenderer.color = _friendlyColor;
        var random = new Random();

        for (int i = 0; i < 3; i++)
        {
            var pos = GetRandomSpawnPoint(0);
            
            var botTankObject = Instantiate(TankPrefab,
                pos, Quaternion.identity, LevelObjects.transform);

            botTankObject.transform.localPosition = pos;
            
            var pl = new Player()
            {
                GameObject = botTankObject,
                Tank = botTankObject.GetComponent<TankUser>()
            };

            pl.Tank.SpriteRenderer.color = _enemyColor;
            pl.Tank.IsBot = true;

            _bots.Add(pl);
        }
    }

    public Vector3 GetRandomSpawnPoint(int teamId)
    {
        var random = new Random();
        var spawn = Vector3.back;      
       
        if (teamId == 0)
            spawn = _enemySpawnPoins[random.Next(0, _enemySpawnPoins.Count)];
        else if(teamId == 1)
            spawn = _enemySpawnPoins[random.Next(0, _enemySpawnPoins.Count)]; // TODO local spawns

        return spawn;
    }
    // Update is called once per frame
    void Update()
    {
    }
}