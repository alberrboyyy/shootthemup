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
        private int _speed = 0;
        private int _direction = 0;
        private int _shootCooldown;                     // shoot cooldown (ms)
        private bool _isShooting;

        public Rectangle BoundingBox
        {
            get { return new Rectangle(_x, _y, _sizeX, _sizeY); }
        }


        // Constructeur
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






        // Cette méthode calcule le nouvel état dans lequel le drone se trouve après
        // que 'interval' millisecondes se sont écoulées
        public void Update()
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

                // damage=1, speed=3 (positive -> down), projectileType=2
                return new Projectile(projX, _y, 1, 3, ProjectileType.Player);
            }
            return null;
        }


        public void Render(BufferedGraphics drawingSpace)
        {
            drawingSpace.Graphics.FillRectangle(Brushes.Green, _x, _y, _sizeX, _sizeY);
            //drawingSpace.Graphics.DrawString($"{this}", TextHelpers.drawFont, TextHelpers.writingBrush, X + 5, Y - 25);
        }

    }
}
