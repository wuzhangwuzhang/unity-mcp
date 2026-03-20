using UnityEngine;

namespace BreakoutGame.Data
{
    /// <summary>
    /// 游戏进度数据，用于持久化存储
    /// Game progress data for persistent storage
    /// </summary>
    [System.Serializable]
    public class GameProgressData
    {
        public int highestLevelUnlocked = 1;  // 最高解锁关卡 / Highest unlocked level
        public int highestScore = 0;          // 最高分数 / Highest score
        public int totalGamesPlayed = 0;      // 总游戏次数 / Total games played
        public int totalVictories = 0;        // 总胜利次数 / Total victories
        
        // 每关的最高分 / Highest score per level
        public int[] levelHighScores = new int[10]; // 支持最多10关 / Support up to 10 levels
    }
    
    /// <summary>
    /// 游戏进度管理器，处理数据的保存和加载
    /// Game progress manager that handles saving and loading data
    /// </summary>
    public class GameProgress : MonoBehaviour
    {
        // 单例实例 / Singleton instance
        public static GameProgress Instance { get; private set; }
        
        // 当前进度数据 / Current progress data
        private GameProgressData progressData;
        
        // 存储键名 / Storage key name
        private const string SAVE_KEY = "BreakoutGameProgress";
        
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
            
            // 加载进度数据 / Load progress data
            LoadProgress();
            
            Debug.Log("[GameProgress] Initialized");
        }
        
        /// <summary>
        /// 加载进度数据 / Load progress data
        /// </summary>
        public void LoadProgress()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                try
                {
                    string json = PlayerPrefs.GetString(SAVE_KEY);
                    progressData = JsonUtility.FromJson<GameProgressData>(json);
                    Debug.Log($"[GameProgress] Loaded - Highest Level: {progressData.highestLevelUnlocked}, High Score: {progressData.highestScore}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[GameProgress] Failed to load progress: {e.Message}");
                    progressData = new GameProgressData();
                }
            }
            else
            {
                Debug.Log("[GameProgress] No saved data found, creating new progress");
                progressData = new GameProgressData();
            }
        }
        
        /// <summary>
        /// 保存进度数据 / Save progress data
        /// </summary>
        public void SaveProgress()
        {
            try
            {
                string json = JsonUtility.ToJson(progressData);
                PlayerPrefs.SetString(SAVE_KEY, json);
                PlayerPrefs.Save();
                Debug.Log("[GameProgress] Progress saved successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[GameProgress] Failed to save progress: {e.Message}");
            }
        }
        
        /// <summary>
        /// 解锁关卡 / Unlock level
        /// </summary>
        public void UnlockLevel(int level)
        {
            if (level > progressData.highestLevelUnlocked)
            {
                progressData.highestLevelUnlocked = level;
                SaveProgress();
                Debug.Log($"[GameProgress] Level {level} unlocked!");
            }
        }
        
        /// <summary>
        /// 更新最高分 / Update high score
        /// </summary>
        public void UpdateHighScore(int score)
        {
            if (score > progressData.highestScore)
            {
                progressData.highestScore = score;
                SaveProgress();
                Debug.Log($"[GameProgress] New high score: {score}!");
            }
        }
        
        /// <summary>
        /// 更新关卡最高分 / Update level high score
        /// </summary>
        public void UpdateLevelHighScore(int level, int score)
        {
            int levelIndex = level - 1;
            if (levelIndex >= 0 && levelIndex < progressData.levelHighScores.Length)
            {
                if (score > progressData.levelHighScores[levelIndex])
                {
                    progressData.levelHighScores[levelIndex] = score;
                    SaveProgress();
                    Debug.Log($"[GameProgress] New high score for level {level}: {score}!");
                }
            }
        }
        
        /// <summary>
        /// 记录游戏完成 / Record game completion
        /// </summary>
        public void RecordGameCompletion(bool isVictory, int finalScore, int level)
        {
            progressData.totalGamesPlayed++;
            
            if (isVictory)
            {
                progressData.totalVictories++;
                
                // 解锁下一关 / Unlock next level
                UnlockLevel(level + 1);
                
                // 更新关卡最高分 / Update level high score
                UpdateLevelHighScore(level, finalScore);
            }
            
            // 更新总最高分 / Update overall high score
            UpdateHighScore(finalScore);
            
            SaveProgress();
        }
        
        /// <summary>
        /// 检查关卡是否解锁 / Check if level is unlocked
        /// </summary>
        public bool IsLevelUnlocked(int level)
        {
            return level <= progressData.highestLevelUnlocked;
        }
        
        /// <summary>
        /// 获取最高解锁关卡 / Get highest unlocked level
        /// </summary>
        public int GetHighestLevelUnlocked()
        {
            return progressData.highestLevelUnlocked;
        }
        
        /// <summary>
        /// 获取最高分 / Get high score
        /// </summary>
        public int GetHighScore()
        {
            return progressData.highestScore;
        }
        
        /// <summary>
        /// 获取关卡最高分 / Get level high score
        /// </summary>
        public int GetLevelHighScore(int level)
        {
            int levelIndex = level - 1;
            if (levelIndex >= 0 && levelIndex < progressData.levelHighScores.Length)
            {
                return progressData.levelHighScores[levelIndex];
            }
            return 0;
        }
        
        /// <summary>
        /// 获取总游戏次数 / Get total games played
        /// </summary>
        public int GetTotalGamesPlayed()
        {
            return progressData.totalGamesPlayed;
        }
        
        /// <summary>
        /// 获取总胜利次数 / Get total victories
        /// </summary>
        public int GetTotalVictories()
        {
            return progressData.totalVictories;
        }
        
        /// <summary>
        /// 重置所有进度（用于测试）/ Reset all progress (for testing)
        /// </summary>
        public void ResetProgress()
        {
            progressData = new GameProgressData();
            SaveProgress();
            Debug.Log("[GameProgress] Progress reset");
        }
        
        /// <summary>
        /// 删除存档 / Delete save data
        /// </summary>
        public void DeleteSaveData()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
            progressData = new GameProgressData();
            Debug.Log("[GameProgress] Save data deleted");
        }
    }
}
