using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spritegenerator : MonoBehaviour
{
    [Header("生成参数")]
    [Tooltip("图片精灵的个数")]
    public int num = -1;
    [Tooltip("图片精灵的随机位置范围设定")]
    public float x = 0f;
    public float y = 0f;
    public float width = 40f;
    public float height = 30f;
    [Tooltip("图片精灵的生成间隔")]
    public float interval = 3f;
    [Tooltip("图片精灵prefab")]
    public GameObject imageSprite;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(x, y), new Vector2(2 * width, 2 * height));
    }
    private float timer = 0f;
    private int sum = 0;
    void generateSprite()
    {
        Vector2 pos = new Vector2(Random.Range(x - width, x + width), Random.Range(y - height, y + height));
        Instantiate(imageSprite, pos, Quaternion.identity);
        timer = interval;
        sum++;
    }
    // Start is called before the first frame update
    void Start()
    {
        x = 0;
        y = 0;
        width = 40;
        height = 17;
        interval = 3;
        num = -1;
        generateSprite();
    }

    // Update is called once per frame
    void Update()
    {
        if (sum == num)
        {
            Destroy(this);
        }
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            generateSprite();
        }
    }
}
