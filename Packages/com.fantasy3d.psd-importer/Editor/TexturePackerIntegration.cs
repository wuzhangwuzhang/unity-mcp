using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace PSDImporter
{
    public class TexturePackerConfig
    {
        public int MaxSize { get; set; }
        public int Padding { get; set; }
        public bool AllowRotation { get; set; }
        public TexturePackerTrimMode TrimMode { get; set; }
        public bool FastAndLooseMethod { get; set; }
        public Dictionary<string, Vector4> SlicedSprites { get; set; } = new Dictionary<string, Vector4>();
    }

    public enum TexturePackerTrimMode
    {
        None,
        Trim,
        CropKeepPos
    }

    public static class TexturePacker
    {
        public static (bool success, List<string> outputPaths, Dictionary<string, SpriteMetaData> spriteDataMap) Pack(
            string[] spritePaths,
            string outputPath,
            TexturePackerConfig config)
        {
            var outputPaths = new List<string>();
            var spriteDataMap = new Dictionary<string, SpriteMetaData>();

            try
            {
                Debug.Log($"开始TexturePacker打包，输入文件数量: {spritePaths.Length}");

                string texturePackerExePath = GetTexturePackerPath();
                string arguments = BuildTexturePackerArguments(spritePaths, outputPath, config);

                Process process = new Process();
                process.StartInfo.FileName = texturePackerExePath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (!string.IsNullOrEmpty(error))
                    Debug.LogError($"TexturePacker错误:\n{error}");

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Debug.LogError($"TexturePacker执行失败，退出码: {process.ExitCode}");
                    return (false, outputPaths, spriteDataMap);
                }

                ParseTexturePackerOutput(output, outputPath, config, outputPaths, spriteDataMap);
                return (true, outputPaths, spriteDataMap);
            }
            catch (Exception e)
            {
                Debug.LogError($"TexturePacker打包异常: {e.Message}\n{e.StackTrace}");
                return (false, outputPaths, spriteDataMap);
            }
        }

        private static string GetTexturePackerPath()
        {
            // 1. 环境变量
            string texturePackerPath = Environment.GetEnvironmentVariable("TEXTURE_PACKER_PATH");

            if (string.IsNullOrEmpty(texturePackerPath))
            {
                // 2. PATH中查找
                string pathVariable = Environment.GetEnvironmentVariable("PATH");
                if (!string.IsNullOrEmpty(pathVariable))
                {
                    string[] paths = pathVariable.Split(Path.PathSeparator);
                    foreach (string path in paths)
                    {
                        string possiblePath = Path.Combine(path, "TexturePacker.exe");
                        if (File.Exists(possiblePath))
                            return possiblePath;
                    }
                }

                // 3. 常见安装路径
                string[] commonPaths = new string[]
                {
                    @"C:\Program Files\CodeAndWeb\TexturePacker\bin\TexturePacker.exe",
                    @"C:\Program Files (x86)\CodeAndWeb\TexturePacker\bin\TexturePacker.exe",
                    "/usr/local/bin/TexturePacker",
                    "/Applications/TexturePacker.app/Contents/MacOS/TexturePacker"
                };

                foreach (string p in commonPaths)
                {
                    if (File.Exists(p))
                        return p;
                }

                throw new FileNotFoundException(
                    "找不到TexturePacker。请确保已安装并配置:\n" +
                    "1. 设置TEXTURE_PACKER_PATH环境变量\n" +
                    "2. 或将TexturePacker添加到PATH\n" +
                    "3. 或安装到默认路径"
                );
            }

            if (!File.Exists(texturePackerPath))
            {
                throw new FileNotFoundException(
                    $"TEXTURE_PACKER_PATH路径无效: {texturePackerPath}"
                );
            }

            return texturePackerPath;
        }

        private static string BuildTexturePackerArguments(string[] spritePaths, string outputPath, TexturePackerConfig config)
        {
            StringBuilder args = new StringBuilder();

            args.Append($"--texture-format png ");
            args.Append($"--format unity-texture2d ");
            args.Append($"--size-constraints POT ");
            args.Append($"--max-size {config.MaxSize} ");
            args.Append($"--padding {config.Padding} ");

            string normalizedOutputPath = outputPath.Replace('\\', '/');
            args.Append($"--sheet \"{normalizedOutputPath}.png\" ");
            args.Append($"--data \"{normalizedOutputPath}.tpsheet\" ");

            foreach (string spritePath in spritePaths)
            {
                string normalizedSpritePath = spritePath.Replace('\\', '/');
                args.Append($"\"{normalizedSpritePath}\" ");
            }

            return args.ToString();
        }

        private static void ParseTexturePackerOutput(
            string output,
            string outputPath,
            TexturePackerConfig config,
            List<string> outputPaths,
            Dictionary<string, SpriteMetaData> spriteDataMap)
        {
            try
            {
                string pngPath = outputPath + ".png";
                string tpsheetPath = outputPath + ".tpsheet";

                if (File.Exists(pngPath))
                {
                    outputPaths.Add(pngPath);
                    string assetPath = PSDImporterUtils.GetAssetPath(pngPath);

                    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

                    if (File.Exists(tpsheetPath))
                    {
                        string[] lines = File.ReadAllLines(tpsheetPath);

                        foreach (string line in lines)
                        {
                            if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                                continue;

                            string[] parts = line.Split(';');
                            if (parts.Length >= 7)
                            {
                                string spriteName = Path.GetFileNameWithoutExtension(parts[0]);
                                float x = float.Parse(parts[1]);
                                float y = float.Parse(parts[2]);
                                float width = float.Parse(parts[3]);
                                float height = float.Parse(parts[4]);

                                SpriteMetaData metaData = new SpriteMetaData
                                {
                                    name = spriteName,
                                    rect = new Rect(x, y, width, height),
                                    pivot = new Vector2(0.5f, 0.5f),
                                    alignment = (int)SpriteAlignment.Center
                                };

                                if (config.SlicedSprites.ContainsKey(spriteName))
                                {
                                    metaData.border = config.SlicedSprites[spriteName];
                                }

                                spriteDataMap[spriteName] = metaData;
                            }
                        }

                        // 设置图集导入器
                        EditorApplication.delayCall += () =>
                        {
                            TextureImporter atlasImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                            if (atlasImporter != null)
                            {
                                atlasImporter.textureType = TextureImporterType.Sprite;
                                atlasImporter.spriteImportMode = SpriteImportMode.Multiple;
                                atlasImporter.mipmapEnabled = false;
                                atlasImporter.isReadable = true;

                                SpriteMetaData[] spritesheet = new SpriteMetaData[spriteDataMap.Count];
                                int index = 0;
                                foreach (var kvp in spriteDataMap)
                                {
                                    spritesheet[index++] = kvp.Value;
                                }

                                atlasImporter.spritesheet = spritesheet;
                                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                            }
                        };
                    }
                }
                else
                {
                    Debug.LogError($"找不到生成的图集文件: {pngPath}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"解析TexturePacker输出出错: {e.Message}\n{e.StackTrace}");
            }
        }

        public static bool UpdateAtlas(
            string[] spritePaths,
            string existingAtlasPath,
            TexturePackerConfig config)
        {
            try
            {
                Debug.Log($"开始更新图集: {existingAtlasPath}");

                string tempDir = Path.Combine(Path.GetTempPath(), "TempAtlas_" + DateTime.Now.Ticks);
                Directory.CreateDirectory(tempDir);

                string tempOutputPath = Path.Combine(tempDir, "temp_atlas");
                var (success, outputPaths, _) = Pack(spritePaths, tempOutputPath, config);

                if (!success)
                {
                    Debug.LogError("生成临时图集失败");
                    return false;
                }

                // 检查现有图集的精灵
                var existingSprites = new HashSet<string>();
                var existingAtlasImporter = AssetImporter.GetAtPath(existingAtlasPath) as TextureImporter;
                if (existingAtlasImporter != null)
                {
                    foreach (var spriteMetaData in existingAtlasImporter.spritesheet)
                        existingSprites.Add(spriteMetaData.name);
                }

                // 验证新图集
                string tpsheetPath = outputPaths[0].Replace(".png", ".tpsheet");
                if (File.Exists(tpsheetPath))
                {
                    var newSprites = new HashSet<string>();
                    foreach (string line in File.ReadAllLines(tpsheetPath))
                    {
                        if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;
                        string spriteName = Path.GetFileNameWithoutExtension(line.Split(';')[0]);
                        newSprites.Add(spriteName);
                    }

                    var missingSpriteNames = existingSprites.Where(name => !newSprites.Contains(name)).ToList();
                    if (missingSpriteNames.Count > 0)
                    {
                        string missingNames = string.Join("\n", missingSpriteNames);
                        Debug.LogError($"新图集缺少以下Sprite:\n{missingNames}");
                        EditorUtility.DisplayDialog("错误",
                            $"新图集缺少 {missingSpriteNames.Count} 个原有Sprite，无法更新。",
                            "确定");
                        return false;
                    }
                }

                AssetDatabase.StartAssetEditing();

                try
                {
                    string newAtlasPath = outputPaths[0];

                    TextureImporter existingImporter = AssetImporter.GetAtPath(existingAtlasPath) as TextureImporter;
                    SpriteMetaData[] existingSpritesheet = existingImporter?.spritesheet;

                    // 更新tpsheet中的文件名引用
                    string tpsheetContent = File.ReadAllText(tpsheetPath);
                    string tempAtlasName = Path.GetFileName(tempOutputPath + ".png");
                    string targetAtlasName = Path.GetFileName(existingAtlasPath);
                    tpsheetContent = tpsheetContent.Replace(tempAtlasName, targetAtlasName);
                    File.WriteAllText(tpsheetPath, tpsheetContent);

                    File.Copy(newAtlasPath, existingAtlasPath, true);
                    File.Copy(tpsheetPath, existingAtlasPath.Replace(".png", ".tpsheet"), true);

                    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

                    TextureImporter atlasImporter = AssetImporter.GetAtPath(existingAtlasPath) as TextureImporter;
                    if (atlasImporter != null)
                    {
                        if (existingImporter != null)
                            EditorUtility.CopySerialized(existingImporter, atlasImporter);

                        atlasImporter.textureType = TextureImporterType.Sprite;
                        atlasImporter.spriteImportMode = SpriteImportMode.Multiple;
                        atlasImporter.mipmapEnabled = false;
                        atlasImporter.isReadable = true;

                        if (existingSpritesheet != null)
                        {
                            var newSpritesheet = new List<SpriteMetaData>();
                            foreach (var oldSprite in existingSpritesheet)
                            {
                                foreach (string line in File.ReadAllLines(tpsheetPath))
                                {
                                    if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;
                                    string[] parts = line.Split(';');
                                    string spriteName = Path.GetFileNameWithoutExtension(parts[0]);

                                    if (spriteName == oldSprite.name)
                                    {
                                        var newSprite = oldSprite;
                                        newSprite.rect = new Rect(
                                            float.Parse(parts[1]),
                                            float.Parse(parts[2]),
                                            float.Parse(parts[3]),
                                            float.Parse(parts[4])
                                        );

                                        if (config.SlicedSprites.ContainsKey(spriteName))
                                            newSprite.border = config.SlicedSprites[spriteName];
                                        else
                                            newSprite.border = Vector4.zero;

                                        newSpritesheet.Add(newSprite);
                                        break;
                                    }
                                }
                            }

                            atlasImporter.spritesheet = newSpritesheet.ToArray();
                        }

                        EditorUtility.SetDirty(atlasImporter);
                        atlasImporter.SaveAndReimport();
                    }
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                }

                Debug.Log("图集更新完成");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"更新图集出错: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }
    }
}
