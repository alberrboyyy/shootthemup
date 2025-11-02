using Shootthemup;
using Shootthemup.Properties;
using System;

namespace Shootthemup //Shootthemup.Model.Enemy.cs
{
    public class Enemy
    {
        public static readonly int MaxHealth = 3;       // Charge maximale de la batterie
        private const float _coreRadius = 8;            // Rayon du noyau de l'ennemi
        private const float shieldRadius = 4;           // Rayon des boucliers de l'ennemi

        private int _health;                            // La charge actuelle de la batterie
        private int _x;                                 // Position en X depuis la gauche de l'espace aérien
        private int _y;                                 // Position en Y depuis le haut de l'espace aérien
        private int _size = 40;                         // Taille de l'ennemi avec les boucliers
        private int _shootCooldown;                     // shoot cooldown (ms)
        private int _speed;                             // Vitesse de déplacement de l'ennemi
        private int _scoreValue;                        // Valeur en points de l'ennemi

        private double _shieldAngle = 0;

        // Constructeur
        public Enemy()
        {
            _x = Config.alea.Next(0, Config.WIDTH);
            _y = Config.alea.Next(0 - Config.HEIGHT, 0);
            _health = Config.alea.Next(3, 10);
            _speed = Config.alea.Next(1, 3);
            _shootCooldown = Config.alea.Next(200, 1000);
            _scoreValue = _health * 50;
        }

        public int Health { get { return _health; } set { _health = value; } }
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public int ScoreValue { get { return _scoreValue; } set { _scoreValue = value; } }
        public int CenterX { get { return _x + (_size / 2); } }
        public int CenterY { get { return _y + (_size / 2); } }
        public double Radius { get { return _size / 2.0; } }

        /// <summary>
        /// // Update la position de l'ennemi en fonction de sa vitesse et gère les collisions avec les obstacles.
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="obstacles"></param>
        public void Update(int interval, List<Obstacle> obstacles)
        {
            int oldY = _y;                                  // Sauvegarde de l'ancienne position Y
            double rotationSpeed = 1.0;                     // Vitesse de rotation des boucliers (radians par seconde)
            double deltaTime = interval / 1000.0;           // Conversion de l'intervalle en secondes
            _shieldAngle += rotationSpeed * deltaTime;      // Mise à jour de l'angle des boucliers

            // Met à jour la position en fonction de la vitesse
            if (_y >= Config.HEIGHT)
            {
                _y = 0;
            }
            _y += _speed;

            // Vérifie les collisions avec les obstacles
            bool isColliding = false;
            foreach (Obstacle obstacle in obstacles)
            {
                // Calcul de la distance au carré entre les centres
                double dx = this.CenterX - obstacle.CenterX;
                double dy = this.CenterY - obstacle.CenterY;
                double dc = (dx * dx) + (dy * dy);

                double d = this.Radius + obstacle.Size;
                double dMin = d * d;

                // Si la distance au carré est inférieure à la distance minimale au carré, il y a collision
                if (dc < dMin)
                {
                    isColliding = true;
                    break;
                }
            }

            // Si une collision est détectée, annule le déplacement
            if (isColliding)
            {
                _y = oldY;
            }

        }

        /// <summary>
        /// Tante de tirer un projectile si le cooldown est écoulé.
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public Projectile TryShoot(int interval)
        {
            _shootCooldown -= interval;

            // Si le cooldown est écoulé, tire un projectile
            if (_shootCooldown <= 0)
            {
                _shootCooldown = 1000 + Config.alea.Next(0, 1000);

                // Position de départ du projectile (au centre en bas de l'ennemi)
                int projX = _x + _size / 2 - Projectile.Size / 2;
                int projY = _y + + _size / 2 + (int)_coreRadius / 2;

                return new Projectile(projX, projY, 1, 3, ProjectileType.Enemy);
            }
            return null;
        }

        /// <summary>
        /// Renderce l'ennemi et ses boucliers.
        /// </summary>
        /// <param name="drawingSpace"></param>
        public void Render(BufferedGraphics drawingSpace)
        {
            // Rayon d'orbite des boucliers autour du noyau
            drawingSpace.Graphics.FillEllipse(Brushes.Red,
                this.CenterX - _coreRadius,
                this.CenterY - _coreRadius,
                _coreRadius * 2,
                _coreRadius * 2);

            // Render des boucliers si la santé est supérieure à 1
            if (_health > 1)
            {
                // Nombre de boucliers à dessiner
                int numShields = _health - 1;

                float angleIncrement = (float)(2 * Math.PI / numShields);

                // Boucle pour dessiner chaque bouclier
                for (int i = 0; i < numShields; i++)
                {
                    // Calcul de l'angle actuel du bouclier avec la rotation
                    float angle = i * angleIncrement + (float)_shieldAngle;

                    // Calcul de la position centrale du bouclier en orbite autour de l'ennemi
                    float shieldCenterX = this.CenterX + (float)(this.Radius * Math.Cos(angle));
                    float shieldCenterY = this.CenterY + (float)(this.Radius * Math.Sin(angle));

                    // Calcul des coordonnées de dessin du bouclier
                    float shieldDrawX = shieldCenterX - shieldRadius;
                    float shieldDrawY = shieldCenterY - shieldRadius;

                    // Dessin du bouclier, ici en aqua.
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

