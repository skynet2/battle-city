using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class PlayerControl : MonoBehaviour
    {
        public KeyCode MoveForward;
        public KeyCode MoveBackwards;
        public KeyCode TurnLeft;
        public KeyCode TurnRight;
        public KeyCode Shoot;

        private TankUser _tank;

        private void Awake()
        {
            _tank = GetComponent<TankUser>();
        }
        private void Update()
        {
            //Player 1
            _tank.rig.velocity = Vector2.zero;

            if (_tank.CanMove && !_tank.IsBot)
            {
                if (Input.GetKey(MoveForward))
                {
                    _tank.Move(1);
                }
                if (Input.GetKey(MoveBackwards))
                {
                    _tank.Move(-1);
                }
                if (Input.GetKey(TurnLeft))
                {
                    _tank.Turn(-1);
                }
                if (Input.GetKey(TurnRight))
                {
                    _tank.Turn(1);
                }
            }

    
            if (Input.GetKeyDown(Shoot) && _tank.CanShoot)
            {
                _tank.Shoot();
            }

            //if (Input.GetKeyDown(p1Shoot) && m_Tank.canShoot)
            //{
            //    m_Tank.Fire();
            //}
        }

    }
}
