using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform player; 
    public Vector3 offset;   
    public float smoothSpeed = 0.125f; 

    void Start()
    {
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = PlayerController.Instance.gameObject.transform.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}
