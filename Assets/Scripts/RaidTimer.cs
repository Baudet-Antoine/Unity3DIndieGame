using System.Collections;
using TMPro;  // Nécessaire pour utiliser TextMeshPro
using UnityEngine;

public class RaidTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;  // Référence au TMPRO texte
    private bool isTimerRunning = false;
    private float elapsedTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        timerText.text = "";  // Ne rien afficher tant que le timer n'est pas démarré
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;  // Augmenter le temps écoulé
            UpdateTimerDisplay();           // Mettre à jour l'affichage
        }
    }

    // Fonction pour démarrer le timer
    public void StartTimer()
    {
        elapsedTime = 0f;  // Remettre le timer à zéro
        isTimerRunning = true;
    }

    // Fonction pour arrêter le timer
    public void StopTimer()
    {
        isTimerRunning = false;
        timerText.text = "";  // Cacher l'affichage du timer
    }

    // Met à jour l'affichage du timer dans le format 00:00
    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);  // Calculer les minutes
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);  // Calculer les secondes
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);  // Format 00:00
    }
}
