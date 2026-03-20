using UnityEngine;

namespace BreakoutGame.Data
{
    /// <summary>
    /// 游戏配置ScriptableObject，存储所有游戏参数
    /// Game configuration ScriptableObject that stores all game parameters
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Breakout/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Ball Settings / 弹球设置")]
        [Tooltip("弹球移动速度 / Ball movement speed")]
        public float ballSpeed = 5f;
        
        [Tooltip("弹球最小反弹角度（度）/ Minimum ball bounce angle (degrees)")]
        [Range(0f, 45f)]
        public float ballMinAngle = 30f;
        
        [Tooltip("弹球发射时的随机角度范围（度）/ Random angle range for ball launch (degrees)")]
        [Range(0f, 45f)]
        public float ballLaunchAngleRange = 30f;
        
        [Header("Paddle Settings / 挡板设置")]
        [Tooltip("挡板移动速度 / Paddle movement speed")]
        public float paddleMoveSpeed = 10f;
        
        [Tooltip("挡板最小X坐标 / Paddle minimum X position")]
        public float paddleMinX = -11f;
        
        [Tooltip("挡板最大X坐标 / Paddle maximum X position")]
        public float paddleMaxX = 11f;
        
        [Tooltip("挡板宽度 / Paddle width")]
        public float paddleWidth = 2f;
        
        [Tooltip("挡板高度 / Paddle height")]
        public float paddleHeight = 0.3f;
        
        [Header("Brick Settings / 砖块设置")]
        [Tooltip("砖块行数 / Number of brick rows")]
        [Range(1, 10)]
        public int brickRows = 5;
        
        [Tooltip("砖块列数 / Number of brick columns")]
        [Range(1, 15)]
        public int brickColumns = 8;
        
        [Tooltip("砖块宽度 / Brick width")]
        public float brickWidth = 1.2f;
        
        [Tooltip("砖块高度 / Brick height")]
        public float brickHeight = 0.5f;
        
        [Tooltip("砖块间距 / Spacing between bricks")]
        public float brickSpacing = 0.15f;
        
        [Tooltip("砖块网格起始位置 / Brick grid start position")]
        public Vector2 brickStartPosition = new Vector2(-5.2f, 2.5f);
        
        [Header("Score Settings / 分数设置")]
        [Tooltip("销毁砖块获得的分数 / Score for destroying a brick")]
        public int brickDestroyScore = 10;
        
        [Header("Boundary Settings / 边界设置")]
        [Tooltip("左边界X坐标 / Left boundary X position")]
        public float leftBoundary = -12f;
        
        [Tooltip("右边界X坐标 / Right boundary X position")]
        public float rightBoundary = 12f;
        
        [Tooltip("顶部边界Y坐标 / Top boundary Y position")]
        public float topBoundary = 7.5f;
        
        [Tooltip("底部边界Y坐标（失败触发线）/ Bottom boundary Y position (fail trigger line)")]
        public float bottomBoundary = -7.5f;
        
        [Tooltip("边界墙厚度 / Boundary wall thickness")]
        public float boundaryThickness = 0.5f;
        
        [Header("Camera Settings / 相机设置")]
        [Tooltip("正交相机大小 / Orthographic camera size")]
        public float cameraSize = 8f;
        
        [Tooltip("相机位置 / Camera position")]
        public Vector3 cameraPosition = new Vector3(0f, 0f, -10f);
        
        [Header("Level Settings / 关卡设置")]
        [Tooltip("总关卡数 / Total number of levels")]
        public int totalLevels = 3;
        
        [System.Serializable]
        public class LevelConfig
        {
            public int rows = 5;
            public int columns = 8;
            public float ballSpeed = 5f;
            public float paddleWidth = 2f;
        }
        
        [Tooltip("每个关卡的配置 / Configuration for each level")]
        public LevelConfig[] levelConfigs = new LevelConfig[]
        {
            new LevelConfig { rows = 5, columns = 8, ballSpeed = 5f, paddleWidth = 2f },    // 第1关
            new LevelConfig { rows = 6, columns = 10, ballSpeed = 6f, paddleWidth = 1.8f }, // 第2关
            new LevelConfig { rows = 7, columns = 12, ballSpeed = 7f, paddleWidth = 1.5f }  // 第3关
        };
    }
}
