using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private readonly float left_edge = -25.5f;
    private readonly float init_position = 25.5f;
    private readonly float speed = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.x>left_edge)
        {
            gameObject.transform.position = new Vector3(
                gameObject.transform.position.x - speed, 
                gameObject.transform.position.y,
                gameObject.transform.position.z);
        }
        else {
            gameObject.transform.position = new Vector3(
                init_position,
                gameObject.transform.position.y,
                gameObject.transform.position.z);
        }
    }
}
