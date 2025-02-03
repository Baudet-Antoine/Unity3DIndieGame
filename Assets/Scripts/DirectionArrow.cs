using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionArrow : MonoBehaviour
{
    void Update()
    {
        transform.position += Vector3.down * 5 * Time.deltaTime;
    }
}
