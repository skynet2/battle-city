using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Code.Objects.Blocks;
using Code.Objects.Common;
using UnityEngine;

namespace Assets.Code
{
    public class Bullet : MonoBehaviour
    {
        //[Header("References")] public GameObject HitEffect;

        [HideInInspector] public TankUser Tank; //The tank which shot this projectile.
        [HideInInspector] public int Damage;
        [HideInInspector] public Rigidbody2D Rig;
        [HideInInspector] private GameController _gameController;

        private void Awake()
        {
            _gameController = FindObjectOfType<GameController>();
            
            Rig = GetComponent<Rigidbody2D>();
        }

        public void OnCollisionEnter2D(Collision2D col)
        {
            var obj = col.gameObject.GetComponent<BaseDestroyable>();

            if (obj != null)
            {
                obj.Damage(Damage, Tank);
            }

            Explode();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Explode()
        {
            Destroy(gameObject);
        }
    }
}