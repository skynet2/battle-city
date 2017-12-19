using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Assets.Code;
using Code.Objects.Common;
using Code.Objects.Maps;
using Code.PathFinding;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using Grid = Code.PathFinding.Grid;
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
    public GameObject TimeGameObject;
    public GameObject KillsCounter;
    
    public Camera Camera;

    private Player _selfPlayer;
    public bool IsGameRunning = true;
    private readonly List<Player> _bots = new List<Player>();
    private readonly List<Vector3> _enemySpawnPoins = new List<Vector3>();
    private readonly List<Vector3> _selfSpawnPoints = new List<Vector3>();

    public Random Random = new Random();
    public TextMesh KillsCounterText;
    private Stopwatch _stopwatch = new Stopwatch();
    
    private TextMesh _timeMesh;
    // Use this for initialization
    public void Start()
    {
        Camera = FindObjectOfType<Camera>();

        TimeGameObject.GetComponent<Renderer>().sortingOrder = 999;
        KillsCounter.GetComponent<Renderer>().sortingOrder = 999;
        _timeMesh = TimeGameObject.GetComponent<TextMesh>();
        KillsCounterText = KillsCounter.GetComponent<TextMesh>();
        
        const float xTopLeftBorder = -18.75f;
        const float xTopRightBorder = 17.25f;
        const float yTopBorder = 9.6f;
        const float yBottonBorder = -8f;

        #region Build Map

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

        var xLeftOffset = -17.25f;
        var xRightOffset = 15.75f;

        var xOffset = xLeftOffset;
        var yOffset = 8.8f;
        var cells = new List<List<Cell>>();

        foreach (var row in mapData.MapRow)
        {
            var cellList = new List<Cell>();

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

                    var refClass = item.ReferenceGameObject.GetComponent<BaseDestroyable>();

                    if (refClass != null)
                        refClass.TeamId = item.TeamId;
                }

                cellList.Add(new Cell()
                {
                    IsOссupiedBy = item.ReferenceGameObject,
                    Pos = new Vector3(xOffset, yOffset)
                });

                if (item.IsSpawn && item.TeamId == 1) // bot
                {
                    _enemySpawnPoins.Add(new Vector3(xOffset, yOffset));
                }
                if (item.IsSpawn && item.TeamId == 2) // bot
                {
                    _selfSpawnPoints.Add(new Vector3(xOffset, yOffset));
                }
                xOffset += 1.5f;

                if (xOffset > xRightOffset)
                {
                    xOffset = xLeftOffset;
                    yOffset -= 1.6f;
                }
            }

            cells.Add(cellList);
        }

        #endregion

        var tilesmap = new float[cells.Count, cells[0].Count];

        for (var i = 0; i < cells.Count - 1; i++)
        {
            for (var y = 0; y < cells[0].Count - 1; y++)
            {
                try
                {
                    tilesmap[i, y] = cells[i][y].Weight;
                }
                catch (Exception e)
                {
                    var z = 0;
                }
            }
        }

        var grid = new Grid(cells.Count, cells[0].Count, tilesmap);
        // create source and target points
        var _from = new Point(1, 1);
        var _to = new Point(5, 5);
        List<Point> path = Pathfinding.FindPath(grid, _from, _to);


        const int myTeam = 2;
            
        var myTeamSpawn = GetRandomSpawnPoint(myTeam);
        
        var selfTankObject = Instantiate(TankPrefab,
            myTeamSpawn, Quaternion.identity, LevelObjects.transform);
        
        selfTankObject.transform.localPosition = myTeamSpawn;
        
        _selfPlayer = new Player()
        {
            GameObject = selfTankObject,
            Tank = selfTankObject.GetComponent<TankUser>()
        };
        _selfPlayer.Tank.TeamId = myTeam;
        _selfPlayer.Tank.SpriteRenderer.color = _friendlyColor;

        for (var i = 0; i < 3; i++)
        {
            const int teamId = 1;
            
            var pos = GetRandomSpawnPoint(teamId);

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
            pl.Tank.TeamId = teamId;
            
            _bots.Add(pl);
        }
        
        _stopwatch.Start();
    }

    public Vector3 GetRandomSpawnPoint(int teamId)
    {
        var spawn = Vector3.back;

        if (teamId == 1)
            spawn = _enemySpawnPoins[Random.Next(0, _enemySpawnPoins.Count * 100) / 100];
        else if (teamId == 2)
            spawn = _selfSpawnPoints[Random.Next(0, _selfSpawnPoints.Count * 100) / 100]; // TODO local spawns

        return spawn;
    }

    // Update is called once per frame
    void Update()
    {
        if(_timeMesh != null)
            _timeMesh.text = string.Format("{0}:{1}", _stopwatch.Elapsed.Minutes, _stopwatch.Elapsed.Seconds);
    }
}