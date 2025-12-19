using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;

public class imagespiritmove : MonoBehaviour
{
    #region 参数设置

    [Header("移动参数")]
    [SerializeField] private float maxDistanceAway = 5f;
    [SerializeField] private float Insert = 0.3f;//线性插值


    [Header("随机范围")]
    [SerializeField] private float minSpeed = 0.1f;
    [SerializeField] private float maxSpeed = 0.5f;
    [SerializeField] private float minDirectionChangeGap = 1f;
    [SerializeField] private float maxDirectionChangeGap = 5f;

    [Header("浮动效果")]
    [SerializeField] private float floatAmplitude = 0.2f;
    [SerializeField] private float floatFrequency = 1f;

    #endregion

    #region 基础变量定义
    private Vector2 startPosition;
    private Vector2 targetDirection;
    private float directionChangeTimer;
    private float currentSpeed;
    private float floatOffSet;

    #endregion

    #region 各类函数实现
    void SetrandomDirection()
    {
        float randomTangle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        targetDirection = new Vector2(Mathf.Cos(randomTangle), Mathf.Sin(randomTangle));

        currentSpeed = Random.Range(minSpeed, maxSpeed);
        directionChangeTimer = Random.Range(minDirectionChangeGap, maxDirectionChangeGap);
    }
    void Movespirit()
    {
        Vector2 movement = targetDirection * currentSpeed * Time.deltaTime;
        float floatY = Mathf.Sin((Time.time + floatOffSet) * floatFrequency) * floatAmplitude;
        movement.y += floatY * Time.deltaTime;
        transform.position += (Vector3)movement;
    }
    void LimitMovementArea()
    {
        float DistanceFromStart = Vector2.Distance(transform.position, startPosition);
        if (DistanceFromStart > maxDistanceAway)
        {
            Vector2 directionToStart = (startPosition - (Vector2)transform.position).normalized;
            targetDirection = Vector2.Lerp(targetDirection, directionToStart, Insert);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(startPosition, maxDistanceAway);
        }
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        floatOffSet = Random.Range(0f, 2f * Mathf.PI);
        SetrandomDirection();
    }

    // Update is called once per frame
    void Update()
    {
        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0f)
        {
            SetrandomDirection();
        }
        Movespirit();
        LimitMovementArea();
    }
}
