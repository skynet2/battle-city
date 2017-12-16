using System.Collections;
using System.Collections.Generic;
using Assets.Code;
using UnityEngine;

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
       
        for (float i = -halfWidth; i < halfWidth ; i += 1.125f)
        {
            print("HalfHeigt - " + (-halfHeight + 0.75f));
            Instantiate(BorderPrefab, new Vector3(i,(-halfHeight + 0.75f)), Quaternion.identity);
            Instantiate(BorderPrefab, new Vector3(i, halfHeight - 0.75f), Quaternion.identity);
        }
        for (float i = -halfHeight; i < halfHeight ; i += 1.125f)
        {
            print("HalfWidth - " + (-halfWidth + 0.75f));
            Instantiate(BorderPrefab, new Vector3((-halfWidth + 0.75f),i), Quaternion.identity);
            Instantiate(BorderPrefab, new Vector3(halfWidth - 0.75f,i), Quaternion.identity);
        }
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
