using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace coroutine
{
    public class Schedule
    {
        public static Schedule Singleton { get; } = new Schedule();

        public class Coroutine
        {
            public IEnumerator routine;
            public Coroutine waitForCoroutine;
            public bool finished = false;
            public Coroutine(IEnumerator routine) { this.routine = routine; }
        }

        public readonly List<Coroutine> coroutines = new List<Coroutine>();

        private Schedule() { }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            Coroutine coroutine = new Coroutine(routine);
            coroutines.Add(coroutine);
            return coroutine;
        }

        public void Update()
        {
            foreach(Coroutine coroutine in coroutines.Reverse<Coroutine>())
            {
                if (coroutine.routine.Current is Coroutine)
                    coroutine.waitForCoroutine = coroutine.routine.Current as Coroutine;
                if (coroutine.waitForCoroutine != null && coroutine.waitForCoroutine.finished)
                    coroutine.waitForCoroutine = null;
                if (coroutine.waitForCoroutine != null)
                    continue;

                // update
                if (coroutine.routine.MoveNext())
                    coroutine.finished = false;
                else
                {
                    coroutines.Remove(coroutine);
                    coroutine.finished = true;
                }
            }
        }
    }
}
