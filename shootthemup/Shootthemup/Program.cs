namespace Shootthemup
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Création de la flotte de drones
            List<Player> players = new List<Player>();
            players.Add(new Player(1, "Joe", 1));


            List<Enemy> enemies = new List<Enemy>();
            enemies.Add(new Enemy(100, 200, "Joe", 1));


            // Démarrage
            Application.Run(new AirSpace(players, enemies));
        }
    }
}