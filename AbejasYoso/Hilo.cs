using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace parcial2
{
    internal class Hilo
    {
        private int tarro, capTarro, numVecesComeOso;
        private int[] abejas;
        private bool puedeComer = false;
        public int contHilosTerminados = 0;
        public Hilo()
        {

        }

        public void IniciaHilo()
        {
            Thread[] t = new Thread[10];
            Thread t1 = new Thread(Oso);

            Random r = new Random();
            abejas = new int[10];
            for (int i = 0; i < abejas.Length; i++)
                abejas[i] = r.Next(1, 6);

            tarro = 0;
            capTarro = 25;

            numVecesComeOso = 0;
            t1.Start();
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = new Thread(Abejas);
                t[i].Start();
                t[i].Join();
            }
        }

        public void Abejas()
        {
            int i = 0;
            while (numVecesComeOso < 3)
            {
                tarro += abejas[i];
                Console.WriteLine("Abeja: {0} produce: {1}", i, abejas[i]);

                if (tarro >= capTarro)
                {
                    Console.WriteLine("Tarro lleno: {0}", tarro);
                    puedeComer = true;
                    while (puedeComer) ;
                }


                i++;
                if (i == abejas.Length)
                    i = 0;

            }
            contHilosTerminados++;
        }
        public void Oso()
        {
            while (numVecesComeOso < 3)
            {
                if (puedeComer)
                {
                    Console.WriteLine("Oso se despierta a comer.");
                    tarro = 0;
                    Console.WriteLine("Oso comio.");
                    Console.WriteLine("Oso se duerme.");
                    numVecesComeOso++;
                    puedeComer = false;
                }
            }

            contHilosTerminados++;
        }

    }
}
