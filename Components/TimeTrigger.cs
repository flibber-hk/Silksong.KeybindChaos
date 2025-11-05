using BepInEx.Configuration;
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

    private void GetDisplayText(StringBuilder sb)
    {
        if (KeybindChaosPlugin.Instance.KeybindMode.Value != Mode.Timer) return;

        if (_time < 10f)
        {
            sb.AppendFormat("Keybind Reset: {0:0.00}\n", _time);
        }
        else
        {
            sb.AppendFormat("Keybind Reset: {0:0}s\n", Mathf.Floor(_time));
        }
    }

    void Awake()
    {
        _as = gameObject.GetComponent<AudioSource>();
        _clip = LoadTimerAudio();
    }

    private AudioClip LoadTimerAudio()
    {
        Assembly a = typeof(TimeTrigger).Assembly;
        using (Stream resFilestream = a.GetManifestResourceStream("KeybindChaos.Resources.countdown.wav"))
        {
            using MemoryStream ms = new();
            resFilestream.CopyTo(ms);
            byte[] ba = ms.ToArray();
            return Satchel.WavUtils.ToAudioClip(ba);
        }
    }

    void Start()
    {
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
        if (KeybindChaosPlugin.Instance.KeybindMode.Value != Mode.Timer) return;

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
    }
}