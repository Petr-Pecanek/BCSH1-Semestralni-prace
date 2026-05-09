using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaShooter.Entities
{
    public class Bullet : Entity
    {
        private const float DefaultBulletSpeed = 12.0f;
        private const int BulletSize = 8;

        private float _velocityX;
        private float _velocityY;

        public Bullet(float startX, float startY, int targetX, int targetY) : base(startX, startY, DefaultBulletSpeed)
        {
            _width = BulletSize;
            _height = BulletSize;

            float diffX = targetX - startX;
            float diffY = targetY - startY;
            float distance = (float)Math.Sqrt(diffX * diffX + diffY * diffY);

            if (distance > 0)
            {
                _velocityX = diffX / distance * _speed;
                _velocityY = diffY / distance * _speed;
            } else
            {
                _velocityX = 0;
                _velocityY = -_speed;
            }
        }

        public override void Update(HashSet<Keys> pressedKeys)
        {
            _posX += _velocityX;
            _posY += _velocityY;
        }

        public override void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.Gold, _posX, _posY, _width, _height);
        }
    }
}
