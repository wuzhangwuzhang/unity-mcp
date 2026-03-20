using UnityEngine;

namespace BreakoutGame.GameObjects
{
    /// <summary>
    /// 弹球控制器，处理物理行为和碰撞
    /// Ball controller that handles physics behavior and collisions
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Ball : MonoBehaviour
    {
        [Header("Movement Settings / 移动设置")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float minAngle = 30f; // 最小反弹角度（度）/ Minimum bounce angle (degrees)
        [SerializeField] private float launchAngleRange = 30f; // 发射角度范围 / Launch angle range
        
        [Header("Boundary Settings / 边界设置")]
        [SerializeField] private float bottomBoundary = -6f;
        
        private Rigidbody2D rb;
        private CircleCollider2D circleCollider;
        private bool isLaunched = false;
        
        private void Awake()
        {
            // 获取组件引用 / Get component references
            rb = GetComponent<Rigidbody2D>();
            circleCollider = GetComponent<CircleCollider2D>();
            
            // 配置Rigidbody2D / Configure Rigidbody2D
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f; // 无重力 / No gravity
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // 连续碰撞检测 / Continuous collision detection
            
            Debug.Log("[Ball] Initialized");
        }
        
        private void Start()
        {
            // 从GameConfig加载配置（如果存在）/ Load config from GameConfig if exists
            var config = Resources.Load<Data.GameConfig>("GameConfig");
            if (config != null)
            {
                // 获取当前关卡配置 / Get current level config
                int currentLevel = 1;
                if (Core.GameManager.Instance != null)
                {
                    currentLevel = Core.GameManager.Instance.GetCurrentLevel();
                }
                
                // 加载关卡特定的球速 / Load level-specific ball speed
                int levelIndex = currentLevel - 1;
                if (levelIndex >= 0 && levelIndex < config.levelConfigs.Length)
                {
                    speed = config.levelConfigs[levelIndex].ballSpeed;
                    Debug.Log($"[Ball] Level {currentLevel} - Speed: {speed}");
                }
                else
                {
                    speed = config.ballSpeed; // 使用默认速度 / Use default speed
                }
                
                minAngle = config.ballMinAngle;
                launchAngleRange = config.ballLaunchAngleRange;
                bottomBoundary = config.bottomBoundary;
                
                Debug.Log($"[Ball] Loaded config - Speed: {speed}, MinAngle: {minAngle}");
            }
            
            // 订阅游戏开始事件 / Subscribe to game started event
            Core.GameEvents.GameStarted += OnGameStarted;
            
            // 如果游戏已经在Playing状态，立即发射（处理事件已触发的情况）
            // If game is already in Playing state, launch immediately (handle case where event already fired)
            if (Core.GameManager.Instance != null && 
                Core.GameManager.Instance.CurrentState == Core.GameState.Playing)
            {
                Debug.Log("[Ball] Game already playing, launching immediately");
                Launch();
            }
        }
        
        private void OnDestroy()
        {
            // 取消订阅事件 / Unsubscribe from events
            Core.GameEvents.GameStarted -= OnGameStarted;
        }
        
        private void Update()
        {
            // 检测底部边界 / Check bottom boundary
            if (isLaunched && transform.position.y < bottomBoundary)
            {
                OnBallFell();
            }
        }
        
        private void FixedUpdate()
        {
            // 在物理更新中归一化速度 / Normalize velocity in physics update
            if (isLaunched)
            {
                NormalizeVelocity();
                PreventHorizontalTrapping();
            }
        }
        
        /// <summary>
        /// 防止球陷入水平来回弹跳 / Prevent ball from getting trapped in horizontal bouncing
        /// </summary>
        private void PreventHorizontalTrapping()
        {
            Vector2 velocity = rb.velocity;
            
            // 如果Y方向速度太小，强制增加Y方向速度 / If Y velocity too small, force increase Y velocity
            if (Mathf.Abs(velocity.y) < speed * 0.3f)
            {
                float newY = Mathf.Sign(velocity.y) * speed * 0.5f;
                if (newY == 0) newY = speed * 0.5f; // 如果Y为0，默认向上
                
                velocity.y = newY;
                velocity.x = Mathf.Sign(velocity.x) * Mathf.Sqrt(speed * speed - velocity.y * velocity.y);
                
                rb.velocity = velocity;
                Debug.LogWarning($"[Ball] Prevented horizontal trapping, adjusted velocity to: {velocity}");
            }
        }
        
        /// <summary>
        /// 游戏开始时发射弹球 / Launch ball when game starts
        /// </summary>
        private void OnGameStarted()
        {
            Launch();
        }
        
        /// <summary>
        /// 以随机角度发射弹球 / Launch ball at random angle
        /// </summary>
        public void Launch()
        {
            if (isLaunched)
            {
                Debug.LogWarning("[Ball] Ball is already launched");
                return;
            }
            
            // 生成随机角度（向上，偏离垂直方向-30到+30度）
            // Generate random angle (upward, -30 to +30 degrees from vertical)
            float randomAngle = Random.Range(-launchAngleRange, launchAngleRange);
            float angleInRadians = (90f + randomAngle) * Mathf.Deg2Rad;
            
            // 计算速度向量 / Calculate velocity vector
            Vector2 launchDirection = new Vector2(
                Mathf.Cos(angleInRadians),
                Mathf.Sin(angleInRadians)
            );
            
            rb.velocity = launchDirection.normalized * speed;
            isLaunched = true;
            
            Debug.Log($"[Ball] Launched at angle: {randomAngle}° with velocity: {rb.velocity}");
        }
        
        /// <summary>
        /// 重置弹球位置 / Reset ball position
        /// </summary>
        public void ResetPosition()
        {
            rb.velocity = Vector2.zero;
            isLaunched = false;
            
            // 重置到挡板上方 / Reset to above paddle
            var paddle = FindObjectOfType<Paddle>();
            if (paddle != null)
            {
                transform.position = new Vector3(paddle.transform.position.x, paddle.transform.position.y + 1f, 0f);
            }
            else
            {
                transform.position = new Vector3(0f, -4f, 0f);
            }
            
            Debug.Log("[Ball] Position reset");
        }
        
        /// <summary>
        /// 归一化速度以保持恒定速度 / Normalize velocity to maintain constant speed
        /// </summary>
        public void NormalizeVelocity()
        {
            float currentSpeed = rb.velocity.magnitude;
            
            // 如果速度偏离目标速度较多，重新归一化 / If speed deviates significantly from target, renormalize
            if (Mathf.Abs(currentSpeed - speed) > 0.5f)
            {
                if (currentSpeed > 0.1f) // 避免除以零 / Avoid division by zero
                {
                    rb.velocity = rb.velocity.normalized * speed;
                    Debug.LogWarning($"[Ball] Speed normalized from {currentSpeed} to {speed}");
                }
                else
                {
                    // 速度过低，重新发射 / Speed too low, relaunch
                    Debug.LogError("[Ball] Speed too low, relaunching");
                    Launch();
                }
            }
        }
        
        /// <summary>
        /// 调整反弹角度避免过小角度 / Adjust bounce angle to avoid too shallow angles
        /// </summary>
        /// <param name="normal">碰撞法线 / Collision normal</param>
        private void AdjustAngle(Vector2 normal)
        {
            Vector2 velocity = rb.velocity;
            
            // 计算与法线的角度 / Calculate angle with normal
            float angle = Vector2.Angle(velocity, normal);
            
            // 如果角度太小（接近平行），调整速度 / If angle too small (nearly parallel), adjust velocity
            if (angle < minAngle || angle > (180f - minAngle))
            {
                // 增加垂直于法线的速度分量 / Increase velocity component perpendicular to normal
                Vector2 tangent = new Vector2(-normal.y, normal.x);
                float tangentComponent = Vector2.Dot(velocity, tangent);
                float normalComponent = Vector2.Dot(velocity, normal);
                
                // 调整法线分量 / Adjust normal component
                if (Mathf.Abs(normalComponent) < speed * 0.5f)
                {
                    normalComponent = Mathf.Sign(normalComponent) * speed * 0.5f;
                }
                
                velocity = tangent * tangentComponent + normal * normalComponent;
                rb.velocity = velocity.normalized * speed;
                
                Debug.Log($"[Ball] Angle adjusted from {angle}° to avoid shallow bounce");
            }
        }
        
        /// <summary>
        /// 处理球掉落 / Handle ball fell
        /// </summary>
        private void OnBallFell()
        {
            if (!isLaunched) return;
            
            Debug.Log("[Ball] Ball fell below bottom boundary");
            
            isLaunched = false;
            rb.velocity = Vector2.zero;
            
            // 触发球掉落事件 / Trigger ball fell event
            Core.GameEvents.TriggerBallFell();
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isLaunched) return;
            
            // 获取碰撞法线 / Get collision normal
            Vector2 normal = collision.contacts[0].normal;
            
            // 处理与挡板的碰撞 / Handle collision with paddle
            if (collision.gameObject.CompareTag("Paddle"))
            {
                HandlePaddleCollision(collision);
            }
            // 处理与砖块的碰撞 / Handle collision with brick
            else if (collision.gameObject.CompareTag("Brick"))
            {
                AdjustAngle(normal);
                Debug.Log("[Ball] Collided with brick");
            }
            // 处理与墙壁的碰撞 / Handle collision with boundary
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Boundary"))
            {
                AdjustAngle(normal);
                Debug.Log("[Ball] Collided with boundary");
            }
        }
        
        /// <summary>
        /// 处理与挡板的碰撞 / Handle collision with paddle
        /// </summary>
        private void HandlePaddleCollision(Collision2D collision)
        {
            // 获取碰撞点 / Get collision point
            Vector2 collisionPoint = collision.contacts[0].point;
            Vector2 paddlePosition = collision.transform.position;
            
            // 计算碰撞点相对于挡板中心的偏移 / Calculate offset from paddle center
            float paddleWidth = collision.collider.bounds.size.x;
            float offset = (collisionPoint.x - paddlePosition.x) / (paddleWidth / 2f);
            offset = Mathf.Clamp(offset, -1f, 1f);
            
            // 根据偏移调整水平速度 / Adjust horizontal velocity based on offset
            float angle = offset * 60f; // 最大偏转60度 / Max deflection 60 degrees
            float angleInRadians = (90f + angle) * Mathf.Deg2Rad;
            
            Vector2 newDirection = new Vector2(
                Mathf.Cos(angleInRadians),
                Mathf.Sin(angleInRadians)
            );
            
            // 确保Y方向向上且有最小速度 / Ensure Y direction is upward with minimum speed
            if (newDirection.y < 0)
            {
                newDirection.y = -newDirection.y;
            }
            
            // 确保Y方向速度不会太小 / Ensure Y velocity is not too small
            if (newDirection.y < 0.5f)
            {
                newDirection.y = 0.5f;
                newDirection.x = Mathf.Sign(newDirection.x) * Mathf.Sqrt(1f - newDirection.y * newDirection.y);
            }
            
            rb.velocity = newDirection.normalized * speed;
            
            Debug.Log($"[Ball] Paddle collision - Offset: {offset:F2}, Angle: {angle:F1}°, Velocity: {rb.velocity}");
        }
        
        #region Public Accessors for Testing / 测试用公共访问器
        
        public float Speed => speed;
        public float MinAngle => minAngle;
        public bool IsLaunched => isLaunched;
        
        #endregion
    }
}
