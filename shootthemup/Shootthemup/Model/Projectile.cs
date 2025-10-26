namespace Shootthemup //Model.Projectiles.cs
{
    public partial class Projectile
    {
        private int _x;
        private int _y;
        private int _damage;
        private int _speed;
        private int _projectileType;

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


        public void Update()
        {
            if (_projectileType == 1)
            {
                _y += _speed;
            }
            if (_projectileType == 2)
            {
                _y -= _speed;
            }
        }
    }
}




