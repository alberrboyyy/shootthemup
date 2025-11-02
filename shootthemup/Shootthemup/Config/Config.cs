using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shootthemup //Shootthemup.Config.cs
{
    /// <summary>
    /// Type de projectile (ennemi ou joueur).
    /// </summary>
    public enum ProjectileType
    {
        Enemy,
        Player
    }

    /// <summary>
    /// Met a disposition les configurations globales du jeu.
    /// </summary>
    internal class Config
    {
        // Largeur et hauteur de la fenêtre de jeu
        public static readonly int WIDTH = 1200;
        public static readonly int HEIGHT = 600;

        // Seed pour la génération aléatoire
        public static int GameSeed = 12344;

        // Générateur de nombres aléatoires
        public static Random alea = new Random(GameSeed);

    }
}
