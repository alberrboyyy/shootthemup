using System;

namespace Shootthemup //Shootthemup.Form.cs
{
    public partial class AirSpace : System.Windows.Forms.Form
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

        private List<Projectile> _projectiles = new List<Projectile>();

        private BufferedGraphics _from;

        public AirSpace()
        {
            InitializeComponent();
            ticker.Interval = 10;

            KeyPreview = true;
            KeyDown += AirspaceKeyDown;
            KeyUp += AirSpaceKeyUp;

            this._from = BufferedGraphicsManager.Current.Allocate(this.CreateGraphics(), this.DisplayRectangle);
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
                        double distanceSquared = (dx * dx) + (dy * dy);

                        double radiusSum = projectile.Radius + enemy.Radius;
                        double radiusSumSquared = radiusSum * radiusSum;

                        if (distanceSquared < radiusSumSquared)
                        {
                            projectileHit = true;
                            enemy.Health -= projectile.Damage;

                            if (enemy.Health <= 0)
                            {
                                _enemies.RemoveAt(j);
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
                        double distanceSquared = (dx * dx) + (dy * dy);

                        double radiusSum = projectile.Radius + player.Radius;
                        double radiusSumSquared = radiusSum * radiusSum;

                        if (distanceSquared < radiusSumSquared)
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
            _from.Graphics.Clear(Color.Black);

            foreach (Player player in _player)
            {
                player.Render(_from);
            }


            foreach (Enemy enemy in _enemies.ToList())
            {
                enemy.Render(_from);
            }

            foreach (Projectile projectile in _projectiles)
            {
                projectile.Render(_from);
            }

            _from.Render();
        }


        private void AirspaceKeyDown(object sender, KeyEventArgs e)
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
        private void AirSpaceKeyUp(object sender, KeyEventArgs e)
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