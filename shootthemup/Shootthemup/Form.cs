using System;

namespace Shootthemup //Shootthemup.Form.cs
{
    public partial class Form : System.Windows.Forms.Form
    {
        private List<Player> _player = new List<Player>()
        {
            new Player(1, 10)
        };

        private List<Enemy> _enemies = new List<Enemy>();
        private List<Obstacle> _obstacles = new List<Obstacle>();
        private List<Projectile> _projectiles = new List<Projectile>();

        private int _score = 0;
        private int _kills = 0;
        private int _waveNumber = 1;
        private int _maxEnemies = Config.alea.Next(60, 100);
        private const int _totalWaves = 10;
        private bool _gameIsWon = false;

        private BufferedGraphics _form;

        public Form()
        {
            InitializeComponent();
            ticker.Interval = 10;

            KeyPreview = true;
            KeyDown += FormKeyDown;
            KeyUp += FormKeyUp;

            GenerateObstacles();

            _form = BufferedGraphicsManager.Current.Allocate(this.CreateGraphics(), this.DisplayRectangle);
        }

        private void NewFrame(object sender, EventArgs e)
        {
            Update(ticker.Interval);
            Render();
        }

        private void Update(int interval)
        {
            if (_gameIsWon || !ticker.Enabled) return;

            if (_enemies.Count == 0)
            {
                if (_waveNumber >= _totalWaves)
                {
                    HandleWin();
                }
                else
                {
                    _waveNumber++;
                    GenerateWave();
                    GenerateObstacles();
                }
            }
            foreach (Player player in _player)
            {
                player.Update(interval, _obstacles);
                if (player.IsShooting)
                {
                    Projectile newProj = player.TryShoot(interval);
                    if (newProj != null)
                    {
                        _projectiles.Add(newProj);
                    }
                }
            }

            foreach (Enemy enemy in _enemies)
            {
                enemy.Update(interval, _obstacles);

                Projectile newProj = enemy.TryShoot(interval);
                if (newProj != null)
                {
                    _projectiles.Add(newProj);
                }
            }

            for (int i = _projectiles.Count - 1; i >= 0; i--)
            {
                Projectile projectile = _projectiles[i];
                bool projectileHit = false;
                projectile.Update();

                for (int k = _obstacles.Count - 1; k >= 0; k--)
                {
                    Obstacle obstacle = _obstacles[k];

                    double dx = projectile.CenterX - obstacle.CenterX;
                    double dy = projectile.CenterY - obstacle.CenterY;
                    double dc = (dx * dx) + (dy * dy);

                    double d = projectile.Radius + obstacle.Size;
                    double dMin = d * d;

                    if (dc < dMin)
                    {
                        projectileHit = true;
                        obstacle.Health -= projectile.Damage;

                        if (obstacle.Health <= 0)
                        {
                            _obstacles.RemoveAt(k);
                        }
                        break;
                    }
                }

                if (projectile.Type == ProjectileType.Player)
                {
                    for (int j = _enemies.Count - 1; j >= 0; j--)
                    {
                        Enemy enemy = _enemies[j];

                        double dx = projectile.CenterX - enemy.CenterX;
                        double dy = projectile.CenterY - enemy.CenterY;
                        double dc = (dx * dx) + (dy * dy);

                        double d = projectile.Radius + enemy.Radius;
                        double dMin = d * d;

                        if (dc < dMin)
                        {
                            projectileHit = true;
                            enemy.Health -= projectile.Damage;

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
                else if (projectile.Type == ProjectileType.Enemy)
                {
                    foreach (Player player in _player)
                    {
                        double dx = projectile.CenterX - player.CenterX;
                        double dy = projectile.CenterY - player.CenterY;
                        double dc = (dx * dx) + (dy * dy);

                        double d = projectile.Radius + player.Radius;
                        double dMin = d * d;

                        if (dc < dMin)
                        {
                            projectileHit = true;
                            player.Health -= projectile.Damage;

                            if (player.Health <= 0)
                            {
                                ticker.Stop();
                            }

                            break;
                        }
                    }
                }
                if (projectileHit || projectile.Y > Config.HEIGHT || projectile.Y < 0)
                {
                    _projectiles.RemoveAt(i);
                }
            }
        }

        private void Render()
        {
            _form.Graphics.Clear(Color.Black);

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

            Font scoreFont = new Font("Arial", 16, FontStyle.Bold);
            Brush scoreBrush = Brushes.White;

            _form.Graphics.DrawString($"Score: {_score}", scoreFont, scoreBrush, 2, 2);
            _form.Graphics.DrawString($"Kills: {_kills}", scoreFont, scoreBrush, 2, 24);
            _form.Render();
        }


        private void FormKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    foreach (Player player in _player)
                    {
                        player.Direction = -2;
                    }
                    break;
                case Keys.D:
                    foreach (Player player in _player)
                    {
                        player.Direction = 2;
                    }
                    break;
                case Keys.Space:
                    foreach (Player player in _player)
                    {
                        player.IsShooting = true;
                    }
                    break;
                case Keys.Q:
                case Keys.Escape:
                    Environment.Exit(0);
                    break;
            }
        }
        private void FormKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    foreach (Player player in _player)
                    {
                        player.IsShooting = false;
                    }
                    break;
                case Keys.A:
                case Keys.D:
                    foreach (Player player in _player)
                    {
                        player.Direction = 0;
                    }
                    break;
            }
        }
    
        private void GenerateObstacles()
        {
            int obstacleCount = Config.alea.Next(5, 9);

            for (int i = 0; i < obstacleCount; i++)
            {
                bool ValidPos = false;
                Obstacle newObstacle = null;

                while (!ValidPos)
                {
                    int xPos = Config.alea.Next(100, Config.WIDTH - 100);
                    int yPos = Config.alea.Next(100, Config.HEIGHT - 150);
                    int health = Config.alea.Next(5, 10);
                    int size = Config.alea.Next(20, 40);

                    newObstacle = new Obstacle(xPos, yPos, health, size);

                    ValidPos = true;
                    foreach (Obstacle obstacle in _obstacles)
                    {
                        double dx = newObstacle.CenterX - obstacle.CenterX;
                        double dy = newObstacle.CenterY - obstacle.CenterY;
                        double dc = (dx * dx) + (dy * dy);

                        double margin = 10.0;
                        double d = newObstacle.Size + obstacle.Size + margin;
                        double dMin = d * d;

                        if (dc < dMin)
                        {
                            ValidPos = false;
                            break;
                        }
                    }
                }

                if (ValidPos)
                {
                    _obstacles.Add(newObstacle);
                }
            }
        }

        private void GenerateWave()
        {
            int remainingBudget = _maxEnemies - _kills;
            if (remainingBudget <= 0) return;

            int baseWaveSize = Config.alea.Next(5, 10) + _waveNumber;
            int enemiesToSpawn = Math.Min(baseWaveSize, remainingBudget);

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                _enemies.Add(new Enemy());
            }
        }

        private void HandleWin()
        {
            _gameIsWon = true;
            ticker.Stop();
            MessageBox.Show(
                $"Félicitations !\nVous avez survécu aux {_totalWaves} vagues.\n\nScore final : {_score}",
                "Victoire !",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}