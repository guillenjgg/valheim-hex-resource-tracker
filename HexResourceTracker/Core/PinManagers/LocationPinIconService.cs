using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace HexResourceTracker.Core.PinManagers
{
    internal static class LocationPinIconService
    {
        private static readonly Dictionary<string, string> ResourceNames = new Dictionary<string, string>
        {
            { "NorthMemorialPlace", "HexResourceTracker.Assets.Icons.rune-stone.png" },
            { "CharredFortress", "HexResourceTracker.Assets.Icons.castle.png" }
        };
        private static MethodInfo _loadImage;
        private static readonly Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();

        internal static void Apply(Minimap.PinData pin, string prefabName)
        {
            if (pin == null || !ResourceNames.TryGetValue(prefabName, out string resourceName))
            {
                return;
            }

            if (!Sprites.TryGetValue(resourceName, out Sprite sprite))
            {
                sprite = LoadSprite(resourceName);
                // Cache failures too; leave the standard icon and report the problem only once.
                Sprites[resourceName] = sprite;
            }
            if (sprite == null)
            {
                return;
            }

            pin.m_icon = sprite;
            if (pin.m_iconElement != null)
            {
                pin.m_iconElement.sprite = sprite;
            }
        }

        private static Sprite LoadSprite(string resourceName)
        {
            Texture2D texture = null;
            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        throw new InvalidOperationException($"Embedded icon not found: {resourceName}");
                    }
                    using (var buffer = new MemoryStream())
                    {
                        stream.CopyTo(buffer);
                        texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                        // The runtime PNG module targets netstandard 2.1; resolve its public API without
                        // adding that dependency to this .NET Framework 4.8 plugin.
                        if (_loadImage == null)
                        {
                            _loadImage = Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule", true)
                                .GetMethod("LoadImage", new[] { typeof(Texture2D), typeof(byte[]), typeof(bool) });
                        }
                        if (!(_loadImage.Invoke(null, new object[] { texture, buffer.ToArray(), true }) is bool loaded) || !loaded)
                        {
                            throw new InvalidOperationException($"Unable to decode icon: {resourceName}");
                        }
                    }
                }

                texture.name = resourceName;
                texture.filterMode = FilterMode.Bilinear;
                texture.wrapMode = TextureWrapMode.Clamp;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                sprite.name = resourceName;
                return sprite;
            }
            catch (Exception exception)
            {
                if (texture != null)
                {
                    UnityEngine.Object.Destroy(texture);
                }
                Plugin.Log.LogWarning($"Unable to load location icon. Using the standard map icon. {exception.Message}");
                return null;
            }
        }

        internal static void Clear()
        {
            foreach (Sprite sprite in Sprites.Values)
            {
                if (sprite != null)
                {
                    UnityEngine.Object.Destroy(sprite.texture);
                    UnityEngine.Object.Destroy(sprite);
                }
            }
            Sprites.Clear();
        }
    }
}
