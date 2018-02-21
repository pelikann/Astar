using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class Vrchol
    {
        public int x;
        public int y;

        public double vzdusne;
        public double delkadohromady;

        public Vrchol[] sousede;
        public Vrchol predchozi;
        public int delka;

        public Vrchol(int x, int y, double vzdusne)
        {
            this.x = x;
            this.y = y;
            this.vzdusne = vzdusne;
            this.delka = int.MaxValue;
            this.delkadohromady = int.MaxValue;
            sousede = new Vrchol[4];
        }
    }
}
