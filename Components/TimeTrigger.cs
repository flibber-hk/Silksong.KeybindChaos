using BepInEx.Configuration;
using Silksong.UnityHelper.Extensions;
using Silksong.UnityHelper.Util;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace KeybindChaos.Components;

public class TimeTrigger : MonoBehaviour
{
    private float _time;
    private AudioSource _as;
    private AudioClip _clip;

    private const float audioStartTime = 4f;
    private bool _startedAudio = false;

    private Display _display;

    public string GetDisplayText()
    {
        if (KeybindChaosPlugin.Instance.KeybindMode.Value != ShuffleMode.Timer) return string.Empty;

        if (_time < 10f)
        {
            return string.Format("Keybind Reset: {0:0.00}\n", _time);
        }
        else
        {
            return string.Format("Keybind Reset: {0:0}s\n", Mathf.Floor(_time));
        }
    }

    private void Awake()
    {
        _as = gameObject.GetComponent<AudioSource>();
        _clip = LoadTimerAudio();
    }

    private AudioClip LoadTimerAudio() => WavUtil.AudioClipFromEmbeddedResource("KeybindChaos.Resources.countdown.wav", typeof(TimeTrigger).Assembly);

    void Start()
    {
        _display = gameObject.GetOrAddComponent<Display>();

        _time = KeybindChaosPlugin.Instance.ResetTime.Value;
        KeybindChaosPlugin.Instance.Config.SettingChanged += OnSettingsChanged;
    }

    void OnDestroy()
    {
        KeybindChaosPlugin.Instance.Config.SettingChanged -= OnSettingsChanged;
    }


    private void OnSettingsChanged(object sender, SettingChangedEventArgs e)
    {
        _time = KeybindChaosPlugin.Instance.ResetTime.Value;
    }

    void Update()
    {
        if (KeybindChaosPlugin.Instance.KeybindMode.Value != ShuffleMode.Timer) return;

        _time -= Time.deltaTime;

        if (_time < 0f)
        {
            _time += KeybindChaosPlugin.Instance.ResetTime.Value;
            _startedAudio = false;
            KeybindPermuter.RandomizeBinds();
        }
        if (_time < audioStartTime && !_startedAudio)
        {
            _startedAudio = true;
            if (KeybindChaosPlugin.Instance.PlayTimerAudio.Value && _clip != null)
            {
                _as.PlayOneShot(_clip);
            }
        }

        _display.UpdateText(GetDisplayText());
    }
}