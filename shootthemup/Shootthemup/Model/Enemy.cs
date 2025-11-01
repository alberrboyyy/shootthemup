using Shootthemup;
using Shootthemup.Properties;
using System;

namespace Shootthemup //Shootthemup.Model.Enemy.cs
{
    public class Enemy
    {
        public static readonly int MaxHealth = 3;       // Charge maximale de la batterie
        private int _health;                            // La charge actuelle de la batterie
        private string _name;                           // Un nom
        private int _x;                                 // Position en X depuis la gauche de l'espace aérien
        private int _y;                                 // Position en Y depuis le haut de l'espace aérien
        private int _sizeX = 16;
        private int _sizeY = 32;
        private int _count;
        private int _shootCooldown;                     // shoot cooldown (ms)

        public Rectangle BoundingBox
        {
            get { return new Rectangle(_x, _y, _sizeX, _sizeY); }
        }




        // Constructeur
        public Enemy(int x, int y, int health)
        {
            _x = x;
            _y = y;
            _health = health;

            _shootCooldown = Config.alea.Next(500, 2000);
        }

        public int Health { get { return _health; } set { _health = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public int Count { get { return _count; } set { _count = value; } }

        public void Update(int interval)
        {
            if (_y >= Config.HEIGHT)
            {
                _y = 0;
            }
            _y++;
        }

        public Projectile TryShoot(int interval)
        {
            _shootCooldown -= interval;
            if (_shootCooldown <= 0)
            {
                _shootCooldown = 1000 + Config.alea.Next(0, 1000);

                const int enemyWidthHalf = 8;
                const int enemyHeight = 32;
                const int projectileWidthHalf = 3;

                int projX = _x + enemyWidthHalf - projectileWidthHalf;
                int projY = _y + enemyHeight;

                return new Projectile(projX, projY, 1, 3, ProjectileType.Enemy);
            }
            return null;
        }


        public void Render(BufferedGraphics drawingSpace)
        {
            drawingSpace.Graphics.FillRectangle(Brushes.Red, _x, _y, _sizeX, _sizeY);
        }
    }
}

