using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoChiemThanh
{
    public class Piece
    {
        private int row;
        private int col;
        private string label;

        public Piece(int row, int col, string label)
        {
            this.row = row;
            this.col = col;
            this.label = label;
        }

        public int Row
        {
            get
            {
                return row;
            }

            set
            {
                row = value;
            }
        }

        public int Col
        {
            get
            {
                return col;
            }

            set
            {
                col = value;
            }
        }

        public string Label
        {
            get
            {
                return label;
            }

            set
            {
                label = value;
            }
        }
    }
}
