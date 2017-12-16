using System.Collections;
using System.Collections.Generic;
using Assets.Code;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player
{
    public TankUser Tank { get; set; }
    public GameObject GameObject { get; set; }
}
public class GameController : MonoBehaviour
{

    private readonly Color _friendlyColor = Color.green;
    private readonly Color _enemyColor = Color.red;
    
    [Header("References")]
    public GameObject TankPrefab;
    public GameObject BorderPrefab;
    public GameObject LevelObjects;
    
    public Camera Camera;
    
    [Header("Spawn Points")]
    public List<GameObject> spawnPoints = new List<GameObject>();

    public bool canSelfDamage = true;

    
    private Player _selfPlayer;
    private readonly List<Player> _bots = new List<Player>();
    
    // Use this for initialization
    public void Start ()
    {
        Camera = FindObjectOfType<Camera>();
        var halfHeight = Camera.orthographicSize;
        var halfWidth = Camera.orthographicSize* Camera.aspect;
         print(Camera.aspect);

        var offset = 0.75f;
        
        for (float i = -halfWidth; i < halfWidth + 1 ; i += 1.125f)
        {
            print("HalfHeigt - " + (-halfHeight + offset));
            Instantiate(BorderPrefab, new Vector3(i,(-halfHeight + offset - 1.7f)), Quaternion.identity);
            Instantiate(BorderPrefab, new Vector3(i, halfHeight - offset), Quaternion.identity);
        }
        for (float i = -halfHeight; i < halfHeight ; i += 1.125f)
        {
            print("HalfWidth - " + (-halfWidth + offset));
            Instantiate(BorderPrefab, new Vector3((-halfWidth + offset),i), Quaternion.identity);
            Instantiate(BorderPrefab, new Vector3(halfWidth - offset +1.7f,i), Quaternion.identity);
        }
        var grid = LevelObjects.GetComponent<Grid>();
        var mep = LevelObjects.GetComponent<Tilemap>();
        var selfTankObject = Instantiate(TankPrefab, spawnPoints[0].transform.position, Quaternion.identity);
        
        _selfPlayer = new Player()
        {
            GameObject = selfTankObject,
            Tank = selfTankObject.GetComponent<TankUser>()
        };
        
        _selfPlayer.Tank.SpriteRenderer.color = _friendlyColor;

        for (int i = 0; i < 3; i++)
        {
            var botTankObject = Instantiate(TankPrefab, spawnPoints[0].transform.position, Quaternion.identity);
            
            var pl = new Player()
            {
                GameObject = botTankObject,
                Tank =  botTankObject.GetComponent<TankUser>()
            };
            
            pl.Tank.SpriteRenderer.color = _enemyColor;
            pl.Tank.IsBot = true;
            
            _bots.Add(pl);
        }        
    }

    // Update is called once per frame
    void Update () {

	}
}
