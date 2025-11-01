namespace Shootthemup //Shootthemup.Model.Player.cs
{
    public class Player
    {
        public static readonly int MaxHealth = 3;      // Charge maximale de la batterie
        private int _health;                            // La charge actuelle de la batterie
        private int _x;                                 // Position en X depuis la gauche de l'espace aérien
        private int _y;                                 // Position en Y depuis le haut de l'espace aérien
        private int _sizeX = 16;
        private int _sizeY = 32;
        private int _size = 40;
        private int _speed = 0;
        private int _direction = 0;
        private int _shootCooldown;                     // shoot cooldown (ms)
        private bool _isShooting;
        private double _shieldAngle = 0;
        private const float _coreSize = 16;
        private const float shieldRadius = 4;

        public Rectangle BoundingBox
        {
            get { return new Rectangle(_x, _y, _sizeX, _sizeY); }
        }

        public Player(int x, int health)
        {
            _x = x;
            _y = Config.HEIGHT - 100;
            _health = health;

            _shootCooldown = 0;
        }

        public int Health { get { return _health; } set { _health = value; } }
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public int Direction { get { return _direction; } set { _direction = value; } }
        public bool IsShooting { get { return _isShooting; } set { _isShooting = value; } }


        public void Update(int interval)
        {
            if (_direction != 0)
            {
                _x += _direction;
            }
            if (_x >= Config.WIDTH)
            {
                _x = 0;
            }
            else if (_x <= 0)
            {
                _x = Config.WIDTH;
            }


            double rotationSpeed = 1.0;
            double deltaTime = interval / 1000.0;
            _shieldAngle += rotationSpeed * deltaTime;
        }
        public Projectile TryShoot(int interval)
        {
            _shootCooldown -= interval;
            if (_shootCooldown <= 0)
            {
                _shootCooldown = 100;

                const int playerWidthHalf = 8;
                const int projectileWidthHalf = 3;

                int projX = _x + playerWidthHalf - projectileWidthHalf;


                return new Projectile(projX, _y, 1, 3, ProjectileType.Player);
            }
            return null;
        }


        public void Render(BufferedGraphics drawingSpace)
        {
            float centerX = (float)_x + (_size / 2);
            float centerY = (float)_y + (_size / 2);
            float orbitRadius = _size / 2;
            
            float coreRadius = _coreSize / 2;

            drawingSpace.Graphics.FillEllipse(Brushes.Green,
                centerX - coreRadius,
                centerY - coreRadius,
                coreRadius * 2,
                coreRadius * 2);

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

                    drawingSpace.Graphics.FillEllipse(Brushes.Yellow,
                        shieldDrawX,
                        shieldDrawY,
                        shieldRadius * 2,
                        shieldRadius * 2);
                }
            }
        }
    }
}
