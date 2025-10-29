namespace Shootthemup //Model.Projectiles.cs
{
    public partial class Projectile
    {
        private int _x;
        private int _y;
        private int _damage;
        private int _speed;
        private int _projectileType;

        private int _shootCooldown;

        public static List<Projectile> Projectiles { get; set; } = new List<Projectile>()
        {
                        new Projectile(100, 500, 1, 3, 2)
        };


        // Constructeur
        public Projectile(int x, int y, int damage, int speed, int projectileType)
        {
            _x = x;
            _y = y;
            _damage = damage;
            _speed = speed;
            _projectileType = projectileType;
        }

        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public int Damage { get { return _damage; } set { _damage = value; } }
        public int Speed { get { return _speed; } set { _speed = value; } }
        public int projectileType { get { return _projectileType; } set { _projectileType = value; } }


        public void Update(int interval, List<Enemy> enemies)
        {
            if (_projectileType == 1)
            {
                _y += _speed;
            }
            if (_projectileType == 2)
            {
                _y -= _speed;
            }

            _shootCooldown -= interval;

            if (_shootCooldown <= 0)
            {
                _shootCooldown = 1000 + GlobalHelpers.alea.Next(0, 1000);

                const int enemyWidthHalf = 8;
                const int enemyHeight = 32;
                const int projectileWidthHalf = 3;
                foreach (Enemy enemy in enemies)
                {
                    int projX = enemy.X + enemyWidthHalf - projectileWidthHalf;
                    int projY = enemy.Y + enemyHeight;
                    // damage=1, speed=3, projectileType=1
                    Projectiles.Add(new Projectile(projX, projY, 1, 3, 1));
                }
            }
            if (_y > AirSpace.HEIGHT || _y < 0)
            {
                Projectiles.Remove(this);
            }
        }
    }
}




