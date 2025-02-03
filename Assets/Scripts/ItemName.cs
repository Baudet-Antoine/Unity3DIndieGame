using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemName : MonoBehaviour
{
    private Camera mainCamera;
    public Vector3 RandomizeIntensity = new Vector3(0.5f, 0, 0);
    public TextMeshPro itemNameText;  // Référence au texte (TextMeshPro 3D)
    public bool isHovered = false;   // Vérifie si la souris survole l'objet parent

    private Transform parentObject;   // Référence au parent de l'objet
    public float hoverDistance = 2.0f; // Distance permise pour "hover" l'objet (plus permissive)

    void Start()
    {
        mainCamera = Camera.main;
        parentObject = transform.parent;  // Référence au parent (l'objet à viser avec la souris)

        // Randomisation de la position du texte
        transform.localPosition += new Vector3(Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x),
                                               Random.Range(-RandomizeIntensity.y, RandomizeIntensity.y),
                                               Random.Range(-RandomizeIntensity.z, RandomizeIntensity.z)); 

        itemNameText.text = parentObject.GetComponent<ItemHolder>().ItemData.itemName.ToUpper();  // Initialement, le texte est vide
    }

    void Update()
    {
        LookAtCamera();

        // Utilisation de la distance entre la souris et l'objet parent pour déterminer si on est en hover
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        

        if (Physics.Raycast(ray, out hit))
        {
            float distanceToParent = Vector3.Distance(hit.point, parentObject.position); // Distance entre le point du ray et l'objet parent

            if (distanceToParent <= hoverDistance)  // Si la souris est proche de l'objet parent selon une certaine distance
            {
                if (!isHovered)  // Si la souris vient d'entrer dans la zone permise
                {
                    OnMouseEnter();
                }
            }
            else
            {
                if (isHovered)  // Si la souris quitte la zone permise
                {
                    OnMouseExit();
                }
            }
        }
        else
        {
            if (isHovered)
            {
                OnMouseExit();
            }
        }
    }

    void LookAtCamera()
    {
        // Le texte regarde toujours la caméra
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);
    }

    // Méthode appelée lorsque la souris entre en contact avec l'objet parent
    void OnMouseEnter()
    {
        isHovered = true;
        parentObject.GetComponent<Collection>().isHover = isHovered;
        itemNameText.text = "PRESS E TO COLLECT";  // Change le texte (personnalise-le ici)
    }

    // Méthode appelée lorsque la souris quitte la zone autour de l'objet parent
    void OnMouseExit()
    {
        isHovered = false;
        parentObject.GetComponent<Collection>().isHover = isHovered;
        itemNameText.text = parentObject.GetComponent<ItemHolder>().ItemData.itemName.ToUpper();  // Cache le texte
    }
}
