using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    public Transform bar; 
    public PlayerController PC; 
    private Camera mainCamera; 

    private Vector3 initialScale; 

    void Start()
    {
        mainCamera = Camera.main; 
        if (PC == null)
        {
            PC = GetComponentInParent<PlayerController>(); 
        }
        initialScale = bar.localScale; 
    }

    void Update()
    {
        if(!mainCamera)
        {
            mainCamera = Camera.main; 
        }
        UpdateHealthBar();

        LookAtCamera();
    }

    void UpdateHealthBar()
    {
        if (PC != null)
        {
            float healthPercent = (float)PC.currentHealth / PC.maxHealth;

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