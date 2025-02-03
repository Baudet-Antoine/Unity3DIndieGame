using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;    
using UnityEngine.SceneManagement;

public class hudController : MonoBehaviour
{
    public GameObject Inventory;
    public GameObject FastInventory;

    public GameObject EscapeMenu;
    public List<GameObject> CurrentHUD;
    public TextMeshProUGUI BalanceText;
    public RectTransform InventoryCenter;  // Centre du cercle
    public List<GameObject> WeaponSlots;   // 6 objets représentant les armes
    public static hudController Instance;
    public Vector3 mousePos;
    private Vector3 fastInventoryInitialPos;  // Position initiale de l'inventaire rapide

    public bool IsFIOpened = false;

    private int selectedWeaponIndex = -1;
    public int CurrentChest;
    
    public GameObject DontDestroyOnLoad;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateBalance();

        // Stocke la position initiale de FastInventory
        fastInventoryInitialPos = FastInventory.GetComponent<RectTransform>().position;
    }

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(CurrentHUD.Count == 0)
            {
                foreach (Transform child in EscapeMenu.transform)
                {
                    CurrentHUD.Add(child.gameObject);
                }
                OpenHUD(CurrentHUD); 
                
            }
            else
            {
                CloseHUD(CurrentHUD);
            }
        }

        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(CurrentHUD.Count == 0)
            {
                foreach (Transform child in Inventory.transform)
                {
                    CurrentHUD.Add(child.gameObject);
                }
                for (int i = 0; i < Inventory.transform.childCount; i++) {
                OpenHUD(CurrentHUD); 
                }
            }
            else
            {
                CloseHUD(CurrentHUD);
            }
        }

        // Gestion de l'inventaire rapide
        if(CurrentHUD.Count == 0 || IsFIOpened == true)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            { 
                SetFastInventoryPosition();  // Placer l'inventaire rapide à la position de la souris
                FastInventory.SetActive(true);
                CurrentHUD.Add(FastInventory);
                IsFIOpened = true;
                DetectMouseDirection();  // Vérifie la direction de la souris pour sélectionner une arme
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                DetectMouseDirection();  // Vérifie la direction tant que LeftControl est maintenue
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                FastInventory.SetActive(false);
                CurrentHUD.Remove(FastInventory);
                IsFIOpened = false;
                FastInventory.GetComponent<RectTransform>().position = fastInventoryInitialPos;  // Réinitialise la position
                SelectWeapon();  // Confirme la sélection de l'arme
            }
        }
        
    }

    public void UpdateBalance()
    {
        BalanceText.text = PlayerController.Instance.Balance.ToString();
    }

    private void SetFastInventoryPosition()
    {
        // Obtenir la position de la souris en coordonnées écran
        mousePos = Input.mousePosition;

        // Positionner l'inventaire rapide à la position de la souris
        RectTransform fastInventoryRect = FastInventory.GetComponent<RectTransform>();

        // Convertir la position de la souris en coordonnées locales du parent si nécessaire
        fastInventoryRect.position = mousePos;

        // Optionnel : Ajuster l'offset ou les limites de l'écran ici si nécessaire
    }

    private void DetectMouseDirection()
    {
        Vector3 mousePos2 = Input.mousePosition;

        // Calculer la direction (vecteur) entre le centre du cercle et la position de la souris
        Vector2 direction = mousePos2 - mousePos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Ajuster l'angle pour qu'il soit entre 0 et 360 degrés
        if (angle < 0)
        {
            angle += 360;
        }

        // Définir un seuil de distance
        float thresholdDistance = 20f; // Par exemple, 50 pixels

        // Vérifier la distance au centre
        if (direction.magnitude > thresholdDistance)
        {
            // Diviser le cercle en 8 sections égales de 45 degrés
            float sectionAngle = 360f / 8;
            int index = Mathf.FloorToInt(angle / sectionAngle) - 1;

            // Ajuster l'index pour corriger le décalage
            index = (index + WeaponSlots.Count - 1) % WeaponSlots.Count;

            if (index != selectedWeaponIndex)
            {
                selectedWeaponIndex = index;
                HighlightWeapon(selectedWeaponIndex + 1);  // Mettre en surbrillance la sélection
            }
        }
        else
        {
            // Si la souris est trop proche du centre, réinitialiser la sélection
            selectedWeaponIndex = -1;
            HighlightWeapon(selectedWeaponIndex);  // Réinitialiser la surbrillance
        }
    }

    private void HighlightWeapon(int index)
    {
        // Mettre en surbrillance l'arme correspondante
        for (int i = 0; i < WeaponSlots.Count; i++)
        {
            if (i == index)
            {
                // Mettre en évidence l'arme sélectionnée
                WeaponSlots[i].transform.GetChild(0).GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                // Réinitialiser les autres armes
                WeaponSlots[i].transform.GetChild(0).GetComponent<Image>().color = Color.white;
            }
        }
    }

    private void SelectWeapon()
    {
        if (selectedWeaponIndex >= 0 && selectedWeaponIndex < WeaponSlots.Count)
        {
            PlayerController.Instance.EquipWeapon(selectedWeaponIndex);
        }

        selectedWeaponIndex = -1;
    }

    public void OpenHUD(GameObject HUD)
    {
        CurrentHUD.Add(HUD);
        HUD.SetActive(true);
        Inventory.transform.GetChild(0).gameObject.SetActive(true);
        Inventory.transform.GetChild(1).gameObject.SetActive(true);
        Inventory.GetComponent<RectTransform>().localPosition = new Vector3(-200,-25,0);
    }

    public void OpenHUD(List<GameObject> HUD)
    {
        CurrentHUD = HUD;
        foreach (var child in HUD)
        {
            child.SetActive(true);
        }
    }

    public void CloseHUD(GameObject HUD)
    {

        HUD.SetActive(false);

        CurrentHUD.Clear();
        CurrentChest = -1;
    }

    public void CloseHUD(List<GameObject> HUD)
    {
        foreach (var child in HUD)
        {
            child.SetActive(false);
        }
        CurrentHUD.Clear();
        CurrentChest = -1;
        Inventory.transform.GetChild(0).gameObject.SetActive(false);
        Inventory.transform.GetChild(1).gameObject.SetActive(false);
        Inventory.GetComponent<RectTransform>().localPosition = new Vector3(0,-25,0);
    }

    public void ExitGame()
    {
        DataPersistenceManager.instance.SaveGame();
        DataPersistenceManager.instance.ChangeSelectedProfileId("");
        SceneManager.LoadScene("MainMenu");
        Destroy(DontDestroyOnLoad);
    }
}
