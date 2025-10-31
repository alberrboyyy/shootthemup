namespace Shootthemup //Shootthemup.Model.Projectiles.cs
{
    public class Projectile
    {
        private int _x;
        private int _y;
        private int _sizeX = 6;
        private int _sizeY = 6;
        private int _damage;
        private int _speed;
        private ProjectileType _type;

        public Rectangle BoundingBox
        {
            get { return new Rectangle(_x, _y, _sizeX, _sizeY); }
        }



        // Constructeur
        public Projectile(int x, int y, int damage, int speed, ProjectileType type)
        {
            _x = x;
            _y = y;
            _damage = damage;
            _speed = speed;
            _type = type;
        }

        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public int Damage { get { return _damage; } set { _damage = value; } }
        public int Speed { get { return _speed; } set { _speed = value; } }
        public ProjectileType Type { get { return _type; } set { _type = value; } }


        public void Update()
        {
            if (_type == ProjectileType.Enemy)
            {
                _y += _speed;
            }
            if (_type == ProjectileType.Player)
            {
                _y -= _speed;
            }

        }


        public void Render(BufferedGraphics drawingSpace)
        {
            drawingSpace.Graphics.FillRectangle(Brushes.Blue, _x, _y, _sizeX, _sizeY);
        }

    }
}




