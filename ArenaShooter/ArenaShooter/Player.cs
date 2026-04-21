using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ArenaShooter
{
    public class Player : Entity
    {
        public Player(float startX, float startY) : base(startX, startY, 5.0f) 
        {

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
                PosX += (moveX / length) * Speed;
                PosY += (moveY / length) * Speed;
            }
        }

        public override void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Blue, PosX, PosY, Width, Height);
        }
    }
}
