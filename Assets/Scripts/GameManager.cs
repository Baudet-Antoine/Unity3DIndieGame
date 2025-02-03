using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<Item> ItemList;
    public List<GameObject> ChestList;

    public Item GetItemByID(int ID)
    {
        return ItemList.FirstOrDefault(item => item.itemID == ID);
    }

    void Start()
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
    public void Respawn()
    {
        PlayerController.Instance.gameObject.transform.position = new Vector3(0f, 0f, 0f);
        PlayerController.Instance.currentHealth = PlayerController.Instance.maxHealth;
        Time.timeScale = 1.0f;
        PlayerController.Instance.DyingScreen.SetActive(false);
    }

    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
