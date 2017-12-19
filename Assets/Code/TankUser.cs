using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Code.Objects.Achievements;
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
                case 4:
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
        [HideInInspector] public User User;
        [HideInInspector] public Vector3 direction;
        [Header("References")] public Rigidbody2D rig;

        public GameObject Bullet; //The projectile prefab of which the tank can shoot.
        public Transform CannonFront;
        private AchievementService _achievementService = new AchievementService();

        public void Update()
        {
            TankTitle.transform.position = transform.position + new Vector3(-2, -1, 0);

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
            Name = String.Format("Bot {0}", _random.Next(0, 500));
            
            if(!IsBot)
                StartCoroutine(ApplyAchievements());
            
            if (HealthBar)
            {
                TankTitle.text = Name;
            }
        }

        private void Awake()
        {
            _gameController = FindObjectOfType<GameController>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            _random = _gameController.Random;

            var ob = Instantiate(HealthBar, CannonFront.transform.position,
                Quaternion.identity);
            TankTitle = ob.GetComponent<TextMesh>();
            TankTitle.fontSize = 12;
            var rend = ob.GetComponent<Renderer>();

            if (rend != null)
            {
                rend.sortingOrder = 99;
            }
        }

        public void Move(float y)
        {
            if (!_gameController.IsGameRunning)
                return;

            rig.velocity = direction * y * moveSpeed * Time.deltaTime;
        }

        public override bool Damage(int amount, TankUser tank)
        {
            if (TeamId == tank.TeamId)
                return false;

            Health = Health - amount;
            var isDestroyed = Health <= 0;

            if (isDestroyed)
            {
                CanMove = false;
                CanShoot = false;
                transform.position = new Vector3(0, 1000, 0); // Hide tank
                TankTitle.transform.position = new Vector3(0, 1000, 0); // Hide text

                if (User != null)
                    User.Deaths++;

                if (tank.User != null)
                    tank.User.KilledEnemies++;

                if (IsBot)
                    _gameController.KillsCounterText.text =
                        (int.Parse(_gameController.KillsCounterText.text) + 1).ToString();

                StartCoroutine(RespawnTimer());
            }

            return isDestroyed;
        }

        //Called when the tank dies, and needs to wait a certain time before respawning.
        private IEnumerator RespawnTimer()
        {
            yield return new WaitForSecondsRealtime(5); 
            Respawn(); 
        }

        //Called when the tank has been dead and is ready to rejoin the game.
        public void Respawn()
        {
            CanMove = true;
            CanShoot = true;
            Health = 100;

            if (_gameController.IsGameRunning)
            {
                transform.localPosition = _gameController.GetRandomSpawnPoint(TeamId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        public void Turn(float x)
        {
            transform.Rotate(-Vector3.forward * x * turnSpeed * Time.deltaTime);
            direction = transform.rotation * Vector3.up;
        }


        public void Shoot()
        {
            var proj = Instantiate(Bullet, CannonFront.transform.position,
                Quaternion.identity); //Spawns the projectile at the muzzle.

            var projScript = proj.GetComponent<Bullet>(); //Gets the Bullet component of the projectile object.
            projScript.Tank = this; //Sets the projectile's tankId, so that it knows which tank it was shot by.
            projScript.Damage = DamagePerShoot; //Sets the projectile's damage.

            projScript.Rig.velocity =
                direction * BulletSpeed *
                Time.deltaTime; //Makes the projectile move in the same direction that the tank is facing. 
        }

        private int _frameCount = 0;
        private Queue<Achievement> _achievements = new Queue<Achievement>();

        //Called when the tank dies, and needs to wait a certain time before respawning.
        private IEnumerator ApplyAchievements()
        {
            print("cleared");
            _gameController.AchivementText.text = "";
            
            var achivements = _achievementService.ApplyAchievements(User);

            if (achivements != null && achivements.Any())
                foreach (var ac in achivements)
                    _achievements.Enqueue(ac);

            if (_achievements.Count != 0)
            {
                var ach = _achievements.Dequeue();
                print(ach.Title);
                _gameController.AchivementText.text = ach.Title;
            }
      
            yield return new WaitForSecondsRealtime(5);

            StartCoroutine(ApplyAchievements());
        }


        private void FixedUpdate()
        {
            if (_frameCount % 100 == 0 && !IsBot && User != null)
            {
                var achivements = _achievementService.ApplyAchievements(User);

                if (achivements != null && achivements.Any())
                    foreach (var ac in achivements)
                        _achievements.Enqueue(ac);
            }

            if (!_gameController.IsGameRunning)
            {
                transform.position = new Vector3(0, 1000, 0); // Hide tank
                TankTitle.transform.position = new Vector3(0, 1000, 0); // Hide text
                return;
            }

            ++_frameCount;

            var pos = transform.localPosition;

            if (IsBot && pos.y > 7.8f && Math.Abs(direction.x - (-1)) < 0.01)
            {
                this.Turn(-45);
            }

            if (IsBot && pos.y > 7.8f && Math.Abs(direction.y - 1) < 0.01)
            {
                this.Turn(-90);
            }

            if (IsBot && pos.y > 7.8f && Math.Abs(direction.x - 1) < 0.01) // from left to right
            {
                this.Turn(45);
            }

            if (IsBot && pos.y < 1.6 && pos.y > 0 && Math.Abs(direction.y - 1) < 0.01) // frpm down to up
            {
                this.Turn(90);
            }

            if (IsBot && _frameCount % 50 == 0 && !(transform.localPosition.y > 5.8f))
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