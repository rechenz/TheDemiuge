using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterbounce : MonoBehaviour
{
    GameObject MC;
    scoreUI scoreui;
    // Start is called before the first frame update
    void Start()
    {
        MC = GameObject.Find("Main Camera");
        if (MC != null)
        {
            scoreui = MC.GetComponent<scoreUI>();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("imageSprite"))
        {
            Debug.Log("collision detected with image sprite");
            Destroy(col.gameObject);
            scoreui.AddScore(1); //增加分数
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
