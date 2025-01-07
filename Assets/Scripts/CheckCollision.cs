using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision!!!");
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Uncollision!!!");
    }

    private void OnTriggerEnter(Collider other)
    {
        
        Debug.Log("Trigger!!!");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Untrigger!!!");
        
    }
}
