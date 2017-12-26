using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Assets.Code;
using Code.Objects.Common;
using Code.Objects.Maps;
using UnityEngine;
using UnityEngine.UI;
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
    public GameObject WinCanvas;
    public GameObject WinTextObject;
    public GameObject AchivementTextGameObject;


    [HideInInspector] public Random Random = new Random();
    [HideInInspector] public bool IsGameRunning = true;
    [HideInInspector] public TextMesh KillsCounterText;
    [HideInInspector] public Text WinText;
    [HideInInspector] public Text AchivementText;
    [HideInInspector] private TextMesh _timeMesh;

    private Player _selfPlayer;
    private readonly List<Player> _bots = new List<Player>();
    private readonly Dictionary<int, List<Vector3>> _spawnPoins = new Dictionary<int, List<Vector3>>();
    private readonly Stopwatch _stopwatch = new Stopwatch();


    // Use this for initialization
    public void Start()
    {
        #region Prepare Components

        TimeGameObject.GetComponent<Renderer>().sortingOrder = 999;
        KillsCounter.GetComponent<Renderer>().sortingOrder = 999;
        _timeMesh = TimeGameObject.GetComponent<TextMesh>();
        KillsCounterText = KillsCounter.GetComponent<TextMesh>();
        WinText = WinTextObject.GetComponent<Text>();
        WinCanvas.GetComponent<Canvas>().sortingOrder = -2;
        AchivementText = AchivementTextGameObject.GetComponent<Text>();
        AchivementText.text = "";

        #endregion

        #region Build Map

        const float xTopLeftBorder = -18.75f;
        const float xTopRightBorder = 17.25f;
        const float yTopBorder = 12f;
        const float yBottonBorder = -10.4f;

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
        
        var mapText = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "map1.json"));

        var mapData =
            JsonUtility.FromJson<Map>(mapText);

        const float xLeftOffset = -17.25f;
        const float xRightOffset = 15.75f;

        var xOffset = xLeftOffset;
        var yOffset = 10.4f;
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

                if (item.IsSpawn)
                {
                    if (!_spawnPoins.ContainsKey(item.TeamId))
                        _spawnPoins.Add(item.TeamId, new List<Vector3>());

                    _spawnPoins[item.TeamId].Add(new Vector3(xOffset, yOffset));
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

        #region Spawn tanks

        _selfPlayer = SpawnTank(2, false);

        for (var i = 0; i < 3; i++)
        {
            const int teamId = 1;

            _bots.Add(SpawnTank(teamId, true));
        }

        #endregion

        _stopwatch.Start();
    }

    private Player SpawnTank(int teamId, bool isBot)
    {
        var pos = GetRandomSpawnPoint(teamId);

        var botTankObject = Instantiate(TankPrefab,
            pos, Quaternion.identity, LevelObjects.transform);

        botTankObject.transform.localPosition = pos;

        var pl = new Player()
        {
            GameObject = botTankObject,
            Tank = botTankObject.GetComponent<TankUser>()
        };

        pl.Tank.SpriteRenderer.color = teamId == 1 ? _enemyColor : _friendlyColor;
        pl.Tank.IsBot = isBot;
        pl.Tank.TeamId = teamId;

        if (!isBot)
            pl.Tank.User = new User();
        
        return pl;
    }

    public Vector3 GetRandomSpawnPoint(int teamId)
    {
        var points = _spawnPoins[teamId];

        return points[Random.Next(0, points.Count * 100) / 100];
    }

    // Update is called once per frame
    public void Update()
    {
        if (!IsGameRunning)
        {
            WinCanvas.GetComponent<Canvas>().sortingOrder = 9999;
        }

        if (_timeMesh != null && IsGameRunning)
            _timeMesh.text = string.Format("{0:00}:{1:00}", _stopwatch.Elapsed.Minutes, _stopwatch.Elapsed.Seconds);
    }
}