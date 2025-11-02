using System;

namespace Shootthemup //Shootthemup.Form.cs
{
    public partial class Form : System.Windows.Forms.Form
    {

        private List<Player> _player = new List<Player>()
        {
            new Player(1, 10)
        };

        private List<Enemy> _enemies = new List<Enemy>()
        {
            new Enemy(100, 100, 10),
            new Enemy(200, 100, 100),
            new Enemy(300, 100, 3),
            new Enemy(400, 100, 3),
            new Enemy(500, 100, 3)
        };

        private List<Obstacle> _obstacles = new List<Obstacle>();

        private List<Projectile> _projectiles = new List<Projectile>();

        private static int _gameSeed = 12344;

        private int _score = 0;
        private int _kills = 0;

        private BufferedGraphics _form;

        public static int GameSeed { get { return _gameSeed; } }

        public Form()
        {
            InitializeComponent();
            ticker.Interval = 10;

            KeyPreview = true;
            KeyDown += FormKeyDown;
            KeyUp += FormKeyUp;

            int numObstacles = Config.alea.Next(5, 9);

            for (int i = 0; i < numObstacles; i++)
            {
                bool positionValide = false;
                Obstacle newObstacle = null;

                while (!positionValide)
                {
                    int xPos = Config.alea.Next(100, Config.WIDTH - 100);
                    int yPos = Config.alea.Next(100, Config.HEIGHT - 150);
                    int health = Config.alea.Next(5, 10);
                    int size = Config.alea.Next(20, 40);

                    newObstacle = new Obstacle(xPos, yPos, health, size);

                    positionValide = true;
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
                            positionValide = false;
                            break;
                        }
                    }
                }

                if (positionValide)
                {
                    _obstacles.Add(newObstacle);
                }
            }

            _form = BufferedGraphicsManager.Current.Allocate(this.CreateGraphics(), this.DisplayRectangle);
        }

        private void NewFrame(object sender, EventArgs e)
        {
            Update(ticker.Interval);
            Render();
        }

        private void Update(int interval)
        {
            foreach (Player player in _player)
            {
                player.Update(interval);
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
                enemy.Update(interval);

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
                                _score += Enemy.Score;
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
    }
}