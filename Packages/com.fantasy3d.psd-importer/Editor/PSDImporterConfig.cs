using UnityEngine;
using UnityEditor;
using System;

namespace PSDImporter
{
    [Serializable]
    public class PSDImporterConfig
    {
        // 路径配置
        public string JsonPath = "";
        public string ImageFolderPath = "";
        public string FunctionName = "";
        public string PrefabSavePath = "Assets/UI/Prefabs/";
        public string CustomPrefabName = "";
        public string DefaultFontPath = "";

        // 设计稿尺寸配置
        public float DesignWidth = 1920f;
        public float DesignHeight = 1080f;

        // 导入设置
        public bool UseCompression = true;
        public int MaxTextureSize = 2048;
        public TextureImporterFormat TextureFormat = TextureImporterFormat.ASTC_6x6;
        public int CompressionQuality = 50;
        public bool GenerateLayoutGroup = false;
        public bool AutoGenerateButton = false;
        public float UserScaleRatio = 1.0f;
        public float OffsetX = 0f;
        public float OffsetY = 0f;
        public string ButtonNameRules = "btn,button";

        // 配置键前缀，避免与其他工具冲突
        private const string PREFIX = "PSDImporter_";
        private const string CONFIG_JSON_PATH = PREFIX + "JsonPath";
        private const string CONFIG_IMAGE_PATH = PREFIX + "ImagePath";
        private const string CONFIG_PREFAB_PATH = PREFIX + "PrefabPath";
        private const string CONFIG_FUNCTION_NAME = PREFIX + "FunctionName";
        private const string CONFIG_CUSTOM_PREFAB_NAME = PREFIX + "CustomPrefabName";
        private const string CONFIG_DEFAULT_FONT_PATH = PREFIX + "DefaultFontPath";
        private const string CONFIG_USE_COMPRESSION = PREFIX + "UseCompression";
        private const string CONFIG_MAX_TEXTURE_SIZE = PREFIX + "MaxTextureSize";
        private const string CONFIG_TEXTURE_FORMAT = PREFIX + "TextureFormat";
        private const string CONFIG_COMPRESSION_QUALITY = PREFIX + "CompressionQuality";
        private const string CONFIG_GENERATE_LAYOUT = PREFIX + "GenerateLayout";
        private const string CONFIG_AUTO_BUTTON = PREFIX + "AutoButton";
        private const string CONFIG_BUTTON_RULES = PREFIX + "ButtonRules";
        private const string CONFIG_SCALE_RATIO = PREFIX + "ScaleRatio";
        private const string CONFIG_OFFSET_X = PREFIX + "OffsetX";
        private const string CONFIG_OFFSET_Y = PREFIX + "OffsetY";

        public void SaveConfig()
        {
            EditorPrefs.SetString(CONFIG_JSON_PATH, JsonPath);
            EditorPrefs.SetString(CONFIG_IMAGE_PATH, ImageFolderPath);
            EditorPrefs.SetString(CONFIG_PREFAB_PATH, PrefabSavePath);
            EditorPrefs.SetString(CONFIG_FUNCTION_NAME, FunctionName);
            EditorPrefs.SetString(CONFIG_CUSTOM_PREFAB_NAME, CustomPrefabName);
            EditorPrefs.SetString(CONFIG_DEFAULT_FONT_PATH, DefaultFontPath);
            EditorPrefs.SetBool(CONFIG_USE_COMPRESSION, UseCompression);
            EditorPrefs.SetInt(CONFIG_MAX_TEXTURE_SIZE, MaxTextureSize);
            EditorPrefs.SetInt(CONFIG_TEXTURE_FORMAT, (int)TextureFormat);
            EditorPrefs.SetInt(CONFIG_COMPRESSION_QUALITY, CompressionQuality);
            EditorPrefs.SetBool(CONFIG_GENERATE_LAYOUT, GenerateLayoutGroup);
            EditorPrefs.SetBool(CONFIG_AUTO_BUTTON, AutoGenerateButton);
            EditorPrefs.SetString(CONFIG_BUTTON_RULES, ButtonNameRules);
            EditorPrefs.SetFloat(CONFIG_SCALE_RATIO, UserScaleRatio);
            EditorPrefs.SetFloat(CONFIG_OFFSET_X, OffsetX);
            EditorPrefs.SetFloat(CONFIG_OFFSET_Y, OffsetY);
        }

        public void LoadConfig()
        {
            JsonPath = EditorPrefs.GetString(CONFIG_JSON_PATH, "");
            ImageFolderPath = EditorPrefs.GetString(CONFIG_IMAGE_PATH, "");
            PrefabSavePath = EditorPrefs.GetString(CONFIG_PREFAB_PATH, "Assets/UI/Prefabs/");
            FunctionName = EditorPrefs.GetString(CONFIG_FUNCTION_NAME, "");
            CustomPrefabName = EditorPrefs.GetString(CONFIG_CUSTOM_PREFAB_NAME, "");
            DefaultFontPath = EditorPrefs.GetString(CONFIG_DEFAULT_FONT_PATH, "");
            UseCompression = EditorPrefs.GetBool(CONFIG_USE_COMPRESSION, true);
            MaxTextureSize = EditorPrefs.GetInt(CONFIG_MAX_TEXTURE_SIZE, 2048);
            TextureFormat = (TextureImporterFormat)EditorPrefs.GetInt(CONFIG_TEXTURE_FORMAT, (int)TextureImporterFormat.ASTC_6x6);
            CompressionQuality = EditorPrefs.GetInt(CONFIG_COMPRESSION_QUALITY, 50);
            GenerateLayoutGroup = EditorPrefs.GetBool(CONFIG_GENERATE_LAYOUT, false);
            AutoGenerateButton = EditorPrefs.GetBool(CONFIG_AUTO_BUTTON, false);
            ButtonNameRules = EditorPrefs.GetString(CONFIG_BUTTON_RULES, "btn,button");
            UserScaleRatio = EditorPrefs.GetFloat(CONFIG_SCALE_RATIO, 1.0f);
            OffsetX = EditorPrefs.GetFloat(CONFIG_OFFSET_X, 0f);
            OffsetY = EditorPrefs.GetFloat(CONFIG_OFFSET_Y, 0f);
        }
    }
}
