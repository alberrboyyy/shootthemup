using Shootthemup;
using System;

namespace Shootthemup //Model.Enemy.cs
{
    public partial class Enemy
    {
        public static readonly int MaxHealth = 3;       // Charge maximale de la batterie
        private int _health;                            // La charge actuelle de la batterie
        private string _name;                           // Un nom
        private int _x;                                 // Position en X depuis la gauche de l'espace aérien
        private int _y;                                 // Position en Y depuis le haut de l'espace aérien
        private int _shootCooldown;                     // shoot cooldown (ms)


        public static List<Enemy> Enemies { get; } = new List<Enemy>()
        {
            new Enemy(100, 50, "Enemy1", 3),
            new Enemy(300, 150, "Enemy2", 3),
            new Enemy(500, 100, "Enemy3", 3),
            new Enemy(700, 200, "Enemy4", 3),
            new Enemy(900, 75, "Enemy5", 3)
        };


        // Constructeur
        public Enemy(int x, int y, string name, int health)
        {
            _x = x;
            _y = y;
            _name = name;
            _health = health;

            _shootCooldown = GlobalHelpers.alea.Next(500, 2000);
        }

        public int Health { get { return _health; } set { _health = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }

        public void Update(int interval, List<Enemy> enemies)
        {
            if (_y >= AirSpace.HEIGHT)
            {
                _y = 0;
            }
            _y++;
        }

        /*
        public void TryShoot(int interval)
        {
            _shootCooldown -= interval;
            if (_shootCooldown <= 0)
            {
                _shootCooldown = 1000 + GlobalHelpers.alea.Next(0, 1000);

                const int enemyWidthHalf = 8;
                const int enemyHeight = 32;
                const int projectileWidthHalf = 3;

                int projX = _x + enemyWidthHalf - projectileWidthHalf;
                int projY = _y + enemyHeight;

                // damage=1, speed=3 (positive -> down), projectileType=1
                Projectile.Projectiles.Add(projX, _y, 1, 3, 2);
            }
        }*/
    }
}

