using System.Drawing;

namespace Shootthemup //Shootthemup.Model.Obstacles.cs
{
    public class Obstacle
    {
        private double _x;                      // Position en X depuis la gauche de l'espace aérien
        private double _y;                      // Position en Y depuis le haut de l'espace aérien
        private int _health;                    // Points de vie de l'obstacle
        private int _size;                      // Taille de l'obstacle (largeur et hauteur)


        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public int Health { get { return _health; } set { _health = value; } }
        public int Size { get { return _size; } }
        public double CenterX { get { return _x + (_size / 2.0); } }
        public double CenterY { get { return _y + (_size / 2.0); } }


        // Constructeur
        public Obstacle(double x, double y, int health, int size)
        {
            _x = x;
            _y = y;
            _health = health;
            _size = size;
        }

        /// <summary>
        /// Render un obstacle en forme de cercle marron.
        /// </summary>
        /// <param name="drawingSpace"></param>
        public void Render(BufferedGraphics drawingSpace)
        {
            // Render du noyau de l'obstacle (sans les potentiels boucliers)
            float centerX = (float)_x + (_size / 2);
            float centerY = (float)_y + (_size / 2);

            drawingSpace.Graphics.FillEllipse(Brushes.SaddleBrown,
                centerX - _size,
                centerY - _size,
                _size * 2,
                _size * 2);
        }
    }
}