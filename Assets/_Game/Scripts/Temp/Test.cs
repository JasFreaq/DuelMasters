using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) 
            Move();
    }

    private void Move()
    {
        print($"1: {transform.position}");
        transform.position = Vector3.one;
        print($"2: {transform.position}");
        transform.position = Vector3.zero;
        print($"3: {transform.position}");
    }
}
