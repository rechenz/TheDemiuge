using UnityEngine;

public class AdaptiveGameCamera : MonoBehaviour
{
    [System.Serializable]
    public class GameArea
    {
        public float width = 90f;
        public float height = 50f;
        public Color gizmoColor = Color.cyan;
    }

    [System.Serializable]
    public class AdaptiveSettings
    {
        [Range(0, 1)]
        public float horizontalFit = 0.5f;    // 0=完全水平适配，1=完全垂直适配
        public bool showGameArea = true;
        public bool showSafeArea = true;
    }

    [Header("游戏区域")]
    public GameArea gameArea = new GameArea();

    [Header("自适应设置")]
    public AdaptiveSettings settings = new AdaptiveSettings();

    [Header("安全区域")]
    public float safeAreaPadding = 0.5f;      // 安全区域边距

    private Camera cam;
    private Vector2 lastScreenSize;

    void Awake()
    {
        cam = GetComponent<Camera>();
        lastScreenSize = new Vector2(Screen.width, Screen.height);
        AdaptToScreen();
    }

    void Update()
    {
        // 检查窗口变化
        if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
        {
            AdaptToScreen();
            lastScreenSize = new Vector2(Screen.width, Screen.height);
        }
    }

    void AdaptToScreen()
    {
        if (cam == null) return;

        float screenAspect = (float)Screen.width / Screen.height;
        float gameAspect = gameArea.width / gameArea.height;

        // 计算基于宽高比的适配
        float horizontalSize = gameArea.height / 2f;
        float verticalSize = (gameArea.width / screenAspect) / 2f;

        // 混合适配（根据horizontalFit参数）
        float targetSize = Mathf.Lerp(horizontalSize, verticalSize, settings.horizontalFit);

        cam.orthographicSize = targetSize;

        // 确保游戏区域完全可见
        EnsureGameAreaVisible();

        Debug.Log($"游戏区域适配: {gameArea.width}x{gameArea.height}, 摄像机大小: {cam.orthographicSize:F2}");
    }

    // void EnsureGameAreaVisible()
    // {
    //     float visibleWidth = cam.orthographicSize * 2 * cam.aspect;
    //     float visibleHeight = cam.orthographicSize * 2;

    //     // 如果可见区域小于游戏区域，调整摄像机大小
    //     if (visibleWidth < gameArea.width)
    //     {
    //         cam.orthographicSize = (gameArea.width / cam.aspect) / 2f;
    //     }

    //     if (visibleHeight < gameArea.height)
    //     {
    //         cam.orthographicSize = gameArea.height / 2f;
    //     }
    // }
    void EnsureGameAreaVisible()
    {
        // 计算确保宽度可见所需的摄像机大小
        float sizeForWidth = (gameArea.width / cam.aspect) / 2f;
        // 计算确保高度可见所需的摄像机大小
        float sizeForHeight = gameArea.height / 2f;

        // 选择较大的值以确保两个维度都可见
        cam.orthographicSize = Mathf.Max(sizeForWidth, sizeForHeight);
    }

    void OnDrawGizmos()
    {
        if (!settings.showGameArea) return;

        // 绘制游戏区域
        Gizmos.color = gameArea.gizmoColor;
        Vector3 center = transform.position;
        Vector3 size = new Vector3(gameArea.width, gameArea.height, 0.1f);
        Gizmos.DrawWireCube(center, size);

        // 绘制安全区域
        if (settings.showSafeArea)
        {
            Gizmos.color = Color.yellow;
            Vector3 safeSize = new Vector3(
                gameArea.width - safeAreaPadding * 2,
                gameArea.height - safeAreaPadding * 2,
                0.1f
            );
            Gizmos.DrawWireCube(center, safeSize);
        }

        // 绘制摄像机可见区域
        if (cam != null)
        {
            Gizmos.color = Color.green;
            float camHeight = cam.orthographicSize * 2;
            float camWidth = camHeight * cam.aspect;
            Vector3 camSize = new Vector3(camWidth, camHeight, 0.1f);
            Gizmos.DrawWireCube(center, camSize);
        }
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;

        GUILayout.BeginArea(new Rect(10, Screen.height - 150, 350, 140));

        GUILayout.Label("=== 自适应游戏摄像机 ===", style);
        GUILayout.Label($"游戏区域: {gameArea.width}×{gameArea.height}", style);
        GUILayout.Label($"窗口: {Screen.width}×{Screen.height}", style);
        GUILayout.Label($"摄像机大小: {cam.orthographicSize:F2}", style);
        GUILayout.Label($"适配权重: {settings.horizontalFit:F2}", style);

        // 滑动条控制适配权重
        settings.horizontalFit = GUILayout.HorizontalSlider(settings.horizontalFit, 0, 1);
        GUILayout.Label($"水平适配 ← {settings.horizontalFit:F2} → 垂直适配", style);

        GUILayout.EndArea();
    }

    // 公共方法
    public void SetGameArea(float width, float height)
    {
        gameArea.width = width;
        gameArea.height = height;
        AdaptToScreen();
    }

    public void SetAdaptiveWeight(float weight)
    {
        settings.horizontalFit = Mathf.Clamp01(weight);
        AdaptToScreen();
    }
}