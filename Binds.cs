using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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
}
