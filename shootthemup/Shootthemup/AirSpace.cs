using System;

namespace Shootthemup //Shootthemup.Form.cs
{
    public partial class AirSpace : System.Windows.Forms.Form
    {

        private List<Player> _player = new List<Player>()
        {
            new Player(0, 3)
        };

        private List<Enemy> _enemies = new List<Enemy>()
        {
            new Enemy(100, 100, 3),
            new Enemy(200, 100, 3),
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
                player.Update();
                if (player.IsShooting)
                {
                    // On essaie de tirer (en respectant le cooldown)
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
                projectile.Update();
                if (projectile.Y > Config.HEIGHT || projectile.Y < 0)
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