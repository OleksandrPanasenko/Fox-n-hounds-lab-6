using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fox_n_Hounds_checkers
{
    internal class Fox
    {
        internal int Row;
        internal int Column;
        internal bool Selected;
        public Field field;
        public Fox(Field field)
        {
            this.field = field;
        }
        public Fox(int row, int col, Field field):this(field)
        {
            Row = row;
            Column = col;
        }
        public bool MoveTo(int row, int col)
        {
            if(CanMoveTo(row, col))
            {
                this.Row = row;
                this.Column = col;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CanMoveTo(int row, int col)
        {
            return field.IsEmpty(row, col) && Math.Abs(Row - row) == 1 && Math.Abs(Column - col) == 1;
        }
    }
}
