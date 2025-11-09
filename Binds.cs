using Silksong.UnityHelper.Util;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace KeybindChaos;

public static class Binds
{
    public static readonly Dictionary<string, FieldInfo> BindFieldInfos = new()
    {
        ["Jump"] = typeof(HeroActions).GetField(nameof(HeroActions.Jump)),
        ["Dash"] = typeof(HeroActions).GetField(nameof(HeroActions.Dash)),
        ["Attack"] = typeof(HeroActions).GetField(nameof(HeroActions.Attack)),
        ["QuickCast"] = typeof(HeroActions).GetField(nameof(HeroActions.QuickCast)),
        ["QuickMap"] = typeof(HeroActions).GetField(nameof(HeroActions.QuickMap)),
        ["Needolin"] = typeof(HeroActions).GetField(nameof(HeroActions.DreamNail)),
        ["Harpoon"] = typeof(HeroActions).GetField(nameof(HeroActions.SuperDash)),
        ["Focus"] = typeof(HeroActions).GetField(nameof(HeroActions.Cast)),
        ["Taunt"] = typeof(HeroActions).GetField(nameof(HeroActions.Taunt)),
    };

    public static IEnumerable<string> GetAvailableBindNames() => BindFieldInfos.Keys;

    private static Dictionary<string, Sprite>? _sprites;

    public static Dictionary<string, Sprite> Sprites
    {
        get
        {
            if (_sprites == null)
            {
                _sprites = [];
                foreach (string bindName in GetAvailableBindNames())
                {
                    Sprite sprite = SpriteUtil.LoadEmbeddedSprite(typeof(Binds).Assembly, $"KeybindChaos.Resources.{bindName}.png");
                    _sprites[bindName] = sprite;
                }
            }
            return _sprites;
        }
    }
}
