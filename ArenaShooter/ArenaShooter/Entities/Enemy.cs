using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaShooter.Entities
{
    public class Enemy : Entity
    {
        private const float DefaultEnemySpeed = 2.0f;
        private const int EnemyHitboxOffset = 5;
        private const float AnimSpeed = 0.15f;
        private const float AnimWobbleAmount = 15f;

        private Player _target;
        private float _animTimer = 0;

        public Enemy(float startX, float startY, Player playerTarget, Image enemyImg) : base(startX, startY, DefaultEnemySpeed)
        {
            _target = playerTarget;
            _sprite = enemyImg;
            _hitboxOffset = EnemyHitboxOffset;
        }

        public override void Update(HashSet<Keys> pressedKeys)
        {
            if (_target != null)
            {
                float diffX = _target.X - _posX;
                float diffY = _target.Y - _posY;
                float distance = (float)Math.Sqrt(diffX * diffX + diffY * diffY);

                if (distance > 0)
                {
                    _rotation = (float)(Math.Atan2(diffY, diffX) * 180 / Math.PI);

                    _animTimer += AnimSpeed;
                    _aimOffset = (float)Math.Sin(_animTimer) * AnimWobbleAmount;

                    _posX += diffX / distance * _speed;
                    _posY += diffY / distance * _speed;
                }
            }
        }
    }
}
