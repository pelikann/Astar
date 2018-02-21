using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    internal class HerniPlocha
    {
        private int _px, _py;
        private bool[,] _znacky;
        private bool[,] _zdi;

        public int Sirka { get; private set; }
        public int Vyska { get; private set; }
        public int StartX { get; private set; }
        public int StartY { get; private set; }
        public int CilX { get; private set; }
        public int CilY { get; private set; }
        public bool Prekreslovat { get; set; }
        public int PostavickaX
        {
            get { return _px; }
            set
            {
                int stare = _px;
                _px = value;
                Prekresli(stare, _py);
                Prekresli(_px, _py);
            }
        }
        public int PostavickaY
        {
            get { return _py; }
            set
            {
                int stare = _py;
                _py = value;
                Prekresli(_px, stare);
                Prekresli(_px, _py);
            }
        }

        public HerniPlocha(int sirka, int vyska)
        {
            Console.SetWindowSize(sirka + 1, vyska + 1);
            Console.SetBufferSize(sirka + 1, vyska + 1);
            Console.CursorVisible = false;
            Prekreslovat = false;
            Sirka = sirka;
            Vyska = vyska;                         
            SmazZdi();
            SmazZnacky();
            StartX = StartY = CilX = CilY = -1;       
        }      

        public void Oznac(int x, int y)
        {
            _znacky[x, y] = true;
            Prekresli(x, y);
        }
        public void Odznac(int x, int y)
        {
            _znacky[x, y] = false;
            Prekresli(x, y);
        }
        public bool JeZnacka(int x, int y)
        {
            return _znacky[x, y];
        }
        private void SmazZnacky()
        {
            _znacky = new bool[Sirka, Vyska];
        }

        public void NastavZed(int x, int y, bool zed)
        {
            if (x == CilX && y == CilY) return;
            if (x == StartX && y == StartY) return;
            _zdi[x, y] = zed;
            Prekresli(x, y);
        }
        public bool JeZed(int x, int y)
        {
            return _zdi[x, y];
        }
        private void SmazZdi()
        {
            _zdi = new bool[Sirka, Vyska];
        }

        public bool NastavStartACil(int sx, int sy, int cx, int cy)
        {
            if (JeZed(sx, sy)) return false;
            if (JeZed(cx, cy)) return false;

            StartX = sx;
            StartY = sy;
            CilX = cx;
            CilY = cy;
            PostavickaX = sx;
            PostavickaY = sy;
            Prekresli(cx, cy);

            return true;
        }

        public void Prekresli()
        {
            if (Prekreslovat)
                for (int y = 0; y < Vyska; y++)
                    for (int x = 0; x < Sirka; x++)
                        Prekresli(x, y);
        }
        public void Prekresli(int x, int y)
        {
            if (!Prekreslovat) return;

            ConsoleColor pozadi = ConsoleColor.Black;
            ConsoleColor popredi = ConsoleColor.White;

            if (x == CilX && y == CilY) pozadi = ConsoleColor.DarkGreen;
            else if (x == StartX && y == StartY) pozadi = ConsoleColor.DarkBlue;
            else if (_zdi[x,y]) pozadi = ConsoleColor.DarkRed;
            else if (_znacky[x, y]) pozadi = ConsoleColor.DarkYellow;

            bool hrac = (x == PostavickaX && y == PostavickaY);

            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = popredi;
            Console.BackgroundColor = pozadi;
            Console.Write(hrac ? '@' : ' ');
            Console.ResetColor();
        }
    }
}
