namespace Drones
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
            List<Player> crew= new List<Player>();
            crew.Add(new Player(100, 100, "Joe",1));

            // Démarrage
            Application.Run(new AirSpace(crew));
        }
    }
}