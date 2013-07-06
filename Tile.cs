//-------------------------------------
// Tile.cs (c) 2006 by Charles Petzold
//-------------------------------------
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace JoeChakra.Games
{
    public abstract class Slot : Canvas
    {
        const int SIZE = 64;    // 2/3 inch
        const int BORD = 6;     // 1/16 inch
        protected abstract Brush GetBrush();
        public Slot()
        {
            Width = SIZE;
            Height = SIZE;
               
            // Upper-left shadowed border.
            Polygon poly = new Polygon();
            poly.Points = new PointCollection(new Point[] 
                { 
                    new Point(0, 0), new Point(SIZE, 0), 
                    new Point(SIZE-BORD, BORD),
                    new Point(BORD, BORD), 
                    new Point(BORD, SIZE-BORD),
                    new Point(0, SIZE)
                });
            poly.Fill = SystemColors.ControlLightLightBrush;
            Children.Add(poly);

            // Lower-right shadowed border.
            poly = new Polygon();
            poly.Points = new PointCollection(new Point[] 
                { 
                    new Point(SIZE, SIZE), new Point(SIZE, 0), 
                    new Point(SIZE-BORD, BORD),
                    new Point(SIZE-BORD, SIZE-BORD), 
                    new Point(BORD, SIZE-BORD), 
                    new Point(0, SIZE)
                });
            poly.Fill = SystemColors.ControlDarkBrush;
            Children.Add(poly);

            // Host for centered text.
            Border bord = new Border();
            bord.Width = SIZE - 2 * BORD;
            bord.Height = SIZE - 2 * BORD;
            bord.Background = new SolidColorBrush(Colors.Gray);
            Children.Add(bord);
            SetLeft(bord, BORD);
            SetTop(bord, BORD);
            Ellipse el = new Ellipse();
            el.Fill = this.GetBrush();
            bord.Child = el;
        }
   
    }
    //public class Peg : Slot
    //{
    //    public RadialGradientBrush br;
    //    public bool Selected { get; set; }
    //    protected override Brush GetBrush()
    //    {
    //        br = new RadialGradientBrush();
    //        br.GradientStops.Add(new GradientStop(Colors.White, 0));
    //        br.GradientStops.Add(new GradientStop(Colors.Black, 1.0));
    //        br.GradientOrigin = new Point(0.25, 0.25);
    //        return br;
    //    }
    //    public Peg()
    //    {
    //        Selected = false;  
    //    }
    //}
    public class Hole : Slot
    {
        protected override Brush GetBrush()
        {
            RadialGradientBrush br = new RadialGradientBrush();
            br.GradientStops.Add(new GradientStop(Colors.Black, 0));
            br.GradientStops.Add(new GradientStop(Colors.Gray, 1.0));

            return br;
        }
    }
}
