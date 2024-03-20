using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AbejasYoso
{
    internal class Hilo
    {
        private int tarro, capTarro;
        private int numVecesComeOso;
        private readonly Thread[] abejas;
        private SemaphoreSlim semaforo; // Semáforo para controlar el acceso a la función Abejas
        private SemaphoreSlim semaforoOso; // Semáforo para controlar el acceso a la función Oso
        private bool continuarProduccion = true; // Nueva bandera para controlar la producción de las abejas

        public Hilo()
        {
            abejas = new Thread[10];
            semaforo = new SemaphoreSlim(1, 1); // Inicializa el semáforo con un límite de 1
            semaforoOso = new SemaphoreSlim(0, 1); // Inicializa el semáforo del oso con un límite de 1
        }

        public void inicializa()
        {
            Random r = new Random();
            for (int i = 0; i < abejas.Length; i++)
            {
                int index = i + 1;
                int randomValue = r.Next(1, 6);
                abejas[i] = new Thread(() => Abejas(index, randomValue));
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

        public async void Abejas(int index, int random)
        {
            while (continuarProduccion) // Utilizamos esta bandera para controlar la producción
            {
                await semaforo.WaitAsync(); // Espera hasta que el semáforo esté disponible

                if (!continuarProduccion) // Salir del bucle si se indica que la producción debe detenerse
                {
                    semaforo.Release(); // Libera el semáforo antes de salir
                    break;
                }

                tarro += random;
                Console.WriteLine($"Abeja {index} produce: {random}");
                if (tarro >= capTarro)
                {
                    semaforoOso.Release(); // Despierta al oso
                    await semaforoOso.WaitAsync(); // Espera a que el oso termine de comer
                }

                semaforo.Release(); // Libera el semáforo para que la siguiente abeja pueda producir
            }
        }

        public void Oso()
        {
            while (numVecesComeOso < 3)
            {
                semaforoOso.Wait(); // Espera hasta que el tarro esté lleno
                Console.WriteLine($"Tarro lleno: {tarro}");

                Console.WriteLine("Oso se despierta y se come la miel");
                tarro = 0;
                Console.WriteLine("Oso se duerme");
                numVecesComeOso++;
                if (numVecesComeOso < 3)
                    semaforoOso.Release(); // Permite que las abejas continúen produciendo
                else // Si el oso ha comido tres veces, indicamos que la producción de las abejas debe detenerse
                    continuarProduccion = false;
            }
        }
    }
}
