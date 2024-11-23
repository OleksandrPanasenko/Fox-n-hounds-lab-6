using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fox_n_Hounds_checkers
{
    internal class TreeMove
    {
        internal static int MaxDepth=8;
        internal int Depth;
        internal Hound[] houndPositions;
        Fox foxPosition;
        public List<TreeMove> Children;
        internal int Value;
        public TreeMove(Hound[]houndSource, Fox foxSource) { 
            Depth = 0;
            foxPosition = foxSource.Copy();
            houndPositions = new Hound[houndSource.Length];
            for(int i = 0; i < houndSource.Length; i++)
            {
                houndPositions[i] = houndSource[i].Copy();
            }
        }
        public void GenerateChildren()
        {
            TreeMove child = null;
            Children = new List<TreeMove>();
            if (FoxTurn)
            {
                if (foxPosition.CanMoveTo(foxPosition.Row + 1, foxPosition.Column + 1))
                {
                    foxPosition.Row++;
                    foxPosition.Column++;
                    Children.Add(new TreeMove(houndPositions, foxPosition));
                    foxPosition.Row--;
                    foxPosition.Column--;
                }
                if (foxPosition.CanMoveTo(foxPosition.Row + 1, foxPosition.Column - 1))
                {
                    foxPosition.Row++;
                    foxPosition.Column--;
                    Children.Add(new TreeMove(houndPositions, foxPosition));
                    foxPosition.Row--;
                    foxPosition.Column++;
                }
                if (foxPosition.CanMoveTo(foxPosition.Row - 1, foxPosition.Column + 1))
                {
                    foxPosition.Row--;
                    foxPosition.Column++;
                    Children.Add(new TreeMove(houndPositions, foxPosition));
                    foxPosition.Row++;
                    foxPosition.Column--;
                }
                if (foxPosition.CanMoveTo(foxPosition.Row - 1, foxPosition.Column - 1))
                {
                    foxPosition.Row--;
                    foxPosition.Column--;
                    Children.Add(new TreeMove(houndPositions, foxPosition));
                    foxPosition.Row++;
                    foxPosition.Column++;
                }
            }
            else
            {
                foreach(Hound hound in houndPositions)
                {
                    if (hound.CanMoveTo(hound.Row + 1, hound.Column - 1))
                    {
                        hound.Row++;
                        hound.Column--;
                        Children.Add(new TreeMove(houndPositions, foxPosition));
                        hound.Row--;
                        hound.Column++;
                    }
                    if (hound.CanMoveTo(hound.Row+1, hound.Column + 1))
                    {
                        hound.Row++;
                        hound.Column++;
                        Children.Add(new TreeMove(houndPositions, foxPosition));
                        hound.Row--;
                        hound.Column--;
                    }
                    
                }
            }
        }
        public void Evaluate()
        {
            foxPosition.field.fox = foxPosition;
            foxPosition.field.Hounds = houndPositions;
            int HoundMinRow = houndPositions[0].Row;
            int HoundMaxRow = houndPositions[0].Row;
            foreach (Hound hound in houndPositions)
            {
                if (hound.Row > HoundMaxRow) HoundMaxRow = hound.Row;
                if (hound.Row < HoundMinRow) HoundMinRow = hound.Row;
            }
            int Curvalue = HoundMaxRow - HoundMinRow;
            if (foxPosition.Row == 0) Value = int.MaxValue; //Here changed
            else if (foxPosition.Contained()) Value = -1;
            else if (Depth == MaxDepth)
            {
                Value = Curvalue;
            }
            else{
                GenerateChildren();
                int MaxValue=-2;
                int MinValue=int.MaxValue;
                foreach(TreeMove move in Children)
                {
                    move.Depth = Depth + 1;
                    move.Evaluate();
                    if (move.Value == -2) throw new Exception("BadTree");
                    if(move.Value > MaxValue) MaxValue = move.Value;
                    if(move.Value<MinValue) MinValue = move.Value;
                }
                //if (!FoxTurn && MinValue == int.MaxValue) throw new Exception("Conscious suicide");
                if (FoxTurn)
                {
                    if (MaxValue > Curvalue) Value = MaxValue;
                    else Value = Curvalue;
                }
                else
                {
                    if(MinValue<Curvalue) Value= MinValue;
                    else Value = Curvalue;
                }
                
            }
            if (Depth == 0)
            {
                foxPosition.field.fox = foxPosition;
                foxPosition.field.Hounds = houndPositions;
            }
        }
        bool FoxTurn
        {
            get
            {
                if(Depth%2 == 0) return false;
                return true;
            }
        }
    }
}
