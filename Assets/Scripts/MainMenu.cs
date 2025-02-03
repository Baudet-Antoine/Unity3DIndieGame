using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.SceneManagement;



public class MainMenu : MonoBehaviour
{
    
    public string savePath;
    public GameObject SaveButtonPrefab;
    public Transform Grid;
    public GameObject MainMenuParent;
    public GameObject SinglePlayerParent;
    public GameObject MultiPlayerParent;
    public GameObject SettingsParent;
    public ScrollRect scrollRect;
    public string InputFieldValue;
    public GameObject CreationErrorMessage;
    
    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "Saves");
        if (!System.IO.Directory.Exists(savePath))
        {
            System.IO.Directory.CreateDirectory(savePath);
        }
    }

    public void ReadInputField(string input)
    {
        InputFieldValue = input;
    }

    public void StartGame(string saveName)
    {
        DataPersistenceManager.instance.ChangeSelectedProfileId(saveName);
        SceneManager.LoadScene("Tower");
    }

    public void CreateGame()
    {
        if(InputFieldValue != null)
        {
            if(DataPersistenceManager.instance.NewGame(InputFieldValue.ToUpper()))
            {
                SceneManager.LoadScene("Tower");
            }
            else
            {
                CreationErrorMessage.SetActive(true);
            }
        }   
    }
    public void LeaveGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OpenMenu(GameObject menu)
    {
        MainMenuParent.SetActive(false);
        SinglePlayerParent.SetActive(false);
        MultiPlayerParent.SetActive(false);
        SettingsParent.SetActive(false);
        menu.SetActive(true);
    }

    public void SinglePlayerButton()
    {   
        OpenMenu(SinglePlayerParent);
        foreach (Transform child in Grid)
        {
            Destroy(child.gameObject);
        }
        string[] directories = System.IO.Directory.GetDirectories(savePath);
        Array.Sort(directories, (x, y) => File.GetLastWriteTime(y).CompareTo(File.GetLastWriteTime(x)));
        foreach (string directory in directories)
        {
            GameObject SaveButton = Instantiate(SaveButtonPrefab, Grid);
            SaveButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = Path.GetFileName(directory);

            SaveButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => StartGame(Path.GetFileName(directory)));
        }
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
