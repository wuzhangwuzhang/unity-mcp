using UnityEngine;
using UnityEngine.UI;

namespace BreakoutGame.UI
{
    /// <summary>
    /// 游戏界面HUD，显示分数和游戏状态
    /// Game UI HUD that displays score and game status
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        [Header("UI References / UI引用")]
        [SerializeField] private Text scoreText;
        
        private void Start()
        {
            // 验证UI组件引用 / Validate UI component references
            if (scoreText == null)
            {
                Debug.LogError("[GameUI] ScoreText reference is missing! Please assign it in the Inspector.");
                
                // 尝试自动查找 / Try to find automatically
                scoreText = GameObject.Find("ScoreText")?.GetComponent<Text>();
                
                if (scoreText == null)
                {
                    Debug.LogError("[GameUI] Could not find ScoreText automatically!");
                    return;
                }
                else
                {
                    Debug.LogWarning("[GameUI] ScoreText found automatically, but please assign it in Inspector for better performance");
                }
            }
            
            // 订阅分数变化事件 / Subscribe to score changed event
            Core.GameEvents.ScoreChanged += OnScoreChanged;
            
            // 初始化分数显示 / Initialize score display
            UpdateScore(0);
            
            Debug.Log("[GameUI] Initialized");
        }
        
        private void OnDestroy()
        {
            // 取消订阅事件 / Unsubscribe from events
            Core.GameEvents.ScoreChanged -= OnScoreChanged;
        }
        
        /// <summary>
        /// 分数变化事件处理 / Score changed event handler
        /// </summary>
        private void OnScoreChanged(int newScore)
        {
            UpdateScore(newScore);
        }
        
        /// <summary>
        /// 更新分数显示 / Update score display
        /// </summary>
        public void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
                Debug.Log($"[GameUI] Score updated to: {score}");
            }
            else
            {
                Debug.LogWarning("[GameUI] Cannot update score - ScoreText is null");
            }
        }
        
        #region Public Accessors for Testing / 测试用公共访问器
        
        public Text ScoreText => scoreText;
        
        #endregion
    }
}
