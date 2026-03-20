using UnityEngine;
using UnityEngine.UI;

namespace BreakoutGame.UI
{
    /// <summary>
    /// 结束界面UI，显示游戏结束信息和操作按钮
    /// End screen UI that displays game over information and action buttons
    /// </summary>
    public class EndScreenUI : MonoBehaviour
    {
        [Header("UI References / UI引用")]
        [SerializeField] private GameObject endScreenPanel;
        [SerializeField] private Text messageText;
        [SerializeField] private Text finalScoreText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button mainMenuButton;
        
        private void Start()
        {
            // 验证UI组件引用 / Validate UI component references
            ValidateReferences();
            
            // 订阅游戏结束事件 / Subscribe to game ended event
            Core.GameEvents.GameEnded += OnGameEnded;
            
            // 初始隐藏结束界面 / Initially hide end screen
            if (endScreenPanel != null)
            {
                endScreenPanel.SetActive(false);
            }
            
            // 配置按钮事件 / Configure button events
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartButtonClicked);
            }
            
            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            }
            
            Debug.Log("[EndScreenUI] Initialized");
        }
        
        private void OnDestroy()
        {
            // 取消订阅事件 / Unsubscribe from events
            Core.GameEvents.GameEnded -= OnGameEnded;
            
            // 移除按钮监听器 / Remove button listeners
            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            }
            
            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClicked);
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
            }
        }
        
        /// <summary>
        /// 验证UI组件引用 / Validate UI component references
        /// </summary>
        private void ValidateReferences()
        {
            if (endScreenPanel == null)
            {
                Debug.LogError("[EndScreenUI] EndScreenPanel reference is missing!");
            }
            
            if (messageText == null)
            {
                Debug.LogError("[EndScreenUI] MessageText reference is missing!");
            }
            
            if (finalScoreText == null)
            {
                Debug.LogError("[EndScreenUI] FinalScoreText reference is missing!");
            }
            
            if (restartButton == null)
            {
                Debug.LogError("[EndScreenUI] RestartButton reference is missing!");
            }
            
            if (mainMenuButton == null)
            {
                Debug.LogError("[EndScreenUI] MainMenuButton reference is missing!");
            }
        }
        
        /// <summary>
        /// 游戏结束事件处理 / Game ended event handler
        /// </summary>
        private void OnGameEnded(bool isVictory)
        {
            Show(isVictory);
        }
        
        /// <summary>
        /// 显示结束界面 / Show end screen
        /// </summary>
        public void Show(bool isVictory)
        {
            if (endScreenPanel == null)
            {
                Debug.LogError("[EndScreenUI] Cannot show end screen - EndScreenPanel is null!");
                return;
            }
            
            // 显示面板 / Show panel
            endScreenPanel.SetActive(true);
            
            // 更新消息文本 / Update message text
            if (messageText != null)
            {
                if (isVictory)
                {
                    // 检查是否还有下一关 / Check if there is a next level
                    bool hasNextLevel = Core.GameManager.Instance != null && 
                                       Core.GameManager.Instance.HasNextLevel();
                    
                    if (hasNextLevel)
                    {
                        int currentLevel = Core.GameManager.Instance.GetCurrentLevel();
                        messageText.text = $"Level {currentLevel} Complete!";
                    }
                    else
                    {
                        messageText.text = "All Levels Complete!";
                    }
                }
                else
                {
                    messageText.text = "Game Over";
                }
            }
            
            // 更新最终分数 / Update final score
            if (finalScoreText != null)
            {
                int finalScore = Core.ScoreSystem.Instance != null ? 
                    Core.ScoreSystem.Instance.CurrentScore : 0;
                
                int highScore = 0;
                if (Data.GameProgress.Instance != null)
                {
                    highScore = Data.GameProgress.Instance.GetHighScore();
                }
                
                finalScoreText.text = $"Score: {finalScore}\nHigh Score: {highScore}";
            }
            
            // 根据胜利状态显示/隐藏下一关按钮 / Show/hide next level button based on victory
            if (nextLevelButton != null)
            {
                bool hasNextLevel = isVictory && 
                                   Core.GameManager.Instance != null && 
                                   Core.GameManager.Instance.HasNextLevel();
                nextLevelButton.gameObject.SetActive(hasNextLevel);
            }
            
            Debug.Log($"[EndScreenUI] End screen shown - Victory: {isVictory}");
        }
        
        /// <summary>
        /// 重新开始按钮点击处理 / Restart button click handler
        /// </summary>
        private void OnRestartButtonClicked()
        {
            Debug.Log("[EndScreenUI] Restart button clicked");
            
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.RestartGame();
            }
            else
            {
                Debug.LogError("[EndScreenUI] GameManager instance not found!");
            }
        }
        
        /// <summary>
        /// 下一关按钮点击处理 / Next level button click handler
        /// </summary>
        private void OnNextLevelButtonClicked()
        {
            Debug.Log("[EndScreenUI] Next level button clicked");
            
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.LoadNextLevel();
            }
            else
            {
                Debug.LogError("[EndScreenUI] GameManager instance not found!");
            }
        }
        
        /// <summary>
        /// 返回主菜单按钮点击处理 / Main menu button click handler
        /// </summary>
        private void OnMainMenuButtonClicked()
        {
            Debug.Log("[EndScreenUI] Main menu button clicked");
            
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.LoadStartMenu();
            }
            else
            {
                Debug.LogError("[EndScreenUI] GameManager instance not found!");
            }
        }
        
        #region Public Accessors for Testing / 测试用公共访问器
        
        public GameObject EndScreenPanel => endScreenPanel;
        public Text MessageText => messageText;
        public Text FinalScoreText => finalScoreText;
        
        #endregion
    }
}
