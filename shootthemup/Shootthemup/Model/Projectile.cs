namespace Shootthemup //Shootthemup.Model.Projectiles.cs
{
    public class Projectile
    {
        private static int _size = 6;               // Taille du projectile
        private int _x;                             // Position en X depuis la gauche de l'espace aérien
        private int _y;                             // Position en Y depuis le haut de l'espace aérien
        private int _damage;                        // Dégâts infligés par le projectile
        private int _speed;                         // Vitesse de déplacement du projectile
        private ProjectileType _type;               // Type de projectile (ennemi ou joueur)


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
        public static int Size { get { return _size; } }
        public ProjectileType Type { get { return _type; } set { _type = value; } }
        public double CenterX { get { return _x + (_size / 2.0); } }
        public double CenterY { get { return _y + (_size / 2.0); } }
        public double Radius { get { return _size / 2.0; } }

        /// <summary>
        /// Update la position du projectile en fonction de son type (ennemi ou joueur).
        /// </summary>
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

        /// <summary>
        /// Render le projectile en forme de point bleu.
        /// </summary>
        /// <param name="drawingSpace"></param>
        public void Render(BufferedGraphics drawingSpace)
        {
            drawingSpace.Graphics.FillRectangle(Brushes.Blue, _x, _y, _size, _size);
        }

    }
}




