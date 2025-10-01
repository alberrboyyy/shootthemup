using Drones.Properties;

namespace Drones
{
    public partial class Enemy
    {
        // De manière graphique
        public void Render(BufferedGraphics drawingSpace)
        {
            //drawingSpace.Graphics.DrawString($"{this}", TextHelpers.drawFont, TextHelpers.writingBrush, X + 5, Y - 25);

            drawingSpace.Graphics.DrawRectangle(new Pen(Color.Black, 3), new Rectangle(X, Y, 200, 200));

            /* WOP
            double xMin = -10.0;
            double xMax = 10.0;

            double yMin = -2.0;
            double yMax = 2.0;

            using (Pen pen = new Pen(Color.Blue, 2))
            {
                int w = 200;
                int h = 100;

                PointF previousPoint = PointF.Empty;

                for (int i = 0; i < w; i++)
                {
                    double x = xMin + (xMax - xMin) * ((double)i / w);
                    double y = Math.Sin(x);
                    double normalizedY = (y - yMin) / (yMax - yMin);
                    int yPixel = h - (int)(normalizedY * h);

                    PointF currentPoint = new PointF(i, yPixel);

                    if (i > 0)
                    {
                        drawingSpace.Graphics.DrawLine(pen, previousPoint, currentPoint);
                    }
                    previousPoint = currentPoint;
                }
            }*/
        }

        // De manière textuelle
        public override string ToString()
        {
            return $"{Name} ({((int)((double)Health)).ToString()})";
        }
    }
}
