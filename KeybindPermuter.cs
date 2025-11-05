using BepInEx.Configuration;
using BepInEx.Logging;
using InControl;
using JetBrains.Annotations;
using MonoDetour.DetourTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Logger = BepInEx.Logging.Logger;

namespace KeybindChaos
{
    /// <summary>
    /// Class to help with randomizing the binds.
    /// </summary>
    [PublicAPI]
    public static class KeybindPermuter
    {
        internal static void EnsureHooked()
        {
            Md.InputHandler.SendKeyBindingsToGameSettings.ControlFlowPrefix(PreventSavingBindsHook);
            Md.InputHandler.SendButtonBindingsToGameSettings.ControlFlowPrefix(PreventSavingBindsHook);
        }

        private static ReturnFlow PreventSavingBindsHook(InputHandler self)
        {
            if (!_preventSavingBinds)
            {
                return ReturnFlow.None;
            }
            Log.LogInfo("Not saving binds to settings");
            return ReturnFlow.SkipOriginal;
        }

        private static readonly ManualLogSource Log = Logger.CreateLogSource($"{nameof(KeybindChaos)}:{nameof(KeybindPermuter)}");

        [PublicAPI] public static event Action? OnRandomize;
        [PublicAPI] public static event Action? OnRestore;

        private static List<PlayerAction>? _storedBindings = null;
        private static readonly Random _rng = new();
        private static bool _preventSavingBinds = false;

        private static bool IsRandomized => _storedBindings is not null;

        private static void PreventSavingBinds()
        {
            _preventSavingBinds = true;
        }

        private static void AllowSavingBinds()
        {
            _preventSavingBinds = false;
        }

        [PublicAPI]
        public static void RandomizeBinds() => RandomizeBinds(_rng);


        [PublicAPI]
        public static void RandomizeBinds(Random rng)
        {
            // Idempotent so it is safe to reuse
            PreventSavingBinds();

            List<PlayerAction> actions = RetrieveBinds();
            rng.PermuteInPlace(actions);
            AssignBinds(actions);

            OnRandomize?.SafeInvoke();
        }

        [PublicAPI]
        public static void RestoreBinds()
        {
            if (!IsRandomized) return;

            AssignBinds(_storedBindings!);
            _storedBindings = null;
            AllowSavingBinds();

            OnRestore?.SafeInvoke();
        }

        /// <summary>
        /// Return a list of the current binds, in order.
        /// </summary>
        public static List<(string key, PlayerAction action)> RetrieveBindPairs()
        {
            List<(string key, PlayerAction action)> actions = new();
            HeroActions ia = InputHandler.Instance.inputActions;
            Dictionary<string, ConfigEntry<bool>> config = KeybindChaosPlugin.Instance.BindConfig;

            if (config["Jump"].Value) actions.Add(("Jump", ia.Jump));
            if (config["Dash"].Value) actions.Add(("Dash", ia.Dash));
            if (config["Attack"].Value) actions.Add(("Attack", ia.Attack));
            if (config["QuickCast"].Value) actions.Add(("QuickCast", ia.QuickCast));
            if (config["QuickMap"].Value) actions.Add(("QuickMap", ia.QuickMap));
            if (config["Needolin"].Value) actions.Add(("Needolin", ia.DreamNail));
            if (config["Harpoon"].Value) actions.Add(("Harpoon", ia.SuperDash));
            if (config["Focus"].Value) actions.Add(("Focus", ia.Cast));
            if (config["Taunt"].Value) actions.Add(("Taunt", ia.Taunt));

            return actions;
        }

        public static List<PlayerAction> RetrieveBinds()
        {
            return RetrieveBindPairs().Select(pair => pair.action).ToList();
        }

        /// <summary>
        /// Assign the ordered list of actions to the player actions.
        /// </summary>
        private static void AssignBinds(List<PlayerAction> actions)
        {
            int index = 0;

            HeroActions ia = InputHandler.Instance.inputActions;

            foreach ((string key, FieldInfo fi) in Binds.BindFieldInfos)
            {
                if (KeybindChaosPlugin.Instance.BindConfig[key].Value)
                {
                    fi.SetValue(ia, actions[index]);
                    index++;
                }
            }
        }
    }
}
