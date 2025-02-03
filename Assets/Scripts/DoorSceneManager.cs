using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSceneManager : MonoBehaviour
{
    public void EnterTower()
    {
        StartCoroutine(LoadSceneAndSetPosition("Tower", new Vector3(0, 0, 0)));
        gameObject.SetActive(false);
        PlayerController.Instance.EnterBase();
    }

    public void LeaveTower()
    {
        StartCoroutine(LoadSceneAndSetPosition("WorldScene", new Vector3(0, 0, 0)));
        gameObject.SetActive(false);
        PlayerController.Instance.LeaveBase();
    }

    private IEnumerator LoadSceneAndSetPosition(string sceneName, Vector3 newPosition)
    {
        PlayerController.Instance.targetPosition = Vector3.zero;
        PlayerController.Instance.isMoving = false;
        PlayerController.Instance.onCombat = !PlayerController.Instance.onCombat;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        PlayerController.Instance.transform.position = newPosition;
    }
}
