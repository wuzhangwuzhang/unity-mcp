using UnityEngine;
using UnityEngine.UI;

namespace BreakoutGame.UI
{
    /// <summary>
    /// 开始菜单UI控制器 / Start menu UI controller
    /// </summary>
    public class StartMenuUI : MonoBehaviour
    {
        [Header("UI References / UI引用")]
        [SerializeField] private Button startButton;
        [SerializeField] private Text titleText;
        [SerializeField] private Text instructionsText;
        [SerializeField] private Text progressText;
        
        private void Start()
        {
            // 验证UI组件引用 / Validate UI component references
            if (startButton == null)
            {
                Debug.LogError("[StartMenuUI] Start button reference is missing!");
                return;
            }
            
            if (titleText == null)
            {
                Debug.LogWarning("[StartMenuUI] Title text reference is missing!");
            }
            
            if (instructionsText == null)
            {
                Debug.LogWarning("[StartMenuUI] Instructions text reference is missing!");
            }
            
            // 绑定按钮点击事件 / Bind button click event
            startButton.onClick.AddListener(OnStartButtonClicked);
            
            // 设置默认文本（如果未在编辑器中设置）/ Set default text (if not set in editor)
            if (titleText != null && string.IsNullOrEmpty(titleText.text))
            {
                titleText.text = "弹球打砖块\nBreakout Game";
            }
            
            if (instructionsText != null && string.IsNullOrEmpty(instructionsText.text))
            {
                instructionsText.text = "操作说明 / Controls:\n" +
                                       "A / ← : 向左移动挡板 / Move paddle left\n" +
                                       "D / → : 向右移动挡板 / Move paddle right\n\n" +
                                       "目标 / Goal:\n" +
                                       "击碎所有砖块获得胜利！\n" +
                                       "Destroy all bricks to win!";
            }
            
            // 显示游戏进度信息 / Display game progress info
            UpdateProgressDisplay();
            
            Debug.Log("[StartMenuUI] Initialized");
        }
        
        /// <summary>
        /// 更新进度显示 / Update progress display
        /// </summary>
        private void UpdateProgressDisplay()
        {
            if (progressText != null && Data.GameProgress.Instance != null)
            {
                int highestLevel = Data.GameProgress.Instance.GetHighestLevelUnlocked();
                int highScore = Data.GameProgress.Instance.GetHighScore();
                int totalGames = Data.GameProgress.Instance.GetTotalGamesPlayed();
                int totalVictories = Data.GameProgress.Instance.GetTotalVictories();
                
                progressText.text = $"Progress / 进度:\n" +
                                   $"Highest Level: {highestLevel}\n" +
                                   $"High Score: {highScore}\n" +
                                   $"Games Played: {totalGames}\n" +
                                   $"Victories: {totalVictories}";
            }
            
            Debug.Log("[StartMenuUI] Initialized");
        }
        
        private void OnDestroy()
        {
            // 取消绑定按钮事件 / Unbind button event
            if (startButton != null)
            {
                startButton.onClick.RemoveListener(OnStartButtonClicked);
            }
        }
        
        /// <summary>
        /// 处理开始按钮点击事件 / Handle start button click event
        /// </summary>
        private void OnStartButtonClicked()
        {
            Debug.Log("[StartMenuUI] Start button clicked");
            
            // 检查GameManager是否存在 / Check if GameManager exists
            if (Core.GameManager.Instance == null)
            {
                Debug.LogError("[StartMenuUI] GameManager instance not found!");
                return;
            }
            
            // 加载游戏场景 / Load game scene
            Core.GameManager.Instance.LoadGameScene();
        }
    }
}
