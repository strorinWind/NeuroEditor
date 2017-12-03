using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroEditor
{
    public class ElementVar
    {
        public bool[] Picture { get; set; }
        public char Output { get; set; }

        public ElementVar()
        {
            Picture = new bool[64];
        }

        public ElementVar(bool[] mas)
        {
            if (mas.Length == 64)
                Picture = mas;
            else
                throw new ArgumentException("Wrong length of mas");
        }
    }
}
