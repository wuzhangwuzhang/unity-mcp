using UnityEngine;

namespace BreakoutGame.Core
{
    /// <summary>
    /// 得分系统单例，管理玩家分数
    /// Score system singleton that manages player score
    /// </summary>
    public class ScoreSystem : MonoBehaviour
    {
        // 单例实例 / Singleton instance
        public static ScoreSystem Instance { get; private set; }
        
        // 当前分数 / Current score
        public int CurrentScore { get; private set; }
        
        // 砖块销毁分数 / Score for destroying a brick
        [SerializeField] private int brickDestroyScore = 10;
        public int BrickDestroyScore => brickDestroyScore;
        
        private void Awake()
        {
            // 单例模式实现 / Singleton pattern implementation
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 订阅砖块销毁事件 / Subscribe to brick destroyed event
            GameEvents.BrickDestroyed += OnBrickDestroyed;
        }
        
        private void OnDestroy()
        {
            // 取消订阅事件 / Unsubscribe from events
            if (Instance == this)
            {
                GameEvents.BrickDestroyed -= OnBrickDestroyed;
            }
        }
        
        /// <summary>
        /// 增加分数 / Add score
        /// </summary>
        /// <param name="points">要增加的分数 / Points to add</param>
        public void AddScore(int points)
        {
            if (points < 0)
            {
                Debug.LogWarning($"[ScoreSystem] Attempted to add negative score: {points}");
                return;
            }
            
            CurrentScore += points;
            
            // 触发分数变化事件 / Trigger score changed event
            GameEvents.TriggerScoreChanged(CurrentScore);
            
            Debug.Log($"[ScoreSystem] Score updated: {CurrentScore} (+{points})");
        }
        
        /// <summary>
        /// 重置分数为0 / Reset score to 0
        /// </summary>
        public void ResetScore()
        {
            CurrentScore = 0;
            
            // 触发分数变化事件 / Trigger score changed event
            GameEvents.TriggerScoreChanged(CurrentScore);
            
            Debug.Log("[ScoreSystem] Score reset to 0");
        }
        
        /// <summary>
        /// 处理砖块销毁事件 / Handle brick destroyed event
        /// </summary>
        private void OnBrickDestroyed()
        {
            AddScore(brickDestroyScore);
        }
    }
}
