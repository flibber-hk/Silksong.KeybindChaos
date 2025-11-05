using BepInEx.Logging;
using System;
using System.Collections.Generic;

namespace KeybindChaos
{
    internal static class Extensions
    {
        private static readonly ManualLogSource Log = Logger.CreateLogSource("KeybindChaos.Extensions");

        public static void Swap<T>(this List<T> items, int i, int j)
        {
            T local = items[i];
            items[i] = items[j];
            items[j] = local;
        }

        public static void PermuteInPlace<T>(this Random rng, List<T> items)
        {
            for (int i = 1; i < items.Count; i++)
            {
                int j = rng.Next(i+1);
                items.Swap(i, j);
            }
        }

        public static void SafeInvoke(this Action action)
        {
            foreach (Action action2 in action.GetInvocationList())
            {
                try
                {
                    action2?.Invoke();
                }
                catch (Exception ex)
                {
                    Log.LogError(ex);
                }
            }
        }
    }
}
