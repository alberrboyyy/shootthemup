namespace Shootthemup //View.AirSpace.cs
{
    public partial class AirSpace : Form
    {
        public static readonly int WIDTH = 1200;
        public static readonly int HEIGHT = 600;

        private Player _player = new Player(0, 3);


        

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
            foreach (Enemy enemy in Enemy.Enemies)
            {
                enemy.Render(airspace);
            }

            // render projectiles (small blue squares)
            foreach (Projectile projectile in Projectile.Projectiles.ToList())
            {
                projectile.Render(airspace);
            }

            airspace.Render();
        }

        private void Update(int interval)
        {
            _player.Update(interval);

            foreach (Enemy enemy in Enemy.Enemies)
            {
                enemy.Update(interval, Enemy.Enemies);
            }

            foreach (Projectile projectile in Projectile.Projectiles.ToList())
            {
                projectile.Update(interval, Enemy.Enemies);
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