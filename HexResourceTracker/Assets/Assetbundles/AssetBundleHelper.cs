using System.Reflection;
using UnityEngine;

namespace HexResourceTracker.Assets.Assetbundles
{
    internal static class AssetBundleHelper
    {
        private const string BundleResourceName = "HexResourceTracker.Assets.Assetbundles.customicons";

        private static AssetBundle _assetBundle;

        internal static Sprite LoadSprite(string spriteName)
        {
            if (_assetBundle == null)
            {
                LoadAssetBundle();
            }

            if (_assetBundle == null)
            {
                return null;
            }

            return _assetBundle.LoadAsset<Sprite>(spriteName);
        }

        private static void LoadAssetBundle()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(BundleResourceName))
            {
                if (stream == null)
                {
                    Plugin.Log.LogError($"AssetBundle resource not found: {BundleResourceName}");
                    return;
                }

                var bytes = new byte[stream.Length];
                var bytesRead = 0;

                while (bytesRead < bytes.Length)
                {
                    var count = stream.Read(bytes, bytesRead, bytes.Length - bytesRead);

                    if (count == 0)
                    {
                        break;
                    }

                    bytesRead += count;
                }

                if (bytesRead != bytes.Length)
                {
                    Plugin.Log.LogError("Failed to read embedded AssetBundle.");
                    return;
                }

                _assetBundle = AssetBundle.LoadFromMemory(bytes);

                if (_assetBundle == null)
                {
                    Plugin.Log.LogError("Failed to load customicons AssetBundle.");
                    return;
                }

                Plugin.Log.LogInfo("Custom icons AssetBundle loaded.");
            }
        }
    }
}
