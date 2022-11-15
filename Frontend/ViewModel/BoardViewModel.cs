using Frontend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModel
{
    public class BoardViewModel
    {
        private BoardModel bm;
        public BoardModel Bm { get => bm; }

        public BoardViewModel(BoardModel bm)
        {
            this.bm = bm;
        }
    }
}
