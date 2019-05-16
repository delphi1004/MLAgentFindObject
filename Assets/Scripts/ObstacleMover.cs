// Assignment project of Intelligent Computational Media 
// 16 May , John Cheongun Lee
// 
// this is a ObstacleMover class
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    private int direction;

    void Start()
    {
        direction = 6;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wall") == true)
        {
            direction *= -1;
        }
    }

    void Update()
    {
        transform.Translate(Time.deltaTime * direction, 0, 0);

    }
}
