namespace Shootthemup
{
    public partial class AirSpace : Form
    {
        public static readonly int WIDTH = 1200;
        public static readonly int HEIGHT = 600;

        private Player _player = new Player(0, 3);


        List<Enemy> enemies = new List<Enemy>()
        {
            new Enemy(100, 50, "Enemy1", 3),
            new Enemy(300, 150, "Enemy2", 3),
            new Enemy(500, 100, "Enemy3", 3),
            new Enemy(700, 200, "Enemy4", 3),
            new Enemy(900, 75, "Enemy5", 3)
        };
    List<Projectile> projectiles = new List<Projectile>()
        {
            new Projectile(100, 500, 1, 3, 2)

        };

        BufferedGraphicsContext currentContext;
        BufferedGraphics airspace;

        public AirSpace()
        {
            InitializeComponent();
            currentContext = BufferedGraphicsManager.Current;

            KeyPreview = true;
            KeyDown += AirspaceKeyDown;
            KeyUp += AirSpaceKeyUp;

            airspace = currentContext.Allocate(this.CreateGraphics(), this.DisplayRectangle);
        }

        private void Render()
        {
            airspace.Graphics.Clear(Color.Black);
            
            // render players (green squares)
            _player.Render(airspace);
            

            // render enemies (red squares)
            foreach (Enemy enemy in enemies)
            {
                enemy.Render(airspace);
            }

            // render projectiles (small blue squares)
            foreach (Projectile proj in projectiles)
            {
                proj.Render(airspace);
            }

            airspace.Render();
        }

        private void Update(int interval)
        {
            _player.Update(interval);

            foreach (Enemy enemy in enemies)
            {
                enemy.Update(interval, enemies);

                Projectile enemyProj = enemy.TryShoot(interval);
                if (enemyProj != null)
                {
                    projectiles.Add(enemyProj);
                }
            }

            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();
                if (projectiles[i].Y > HEIGHT || projectiles[i].Y < 0)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        private void NewFrame(object sender, EventArgs e)
        {
            ticker.Interval = 10;
            Update(ticker.Interval);
            Render();
        }
        
        private void AirspaceKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    _player.Direction = -2;
                    break;
                case Keys.D:
                    _player.Direction = 2;
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
                case Keys.A:
                case Keys.D:
                    _player.Direction = 0;
                    break;
            }
        }
    }
}