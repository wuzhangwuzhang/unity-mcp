using System;
using UnityEngine;

namespace BreakoutGame.Core
{
    /// <summary>
    /// 静态事件系统，用于游戏中各组件间的解耦通信
    /// Static event system for decoupled communication between game components
    /// </summary>
    public static class GameEvents
    {
        // 游戏流程事件 / Game flow events
        public static event Action GameStarted;
        public static event Action<bool> GameEnded; // 参数: isVictory / Parameter: isVictory
        
        // 游戏对象事件 / Game object events
        public static event Action BrickDestroyed;
        public static event Action BallFell;
        public static event Action<Vector2> BallPaddleCollision; // 参数: 碰撞位置 / Parameter: collision position
        
        // 分数事件 / Score events
        public static event Action<int> ScoreChanged; // 参数: 新分数 / Parameter: new score
        
        // 事件触发方法，带异常处理 / Event trigger methods with exception handling
        
        public static void TriggerGameStarted()
        {
            SafeInvoke(GameStarted, "GameStarted");
        }
        
        public static void TriggerGameEnded(bool isVictory)
        {
            SafeInvoke(() => GameEnded?.Invoke(isVictory), "GameEnded");
        }
        
        public static void TriggerBrickDestroyed()
        {
            SafeInvoke(BrickDestroyed, "BrickDestroyed");
        }
        
        public static void TriggerBallFell()
        {
            SafeInvoke(BallFell, "BallFell");
        }
        
        public static void TriggerBallPaddleCollision(Vector2 collisionPosition)
        {
            SafeInvoke(() => BallPaddleCollision?.Invoke(collisionPosition), "BallPaddleCollision");
        }
        
        public static void TriggerScoreChanged(int newScore)
        {
            SafeInvoke(() => ScoreChanged?.Invoke(newScore), "ScoreChanged");
        }
        
        // 安全调用包装器，捕获订阅者异常 / Safe invoke wrapper to catch subscriber exceptions
        private static void SafeInvoke(Action action, string eventName)
        {
            if (action == null) return;
            
            foreach (var handler in action.GetInvocationList())
            {
                try
                {
                    handler.DynamicInvoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[GameEvents] Error in {eventName} event handler: {e.Message}\n{e.StackTrace}");
                }
            }
        }
    }
}
