using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] UnityEvent<Collider> onTriggerEnter;
    [SerializeField] UnityEvent<Collider> onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerEnter.Invoke(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerExit.Invoke(other);
        }
    }
}
