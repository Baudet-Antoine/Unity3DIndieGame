using UnityEngine;
using System;

public class BulletMovement : MonoBehaviour
{
    public float bulletSpeed;
    public float lifetime = 5f;

    public bool hasTarget;
    public Weapon Weapon;

    public Transform target;

    void Start()
    {
    }

    public void Initialize(Transform targetTransform, float BS)
    {
        target = targetTransform;
        hasTarget = target != null;
        bulletSpeed = BS;
    }

    void Update()
    {
        if (hasTarget)
        {
            if(target == null)
            {
                Destroy(gameObject);
                hasTarget = false;
                return;
            }
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            
            transform.Translate(directionToTarget * bulletSpeed * Time.deltaTime, Space.World);

            AlignWithDirection();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void AlignWithDirection()
    {
        if (transform.position != target.position)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * bulletSpeed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == target)
        {
            Destroy(gameObject);
        }
    }
}
