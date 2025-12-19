using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundfitscript : MonoBehaviour
{
    Camera cm;
    Transform tr;
    // Start is called before the first frame update
    void Start()
    {
        cm = Camera.main;
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
