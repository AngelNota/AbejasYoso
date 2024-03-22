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
                int index = i; // To avoid capturing the loop variable
                Thread t = new Thread(() => Abejas(index));
                t.Start();
            }

        }

        public void Abejas(int index)
        {
            while (numVecesComeOso < 3)
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
            }
        }

        public void Oso()
        {
            while (numVecesComeOso < 3)
            {
                abejasSemaphore.WaitOne(); // Wait for the tarro to be full

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