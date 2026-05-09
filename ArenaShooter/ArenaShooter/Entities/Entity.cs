using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaShooter.Entities 
{
    public abstract class Entity
    {
        protected float _posX;
        protected float _posY;
        protected int _width = 40;
        protected int _height = 40;
        protected float _speed;
        protected Image _sprite;
        protected float _rotation = 0;
        protected float _aimOffset = 0;
        protected int _hitboxOffset = 0;

        public Entity(float startX, float startY, float startSpeed)
        {
            _posX = startX;
            _posY = startY;
            _speed = startSpeed;
        }

        public float X { get => _posX; set => _posX = value; }
        public float Y { get => _posY; set => _posY = value; }
        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }

        public abstract void Update(HashSet<Keys> pressedKeys);

        public virtual void Draw(Graphics g)
        {
            if (_sprite == null) return;

            var state = g.Save();
            g.TranslateTransform(_posX + _width / 2, _posY + _height / 2);
            g.RotateTransform(_rotation + _aimOffset);
            g.DrawImage(_sprite, -_width / 2, -_height / 2, _width, _height);
            g.Restore(state);
        }

        public virtual Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)_posX + _hitboxOffset,
                    (int)_posY + _hitboxOffset,
                    _width - _hitboxOffset * 2,
                    _height - _hitboxOffset * 2
                );
            }
        }
    }
}
