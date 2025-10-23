namespace Shootthemup
{
    public partial class AirSpace : Form
    {
        public static readonly int WIDTH = 1200;
        public static readonly int HEIGHT = 600;

        List<Player> players = new List<Player>
            {
                new Player(1, "Joe", 1)
            };


        List<Enemy> enemies = new List<Enemy>
            {
                new Enemy(100, 200, "Joe", 1),
                new Enemy(200, 200, "Joe", 1)
            };

        private List<Projectile> projectiles = new List<Projectile>();
        
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
            airspace.Graphics.Clear(Color.White);

            // render players (green squares)
            foreach (Player player in players)
            {
                player.Render(airspace);
            }

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
            foreach (Player player in players)
            {
                player.Update(interval, players);
            }
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(interval, enemies);

                Projectile newProj = enemy.TryShoot(interval);
                if (newProj != null)
                {
                    projectiles.Add(newProj);
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
            Update(ticker.Interval);
            Render();
        }
        
        private void AirspaceKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.H:
                    players[0].Direction = -2;
                    break;
                case Keys.L:
                    players[0].Direction = 2;
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
                case Keys.H:
                case Keys.L:
                    players[0].Direction = 0;
                    break;
            }
        }
    }
}