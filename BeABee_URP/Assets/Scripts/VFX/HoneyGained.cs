using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneyGained : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, 1f);
    }

    void Update()
    {
        transform.position += 2 * Time.deltaTime * Vector3.up;
    }
}
