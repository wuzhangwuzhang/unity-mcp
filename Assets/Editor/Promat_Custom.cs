using System.ComponentModel;
using com.IvanMurzak.McpPlugin;
using com.IvanMurzak.McpPlugin.Common.Model;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    public class Promat_Custom
    {
        [McpPluginPromptType]
        public static class Prompt_ScriptingCode
        {
            [McpPluginPrompt(Name = "add-event-system", Role = Role.User)]
            [Description("Implement UnityEvent-based communication system between GameObjects.")]
            public static  string AddEventSystem()
            {
                return "Create event system using UnityEvents, UnityActions, or custom event delegates for decoupled communication between game systems and components.";
            }
        }
    }
}