namespace Shootthemup //Model.Player.cs
{
    public partial class Player
    {
        public static readonly int MaxHealth = 3;      // Charge maximale de la batterie
        private int _health;                            // La charge actuelle de la batterie
        private int _x;                                 // Position en X depuis la gauche de l'espace aérien
        private int _y;                                 // Position en Y depuis le haut de l'espace aérien
        private int _direction = 0;
        private int _shootCooldown;                     // shoot cooldown (ms)

        // Constructeur
        public Player(int x, int health)
        {
            _x = x;
            _y = AirSpace.HEIGHT - 100;
            _health = health;

            _shootCooldown = 0;
        }

        public int Health { get { return _health; } set { _health = value; } }
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public int Direction { get { return _direction; } set { _direction = value; } }






        // Cette méthode calcule le nouvel état dans lequel le drone se trouve après
        // que 'interval' millisecondes se sont écoulées
        public void Update(int interval)
        {
            if (_direction != 0)
            {
                _x += _direction;
            }
            if (_x >= AirSpace.WIDTH)
            {
                _x = 0;
            }
            else if (_x <= 0)
            {
                _x = AirSpace.WIDTH;
            }
/*
            Projectile newProj = TryShoot(AirSpace.ticker.Interval);
            if (newProj != null)
            {
                projectiles.Add(newProj);
            }*/
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
                return new Projectile(projX, _y, 1, 3, 2);
            }
            return null;
        }

    }
}
