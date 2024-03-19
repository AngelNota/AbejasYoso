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

        private bool puedeComer = false;
        private bool continuarProduccion = true; // Nueva bandera para controlar la producción de las abejas
        private readonly object lockObject = new object();

        public Hilo()
        {
            abejas = new Thread[10];  
        }

        public void inicializa()
        {

            Random r = new Random();
            for (int i = 0; i < abejas.Length; i++)
            {
                int index = i+1; // Para evitar el problema de cierre
                int randomValue = r.Next(1, 6);
                abejas[i] = new Thread(() => Abejas(index,randomValue));
            }
            Thread osoThread = new Thread(Oso);

            tarro = 0;
            capTarro = 25;
            numVecesComeOso = 0;

            osoThread.Start();
            foreach (var t in abejas)
            {
                t.Start();
            }
        }

        public void Abejas(int index, int random)
        { 

            while (continuarProduccion) // Utilizamos esta bandera para controlar la producción
            {
                lock (lockObject)
                {
                    if (!continuarProduccion) // Salir del bucle si se indica que la producción debe detenerse
                        break;

                    tarro += random;
                    Console.WriteLine($"Abeja {index} produce: {random}");
                    if (tarro >= capTarro)
                    {
                       
                        Monitor.Pulse(lockObject);
                        Monitor.Wait(lockObject);
                    }
                }
            }
        }

        public void Oso()
        {
            while (numVecesComeOso < 3)
            {
                lock (lockObject)
                {
                    while (tarro < capTarro) // Esperar hasta que el tarro esté lleno
                        Monitor.Wait(lockObject);
                    Console.WriteLine($"Tarro lleno: {tarro}");
                    
                        Console.WriteLine("Oso se despierta y se come la miel");
                        tarro = 0;
                        Console.WriteLine("Oso se duerme");
                        numVecesComeOso++;
                        if (numVecesComeOso < 3)
                            Monitor.Pulse(lockObject);
                        else // Si el oso ha comido tres veces, indicamos que la producción de las abejas debe detenerse
                            continuarProduccion = false;
                    
                }
            }
            /*
            // Cuando el oso haya comido tres veces, indicamos a las abejas que dejen de producir
            lock (lockObject)
            {
                continuarProduccion = false;
            }

            // Esperar a que todas las abejas terminen
            foreach (var t in abejas)
            {
                t.Join();
            }*/
        }
    }
}