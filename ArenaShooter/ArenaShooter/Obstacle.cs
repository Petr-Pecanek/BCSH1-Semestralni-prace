using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaShooter
{
    public class Obstacle : Entity
    {
        private Color color;

        public Obstacle(float x, float y, int w, int h, Color col) : base(x, y, 0)
        {
            this.width = w;
            this.height = h;
            this.color = col;
        }

        public override void Update(HashSet<Keys> pressedKeys)
        {
            
        }

        public override void Draw(Graphics g)
        {
            using (Brush b = new SolidBrush(color))
            {
                g.FillRectangle(b, posX, posY, width, height);
                g.DrawRectangle(Pens.Black, posX, posY, width, height);
            }
        }
    }
}
