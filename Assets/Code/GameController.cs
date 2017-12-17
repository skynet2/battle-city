﻿using System;
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
        var halfHeight = 9.6f;
        var xRightOffset = 15.75f;

        const float xTopLeftBorder = -18.75f;
        const float xTopRightBorder = 17.25f;
        const float yTopBorder = 9.6f;
        const float yBottonBorder = -8f;

        for (var i = xTopLeftBorder; i < xTopRightBorder + 1; i += 1.125f)
        {
            var obj = Instantiate(BorderPrefab, new Vector3(i, (yTopBorder)), Quaternion.identity,
                LevelObjects.transform);
            obj.transform.localPosition = new Vector3(i, (yTopBorder));

            obj = Instantiate(BorderPrefab, new Vector3(i, yBottonBorder), Quaternion.identity, LevelObjects.transform);
            obj.transform.localPosition = new Vector3(i, (yBottonBorder));
        }
        for (var i = yBottonBorder; i < yTopBorder; i += 1.125f)
        {
            var obj = Instantiate(BorderPrefab, new Vector3(xTopLeftBorder, i), Quaternion.identity,
                LevelObjects.transform);
            obj.transform.localPosition = new Vector3(xTopLeftBorder, i);

            obj = Instantiate(BorderPrefab, new Vector3(xTopRightBorder, i), Quaternion.identity,
                LevelObjects.transform);
            obj.transform.localPosition = new Vector3(xTopRightBorder, i);
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
        else if (teamId == 1)
            spawn = _enemySpawnPoins[random.Next(0, _enemySpawnPoins.Count)]; // TODO local spawns

        return spawn;
    }

    // Update is called once per frame
    void Update()
    {
    }
}