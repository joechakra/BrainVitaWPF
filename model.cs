//-------------------------------------------------
// BrainVita.cs (c) 2011 by Joe Mariadassou
//-------------------------------------------------


using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;


namespace JoeChakra.BrainVita
{

    enum Direction
    {
        East,
        South,
        West,
        North
    }

    public class SetPosEventArgs : EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool Selected { get; set; }
    };

    struct Move
    {
        public int X;
        public int Y;
        public Direction D;
    };
        public delegate void onSet(object sender, SetPosEventArgs args);

    class BVModel
    {
        public event onSet OnSet;


        public static bool IsValid(int i, int j)
        {
            return (i >= 2 && i <= 4 && j >= 0 && j < 7) || (j >= 2 && j <= 4 && i >= 0 && i < 7);
        }
        bool[,] board = new bool[7, 7];
        List<Move> moves = new List<Move>();
        Direction[] dirs = new Direction[] {
                Direction.East,
                Direction.South,
                Direction.West,
                Direction.North
            };

        public BVModel()
        {
            reset();
        }
        Tuple<int, int, int> incr(Tuple<int, int, int> m)
        {
            int i = m.Item1;
            int j = m.Item2;
            int d = m.Item3;
            if (d < 3)
                return new Tuple<int, int, int>(i, j, d + 1);
            else if (i < 6)
                return new Tuple<int, int, int>((i + 1), j, 0);
            else
                return new Tuple<int, int, int>(0, (j + 1), 0);
        }
        public void reset()
        {
            for (int i = 0; i <= 6; ++i)
                for (int j = 0; j <= 6; ++j)
                    board[i, j] = (IsValid(i, j)) && !(i == 3 && j == 3);
        }


        Direction GetDir(int k, int l)
        {
            if (k == 0)
                if (l == 2)
                    return Direction.South;
                else if (l == -2)
                    return Direction.North;
                else
                    throw new ArgumentOutOfRangeException("l", l, "must be -2 0r +2");
            else if (l == 0)
                if (k == 2)
                    return Direction.East;
                else if (k == -2)
                    return Direction.West;
                else
                    throw new ArgumentOutOfRangeException("k", k, "must be -2 0r +2");
            else
                throw new ArgumentOutOfRangeException("k , l");

        }
        private void Set(int x, int y, bool present)
        {
            if (OnSet != null)
            {
                SetPosEventArgs args = new SetPosEventArgs { X = x, Y = y, Selected = present };
                OnSet(this, args);
            }
        }
        bool movableX(int i, int x, int y)
        {
            return board[x, y] && board[x + i, y] && !board[x + 2 * i, y];
        }

        bool movableY(int j, int x, int y)
        {
            return board[x, y] && board[x, y + j] && !board[x, y + 2 * j];
        }
        public bool moveXY(int xd, int yd, int xs, int ys)
        {
            int k = xd - xs;
            int l = yd - ys;

            if ((k == 0 && (l == 2 || l == -2) && (movableY((l / 2), xs, ys))) ||
                (l == 0 && (k == 2 || k == -2) && (movableX((k / 2), xs, ys))))
            {
                board[xs, ys] = false;
                Set(xs, ys, false);
                board[xs + k / 2, ys + l / 2] = false;
                Set(xs + k / 2, ys + l / 2, false);
                board[xd, yd] = true;
                Set(xd, yd, true);
                Move move = new Move();
                move.X = xs;
                move.Y = ys;
                move.D = GetDir(k, l);
                moves.Add(move);
                return true;
            }
            else
                return false;
        }
        bool isMovable(Direction d, int x, int y)
        {
            if (IsValid(x, y))
            {
                switch (d)
                {
                    case Direction.East: return (IsValid((x + 2), y)) && (movableX(1, x, y));
                    case Direction.West: return (IsValid((x - 2), y)) && (movableX(-1, x, y));
                    case Direction.South: return (IsValid(x, (y + 2)) && (movableY(1, x, y)));
                    case Direction.North: return (IsValid(x, (y - 2)) && (movableY(-1, x, y)));
                    default: throw new ArgumentException("Direction");
                }
            }
            else
                return false;
        }
        public bool reverseXY(int i, int j, int x, int y)
        {
            board[x, y] = true;
            Set(x, y, true);
            board[x - i, y - j] = true;
            Set(x - i, y - j, true);
            board[x - 2 * i, y - 2 * j] = false;
            Set(x - 2 * i, y - 2 * j, false);
            return true;
        }
        public Tuple<bool, int, int, int> findMove(int i, int j, int d)
        {
            if (j < 7)
            {
                bool found = isMovable((dirs[d]), i, j);
                if (found)
                {
                    return new Tuple<bool, int, int, int>(true, i,j,d);
                }
                else
                {
                    Tuple<int, int, int> item = incr(new Tuple<int, int, int>(i, j, d));
                    return findMove(item.Item1, item.Item2, item.Item3);
                }
            }
            else
                return new Tuple<bool, int, int, int>(false, i,j,d);
        }

    }
}