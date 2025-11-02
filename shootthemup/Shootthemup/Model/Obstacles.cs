using System.Drawing;

namespace Shootthemup //Shootthemup.Model.Obstacles.cs
{
    public class Obstacle
    {
        private double _x;
        private double _y;
        private int _health;
        private int _size;


        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public int Health { get { return _health; } set { _health = value; } }
        public int Size { get { return _size; } }

        public double CenterX
        {
            get { return _x + (_size / 2.0); }
        }

        public double CenterY
        {
            get { return _y + (_size / 2.0); }
        }


        // Constructeur
        public Obstacle(double x, double y, int health, int size)
        {
            _x = x;
            _y = y;
            _health = health;
            _size = size;
        }

        public void Render(BufferedGraphics drawingSpace)
        {
            float centerX = (float)_x + (_size / 2);
            float centerY = (float)_y + (_size / 2);

            drawingSpace.Graphics.FillEllipse(Brushes.SaddleBrown,
                centerX - _size,
                centerY - _size,
                _size * 2,
                _size * 2);

            //if (_health > 1)
            //{
            //    int numShields = _health - 1;
            //    float shieldRadius = 5;
            //    float angleIncrement = (float)(2 * Math.PI / numShields);

            //    for (int i = 0; i < numShields; i++)
            //    {
            //        float angle = i * angleIncrement;
            //        float shieldCenterX = centerX + (float)(orbitRadius * Math.Cos(angle));
            //        float shieldCenterY = centerY + (float)(orbitRadius * Math.Sin(angle));

            //        drawingSpace.Graphics.FillEllipse(Brushes.Gray,
            //            shieldCenterX - shieldRadius,
            //            shieldCenterY - shieldRadius,
            //            shieldRadius * 2,
            //            shieldRadius * 2);
            //    }
            //}
        }
    }
}