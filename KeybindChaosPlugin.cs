using BepInEx;
using BepInEx.Configuration;
using InControl;
using KeybindChaos.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KeybindChaos;

[BepInAutoPlugin(id: "io.github.flibber-hk.keybindchaos")]
public partial class KeybindChaosPlugin : BaseUnityPlugin
{
    public static KeybindChaosPlugin Instance { get; private set; }

    // Config
    public ConfigEntry<Mode> KeybindMode;
    public ConfigEntry<bool> ShowKeybindDisplay;
    public ConfigEntry<int> ResetTime;
    public ConfigEntry<bool> PlayTimerAudio;
    public Dictionary<string, ConfigEntry<bool>> BindConfig { get; private set; }

    private void Awake()
    {
        Instance = this;

        KeybindPermuter.EnsureHooked();

        KeybindMode = Config.Bind("General", nameof(KeybindMode), Mode.Timer, "Choose what causes the keybinds to get shuffled.");

        ShowKeybindDisplay = Config.Bind("General", nameof(ShowKeybindDisplay), true, "Show a display with the current binds.");

        ResetTime = Config.Bind("Mode.Timer", nameof(ResetTime), 30, "The number of seconds before keybinds are shuffled.");
        PlayTimerAudio = Config.Bind("Mode.Timer", nameof(PlayTimerAudio), true, "Play a countdown audio near the end of the timer.");

        BindConfig = Binds.GetAvailableBindNames()
            .ToDictionary(name => name, name => Config.Bind("General.Binds", name, true, $"Include {name}"));

        Md.HeroController.Start.Postfix(AddTimerComponent);

        // Log binds on shuffle because the display doesn't exist yet
        KeybindPermuter.OnRandomize += LogBinds;

        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
    }

    private void LogBinds()
    {
        static string bindRetrieve(PlayerAction action)
        {
            switch (InputHandler.Instance.lastActiveController)
            {
                case BindingSourceType.None:
                case BindingSourceType.KeyBindingSource:
                case BindingSourceType.MouseBindingSource:
                    return InputHandler.Instance.GetKeyBindingForAction(action).ToString();
                case BindingSourceType.DeviceBindingSource:
                    return InputHandler.Instance.GetButtonBindingForAction(action).ToString();
                default:
                    return "???";
            }
        }

        Logger.LogInfo("=== NEW BINDS ===");

        foreach ((string key, PlayerAction action) in KeybindPermuter.RetrieveBindPairs())
        {
            Logger.LogInfo($"{key}: {bindRetrieve(action)}");
        }
    }

    private void AddTimerComponent(HeroController self)
    {
        self.gameObject.AddComponent<Display>();
        self.gameObject.AddComponent<TimeTrigger>();
    }
}
