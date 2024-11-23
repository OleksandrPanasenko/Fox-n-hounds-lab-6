using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fox_n_Hounds_checkers
{
    internal class Field
    {
        public PictureBox canvas;
        public Graphics g;
        public void HoundsAITurn()
        {
            TreeMove Head = new TreeMove(Hounds, fox);
            Head.Evaluate();
            TreeMove bestTurn = Head.Children[0];
            foreach(TreeMove child in Head.Children)
            {
                if (child.Value < bestTurn.Value)
                {
                    bestTurn = child;
                }
            }
            Hounds = bestTurn.houndPositions;
            DrawField();
            FoxTurn = true;
        }
        public Field(Graphics g)
        {
            FoxTurn = true;
            this.g = g;
        }
        public Field(Graphics g, int size) : this(g)
        {
            this.Size = size;
            this.NumberHounds = size / 2;
        }
        public Field(Graphics g, int size, bool BlackFirst):this(g,size)
        {
            FirstCellBlack = BlackFirst;
            if (FirstCellBlack && Size % 2 == 1)
            {
                NumberHounds++;
            }
        }
        public void Init()
        {
            fox = new Fox(this);
            fox.Row = Size - 1;
            int location = 0;
            if (FirstCellBlack == (Size % 2 == 0)) location++;
            location += (NumberHounds / 2) * 2;
            fox.Column = location;
            if (!FirstCellBlack) location = 1;
            else location = 0;
            Hounds = new Hound[NumberHounds];
            for(int i = 0; i < NumberHounds; i++)
            {
                Hounds[i] = new Hound(0, i*2+location, this);
            }
            if(canvas is not null)
            {
                Xmin = 0;
                Ymin=0;
                if (canvas.Height > canvas.Width)
                {
                    Xmax = canvas.Width;
                    Ymax = canvas.Width;
                }
                else
                {
                    Xmax = canvas.Height;
                    Ymax = canvas.Height;
                }
            }
            CheckerRadius = (int)(0.4 * BoxWidth());
        }
        internal bool IsEmpty(int row, int col)
        {
            if (row < 0||col<0||row>=Size||col>=Size) return false;
            foreach(Hound hound in Hounds)
            {
                if (row == hound.Row && col == hound.Column) return false;
            }
            if(fox.Row==row&&fox.Column==col) return false;
            return true;
        }
        
        public bool SelectClicked(MouseEventArgs e)
        {
            Deselect();
            if (PointInField(e.X,e.Y))
            {
                Point centre;
                foreach (Hound hound in Hounds)
                {
                    centre = BoxCentre(hound.Row, hound.Column);
                    if (Distance(centre.X, e.X, centre.Y, e.Y) <= CheckerRadius) {
                        if (!FoxTurn) { 
                            hound.Selected = true;
                            return true;
                        }
                        else
                        {
                            throw new Exception("You tried select hound during fox's turn!");
                        }
                    };
                }
                centre = BoxCentre(fox.Row, fox.Column);
                if (Distance(centre.X, e.X, centre.Y, e.Y) <= CheckerRadius)
                {
                    if (FoxTurn)
                    {
                        fox.Selected = true;
                        return true;
                    }
                    else
                    {
                        throw new Exception("You tried to select fox turing hounds' turn!");
                    }
                };
                return false;
            }
            else return false;
        }
        public bool MoveToBox(MouseEventArgs e)
        {
            if (PointInField(e.X, e.Y))
            {
                Point cell=CellFromCoordinates(e.X, e.Y);
                if (FoxTurn)
                {
                    if (fox.Selected && fox.CanMoveTo(cell.Y, cell.X))
                    {
                        fox.Row=cell.Y; 
                        fox.Column=cell.X;
                        FoxTurn = false;
                        Deselect();
                        return true;
                    }
                    else
                    {
                        Deselect();
                    }
                }
                else
                {
                    foreach(Hound hound in Hounds)
                    {
                        if (hound.Selected && hound.CanMoveTo(cell.Y, cell.X))
                        {
                            hound.Row = cell.Y;
                            hound.Column=cell.X;
                            FoxTurn = true;
                            Deselect();
                            return true;
                        }
                    }
                    Deselect();
                }
                SelectClicked(e);
            }
            return false;
        }
        public void Deselect()
        {
            foreach (Hound hound in Hounds)
            {
                hound.Selected = false;
            }
            fox.Selected = false;
        }
        public Point CellFromCoordinates(int x, int y)
        {
            int newX=(int)((float)(x-Xmin)/BoxWidth());
            int newY = (int)((float)(y - Ymin) / BoxHeight());
            return new Point(newX, newY);
        }
        public bool IsSelected()
        {
            foreach(Hound hound in Hounds)
            {
                if (hound.Selected) return true;
            }
            if(fox.Selected) return true;
            return false;
        }
        float Distance(int x1, int x2, int y1, int y2)
        {
            return (float)Math.Sqrt((double)(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)));
        }
        public bool PointInField(int  x, int y)
        {
            return x >= Xmin && x <= Xmax && y >= Ymin && y <= Ymax;
        }
        public bool FoxWon()
        {
            return fox.Row == 0;
        }
        public bool FoxLost()
        {
            if(fox.Contained())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DrawField()
        {
            g.Clear(Color.White);
            Pen BorderPen = new Pen(Color.Black, 2);
            for(int i=0; i < Size; i++)
            {
                for(int j=0; j < Size; j++)
                {
                    Point centre = BoxCentre(i, j);
                    Rectangle cell = new Rectangle(centre.X - BoxWidth() / 2, centre.Y - BoxWidth() / 2, BoxWidth(), BoxWidth());
                    if((i%2==j%2)==FirstCellBlack) g.FillRectangle(new SolidBrush(BlackColor), cell);
                    else g.FillRectangle(new SolidBrush(WhiteColor), cell);
                }
            }
            Color CheckerColor;
            foreach(Hound hound in Hounds)
            {
                Point centre = BoxCentre(hound.Row, hound.Column);
                Rectangle checker = new Rectangle(centre.X - CheckerRadius, centre.Y - CheckerRadius, CheckerRadius*2, CheckerRadius*2);

                if (hound.Selected) CheckerColor = HoundSelectedColor;
                else CheckerColor = HoundColor;
                g.FillEllipse(new SolidBrush(CheckerColor), checker);
            }
            Point centre_fox = BoxCentre(fox.Row, fox.Column);
            Rectangle checker_fox = new Rectangle(centre_fox.X - CheckerRadius, centre_fox.Y - CheckerRadius, CheckerRadius*2, CheckerRadius*2);
            if (fox.Selected) CheckerColor = FoxSelectedColor;
            else CheckerColor = FoxColor;
            g.FillEllipse(new SolidBrush(CheckerColor), checker_fox);

            DrawOptions();
        }
        public void DrawOptions()
        {
            canvas.Invalidate();
            if (fox.Selected)
            {
                if (IsEmpty(fox.Row + 1, fox.Column + 1))
                {
                    OptionDot(fox.Row + 1, fox.Column + 1, HintColor);
                }
                if(IsEmpty(fox.Row+1, fox.Column - 1))
                {
                    OptionDot(fox.Row + 1, fox.Column - 1, HintColor);
                }
                if (IsEmpty(fox.Row - 1, fox.Column + 1))
                {
                    OptionDot(fox.Row - 1, fox.Column + 1, HintColor);
                }
                if(IsEmpty(fox.Row-1, fox.Column - 1))
                {
                    OptionDot(fox.Row - 1, fox.Column - 1, HintColor);
                }
            }
            else
            {
                foreach(Hound hound in Hounds)
                {
                    if (hound.Selected)
                    {
                        if (IsEmpty(hound.Row + 1, hound.Column - 1))
                        {
                            OptionDot(hound.Row + 1, hound.Column - 1, HintColor);
                        }
                        if (IsEmpty(hound.Row + 1, hound.Column + 1))
                        {
                            OptionDot(hound.Row + 1, hound.Column + 1, HintColor);
                        }
                    }
                }
            }
        }

        public void OptionDot(int row, int col, Color color)
        {
            Point centre=BoxCentre(row, col);
            Rectangle hint = new Rectangle(centre.X - HintRadius, centre.Y - HintRadius, HintRadius*2, HintRadius*2);
            g.FillEllipse(new SolidBrush(color), hint);
        }

        int Size=8;
        bool FirstCellBlack = false;
        int NumberHounds = 4;
        int Xmax=160;
        int Ymax=160;
        int Xmin=0;
        int Ymin=0;
        int FoxStart;
        public Color WhiteColor = Color.White;
        public Color BlackColor = Color.Black;
        public Color HintColor = Color.Blue;
        public Color HoundColor = Color.LightGray;
        public Color HoundSelectedColor = Color.DarkGray;
        public Color FoxColor = Color.Red;
        public Color FoxSelectedColor = Color.DarkRed;
        public int CheckerRadius=8;
        public int HintRadius=4;
        public Fox fox;
        public Hound[] Hounds;
        public bool FoxTurn;
        public Point BoxCentre(int row, int col)
        {
            int PointX=(BoxWidth()/2)+BoxWidth()*col+Xmin;
            int PointY = (BoxHeight() / 2) + BoxHeight() * row + Ymin;
            return new Point(PointX, PointY);

        }
        public int BoxWidth()
        {
            return (Xmax-Xmin)/Size;
        }
        public int BoxHeight()
        {
            return (Ymax-Ymin)/Size;
        }
    }
}
