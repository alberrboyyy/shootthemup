using System;
using System.IO;

namespace Shootthemup //Shootthemup.Form.cs
{
    public partial class Form : System.Windows.Forms.Form
    {
        // Liste des joueurs, ennemis, obstacles et projectiles dans le jeu
        private List<Player> _player = new List<Player>() { new Player(1, 10) };
        private List<Enemy> _enemies;
        private List<Obstacle> _obstacles = new List<Obstacle>();
        private List<Projectile> _projectiles = new List<Projectile>();

        private int _score = 0;                                 // Score du joueur
        private int _kills = 0;                                 // Nombre d'ennemis tués
        private int _waveNumber = 1;                            // Numéro de la vague actuelle
        private int _maxEnemies = Config.alea.Next(60, 100);    // Nombre maximum d'ennemis à affronter
        private const int _totalWaves = 10;                     // Nombre total de vagues dans le jeu
        private bool _gameIsWon = false;                        // Indique si le jeu est gagné
        private int _highScore = 0;                             // Meilleur score enregistré
        private const string _highScoreFile = "highscore.txt";  // Fichier pour stocker le meilleur score

        private BufferedGraphics _form;

        public Form()
        {
            // Initialisation du formulaire
            InitializeComponent();
            ticker.Interval = 10;
            LoadHighScore();
            _enemies = new List<Enemy>();


            // Gestion des événements clavier
            KeyPreview = true;
            KeyDown += FormKeyDown;
            KeyUp += FormKeyUp;

            // Génération de la première vague d'obstacles
            GenerateObstacles();

            _form = BufferedGraphicsManager.Current.Allocate(this.CreateGraphics(), this.DisplayRectangle);
        }

        /// <summary>
        /// Point d'entrée pour chaque nouvelle frame du jeu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewFrame(object sender, EventArgs e)
        {
            Update(ticker.Interval);
            Render();
        }

        /// <summary>
        /// Update de l'état du jeu : positions des joueurs, ennemis, projectiles, gestion des collisions, etc.
        /// </summary>
        /// <param name="interval"></param>
        private void Update(int interval)
        {
            // Si le jeu est gagné ou en pause, on ne fait rien
            if (_gameIsWon || !ticker.Enabled) return;
            
            // Si tous les ennemis sont éliminés, on génère une nouvelle vague ou on déclare la victoire
            if (_enemies.Count == 0)
            {
                if (_waveNumber >= _totalWaves)
                {
                    HandleWin();
                    SaveHighScoreIfNew();
                }
                else
                {
                    _waveNumber++;
                    GenerateWave();
                    GenerateObstacles();
                }
            }

            // Mise à jour des joueurs
            foreach (Player player in _player)
            {
                player.Update(interval, _obstacles);
                // Gestion du tir du joueur
                if (player.IsShooting)
                {
                    // Essaye de tirer un projectile
                    Projectile newProj = player.TryShoot(interval);
                    // Si un projectile a été tiré, on l'ajoute à la liste des projectiles
                    if (newProj != null)
                    {
                        _projectiles.Add(newProj);
                    }
                }

                // Vérification des collisions entre le joueur et les ennemis
                foreach (Enemy enemy in _enemies)
                {
                    // Calcul de la distance au carré entre le joueur et l'ennemi
                    double dx = player.CenterX - enemy.CenterX;
                    double dy = player.CenterY - enemy.CenterY;
                    double dc = (dx * dx) + (dy * dy);

                    double d = player.Radius + enemy.Radius;
                    double dMin = d * d;

                    // Si une collision est détectée, le joueur subit des dégâts de contact
                    if (dc < dMin)
                    {
                        player.ApplyContactDamage();
                    }
                }
            }

            // Mise à jour des ennemis
            foreach (Enemy enemy in _enemies)
            {
                enemy.Update(interval, _obstacles);

                // Gestion du tir de l'ennemi
                Projectile newProj = enemy.TryShoot(interval);
                // Si un projectile a été tiré, on l'ajoute à la liste des projectiles
                if (newProj != null)
                {
                    _projectiles.Add(newProj);
                }
            }

            // Mise à jour des projectiles et gestion des collisions
            for (int i = _projectiles.Count - 1; i >= 0; i--)
            {
                Projectile projectile = _projectiles[i];
                bool projectileHit = false;
                projectile.Update();

                // Vérification des collisions avec les obstacles
                for (int k = _obstacles.Count - 1; k >= 0; k--)
                {
                    Obstacle obstacle = _obstacles[k];

                    // Calcul de la distance au carré entre le projectile et l'obstacle
                    double dx = projectile.CenterX - obstacle.CenterX;
                    double dy = projectile.CenterY - obstacle.CenterY;
                    double dc = (dx * dx) + (dy * dy);

                    double d = projectile.Radius + obstacle.Size;
                    double dMin = d * d;

                    //  Si une collision est détectée, l'obstacle subit des dégâts
                    if (dc < dMin)
                    {
                        projectileHit = true;
                        obstacle.Health -= projectile.Damage;

                        // Si l'obstacle n'a plus de points de vie, il est supprimé
                        if (obstacle.Health <= 0)
                        {
                            _obstacles.RemoveAt(k);
                        }
                        break;
                    }
                }

                // Vérification des collisions avec les joueurs ou les ennemis selon le type de projectile
                if (projectile.Type == ProjectileType.Player)
                {
                    for (int j = _enemies.Count - 1; j >= 0; j--)
                    {
                        Enemy enemy = _enemies[j];

                        // Calcul de la distance au carré entre le projectile et l'ennemi
                        double dx = projectile.CenterX - enemy.CenterX;
                        double dy = projectile.CenterY - enemy.CenterY;
                        double dc = (dx * dx) + (dy * dy);

                        double d = projectile.Radius + enemy.Radius;
                        double dMin = d * d;

                        // Si une collision est détectée, l'ennemi subit des dégâts
                        if (dc < dMin)
                        {
                            projectileHit = true;
                            enemy.Health -= projectile.Damage;

                            // Si l'ennemi n'a plus de points de vie, il est supprimé et le score est mis à jour
                            if (enemy.Health <= 0)
                            {
                                _enemies.RemoveAt(j);
                                _score += enemy.ScoreValue;
                                _kills++;
                            }
                            break;
                        }
                    }
                }

                // Gestion des collisions pour les projectiles ennemis
                else if (projectile.Type == ProjectileType.Enemy)
                {
                    foreach (Player player in _player)
                    {
                        // Calcul de la distance au carré entre le projectile et le joueur
                        double dx = projectile.CenterX - player.CenterX;
                        double dy = projectile.CenterY - player.CenterY;
                        double dc = (dx * dx) + (dy * dy);

                        double d = projectile.Radius + player.Radius;
                        double dMin = d * d;

                        // Si une collision est détectée, le joueur subit des dégâts
                        if (dc < dMin)
                        {
                            projectileHit = true;
                            player.Health -= projectile.Damage;

                            // Si le joueur n'a plus de points de vie, le jeu s'arrête
                            if (player.Health <= 0)
                            {
                                ticker.Stop();
                                SaveHighScoreIfNew();
                            }

                            break;
                        }
                    }
                }

                // Si le projectile a touché quelque chose ou est hors de l'écran, il est supprimé
                if (projectileHit || projectile.Y > Config.HEIGHT || projectile.Y < 0)
                {
                    _projectiles.RemoveAt(i);
                }
            }
        }
        /// <summary>
        /// Render l'état actuel du jeu : joueurs, ennemis, projectiles, obstacles, score, etc.
        /// </summary>
        private void Render()
        {
            // Efface l'écran en noir
            _form.Graphics.Clear(Color.Black);

            // Render des joueurs, ennemis, projectiles et obstacles
            foreach (Player player in _player)
            {
                player.Render(_form);
            }

            foreach (Enemy enemy in _enemies.ToList())
            {
                enemy.Render(_form);
            }

            foreach (Projectile projectile in _projectiles)
            {
                projectile.Render(_form);
            }

            foreach (Obstacle obstacle in _obstacles)
            {
                obstacle.Render(_form);
            }

            // Render du score et du nombre de kills
            Font scoreFont = new Font("Arial", 16, FontStyle.Bold);
            Brush scoreBrush = Brushes.White;

            _form.Graphics.DrawString($"Score: {_score}", scoreFont, scoreBrush, 2, 2);
            _form.Graphics.DrawString($"Kills: {_kills}", scoreFont, scoreBrush, 2, 24);
            _form.Graphics.DrawString($"Meilleur Score: {_highScore}", scoreFont, scoreBrush, 2, 46);
            _form.Render();
        }

        /// <summary>
        /// S'occupe des entrées clavier pour déplacer le joueur, tirer, ou quitter le jeu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormKeyDown(object sender, KeyEventArgs e)
        {
            // Gestion des touches appuyées
            switch (e.KeyCode)
            {
                // Déplacement à gauche
                case Keys.A:
                    foreach (Player player in _player)
                    {
                        player.Direction = -2;
                    }
                    break;
                // Déplacement à droite
                case Keys.D:
                    foreach (Player player in _player)
                    {
                        player.Direction = 2;
                    }
                    break;
                // Tirer
                case Keys.Space:
                    foreach (Player player in _player)
                    {
                        player.IsShooting = true;
                    }
                    break;
                // Quitter le jeu
                case Keys.Q:
                case Keys.Escape:
                    Environment.Exit(0);
                    break;
            }
        }
        /// <summary>
        /// S'occupe des entrées clavier pour arrêter le déplacement ou le tir du joueur.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormKeyUp(object sender, KeyEventArgs e)
        {
            // Gestion des touches relâchées
            switch (e.KeyCode)
            {
                // Arrêter de tirer
                case Keys.Space:
                    foreach (Player player in _player)
                    {
                        player.IsShooting = false;
                    }
                    break;
                // Arrêter le déplacement
                case Keys.A:
                case Keys.D:
                    foreach (Player player in _player)
                    {
                        player.Direction = 0;
                    }
                    break;
            }
        }

        /// <summary>
        /// Gère la génération aléatoire des obstacles dans l'espace aérien.
        /// </summary>
        private void GenerateObstacles()
        {
            // Détermine un nombre aléatoire d'obstacles à générer
            int obstacleCount = Config.alea.Next(5, 9);

            for (int i = 0; i < obstacleCount; i++)
            {
                bool ValidPos = false;
                Obstacle newObstacle = null;

                // Génère une position valide pour l'obstacle
                while (!ValidPos)
                {
                    // Génération aléatoire des paramètres de l'obstacle
                    int xPos = Config.alea.Next(100, Config.WIDTH - 100);
                    int yPos = Config.alea.Next(100, Config.HEIGHT - 150);
                    int health = Config.alea.Next(5, 10);
                    int size = Config.alea.Next(20, 40);

                    // Création de l'obstacle
                    newObstacle = new Obstacle(xPos, yPos, health, size);

                    // Vérification des collisions avec les autres obstacles
                    ValidPos = true;
                    foreach (Obstacle obstacle in _obstacles)
                    {
                        // Calcul de la distance au carré entre les centres
                        double dx = newObstacle.CenterX - obstacle.CenterX;
                        double dy = newObstacle.CenterY - obstacle.CenterY;
                        double dc = (dx * dx) + (dy * dy);

                        double margin = 10.0;
                        double d = newObstacle.Size + obstacle.Size + margin;
                        double dMin = d * d;

                        // Si la distance au carré est inférieure à la distance minimale au carré, l'obstacle n'est pas valide et on recommence
                        if (dc < dMin)
                        {
                            ValidPos = false;
                            break;
                        }
                    }
                }

                // Ajoute l'obstacle valide à la liste des obstacles
                if (ValidPos)
                {
                    _obstacles.Add(newObstacle);
                }
            }
        }

        /// <summary>
        /// Génère une nouvelle vague d'ennemis en fonction du numéro de la vague et du budget d'enemis restant.
        /// </summary>
        private void GenerateWave()
        {
            int enemyLeft = _maxEnemies - _kills;
            if (enemyLeft <= 0) return;

            // Détermine la taille de la vague en fonction du numéro de la vague
            int baseWaveSize = Config.alea.Next(5, 10) + _waveNumber;
            int enemiesToSpawn = Math.Min(baseWaveSize, enemyLeft);

            // Génère les ennemis et les ajoute à la liste des ennemis
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                _enemies.Add(new Enemy());
            }
        }

        /// <summary>
        /// S'occupe de la fin du jeu en cas de victoire.
        /// </summary>
        private void HandleWin()
        {
            // Marque le jeu comme gagné et affiche un message de victoire
            _gameIsWon = true;
            ticker.Stop();
            MessageBox.Show(
                $"Félicitations !\nVous avez survécu aux {_totalWaves} vagues.\n\nScore final : {_score}",
                "Victoire !",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        /// <summary>
        /// Charge le meilleur score depuis un fichier texte.
        /// </summary>
        private void LoadHighScore()
        {
            // Vérifie si le fichier existe
            if (File.Exists(_highScoreFile))
            {
                try
                {
                    // Lit le contenu du fichier
                    string scoreText = File.ReadAllText(_highScoreFile);

                    // Tente de le convertir en nombre
                    int.TryParse(scoreText, out _highScore);
                }
                catch (Exception ex)
                {
                    // S'il y a une erreur, on remet le score à 0
                    _highScore = 0;
                }
            }
        }
        /// <summary>
        /// Update le meilleur score si le score actuel est supérieur et sauvegarde dans un fichier texte.  
        /// </summary>
        private void SaveHighScoreIfNew()
        {
            // Si le score actuel est meilleur
            if (_score > _highScore)
            {
                _highScore = _score; // Met à jour le high score
                try
                {
                    // Écrase (ou crée) le fichier avec le nouveau score
                    File.WriteAllText(_highScoreFile, _score.ToString());
                }
                catch (Exception ex)
                {
                    // Gère une erreur si on ne peut pas écrire le fichier
                    MessageBox.Show("Erreur de sauvegarde du score : " + ex.Message);
                }
            }
        }
    }
}