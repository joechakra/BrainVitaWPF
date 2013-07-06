//--------------------------------------
// Empty.cs (c) 2006 by Charles Petzold
//--------------------------------------
using System.Windows;
using System.Windows.Media;
namespace JoeChakra.Games
{
    class Empty : System.Windows.FrameworkElement
    {
        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            //drawingContext.DrawRectangle(new SolidColorBrush(
        }
    }
}