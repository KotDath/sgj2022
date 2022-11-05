using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {

        Destroy(gameObject);
    }
}
