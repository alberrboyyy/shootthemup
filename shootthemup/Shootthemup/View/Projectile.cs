using Shootthemup.Properties;

namespace Shootthemup //View.Projectiles.cs
{
    public partial class Projectile
    {
        // De manière graphique : petit carré bleu
        public void Render(BufferedGraphics drawingSpace)
        {
            const int size = 6;
            drawingSpace.Graphics.FillRectangle(Brushes.Blue, X, Y, size, size);
        }
    }
}

