using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BreakoutGame.Core
{
    /// <summary>
    /// 游戏状态枚举 / Game state enumeration
    /// </summary>
    public enum GameState
    {
        StartMenu,  // 开始菜单 / Start menu
        Playing,    // 游戏进行中 / Playing
        GameOver,   // 游戏结束（失败）/ Game over (failed)
        Victory     // 游戏胜利 / Victory
    }

    /// <summary>
    /// 游戏管理器单例，管理游戏整体流程和状态
    /// Game manager singleton that manages overall game flow and state
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // 单例实例 / Singleton instance
        public static GameManager Instance { get; private set; }
        
        // 当前游戏状态 / Current game state
        public GameState CurrentState { get; private set; }
        
        // 剩余砖块计数 / Remaining brick count
        private int remainingBricks = 0;
        
        // 当前关卡 / Current level
        private int currentLevel = 1;
        
        // 场景名称常量 / Scene name constants
        private const string SCENE_START_MENU = "StartMenu";
        private const string SCENE_GAME = "GameScene";
        
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
            
            // 订阅场景加载事件 / Subscribe to scene loaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // 订阅游戏事件 / Subscribe to game events
            GameEvents.BrickDestroyed += OnBrickDestroyed;
            GameEvents.BallFell += OnBallFell;
            
            // 初始化状态 / Initialize state
            CurrentState = GameState.StartMenu;
            
            Debug.Log("[GameManager] Initialized");
        }
        
        private void Start()
        {
            // 如果直接在GameScene场景启动（用于测试），自动开始游戏
            // If starting directly in GameScene (for testing), auto-start the game
            if (SceneManager.GetActiveScene().name == SCENE_GAME && CurrentState == GameState.StartMenu)
            {
                Debug.Log("[GameManager] GameScene detected, auto-starting game");
                StartGame();
            }
        }
        
        private void OnDestroy()
        {
            // 取消订阅事件 / Unsubscribe from events
            if (Instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                GameEvents.BrickDestroyed -= OnBrickDestroyed;
                GameEvents.BallFell -= OnBallFell;
            }
        }
        
        /// <summary>
        /// 场景加载完成回调 / Scene loaded callback
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"[GameManager] Scene loaded: {scene.name}, Current brick count: {remainingBricks}");
            
            // 如果加载的是GameScene，延迟一帧后开始游戏（等待所有Start()执行完毕）
            // If GameScene is loaded, start game after one frame (wait for all Start() to complete)
            if (scene.name == SCENE_GAME)
            {
                Debug.Log("[GameManager] GameScene loaded, will start game after one frame...");
                StartCoroutine(StartGameNextFrame());
            }
        }
        
        /// <summary>
        /// 延迟一帧后开始游戏 / Start game after one frame
        /// </summary>
        private System.Collections.IEnumerator StartGameNextFrame()
        {
            yield return null; // 等待一帧 / Wait one frame
            Debug.Log($"[GameManager] Starting game after frame delay. Brick count: {remainingBricks}");
            StartGame();
        }
        
        #region 场景管理 / Scene Management
        
        /// <summary>
        /// 加载开始菜单场景 / Load start menu scene
        /// </summary>
        public void LoadStartMenu()
        {
            try
            {
                Debug.Log("[GameManager] Loading StartMenu scene");
                CurrentState = GameState.StartMenu;
                SceneManager.LoadScene(SCENE_START_MENU);
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameManager] Error loading StartMenu scene: {e.Message}\n{e.StackTrace}");
            }
        }
        
        /// <summary>
        /// 加载游戏场景 / Load game scene
        /// </summary>
        public void LoadGameScene()
        {
            try
            {
                Debug.Log("[GameManager] Loading GameScene");
                SceneManager.LoadScene(SCENE_GAME);
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameManager] Error loading GameScene: {e.Message}\n{e.StackTrace}");
                LoadStartMenu(); // 回退到主菜单 / Fallback to main menu
            }
        }
        
        /// <summary>
        /// 重新开始游戏 / Restart game
        /// </summary>
        public void RestartGame()
        {
            Debug.Log("[GameManager] Restarting game");
            
            // 重置到第一关 / Reset to level 1
            currentLevel = 1;
            
            // 重置砖块计数 / Reset brick count
            remainingBricks = 0;
            
            // 重置分数 / Reset score
            if (ScoreSystem.Instance != null)
            {
                ScoreSystem.Instance.ResetScore();
            }
            
            // 重新加载游戏场景 / Reload game scene
            LoadGameScene();
        }
        
        /// <summary>
        /// 加载下一关 / Load next level
        /// </summary>
        public void LoadNextLevel()
        {
            currentLevel++;
            Debug.Log($"[GameManager] Loading level {currentLevel}");
            
            // 重置砖块计数 / Reset brick count
            remainingBricks = 0;
            
            // 不重置分数，保留累计分数 / Don't reset score, keep accumulated score
            
            // 重新加载游戏场景 / Reload game scene
            LoadGameScene();
        }
        
        /// <summary>
        /// 获取当前关卡 / Get current level
        /// </summary>
        public int GetCurrentLevel()
        {
            return currentLevel;
        }
        
        /// <summary>
        /// 检查是否还有下一关 / Check if there is a next level
        /// </summary>
        public bool HasNextLevel()
        {
            var config = Resources.Load<Data.GameConfig>("GameConfig");
            if (config != null)
            {
                return currentLevel < config.totalLevels;
            }
            return false;
        }
        
        #endregion
        
        #region 游戏流程控制 / Game Flow Control
        
        /// <summary>
        /// 开始游戏 / Start game
        /// </summary>
        public void StartGame()
        {
            Debug.Log($"[GameManager] Starting game. Current brick count: {remainingBricks}");
            
            CurrentState = GameState.Playing;
            // 不重置remainingBricks，因为砖块已经在场景加载时注册了
            // Don't reset remainingBricks as bricks are already registered during scene load
            
            // 重置分数 / Reset score
            if (ScoreSystem.Instance != null)
            {
                ScoreSystem.Instance.ResetScore();
            }
            
            // 触发游戏开始事件 / Trigger game started event
            GameEvents.TriggerGameStarted();
            
            Debug.Log($"[GameManager] Game started with {remainingBricks} bricks");
        }
        
        /// <summary>
        /// 结束游戏 / End game
        /// </summary>
        /// <param name="isVictory">是否胜利 / Whether victory</param>
        public void EndGame(bool isVictory)
        {
            if (CurrentState != GameState.Playing)
            {
                Debug.LogWarning("[GameManager] EndGame called but game is not in Playing state");
                return;
            }
            
            CurrentState = isVictory ? GameState.Victory : GameState.GameOver;
            
            Debug.Log($"[GameManager] Game ended - {(isVictory ? "Victory" : "Game Over")}");
            
            // 记录游戏进度 / Record game progress
            if (Data.GameProgress.Instance != null && ScoreSystem.Instance != null)
            {
                int finalScore = ScoreSystem.Instance.CurrentScore;
                Data.GameProgress.Instance.RecordGameCompletion(isVictory, finalScore, currentLevel);
            }
            
            // 触发游戏结束事件 / Trigger game ended event
            GameEvents.TriggerGameEnded(isVictory);
        }
        
        #endregion
        
        #region 砖块计数管理 / Brick Count Management
        
        /// <summary>
        /// 注册砖块 / Register brick
        /// </summary>
        public void RegisterBrick()
        {
            remainingBricks++;
            Debug.Log($"[GameManager] Brick registered. Remaining: {remainingBricks}");
        }
        
        /// <summary>
        /// 取消注册砖块 / Unregister brick
        /// </summary>
        public void UnregisterBrick()
        {
            remainingBricks--;
            Debug.Log($"[GameManager] Brick unregistered. Remaining: {remainingBricks}");
            
            // 检查胜利条件 / Check victory condition
            if (remainingBricks <= 0 && CurrentState == GameState.Playing)
            {
                Debug.Log("[GameManager] All bricks destroyed - Victory!");
                EndGame(true);
            }
        }
        
        /// <summary>
        /// 获取剩余砖块数量 / Get remaining brick count
        /// </summary>
        /// <returns>剩余砖块数量 / Remaining brick count</returns>
        public int GetRemainingBricks()
        {
            return remainingBricks;
        }
        
        #endregion
        
        #region 事件处理 / Event Handlers
        
        /// <summary>
        /// 处理砖块销毁事件 / Handle brick destroyed event
        /// </summary>
        private void OnBrickDestroyed()
        {
            UnregisterBrick();
        }
        
        /// <summary>
        /// 处理球掉落事件 / Handle ball fell event
        /// </summary>
        private void OnBallFell()
        {
            if (CurrentState == GameState.Playing)
            {
                Debug.Log("[GameManager] Ball fell - Game Over");
                EndGame(false);
            }
        }
        
        #endregion
    }
}
