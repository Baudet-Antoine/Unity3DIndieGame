using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class Collection : MonoBehaviour
{
    private Camera mainCamera;
    public GameObject Player; 
    public float detectionRadius = 5f; 
    private float speed = 0;
    public bool isHover;
    public bool MovingToPlayer;

    void Start()
    {
        mainCamera = Camera.main;
        
        Destroy(gameObject, 15);  
    }


    void Update()
    {
        if(!Player)
        {
            Player = PlayerController.Instance.gameObject;
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            if((Vector3.Distance(Player.transform.position, transform.position) < detectionRadius) && isHover)
            {
                MovingToPlayer = true;
            }
        }

        if(MovingToPlayer)
        {
            StartCoroutine(MoveToPlayer());
        }
        else
        {
            CheckPlayerInZone();
        }

       
    }

    void CheckPlayerInZone()
    {
        float distance = Vector3.Distance(Player.transform.position, transform.position);

        if (distance < detectionRadius)
        {
            if(gameObject.CompareTag("XP") || gameObject.CompareTag("Coin"))
            {
                MovingToPlayer = true;
            }
        }
    }

    void OnPlayerEnterZone()
    {
        
    }


    public IEnumerator MoveToPlayer()
    {
        if(Vector3.Distance(Player.transform.position, gameObject.transform.position) > 1)
        {
            speed += 0.2f;

            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, Player.transform.position, speed * Time.deltaTime);

            yield return null;
        }
        if(Vector3.Distance(Player.transform.position,gameObject.transform.position) <= 1)
        {
            MovingToPlayer = false;
            if(gameObject.CompareTag("XP"))
            {
                PlayerController.Instance.XP += 5;
                if (PlayerController.Instance.XP >= PlayerController.Instance.XPNeeded)
                {
                    PlayerController.Instance.LevelUp();
                }
                if(!PlayerController.Instance.XPBarManager)
                {
                    PlayerController.Instance.XPBarManager = GameObject.FindGameObjectWithTag("XPBar").GetComponent<XPBarManager>();
                }
                PlayerController.Instance.XPBarManager.UpdateXPBar();
                Destroy(gameObject);
            }
            else if(gameObject.CompareTag("Coin"))
            {
                PlayerController.Instance.Balance++;

                hudController.Instance.UpdateBalance();
                Destroy(gameObject);
            }
            else if(gameObject.CompareTag("Item"))
            {
                bool isPickedUp = Inventory.Instance.Add(transform.GetComponent<ItemHolder>().ItemData);
                
                if(isPickedUp)
                {
                    Destroy(gameObject);
                }
                
            }
        }

        yield return null;
    }
}
