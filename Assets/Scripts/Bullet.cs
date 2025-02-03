using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 1f;

    void Start()
    {
        
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var Enemy = other.gameObject;
            Enemy.GetComponent<EnemyController>().TakeDamage(PlayerController.Instance.Attack,Color.white);
            Destroy(gameObject);   
        }
    }
}
