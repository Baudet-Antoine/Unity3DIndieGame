using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextScript : MonoBehaviour
{
    private Camera mainCamera;
    public Vector3 RandomizeIntensity = new Vector3(0.5f,0,0);

    void Start()
    {
        mainCamera = Camera.main;

        transform.localPosition += new Vector3(Random.Range(-RandomizeIntensity.x,RandomizeIntensity.x),
                                            Random.Range(-RandomizeIntensity.y,RandomizeIntensity.y),
                                            Random.Range(-RandomizeIntensity.z,RandomizeIntensity.z)); 
    }

    void Update()
    {
        LookAtCamera();
    }

    void LookAtCamera()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);
    }
}
