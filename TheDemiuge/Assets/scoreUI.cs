using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoreUI : MonoBehaviour
{
    public int score = 0;

    // 屏幕坐标位置 (0-1)
    public Vector2 scorePosition = new Vector2(0.02f, 0.95f); // 左上角

    // 字体样式
    public int fontSize = 24;
    public Color textColor = Color.white;
    public Font customFont; // 可选：自定义字体

    void OnGUI()
    {
        // 设置字体样式
        GUIStyle style = new GUIStyle();
        style.fontSize = fontSize;
        style.normal.textColor = textColor;
        if (customFont != null) style.font = customFont;

        // 显示当前分数
        Rect scoreRect = new Rect(
            Screen.width * scorePosition.x,
            Screen.height * scorePosition.y,
            300, 50
        );
        GUI.Label(scoreRect, $"分数: {score}", style);
    }
    public void AddScore(int points)
    {
        score += points;
    }
}
