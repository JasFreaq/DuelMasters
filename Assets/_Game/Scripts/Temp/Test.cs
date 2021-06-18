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
        print($"{Camera.main.WorldToScreenPoint(transform.position)}");
    }
}
