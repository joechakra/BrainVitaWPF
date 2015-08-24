using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Collections.Generic;
namespace theSundayProgrammer.Games.BrainVita
{
    public class BrainVita : Window
    {
        public enum Direction
        {
            East,
            South,
            West,
            North
        };


        [Serializable]
        public class Move
        {
            public int xstart { get; set; }
            public int ystart { get; set; }
            public int xend { get; set; }
            public int yend { get; set; }
        };

        const int NumberRows = 7;
        const int NumberCols = 7;

        Stack<Move> moves = new Stack<Move>();
        UniformGrid unigrid;
        int xSelected = -1, ySelected = -1;

        BVModel model = new BVModel();
        public System.Collections.Generic.List<Move> Moves
        {
            get
            {
                System.Collections.Generic.List<Move> lmoves = new List<Move>();
                foreach (var x in moves)
                {
                    lmoves.Add(x);
                }
                return lmoves;
            }
        }
        static private bool isValid(int i, int j)
        {
            return BVModel.IsValid(i, j);
        }
        [STAThread]
        public static void Main()
        {
            Application app = new Application();
            app.Run(new BrainVita());
        }
        public BrainVita()
        {
            Title = "Brain Vita";
            SizeToContent = SizeToContent.WidthAndHeight;
            ResizeMode = ResizeMode.NoResize;
            Background = SystemColors.ControlBrush;
            WindowStyle = WindowStyle.None;
            CommandBinding binding;
            binding = new CommandBinding(ApplicationCommands.New);
            binding.Executed += new ExecutedRoutedEventHandler(restart);
            binding.CanExecute += new CanExecuteRoutedEventHandler(restart_CanExecute);
            this.CommandBindings.Add(binding);
            binding = new CommandBinding(ApplicationCommands.Undo);
            binding.CanExecute += new CanExecuteRoutedEventHandler(restart_CanExecute);
            binding.Executed += new ExecutedRoutedEventHandler(undo_Click);
            CommandBindings.Add(binding);
            // Create Border for aesthetic purposes.
            Border bord = new Border();
            bord.BorderBrush = SystemColors.ControlDarkDarkBrush;
            bord.BorderThickness = new Thickness(1);
            //stack.Children.Add(bord);
            Content = bord;
            // Create Unigrid as Child of Border.
            unigrid = new UniformGrid();
            unigrid.Rows = NumberRows;
            unigrid.Columns = NumberCols;
            bord.Child = unigrid;

            // Create Tile objects to fill all but one cell.
            int k = 0;
            for (int i = 0; i < NumberRows; i++)
                for (int j = 0; j < NumberRows; j++)
                {
                    if (j == 6 && i == 0)
                    {
                        Button btn = MakeButton(@"305_Close_32x32_72.png");
                        ToolTip tip = new System.Windows.Controls.ToolTip();
                        tip.Content = "Close";
                        btn.ToolTip = tip;
                        unigrid.Children.Add(btn);
                        btn.Click += btn_Click;
                    }
                    else if (j == 0 && i == 0)
                    {
                        Button btn = MakeButton(@"112_ArrowReturnLeft_Blue_32x32_72.png");
                        ToolTip tip = new System.Windows.Controls.ToolTip();
                        tip.Content = "Undo previous move";
                        btn.ToolTip = tip;
                        btn.Command = ApplicationCommands.Undo;
                        unigrid.Children.Add(btn);
                    }
                    else if (j == 1 && i == 0)
                    {
                        Button btn = new Button();
                        btn.Content = "Restart";
                        ToolTip tip = new System.Windows.Controls.ToolTip();
                        tip.Content = "Forget all moves and restart";
                        btn.ToolTip = tip;
                        btn.Command = ApplicationCommands.New;
                        unigrid.Children.Add(btn);
                    }
                   else if (!isValid(i, j))
                    {
                        unigrid.Children.Add(new Empty());
                    }
                    else
                    {
                        UIElement tile = MakeSlot(!(i == j && i == 3));
                        unigrid.Children.Add(tile);
                        k++;
                    }
                }
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            model.OnSet += mcb_OnMove;
        }


        void restart_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.moves.Count > 0;
        }


        void restart(object sender, RoutedEventArgs e)
        {
            this.moves.Clear();
            for (int i = 0; i < NumberRows; i++)
                for (int j = 0; j < NumberRows; j++)
                    if (isValid(i, j))
                    {
                        SetTile(i, j, !(i == j && i == 3));
                    }
            model.reset();   
             
        }

        private UIElement MakeSlot(bool isPeg)
        {
            UIElement tile;
            if (!isPeg)
            {
                tile = new BVHole();
                tile.MouseLeftButtonDown += OnHoleClick;
            }
            else
            {
                tile = new BVPeg();
                tile.MouseLeftButtonDown += OnPegClick;
            }
            return tile;
        }

        private static Button MakeButton(string loc)
        {
            Image img = new Image();
            img.Source = new BitmapImage(new Uri("pack://application:,,/" + loc, UriKind.RelativeOrAbsolute));
            img.Stretch = Stretch.None;
            Button btn = new Button();
            btn.Content = img;

            return btn;
        }

        void undo_Click(object sender, RoutedEventArgs e)
        {
            Move move = moves.Pop();
            model.reverseXY((move.xstart - move.xend) / 2, (move.ystart - move.yend) / 2, move.xstart, move.ystart);
        }

        void mcb_OnMove(object sender, SetPosEventArgs move)
        {
           SetTile(move.X, move.Y, move.Selected);    
        }
        void btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        void OnHoleClick(object sender, MouseButtonEventArgs args)
        {
             if (xSelected == -1)
            {
                Console.Beep();
                return;
            }
            UIElement tile = sender as UIElement;
            int iMove = unigrid.Children.IndexOf(tile);
            int xhole = iMove % NumberCols;
            int yhole = iMove / NumberCols;
            if (model.moveXY(xhole,yhole, xSelected, ySelected))
            {
                Move move = new Move();
                move.xstart = xSelected;
                move.ystart = ySelected;
                move.xend = xhole;
                move.yend = yhole;
                moves.Push(move);
                
                System.Tuple<bool, int, int, int> kk = model.findMove(0, 0, 0);
                if (!kk.Item1 && moves.Count <31)
                {
                    MessageBox.Show("You have no more moves");
                }
                else if (moves.Count == 31)
                {
                    MessageBox.Show("You completed the game","Congratualtions");
                }
                xSelected = -1;
                ySelected = -1;
            }
            else
            {
                Console.Beep();
            }

        }

        void OnPegClick(object sender, MouseButtonEventArgs args)
        {
            if (xSelected >= 0) //if inited
            {
                System.Collections.IEnumerator tiles = unigrid.Children.GetEnumerator();

                for (int i = 0; i < ySelected * NumberCols + xSelected+1; ++i)
                    tiles.MoveNext();
                BVPeg utile = tiles.Current as BVPeg;
                
                utile.MouseLeftButtonDown += OnPegClick;
                VisualStateManager.GoToState(utile, "Normal", false);
            }
            BVPeg tile = sender as BVPeg;
            int iMove = unigrid.Children.IndexOf(tile);
            tile.MouseLeftButtonDown += OnPegClick;
            xSelected = iMove % NumberCols;
            ySelected = iMove / NumberCols;
            VisualStateManager.GoToState(tile, "Chosen", false);
            
  
        }

        private void Anim(BVPeg tile, Point dest, String somename)
        {
            PointAnimation animation = new PointAnimation();
            animation.To = dest;
            String tarName = somename;

            NameScope nams = new NameScope();
            NameScope.SetNameScope(this, nams);
            this.RegisterName(tarName, tile.br);

            Storyboard.SetTargetName(animation, tarName);
            Storyboard.SetTargetProperty(animation, new PropertyPath(RadialGradientBrush.GradientOriginProperty));
            Storyboard stboard = new Storyboard();
            stboard.Children.Add(animation);
            stboard.Begin(this);

        }


        void SetTile(int i, int j, bool isPeg)
        {
            int iTile = NumberCols * j + i;
            unigrid.Children.RemoveAt(iTile);
            UIElement tile = MakeSlot(isPeg);
            unigrid.Children.Insert(iTile, tile);
        }
    }

}
