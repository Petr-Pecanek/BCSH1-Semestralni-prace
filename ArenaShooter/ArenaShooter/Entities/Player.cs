using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaShooter.Entities
{
    public class Player : Entity
    {
        private const float DefaultPlayerSpeed = 5.0f;
        private const int PlayerHitboxOffset = 5;
        private const float AnimSpeed = 0.2f;
        private const float AnimWobbleAmount = 12f;

        private float _animTimer = 0;

        public Player(float startX, float startY, Image playerImg) : base(startX, startY, DefaultPlayerSpeed) 
        {
            _sprite = playerImg;
            _hitboxOffset = PlayerHitboxOffset;
        }

        public override void Update(HashSet<Keys> pressedKeys)
        {
            float moveX = 0;
            float moveY = 0;

            if (pressedKeys.Contains(Keys.W)) moveY -= 1;
            if (pressedKeys.Contains(Keys.A)) moveX -= 1;
            if (pressedKeys.Contains(Keys.S)) moveY += 1;
            if (pressedKeys.Contains(Keys.D)) moveX += 1;

            float length = (float)Math.Sqrt(moveX * moveX + moveY * moveY);

            if (length > 0)
            {
                _animTimer += AnimSpeed;
                _aimOffset = (float)Math.Sin(_animTimer) * AnimWobbleAmount;

                _posX += moveX / length * _speed;
                _posY += moveY / length * _speed;
            } else
            {
                _animTimer = 0; 
                _aimOffset = 0;
            }
        }

        public void UpdateRotation(Point mousePos)
        {
            float diffX = mousePos.X - (_posX + Width / 2);
            float diffY = mousePos.Y - (_posY + Height / 2);

            _rotation = (float)(Math.Atan2(diffY, diffX) * 180 / Math.PI);
        }
    }
}
