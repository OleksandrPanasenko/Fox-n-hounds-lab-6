using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fox_n_Hounds_checkers
{
    internal class Hound
    {
        internal int Row;
        internal int Column;
        internal bool Selected=false;
        public Field field;
        public Hound(Field field)
        {
            this.field = field;
        }
        public Hound(int row, int col, Field field) : this(field)
        {
            Row = row;
            Column = col;
        }
        public void MoveTo(int row, int col)
        {
            if (CanMoveTo(row,col))
            {
                this.Row = row;
                this.Column = col;
            }
        }
        public bool CanMoveTo(int row, int col)
        {
            return field.IsEmpty(row, col) && row - Row == 1 && Column - col == 1;
        }
    }
}
