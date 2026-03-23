using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;

namespace PSDImporter
{
    public static class PSDImporterUtils
    {
        public static Vector2 GetTextureSize(string assetPath)
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                object[] args = new object[2] { 0, 0 };
                MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);
                return new Vector2((int)args[0], (int)args[1]);
            }
            return Vector2.zero;
        }

        public static void SetTextureImportSettings(TextureImporter importer, PSDImporterConfig config)
        {
            // 基本设置
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency = true;
            importer.isReadable = false;
            importer.filterMode = FilterMode.Bilinear;

            // Sprite相关参数
            var textureSettings = new TextureImporterSettings();
            importer.ReadTextureSettings(textureSettings);
            textureSettings.spriteMeshType = SpriteMeshType.FullRect;
            textureSettings.spriteExtrude = 1;
            textureSettings.spriteGenerateFallbackPhysicsShape = false;
            importer.SetTextureSettings(textureSettings);

            // 压缩设置
            if (config.UseCompression)
            {
                // 默认平台
                TextureImporterPlatformSettings defaultSettings = new TextureImporterPlatformSettings();
                defaultSettings.name = "DefaultTexturePlatform";
                defaultSettings.maxTextureSize = config.MaxTextureSize;
                defaultSettings.format = TextureImporterFormat.Automatic;
                defaultSettings.textureCompression = TextureImporterCompression.Compressed;
                defaultSettings.compressionQuality = config.CompressionQuality;
                importer.SetPlatformTextureSettings(defaultSettings);

                // Android
                TextureImporterPlatformSettings androidSettings = new TextureImporterPlatformSettings();
                androidSettings.name = "Android";
                androidSettings.overridden = true;
                androidSettings.maxTextureSize = config.MaxTextureSize;
                androidSettings.format = TextureImporterFormat.Automatic;
                androidSettings.textureCompression = TextureImporterCompression.Compressed;
                androidSettings.compressionQuality = config.CompressionQuality;
                importer.SetPlatformTextureSettings(androidSettings);

                // iOS
                TextureImporterPlatformSettings iOSSettings = new TextureImporterPlatformSettings();
                iOSSettings.name = "iPhone";
                iOSSettings.overridden = true;
                iOSSettings.maxTextureSize = config.MaxTextureSize;
                iOSSettings.format = TextureImporterFormat.Automatic;
                iOSSettings.textureCompression = TextureImporterCompression.Compressed;
                iOSSettings.compressionQuality = config.CompressionQuality;
                importer.SetPlatformTextureSettings(iOSSettings);
            }
            else
            {
                TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
                settings.name = "DefaultTexturePlatform";
                settings.maxTextureSize = config.MaxTextureSize;
                settings.format = TextureImporterFormat.RGBA32;
                settings.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SetPlatformTextureSettings(settings);

                importer.ClearPlatformTextureSettings("Android");
                importer.ClearPlatformTextureSettings("iPhone");
            }
        }

        public static string GetAssetPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) return string.Empty;

            string assetPath = fullPath;
            if (assetPath.Contains("Assets"))
            {
                assetPath = assetPath.Substring(assetPath.IndexOf("Assets"));
            }
            return assetPath.Replace('\\', '/');
        }

        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                AssetDatabase.Refresh();
            }
        }
    }
}
