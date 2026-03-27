// using System.ComponentModel;
// using com.IvanMurzak.McpPlugin;
// using com.IvanMurzak.ReflectorNet.Utils;
// using UnityEngine;

// namespace com.IvanMurzak.Unity.MCP.Editor.API
// {
//     public class ToolExecuteKiroCommand
//     {
//         [McpPluginToolType]
//         public class AutoBindUICommand
//         {
//             [McpPluginTool
//             (
//                 "auto-bind-ui",
//                 Title = "Auto Binding UI"
//             )]
//             [Description("auto-bind-ui by kiro.")]
//             public string BindUICommand
//             (
//                 [Description("这里是输入参数.")]
//                 string inputData
//             )
//             {
//                 var rootName = inputData;
//                 AutoBindingUITool.AutoBindingUI(rootName);
//                 return $"[Success] Auto-bind UI completed for root='{rootName}'.";
//                 // do anything in background thread
//                 // return MainThread.Instance.Run(() =>
//                 // {
//                 //     // do something in main thread if needed
//                 //     var rootName = inputData;
//                 //     AutoBindingUITool.AutoBindingUI(rootName);
//                 //     return $"[Success] Auto-bind UI completed for root='{rootName}'.";
//                 // });
//             }
//         }
//     }
// }