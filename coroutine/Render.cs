using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace coroutine
{
    public delegate void RafAction<T1, T2>(T1 lastTicks, T2 currentTicks);

    public class Render
    {
        public static Render SingleTon { get; } = new Render();
        private static readonly double tickLength = 1000f / Stopwatch.Frequency;
        private bool isRunning = false;
        private readonly double frameLength = 1000f / 60;  // 60fps

        // use this counter to stop
        private int counter = 0;

        private Render() { }

        public IEnumerable<int> Run(object context)
        {
            var f = (RafAction<long, long>)context;
            long currentTicks;
            long lastTicks = Stopwatch.GetTimestamp();
            double elapsedTime;
            int sleepTime;
            while (isRunning && counter < 2000)
            {
                currentTicks = Stopwatch.GetTimestamp();
                f(lastTicks, currentTicks);
                elapsedTime = (Stopwatch.GetTimestamp() - currentTicks) * tickLength;
                sleepTime = (int)(Math.Ceiling(elapsedTime / frameLength) * frameLength - elapsedTime);
                Console.WriteLine($"> frame[{(currentTicks - lastTicks) * tickLength:00.00}] elapsed[{elapsedTime:00.00}] sleep[{sleepTime:0.00}]");
                lastTicks = currentTicks;
                yield return sleepTime;
                counter++;
            }
            yield break;
        }
        public void Start()
        {
            isRunning = true;
        }
        public void Stop()
        {
            isRunning = false;
        }
    }
}
