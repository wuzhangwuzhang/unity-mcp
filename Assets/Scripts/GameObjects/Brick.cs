using UnityEngine;

namespace BreakoutGame.GameObjects
{
    /// <summary>
    /// 砖块组件，处理碰撞和销毁
    /// Brick component that handles collision and destruction
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class Brick : MonoBehaviour
    {
        [Header("Brick Settings / 砖块设置")]
        [SerializeField] private int health = 1;
        [SerializeField] private int scoreValue = 10;
        [SerializeField] private Color brickColor = Color.red;
        
        private BoxCollider2D boxCollider;
        private SpriteRenderer spriteRenderer;
        private bool isDestroyed = false; // 标记是否已销毁，防止重复注销 / Flag to prevent duplicate unregistration
        
        private void Awake()
        {
            // 获取组件引用 / Get component references
            boxCollider = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            // 配置BoxCollider2D / Configure BoxCollider2D
            if (boxCollider != null)
            {
                boxCollider.size = new Vector2(1f, 1f);
            }
            
            // 配置SpriteRenderer / Configure SpriteRenderer
            if (spriteRenderer != null)
            {
                spriteRenderer.color = brickColor;
            }
            
            Debug.Log($"[Brick] Initialized - Health: {health}, Score: {scoreValue}");
        }
        
        private void Start()
        {
            // 注册到GameManager / Register with GameManager
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.RegisterBrick();
                Debug.Log($"[Brick] Registered at position: {transform.position}");
            }
            else
            {
                Debug.LogError("[Brick] GameManager.Instance is null, cannot register!");
            }
        }
        
        /// <summary>
        /// 砖块受到伤害 / Brick takes damage
        /// </summary>
        public void TakeDamage(int damage = 1)
        {
            health -= damage;
            
            Debug.Log($"[Brick] Took {damage} damage, remaining health: {health}");
            
            if (health <= 0)
            {
                DestroyBrick();
            }
            else
            {
                // 更新颜色表示生命值 / Update color to indicate health
                UpdateColor();
            }
        }
        
        /// <summary>
        /// 销毁砖块 / Destroy brick
        /// </summary>
        private void DestroyBrick()
        {   
            if(isDestroyed)
                return;
            
            isDestroyed = true;
            Debug.Log($"[Brick] Destroyed at position: {transform.position}");
            
            // 触发砖块销毁事件（GameManager会通过事件处理注销）
            // Trigger brick destroyed event (GameManager will handle unregistration via event)
            Core.GameEvents.TriggerBrickDestroyed();
            
            // 添加分数 / Add score
            if (Core.ScoreSystem.Instance != null)
            {
                Core.ScoreSystem.Instance.AddScore(scoreValue);
            }
            
            // 销毁GameObject / Destroy GameObject
            Destroy(gameObject);
        }
        
        /// <summary>
        /// 根据生命值更新颜色 / Update color based on health
        /// </summary>
        private void UpdateColor()
        {
            if (spriteRenderer != null)
            {
                // 生命值越低，颜色越暗 / Lower health = darker color
                float alpha = Mathf.Clamp01(health / 3f);
                spriteRenderer.color = new Color(brickColor.r, brickColor.g, brickColor.b, Mathf.Max(alpha, 0.5f));
            }
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // 检测与球的碰撞 / Detect collision with ball
            if (collision.gameObject.CompareTag("Ball"))
            {
                Debug.Log($"[Brick] Ball collision detected");
                TakeDamage(1);
            }
        }
        
        private void OnDestroy()
        {
            // 如果已经通过DestroyBrick销毁，跳过（避免重复注销）
            // If already destroyed via DestroyBrick, skip (avoid duplicate unregistration)
            if (isDestroyed)
            {
                return;
            }
            
            // 只在游戏进行中且GameManager存在时注销（场景切换时不注销）
            // Only unregister when game is playing and GameManager exists (don't unregister during scene transitions)
            if (Core.GameManager.Instance != null && 
                Core.GameManager.Instance.CurrentState == Core.GameState.Playing)
            {
                Core.GameManager.Instance.UnregisterBrick();
                Debug.Log("[Brick] Unregistered in OnDestroy (scene cleanup during gameplay)");
            }
            else
            {
                Debug.Log("[Brick] OnDestroy called but not unregistering (scene transition or game not playing)");
            }
        }
        
        #region Public Accessors for Testing / 测试用公共访问器
        
        public int Health => health;
        public int ScoreValue => scoreValue;
        
        #endregion
    }
}
