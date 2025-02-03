using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoad : MonoBehaviour
{
    private static HashSet<string> existingObjects = new HashSet<string>();
    private string uniqueID; // Pour suivre l'ID de cet objet

    void Awake()
    {
        uniqueID = gameObject.name;
        // Si cet objet existe déjà, le détruire
        if (existingObjects.Contains(uniqueID))
        {
            Destroy(gameObject);
        }
        else
        {
            // Sinon, l'ajouter aux objets existants et le rendre persistant
            existingObjects.Add(uniqueID);
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnDestroy()
    {
        // Retirer l'objet de la liste si détruit
        if (existingObjects.Contains(uniqueID))
        {
            existingObjects.Remove(uniqueID);
        }
    }
}
