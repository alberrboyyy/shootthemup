using Shootthemup;
using Shootthemup.Properties;
using System;

namespace Shootthemup //Shootthemup.Model.Enemy.cs
{
    public class Enemy
    {
        public static readonly int MaxHealth = 3;       // Charge maximale de la batterie
        private const float _coreRadius = 8;
        private const float shieldRadius = 4;

        private int _health;                            // La charge actuelle de la batterie
        private int _x;                                 // Position en X depuis la gauche de l'espace aérien
        private int _y;                                 // Position en Y depuis le haut de l'espace aérien
        private int _sizeX = 16;
        private int _sizeY = 32;
        private int _size = 40;
        private int _count;
        private int _shootCooldown;                     // shoot cooldown (ms)

        private double _shieldAngle = 0;

        // Constructeur
        public Enemy(int x, int y, int health)
        {
            _x = x;
            _y = y;
            _health = health;

            _shootCooldown = Config.alea.Next(500, 2000);
        }

        public int Health { get { return _health; } set { _health = value; } }
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public int Count { get { return _count; } set { _count = value; } }
        public static int Score { 
            get 
            {
                return 100;
            }
        }


        public double CenterX
        {
            get { return _x + (_size / 2.0); }
        }

        public double CenterY
        {
            get { return _y + (_size / 2.0); }
        }

        public double Radius
        {
            get
            {
                if (_health > 1)
                    return _size / 2.0;

                return _coreRadius;
            }
        }


        public void Update(int interval)
        {
            if (_y >= Config.HEIGHT)
            {
                _y = 0;
            }
            _y++;

            double rotationSpeed = 1.0;
            double deltaTime = 0.01;
            _shieldAngle += rotationSpeed * deltaTime;
        }

        public Projectile TryShoot(int interval)
        {
            _shootCooldown -= interval;
            if (_shootCooldown <= 0)
            {
                _shootCooldown = 1000 + Config.alea.Next(0, 1000);

                int projX = _x + _size / 2 - Projectile.Size / 2;
                int projY = _y + _size;

                return new Projectile(projX, projY, 1, 3, ProjectileType.Enemy);
            }
            return null;
        }


        public void Render(BufferedGraphics drawingSpace)
        {
            float centerX = _x + (_size / 2);
            float centerY = _y + (_size / 2);
            float orbitRadius = _size / 2;

            drawingSpace.Graphics.FillEllipse(Brushes.Red,
                centerX - _coreRadius,
                centerY - _coreRadius,
                _coreRadius * 2,
                _coreRadius * 2);

            if (_health > 1)
            {
                int numShields = _health - 1;

                float angleIncrement = (float)(2 * Math.PI / numShields);

                for (int i = 0; i < numShields; i++)
                {
                    float angle = i * angleIncrement + (float)_shieldAngle;

                    float shieldCenterX = centerX + (float)(orbitRadius * Math.Cos(angle));
                    float shieldCenterY = centerY + (float)(orbitRadius * Math.Sin(angle));

                    float shieldDrawX = shieldCenterX - shieldRadius;
                    float shieldDrawY = shieldCenterY - shieldRadius;

                    drawingSpace.Graphics.FillEllipse(Brushes.Aqua,
                        shieldDrawX,
                        shieldDrawY,
                        shieldRadius * 2,
                        shieldRadius * 2);
                }
            }
        }
    }
}

