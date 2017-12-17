using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Code.Objects.Common;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Assets.Code
{
    public class TankUser : BaseDestroyable
    {
        public int Id = -1;
        public int DamagePerShoot = 50;
        public float BulletSpeed = 650;
        public int TeamId = 0;
        public int RespawnSeconds = 1;

        [HideInInspector] public bool IsBot = false;
        public GameObject HealthBar;
        public TextMesh TankTitle;

        public bool CanMove = true;
        public bool CanShoot = true;
        public float moveSpeed;
        public float turnSpeed;
        private Random _random;
        private int _botPreviousTurn;
        
        private bool _botDoNothing;
        
        private void HandleColision()
        {
            if (!IsBot)
                return;
            
            if (_botDoNothing && _frameCount % 40 == 0)
            {
                _botDoNothing = false;
            }
            else if (_botDoNothing)
            {
                return;
            }
            var val = _random.Next(0, 500) / 100;

            if (val == _botPreviousTurn)
                val = _random.Next(0, 500) / 100;
            
            
            switch (val)
            {
                case 0:
                    this.Turn(45);
                    break;
                case 1:
                    this.Turn(-45);
                    break;
                case 2:
                    this.Turn(45);
                    break;
                case 3:
                    this.Turn(-45);
                    break;
                case 4 :
                    this._botDoNothing = true;
                    break;
            }
            _botPreviousTurn = val;
        }
        private void OnCollisionStay2D(Collision2D other)
        {
            HandleColision();
        }
        public void OnCollisionEnter2D(Collision2D col)
        {
            HandleColision();
        }

        private string Name;

        /// <summary>
        ///  
        /// </summary>
        [HideInInspector] public SpriteRenderer SpriteRenderer;

        [HideInInspector] private GameController _gameController;

        [HideInInspector] public Vector3 direction;
        [Header("References")] public Rigidbody2D rig;
        public GameObject Bullet; //The projectile prefab of which the tank can shoot.
        public Transform CannonFront;

        public void Update()
        {
          //  if (!IsBot)
                TankTitle.transform.position = transform.position + new Vector3(-2, -1, 0);
          ////  else
               // TankTitle.transform.position = new Vector3(0, 1000, 0);

            if (Health > 70)
                TankTitle.color = Color.green;
            else if (Health >= 40)
                TankTitle.color = Color.yellow;
            else if (Health < 40)
                TankTitle.color = Color.red;
        }

        void Start()
        {
            direction = Vector3.up; //Sets the tank's direction up, as that is the default rotation of the sprite.
            Health = 100;
        }

        //Called when the tank dies, and needs to wait a certain time before respawning.
        void BotLogic()
        {
            HandleColision();
            print("Random!!");
        }

        private void Awake()
        {
            _random = new Random();
            _gameController = FindObjectOfType<GameController>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Name = String.Format("Bot {0}", _random.Next(0, 500));

            if (HealthBar && !IsBot)
            {
                var ob = Instantiate(HealthBar, CannonFront.transform.position,
                    Quaternion.identity);

                TankTitle = ob.GetComponent<TextMesh>();
                TankTitle.text = Name;
                TankTitle.fontSize = 12;
            }
        }

        public void Move(float y)
        {
            //if (game.Finish)
            //    return;

            rig.velocity = direction * y * moveSpeed * Time.deltaTime;
        }

        public override bool Damage(int amount, int tankId, bool isFriendly)
        {
            Health = Health - amount;
            var isDestroyed = Health <= 0;

            if (isDestroyed)
            {
                CanMove = false;
                CanShoot = false;
                transform.position = new Vector3(0, 1000, 0); // Hide tank
                TankTitle.transform.position = new Vector3(0, 1000, 0); // Hide text

                StartCoroutine(RespawnTimer());
            }

            return isDestroyed;
        }

        //Called when the tank dies, and needs to wait a certain time before respawning.
        IEnumerator RespawnTimer()
        {
            print("abcd");
            yield return new WaitForSecondsRealtime(5); //Waits how ever long was set in the Game.cs script.
            print("abcd2");
            Respawn(); //Respawns the tank.
        }

        //Called when the tank has been dead and is ready to rejoin the game.
        public void Respawn()
        {
            CanMove = true;
            CanShoot = true;
            print("abcd3");
            Health = 100;

            transform.localPosition = _gameController.GetRandomSpawnPoint(TeamId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        public void Turn(float x)
        {
            //if (game.Finish)
            //    return;

            transform.Rotate(-Vector3.forward * x * turnSpeed * Time.deltaTime);
            direction = transform.rotation * Vector3.up;
        }


        public void Shoot()
        {
            var proj = Instantiate(Bullet, CannonFront.transform.position,
                Quaternion.identity); //Spawns the projectile at the muzzle.

            var projScript = proj.GetComponent<Bullet>(); //Gets the Bullet component of the projectile object.
            projScript.TankId = Id; //Sets the projectile's tankId, so that it knows which tank it was shot by.
            projScript.Damage = DamagePerShoot; //Sets the projectile's damage.

            projScript.Rig.velocity =
                direction * BulletSpeed *
                Time.deltaTime; //Makes the projectile move in the same direction that the tank is facing. 
        }

        private int _frameCount = 0;
        private void FixedUpdate()
        {
            ++_frameCount;

            var pos = transform.localPosition;
           
          //  if(IsBot && transform.localPosition.y > 5.8f)
         //       HandleColision();
            
            if(IsBot && _frameCount % 50 == 0 && !(transform.localPosition.y > 5.8f))
                Shoot();
            
            if (IsBot && _frameCount % 600 == 0 && !(transform.localPosition.y > 5.8f))
            {
                print("Random!");
                HandleColision();
                _frameCount = 0;
                return;
            }
            if (IsBot)
            {
                Move(1);
            }
        }
    }
}