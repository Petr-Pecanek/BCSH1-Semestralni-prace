using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaShooter.Entities
{
    public class Obstacle : Entity
    {
        private const int DefaultHitboxOffset = 12;

        private Color _color;

        public Obstacle(float x, float y, int width, int height, Image obstacleImg) : base(x, y, 0)
        {
            _width = width;
            _height = height;
            _sprite = obstacleImg;
            _hitboxOffset = DefaultHitboxOffset;
        }

        public Obstacle(float x, float y, int width, int height, Color color) : base(x, y, 0)
        {
            _width = width;
            _height = height;
            _color = color;
        }

        public override void Update(HashSet<Keys> pressedKeys) { }

        public override void Draw(Graphics g)
        {
            if (_sprite != null)
            {
                base.Draw(g);
            }
            else
            {
                using (Brush b = new SolidBrush(_color))
                {
                    g.FillRectangle(b, _posX, _posY, _width, _height);
                    g.DrawRectangle(Pens.Black, _posX, _posY, _width, _height);
                }
            }
        }
    }
}
