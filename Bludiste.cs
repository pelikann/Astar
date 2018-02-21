using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{


    public class Bludiste
    {
        private Random _r;
        private HerniPlocha _plocha;

        public Vrchol[,] VratGraf()
        {
            Vrchol[,] vrcholy = new Vrchol[Sirka, Vyska];
            for (int i = 0; i < Sirka; i++)
            {
                for (int j = 0; j < Vyska; j++)
                {
                    int vzdusnex = 0;
                    int vzdusney = 0;

                    if (i != CilX)
                        vzdusnex = Math.Abs(i - CilX);
                    if (j != CilY)
                        vzdusney = Math.Abs(j - CilY);

                    /*int vzdusnex = 0;
                    if (CilX > i)
                        vzdusnex = CilX - i;
                    else if(CilX < i)
                        vzdusnex = i - CilX;

                    int vzdusney = 0;
                    if (CilY > j)
                        vzdusney = CilY - j;
                    else if (CilY < j)
                        vzdusney = j - CilY;*/

                    double vzdusne;
                    if (vzdusney != 0 && vzdusnex != 0)
                        vzdusne = Math.Sqrt((vzdusnex * vzdusnex) + (vzdusney * vzdusney));
                    else if (vzdusnex != 0)
                        vzdusne = vzdusnex;
                    else
                        vzdusne = vzdusney;

                    

                    if(!_plocha.JeZed(i, j))
                    {
                        Vrchol vrchol = new Vrchol(i, j, vzdusne);
                        vrcholy[i, j] = vrchol;
                    }
                }
            }

            for (int i = 0; i < Sirka; i++)
            {
                for (int j = 0; j < Vyska; j++)
                {
                    if (vrcholy[i, j] == null)
                        continue;
                    if(i > 0) 
                    {
                        vrcholy[i, j].sousede[0] = vrcholy[i - 1, j];
                    }
                    if(j > 0)
                    {
                        vrcholy[i, j].sousede[1] = vrcholy[i, j - 1];
                    }
                    if(i < Sirka - 1)
                    {
                        vrcholy[i, j].sousede[2] = vrcholy[i + 1, j];
                    }
                    if(j < Vyska - 1)
                    {
                        vrcholy[i, j].sousede[3] = vrcholy[i, j + 1];
                    }
                }
            }
            return vrcholy;
        }

        public int PocetKroku { get; private set; }
        public int Level { get; private set; }
        public int CilX { get { return _plocha.CilX; } }
        public int CilY { get { return _plocha.CilY; } }
        public int PostavickaX { get { return _plocha.PostavickaX; } set { _plocha.PostavickaX = value; } }
        public int PostavickaY { get { return _plocha.PostavickaY; } set { _plocha.PostavickaY = value; } }

        public int Sirka { get; private set; }
        public int Vyska { get; private set; }

        public Bludiste(int sirka, int vyska, int seed)
        {
            _r = new Random(seed);
            Level = 0;
            Sirka = sirka;
            Vyska = vyska;
            GenerujLevel();
        }

        public bool MuzuSeverne() { return !_plocha.JeZed(_plocha.PostavickaX, _plocha.PostavickaY - 1); }
        public bool MuzuJizne() { return !_plocha.JeZed(_plocha.PostavickaX, _plocha.PostavickaY + 1); }
        public bool MuzuZapadne() { return !_plocha.JeZed(_plocha.PostavickaX - 1, _plocha.PostavickaY); }
        public bool MuzuVychodne() { return !_plocha.JeZed(_plocha.PostavickaX + 1, _plocha.PostavickaY); }        

        public bool JdiSeverne()
        {
            if (!MuzuSeverne())
                throw new Exception("Na sever to nejde!");
            _plocha.PostavickaY -= 1;
            PocetKroku++;
            return Cil();
        }
        public bool JdiJizne()
        {
            if (!MuzuJizne())
                throw new Exception("Na jih to nejde!");
            _plocha.PostavickaY += 1;
            PocetKroku++;
            return Cil();
        }
        public bool JdiZapadne()
        {
            if (!MuzuZapadne())
                throw new Exception("Na zapad to nejde!");
            _plocha.PostavickaX -= 1;
            PocetKroku++;
            return Cil();
        }
        public bool JdiVychodne()
        {
            if (!MuzuVychodne())
                throw new Exception("Na vychod to nejde!");
            _plocha.PostavickaX += 1;
            PocetKroku++;
            return Cil();
        }

        public void PolozDrobecek() { _plocha.Oznac(_plocha.PostavickaX, _plocha.PostavickaY); }
        public void SeberDrobecek() { _plocha.Odznac(_plocha.PostavickaX, _plocha.PostavickaY); }
        public bool DrobecekSeverne()
        {
            if (!MuzuSeverne()) return false;
            return _plocha.JeZnacka(_plocha.PostavickaX, _plocha.PostavickaY - 1);
        }
        public bool DrobecekJizne()
        {
            if (!MuzuJizne()) return false;
            return _plocha.JeZnacka(_plocha.PostavickaX, _plocha.PostavickaY + 1);
        }
        public bool DrobecekZapadne()
        {
            if (!MuzuZapadne()) return false;
            return _plocha.JeZnacka(_plocha.PostavickaX - 1, _plocha.PostavickaY);
        }
        public bool DrobecekVychodne()
        {
            if (!MuzuVychodne()) return false;
            return _plocha.JeZnacka(_plocha.PostavickaX + 1, _plocha.PostavickaY);
        }
        public bool DrobecekTady()
        {
            return _plocha.JeZnacka(_plocha.PostavickaX, _plocha.PostavickaY);
        }

        public void DalsiLevel()
        {
            if (!Cil()) throw new InvalidOperationException("Nejsi v cili!");
            GenerujLevel();
        }
        private void GenerujLevel()
        {
            Level++;
            //Level = 50;
            _plocha = new HerniPlocha(Sirka, Vyska);
            _plocha.Prekreslovat = false;

            // Zed po obvodu
            for (int x = 0; x < _plocha.Sirka; x++)
            {
                _plocha.NastavZed(x, 0, true);
                _plocha.NastavZed(x, _plocha.Vyska - 1, true);
            }
            for (int y = 0; y < _plocha.Vyska; y++)
            {
                _plocha.NastavZed(0, y, true);
                _plocha.NastavZed(_plocha.Sirka - 1, y, true);
            }

            // Zaklad na kazdem druhem
            List<Tuple<int, int>> zaklady = new List<Tuple<int, int>>();
            for (int y = 2; y < _plocha.Vyska - 2; y += 2)
                for (int x = 2; x < _plocha.Sirka - 2; x += 2)
                    if (!_plocha.JeZed(x, y))
                        zaklady.Add(new Tuple<int, int>(x, y));

            // Stavba bludiste
            while (zaklady.Count > 0)
            {
                var ix = _r.Next(0, zaklady.Count);
                var start = zaklady[ix];
                zaklady.RemoveAt(ix);
                if (_plocha.JeZed(start.Item1, start.Item2))
                    continue; // Policko uz je zazdene

                switch (_r.Next(0, 3))
                {
                    case 0:
                        for (int x = start.Item1; x > 0 && !_plocha.JeZed(x, start.Item2); x--)
                            _plocha.NastavZed(x, start.Item2, true);
                        break;
                    case 1:
                        for (int x = start.Item1; x < _plocha.Sirka && !_plocha.JeZed(x, start.Item2); x++)
                            _plocha.NastavZed(x, start.Item2, true);
                        break;
                    case 2:
                        for (int y = start.Item2; y > 0 && !_plocha.JeZed(start.Item1, y); y--)
                            _plocha.NastavZed(start.Item1, y, true);
                        break;
                    case 3:
                        for (int y = start.Item2; y < _plocha.Vyska && !_plocha.JeZed(start.Item1, y); y++)
                            _plocha.NastavZed(start.Item1, y, true);
                        break;
                    default:
                        throw new Exception("Toto by se stat nemelo!");
                }
            }

            // Udelej z bludiste emental (vic der komplikuje pruchod)
            int pocetDer = (int)((Sirka - 2) * (Vyska - 2) * (0.1f * Math.Min((Level / 5), 9)));
            while (pocetDer-- > 0) _plocha.NastavZed(_r.Next(1, Sirka - 1), _r.Next(1, Vyska - 1), false);

            while (!_plocha.NastavStartACil(_r.Next(1, Sirka - 1), _r.Next(1, Vyska - 1), _r.Next(1, Sirka - 1), _r.Next(1, Vyska - 1)));
            _plocha.Prekreslovat = true;
            _plocha.Prekresli();            
        }
        private bool Cil()
        {
            return _plocha.PostavickaX == _plocha.CilX && _plocha.PostavickaY == _plocha.CilY;
        }
    }
}
