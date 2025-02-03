using UnityEngine;

public class AnimationReceiver : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        // Trouver le PlayerController sur le parent
        playerController = GetComponentInParent<PlayerController>();
    }

    // Cette méthode sera appelée par l'événement d'animation
    public void Shoot()
    {
        playerController.Shoot();
    }
}