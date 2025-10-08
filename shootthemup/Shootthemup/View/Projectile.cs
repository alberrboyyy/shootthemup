using Shootthemup.Properties;

namespace Shootthemup //View.Projectiles.cs
{
    public partial class Projectile
    {
        // De manière graphique
        public void Render(BufferedGraphics drawingSpace)
        {
            drawingSpace.Graphics.DrawImage(Resources.enemy, X, Y);

        }
    }
}

