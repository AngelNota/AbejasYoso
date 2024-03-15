using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AbejasYoso
{
    internal class Hilo
    {
        private int tarro, capTarro;
        private int numVecesComeOso;
        private readonly Thread[] abejas;
        public int contHilosTerminados = 0;

        private bool puedeComer = false;
        private readonly object lockObject = new object();

        public Hilo()
        {
            abejas = new Thread[10];
        }

        public void inicializa()
        {
            for (int i = 0; i < abejas.Length; i++)
            {
                int index = i; // To avoid closure issue
                abejas[i] = new Thread(() => Abejas(index));
            }

            Thread osoThread = new Thread(Oso);

            Random r = new Random();
            tarro = 0;
            capTarro = 25;
            numVecesComeOso = 0;

            foreach (var t in abejas)
            {
                t.Start();
            }

            osoThread.Start();
        }

        public void Abejas(int index)
        {
            while (numVecesComeOso < 3)
            {
                int cantidad = new Random().Next(2, 6);
                lock (lockObject)
                {
                    tarro += cantidad;
                    Console.WriteLine($"Abeja {index} produce: {cantidad}");
                    if (tarro >= capTarro)
                    {
                        Console.WriteLine($"Tarro lleno: {tarro}");
                        puedeComer = true;

                        // Notificar al oso que el tarro está lleno
                        Monitor.Pulse(lockObject);

                        // Esperar a que el oso termine de comer
                        while (puedeComer)
                        {
                            Monitor.Wait(lockObject);
                        }
                    }
                }
            }
            Interlocked.Increment(ref contHilosTerminados);
        }

        public void Oso()
        {
            while (numVecesComeOso < 3)
            {
                lock (lockObject)
                {
                    // Esperar a que el tarro esté lleno
                    while (!puedeComer)
                    {
                        Monitor.Wait(lockObject);
                    }

                    // El oso come cuando el tarro está lleno
                    Console.WriteLine("Oso se despierta y se come la miel");
                    tarro = 0;
                    Console.WriteLine("Oso se duerme");
                    numVecesComeOso++;
                    puedeComer = false;

                    // Notificar a las abejas que el oso ha comido
                    Monitor.Pulse(lockObject);
                }
            }
            Interlocked.Increment(ref contHilosTerminados);
        }
    }
}