namespace Shootthemup //Shootthemup.Model.Player.cs
{
    public class Player
    {
        public static readonly int MaxHealth = 3;      // Charge maximale de la batterie
        private const int _coreRadius = 8;
        private const float shieldRadius = 4;
        private int _health;                            // La charge actuelle de la batterie
        private int _x;                                 // Position en X depuis la gauche de l'espace aérien
        private int _y;                                 // Position en Y depuis le haut de l'espace aérien
        private int _size = 40;                         // Taille du joueur avec les boucliers
        private int _direction = 0;                     // Direction de déplacement (-1 gauche, 1 droite, 0 immobile)
        private int _shootCooldown;                     // Cooldown de shoot (ms)
        private bool _isShooting;                       // Indique si le joueur tire ou non
        private double _shieldAngle = 0;                // Angle de rotation des boucliers
        private double _contactDamageCooldown = 0.0;    // Cooldown pour les dégâts de contact (en secondes)

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
        public int CenterX { get { return _x + (_size / 2); } }
        public int CenterY { get { return _y + (_size / 2); } }
        public double Radius { get { return _size / 2.0; } }


        /// <summary>
        /// Update la position du joueur en fonction de la direction et gère les collisions avec les obstacles.
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="obstacles"></param>
        public void Update(int interval, List<Obstacle> obstacles)
        {
            int oldX = _x;                                  // Sauvegarde de l'ancienne position X
            double deltaTime = interval / 1000.0;           // Conversion de l'intervalle en secondes
            double rotationSpeed = 1.0;                     // Vitesse de rotation des boucliers (radians par seconde)
            _shieldAngle += rotationSpeed * deltaTime;      // Mise à jour de l'angle des boucliers

            // Met à jour le cooldown des dégâts de contact
            if (_contactDamageCooldown > 0)
            {
                _contactDamageCooldown -= deltaTime;
            }

            // Met à jour la position en fonction de la direction
            if (_direction != 0)
            {
                _x += _direction;
            }

            // Gestion de la teleportation horizontale
            if (_x >= Config.WIDTH) 
            { 
                _x = 0; 
            }
            else if (_x <= 0) 
            { 
                _x = Config.WIDTH; 
            }

            // Vérifie les collisions avec les obstacles
            bool isColliding = false;
            foreach (Obstacle obstacle in obstacles)
            {
                // Calcul de la distance au carré entre le joueur et l'obstacle avec le theoreme de Pythagore
                double dx = _x - obstacle.CenterX;
                double dy = _y - obstacle.CenterY;
                double dc = (dx * dx) + (dy * dy);

                double d = _coreRadius + obstacle.Size;
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
                _x = oldX;
            }

        }

        /// <summary>
        /// Tante de tirer un projectile si le cooldown est terminé.
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public Projectile TryShoot(int interval)
        {
            _shootCooldown -= interval;

            // Si le cooldown est terminé, crée un nouveau projectile
            if (_shootCooldown <= 0)
            {
                _shootCooldown = 100;

                int projX = _x + _size / 2 - Projectile.Size / 2;

                return new Projectile(projX, _y, 1, 3, ProjectileType.Player);
            }
            return null;
        }

        /// <summary>
        /// Applique 1 dégâts de contact si le cooldown de 0.5s est terminé.
        /// </summary>
        public void ApplyContactDamage()
        {
            if (_contactDamageCooldown <= 0)
            {
                _health -= 1;

                _contactDamageCooldown = 0.5;
            }
        }

        /// <summary>
        /// Render le joueur et ses boucliers.
        /// </summary>
        /// <param name="drawingSpace"></param>
        public void Render(BufferedGraphics drawingSpace)
        {
            // Render du noyau du joueur
            drawingSpace.Graphics.FillEllipse(Brushes.Green,
                this.CenterX - _coreRadius,
                this.CenterY - _coreRadius,
                _coreRadius * 2,
                _coreRadius * 2);

            // Render des boucliers si la santé est supérieure à 1
            if (_health > 1)
            {
                int numShields = _health - 1;                                   // Nombre de boucliers à dessiner

                float angleIncrement = (float)(2 * Math.PI / numShields);       // Incrément d'angle entre chaque bouclier

                // Boucle pour dessiner chaque bouclier
                for (int i = 0; i < numShields; i++)
                {
                    // Calcul de l'angle actuel du bouclier avec la rotation
                    float angle = i * angleIncrement + (float)_shieldAngle;

                    // Calcul de la position centrale du bouclier en orbite autour du joueur
                    float shieldCenterX = this.CenterX + (float)(this.Radius * Math.Cos(angle));
                    float shieldCenterY = this.CenterY + (float)(this.Radius * Math.Sin(angle));

                    // Calcul de la position de dessin du bouclier
                    float shieldDrawX = shieldCenterX - shieldRadius;
                    float shieldDrawY = shieldCenterY - shieldRadius;

                    // Dessin du bouclier, ici en jaune.
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
