using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    public float distanceFromGround = 0.5f; // Distance fixe au-dessus du sol

    void Update()
    {
        AdjustHeightAboveGround();
    }

    void AdjustHeightAboveGround()
    {
        RaycastHit hit;

        // Lancer un raycast vers le bas depuis la position de l'objet
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            // Calculer la nouvelle position pour maintenir une distance fixe au-dessus du sol
            Vector3 newPosition = hit.point + new Vector3(0, distanceFromGround, 0);
            transform.position = newPosition;
        }
    }
}
