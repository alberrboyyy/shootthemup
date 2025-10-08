namespace Shootthemup
{
    public partial class Player
    {
        public static readonly int _MaxHealth = 3;      // Charge maximale de la batterie
        private int _health;                            // La charge actuelle de la batterie
        private string _name;                           // Un nom
        private int _x;                                 // Position en X depuis la gauche de l'espace aérien
        private int _y;                                 // Position en Y depuis le haut de l'espace aérien
        private int _direction = 1;


        // Constructeur
        public Player(int x, string name, int health)
        {
            _x = x;
            _y = AirSpace.HEIGHT - 100;
            _name = name;
            _health = health;
        }

        public int Health { get { return _health; } set { _health = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public int Direction { get { return _direction; } set { _direction = value; } }






        // Cette méthode calcule le nouvel état dans lequel le drone se trouve après
        // que 'interval' millisecondes se sont écoulées
        public void Update(int interval, List<Player> players)
        {
            foreach (Player player in players)
            {
                if (_direction != 0)
                {
                    player.X += _direction;
                }
                if (player.X >= AirSpace.WIDTH)
                {
                    player.X = 0;
                }
                else if (player.X <= 0)
                {
                    player.X = AirSpace.WIDTH;
                }
            }
        }
    }
}
