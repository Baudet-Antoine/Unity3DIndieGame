using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    public Transform bar;
    public EnemyController enemy;
    private Camera mainCamera; 

    private Vector3 initialScale; 

    void Start()
    {
        mainCamera = Camera.main; 
        if (enemy == null)
        {
            enemy = GetComponentInParent<EnemyController>(); 
        }
        initialScale = bar.localScale; 
    }

    void Update()
    {
        UpdateHealthBar();

        LookAtCamera();
    }

    void UpdateHealthBar()
    {
        if (enemy != null)
        {
            float healthPercent = (float)enemy.currentHealth / enemy.maxHealth;

            healthPercent = Mathf.Clamp01(healthPercent);

            bar.localScale = new Vector3(initialScale.x * healthPercent, initialScale.y, initialScale.z);
        }
    }

    void LookAtCamera()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);
    }
}