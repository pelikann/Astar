using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Framework;

namespace Bludiste
{
    static class Program
    {
        static void Main()
        {
            Framework.Bludiste b = new Framework.Bludiste(150, 50, 12345);
           /* var cesta = Dijkstra(b);
            foreach (var v in cesta)
            {
                b.PostavickaX = v.x;
                b.PostavickaY = v.y;
                b.PolozDrobecek();
                System.Threading.Thread.Sleep(20);
            }*/
            while (true)
            {
                if (DalsiKrok(b, 0xFF))
                {
                    Console.Title = string.Format("Bludiste - Level {0}, Kroku {1}", b.Level, b.PocetKroku);
                    if (b.Level == 50)
                    {
                        Console.ResetColor();
                        Console.Clear();
                        Console.WriteLine("Vyhra!");
                        Console.ReadKey();
                        System.Threading.Thread.Sleep(2000);
                        return;
                    }
                    b.DalsiLevel();
                    //System.Threading.Thread.Sleep(2000);
                }
                else
                {
                    Console.ResetColor();
                    Console.Clear();
                    Console.WriteLine("Sorry, ale reseni neexistuje!");
                    Console.ReadKey();
                    System.Threading.Thread.Sleep(2000);
                    return;
                }
            }
        }

        #region Dijkstra Algorithm
        public static Vrchol[] Dijkstra(Framework.Bludiste b)
        {
            Vrchol[,] vrcholy = b.VratGraf();
            vrcholy[b.PostavickaX, b.PostavickaY].delka = 0;                    //je tam nula, aby se mi tento prvek vybral pri prvnim volani extractmin
            vrcholy[b.PostavickaX, b.PostavickaY].delkadohromady = 0;
            List<Vrchol> nenavstivene = new List<Vrchol>(vrcholy.OfType<Vrchol>().Where(i => i != null));
            // BuildMinHeap(nenavstivene, nenavstivene.Count);                  //snaha o řešení pomocí haldy - nefunguje

            while (nenavstivene.Count > 0)
            {
                //Vrchol vrchol = ExtractMin(nenavstivene);
                Vrchol vrchol = ExtractMinCombined(nenavstivene);               //Astar feature
                nenavstivene.Remove(vrchol);
                // var v = nenavstivene[0];    //k haldě
                //nenavstivene.RemoveAt(0);     // k haldě
                foreach (Vrchol soused in vrchol.sousede.Where(i => i != null))
                {
                    int temp = vrchol.delka + 1;
                    if (temp < soused.delka)
                    {
                        soused.delka = temp;
                        soused.delkadohromady = temp + soused.vzdusne;          //Astar feature
                        soused.predchozi = vrchol;
                    }
                    // BuildMinHeap(nenavstivene, nenavstivene.Count);          //Heap feature
                }
            }

            List<Vrchol> cesta = new List<Vrchol>();
            Vrchol x = vrcholy[b.CilX, b.CilY];
            cesta.Add(x);
            while (x.predchozi != null)
            {
                cesta.Add(x.predchozi);
                x = x.predchozi;
            }
            cesta.Reverse();
            return cesta.ToArray();
        }

        public static Vrchol ExtractMinCombined(IEnumerable<Vrchol> list)
        {
            Vrchol min = list.First();
            foreach(Vrchol vrchol in list.Skip(1))
            {
                if(vrchol.delkadohromady < min.delkadohromady)
                    min = vrchol;
            }
            return min;
        }

        public static Vrchol ExtractMin(IEnumerable<Vrchol> list)
        {
            Vrchol min = list.First();        
            foreach(Vrchol vrchol in list.Skip(1))
            {
                if (vrchol.delka < min.delka)
                    min = vrchol;
            }
            return min;
        }
        #endregion

        #region HeapSort
        static void Heapify(List<Vrchol> pole, int i, int delka)
        {
            int indexOfLargest = i;
            if (2 * i + 1 < delka && pole[2 * i + 1].delka < pole[i].delka)
            {
                indexOfLargest = 2 * i + 1;
            }
            if (2 * i + 2 < delka && pole[2 * i + 2].delka < pole[indexOfLargest].delka)
            {
                indexOfLargest = 2 * i + 2;
            }
            if (indexOfLargest != i)
            {
                int a = pole[indexOfLargest].delka;
                pole[indexOfLargest] = pole[i];
                pole[i].delka = a;
                Heapify(pole, indexOfLargest, delka);
            }
        }

        static void BuildMinHeap(List<Vrchol> pole, int delka)
        {
            double listy = Math.Round((double)(delka / 2));
            int stred = delka - (int)listy - 1;
            for (int i = stred; i >= 0; i--)
            {
                Heapify(pole, i, delka);
            }
        }

        static void HeapSort(List<Vrchol> pole)
        {
            for (int delka = pole.Count; delka > 0; delka--)
            {
                BuildMinHeap(pole, delka);
                Vrchol a = pole[0];
                pole[0] = pole[delka - 1];
                pole[delka - 1] = a;
            }
        }
        #endregion

        #region ALGORITMUS - pruchod bludistem
        static int Vzdalenost(Framework.Bludiste b, int dx, int dy)
        {
            int ddx = (b.PostavickaX + dx) - b.CilX;
            int ddy = (b.PostavickaY + dy) - b.CilY;
            return ddx * ddx + ddy * ddy;
        }

        static bool DalsiKrok(Framework.Bludiste b, byte povoleneSmery)
        {
            Func<bool> navrat = null;
          //  List<Tuple<int, Func<bool>>> cesty = new List<Tuple<int, Func<bool>>>(4);

            // Oznac a zmen titulek
            b.PolozDrobecek();
            Console.Title = string.Format("Bludiste - Level {0}, Kroku {1}", b.Level, b.PocetKroku);

            // Najdi zpatecni smer a uloz mozne cesty vcetne priority
            if ((povoleneSmery & 0x01) != 0)
            {
                cesty.Add(new Tuple<int, Func<bool>>(Vzdalenost(b, 0, -1), () =>
                {
                    if (b.MuzuSeverne() && !b.DrobecekSeverne())
                    {
                        if (b.JdiSeverne())
                            return true;

                        if (DalsiKrok(b, unchecked((byte)~0x04)))
                            return true;
                    }
                    return false;
                }));
            }
            else
                navrat = () => b.JdiSeverne();


            if ((povoleneSmery & 0x02) != 0)
            {
                cesty.Add(new Tuple<int, Func<bool>>(Vzdalenost(b, 1, 0), () =>
                {
                    if (b.MuzuVychodne() && !b.DrobecekVychodne())
                    {
                        if (b.JdiVychodne())
                            return true;

                        if (DalsiKrok(b, unchecked((byte)~0x08)))
                            return true;
                    }
                    return false;
                }));
            }
            else
                navrat = () => b.JdiVychodne();

            if ((povoleneSmery & 0x04) != 0)
            {
                cesty.Add(new Tuple<int, Func<bool>>(Vzdalenost(b, 0, 1), () =>
                {
                    if (b.MuzuJizne() && !b.DrobecekJizne())
                    {
                        if (b.JdiJizne())
                            return true;

                        if (DalsiKrok(b, unchecked((byte)~0x01)))
                            return true;
                    }
                    return false;
                }));
            }
            else
                navrat = () => b.JdiJizne();

            if ((povoleneSmery & 0x08) != 0)
            {
                cesty.Add(new Tuple<int, Func<bool>>(Vzdalenost(b, -1, 0), () =>
                {
                    if (b.MuzuZapadne() && !b.DrobecekZapadne())
                    {
                        if (b.JdiZapadne())
                            return true;

                        if (DalsiKrok(b, unchecked((byte)~0x02)))
                            return true;
                    }
                    return false;
                }));
            }
            else
                navrat = () => b.JdiZapadne();

            // Projdi cesty serazene od nejlepsi
            foreach (var cesta in cesty.OrderBy(i => i.Item1))
                if (cesta.Item2())
                    return true;

            if (navrat != null)
                return navrat();

            return false;
        }
#endregion
    }
}
