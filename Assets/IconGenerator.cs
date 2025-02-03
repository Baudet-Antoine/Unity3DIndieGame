using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class IconGenerator : MonoBehaviour
{
    
    public Camera iconCamera; // Assigner la caméra de capture
    public List<GameObject> objectsToCapture = new List<GameObject>(); // Liste des objets à capturer
    public RenderTexture renderTexture; // La Render Texture assignée à la caméra

    void Start()
    {
        StartCoroutine(CaptureIcons());
    }

    IEnumerator CaptureIcons()
    {
        // Crée un dossier "Icons" dans le répertoire du projet s'il n'existe pas
        string path = Application.dataPath + "/Icons";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        foreach (GameObject obj in objectsToCapture)
        {
            obj.SetActive(true); // Active l'objet pour la capture

            yield return new WaitForEndOfFrame(); // Attends la fin de la frame pour capturer

            // Capture l'image à partir de la Render Texture
            RenderTexture.active = renderTexture;
            Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            iconCamera.Render();
            screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            screenshot.Apply();
            RenderTexture.active = null; // Libère la RenderTexture

            // Convertir l'image en PNG
            byte[] bytes = screenshot.EncodeToPNG();
            File.WriteAllBytes(path + "/" + obj.name + "_icon.png", bytes); // Sauvegarde l'icône

            Debug.Log("Icon saved for " + obj.name);

            obj.SetActive(false); // Désactive l'objet après capture
        }

        Debug.Log("Icon generation complete");
    }
}
