using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

namespace coroutine
{
    class Program
    {
        private static readonly Random rand = new Random();

        static void Main(string[] args)
        {
            Schedule.Singleton.StartCoroutine(Start());
            // stop if no routines to run
            while (Schedule.Singleton.coroutines.Count != 0)
                Update();
        }

        static void Update()
        {
            Schedule.Singleton.Update();
        }

        static IEnumerator Start()
        {
            var render = Render.SingleTon;
            render.Start();
            foreach (var sleepTime in render.Run(new RafAction<long, long>((long lastTicks, long currentTicks) => {
                Thread.Sleep(rand.Next(5, 10));
                currentTicks = Stopwatch.GetTimestamp();
                Console.WriteLine("[Waiting for " + (currentTicks - lastTicks) * 1000f / Stopwatch.Frequency + " ms]");
            })))
            {
                Console.WriteLine();
                yield return Schedule.Singleton.StartCoroutine(WaitMs(sleepTime));
            }

            yield break;
        }

        static IEnumerator WaitMs(int ms)
        {
            long timer = Stopwatch.GetTimestamp() + ms / 1000 * Stopwatch.Frequency;
            while (Stopwatch.GetTimestamp() <= timer)
            {
                yield return null;
            }

            yield break;
        }
    }
}
