using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KeybindChaos.Import;

internal static class SpriteUtils
{
    private static readonly Assembly _asm = typeof(SpriteUtils).Assembly;

    private static readonly Dictionary<string, Sprite> _sprites = new();

    private static Sprite LoadEmbeddedSpriteInternal(string resourceName)
    {
        using Stream imageStream = _asm.GetManifestResourceStream(resourceName);
        byte[] buffer = new byte[imageStream.Length];
        _ = imageStream.Read(buffer, 0, buffer.Length);

        Texture2D tex = new(1, 1);
        tex.LoadImage(buffer.ToArray());

        Sprite sprite = Sprite.Create(tex, new(0, 0, tex.width, tex.height), new(0.5f, 0.5f));

        return sprite;
    }

    public static Sprite LoadEmbeddedSprite(string resourceName)
    {
        if (!_sprites.TryGetValue(resourceName, out Sprite sprite))
        {
            sprite = LoadEmbeddedSpriteInternal(resourceName);
            _sprites.Add(resourceName, sprite);
        }
        return sprite;
    }
}
