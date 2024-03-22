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
        private Semaphore abejasSemaphore;
        private object osoLock = new object();
        private int abejaActual = 0; // Variable global para controlar qué abeja debe producir miel

        public Hilo()
        {

        }

        public void IniciaHilo()
        {
            Thread t1 = new Thread(Oso);

            Random r = new Random();
            abejas = new int[10];
            for (int i = 0; i < abejas.Length; i++)
                abejas[i] = r.Next(1, 6);

            tarro = 0;
            capTarro = 25;
            numVecesComeOso = 0;

            abejasSemaphore = new Semaphore(0, 10);

            t1.Start();
            for (int i = 0; i < 10; i++)
            {
                int index = i;
                Thread t = new Thread(() => Abejas(index));
                
                t.Start();
                
            }

        }

        public void Abejas(int index)
        {
            while (numVecesComeOso < 3)
            {
                if(numVecesComeOso < 3)
                if (index == abejaActual)
                {
                    int cantidad = 0;
                    lock (abejas)
                    {
                        if (index < abejas.Length)
                        {
                            cantidad = abejas[index];
                            Console.WriteLine("Abeja {0}: produce {1}", index, cantidad);
                            tarro += cantidad;
                            if (tarro >= capTarro)
                            {
                                abejasSemaphore.Release();
                            }
                        }
                    }

                    Thread.Sleep(500);
                    abejaActual = (abejaActual + 1) % abejas.Length; // Pasar a la siguiente abeja
                }

            }
        }

        public void Oso()
        {
            while (numVecesComeOso < 3)
            {
                abejasSemaphore.WaitOne();

                lock (osoLock)
                {
                    Console.WriteLine("Tarro lleno: " + tarro);
                    Console.WriteLine("Oso se despierta a comer.");
                    tarro = 0;
                    Console.WriteLine("Oso comió.");
                    Console.WriteLine("Oso se duerme.");
                    numVecesComeOso++;
                }
            }
        }

    }
}