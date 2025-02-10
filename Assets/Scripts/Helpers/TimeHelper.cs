using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class TimeHelper
    {
        public const int TicksPerSecond = 50;

        public static int SecondsToTicks(float seconds)
        {
            return Mathf.CeilToInt(seconds * TicksPerSecond);
        }

        public static float TicksToSeconds(int ticks)
        {
            return ticks / (float)TicksPerSecond;
        }
    }
}