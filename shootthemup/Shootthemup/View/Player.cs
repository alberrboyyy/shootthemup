using Shootthemup.Properties;

namespace Shootthemup
{
    // Cette partie de la classe Drone définit comment on peut voir un drone

    public partial class Player
    {
        // De manière graphique
        public void Render(BufferedGraphics drawingSpace)
        {
            drawingSpace.Graphics.DrawImage(Resources.player, X, Y);
            //drawingSpace.Graphics.DrawString($"{this}", TextHelpers.drawFont, TextHelpers.writingBrush, X + 5, Y - 25);
        }

        // De manière textuelle
        public override string ToString()
        {
            return $"{Name} ({((int)((double)Health)).ToString()})";
        }

    }
}
