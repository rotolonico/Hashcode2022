using System;
using System.Collections.Generic;

namespace Hashcode2022
{
    public static class TimeHandler
    {
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        private static Dictionary<string, long> timers = new Dictionary<string, long>();

        public static void StartTimer(string timerId)
        {
            if (timers.ContainsKey(timerId))
            {
                Console.WriteLine("Timer already started!");
                return;
            }
            
            Console.WriteLine("Timer " + timerId + " started");
            if (timers == null) timers = new Dictionary<string, long>();
            timers.Add(timerId, GetTime());
        }

        public static long EndTimer(string timerId)
        {
            if (!timers.ContainsKey(timerId))
            {
                Console.WriteLine("Timer not started!");
                return 0;
            }
            
            var milliseconds = GetTime() - timers[timerId];
            
            Console.WriteLine("Timer " + timerId + " lasted " + milliseconds + " milliseconds");
            return milliseconds;
        }

        private static long GetTime() => (long) (DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
    }
}