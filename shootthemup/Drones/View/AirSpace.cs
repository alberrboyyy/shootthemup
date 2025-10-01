namespace Drones
{
    // La classe AirSpace représente le territoire au dessus duquel les drones peuvent voler
    // Il s'agit d'un formulaire (une fenêtre) qui montre une vue 2D depuis en dessus
    // Il n'y a donc pas de notion d'altitude qui intervient

    public partial class AirSpace : Form
    {
        public static readonly int WIDTH = 1200;        // Dimensions of the airspace
        public static readonly int HEIGHT = 600;
        private int _direction = 0;

        public int Direction { get { return _direction; } set { _direction = value; } }

        // La flotte est l'ensemble des drones qui évoluent dans notre espace aérien
        private List<Player> players;
        private List<Enemy> enemies;

        BufferedGraphicsContext currentContext;
        BufferedGraphics airspace;

        // Initialisation de l'espace aérien avec un certain nombre de drones
        public AirSpace(List<Player> players, List<Enemy> enemies)
        {
            InitializeComponent();
            // Gets a reference to the current BufferedGraphicsContext
            currentContext = BufferedGraphicsManager.Current;

            KeyPreview = true;
            KeyDown += AirspaceKeyDown;
            KeyUp += AirSpaceKeyUp;

            // Creates a BufferedGraphics instance associated with this form, and with
            // dimensions the same size as the drawing surface of the form.
            airspace = currentContext.Allocate(this.CreateGraphics(), this.DisplayRectangle);

            this.players = players;
            this.enemies = enemies;
        }


        // Affichage de la situation actuelle
        private void Render()
        {
            airspace.Graphics.Clear(Color.White);

            // draw drones
            foreach (Player player in players)
            {
                player.Render(airspace);
            }
            foreach (Enemy enemy in enemies)
            {
                enemy.Render(airspace);
            }


            airspace.Render();
        }

        // Calcul du nouvel état après que 'interval' millisecondes se sont écoulées
        private void Update(int interval)
        {
            foreach (Player player in players)
            {
                player.Update(interval);
            }
            foreach (Enemy enemies in enemies)
            {
                enemies.Update(interval);
            }
        }

        // Méthode appelée à chaque frame
        private void NewFrame(object sender, EventArgs e)
        {
            if (_direction != 0)
            {
                players[0].X += _direction;
            }

            Update(ticker.Interval);
            Render();
        }
        private void AirspaceKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.H:
                    _direction = -2;
                    break;
                case Keys.L:
                    _direction = 2;
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
                    _direction = 0;
                    break;
            }
        }
    }
}