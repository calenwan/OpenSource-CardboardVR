using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                print("KeyCode down: " + kcode);
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            // button A
            transform.position -= new Vector3(-0.1f, 0f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            // button B
            transform.position -= new Vector3(0.1f, 0f, 0f);
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            // button X
            transform.position -= new Vector3(0f, 0.1f, 0f);
        }
        
        if (Input.GetKeyDown(KeyCode.U))
        {
            // button Y
            transform.position -= new Vector3(0f, -0.1f, 0f);
        }
    }
}
