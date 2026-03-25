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
            if (pressedKeys.Contains(Keys.W)) PosY -= Speed;
            if (pressedKeys.Contains(Keys.A)) PosX -= Speed;
            if (pressedKeys.Contains(Keys.S)) PosY += Speed;
            if (pressedKeys.Contains(Keys.D)) PosX += Speed;
        }

        public override void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Blue, PosX, PosY, Width, Height);
        }
    }
}
