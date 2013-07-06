using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JoeChakra.Games
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : Window
    {
        public History(List<BrainVita.Move> moves)
        {
           InitializeComponent();
           GridView gv = new GridView();
           lv.View = gv;
           lv.AlternationCount = 2;
           GridViewColumn col = new GridViewColumn();
           col.Header = "Start X";
           col.DisplayMemberBinding = new Binding("xstart");
           gv.Columns.Add(col);

           col = new GridViewColumn();
           col.Header = "Start Y";
           col.DisplayMemberBinding = new Binding("ystart");
           gv.Columns.Add(col);

           col = new GridViewColumn();
           col.Header = "End X";
           col.DisplayMemberBinding = new Binding("xend");
           gv.Columns.Add(col);

           col = new GridViewColumn();
           col.Header = "End Y";
           col.DisplayMemberBinding = new Binding("yend");
           gv.Columns.Add(col);
            moves.Reverse();
            foreach (var x in moves)
               lv.Items.Add(x);
        }
    }
}
