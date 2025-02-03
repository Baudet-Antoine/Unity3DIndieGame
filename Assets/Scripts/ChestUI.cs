using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestUI : MonoBehaviour
{
    public GameObject CUI;

    public List<InventorySlot> ChestSlots;
    public List<Item> ChestInventory;

    // Start is called before the first frame update
    void Start()
    {  
        if(!CUI)
        {
            CUI = GameObject.Find("InsideCanvas").transform.Find("ChestUI").gameObject;
        }
        ChestInventory = this.gameObject.GetComponent<ChestContent>().Content;
    }

    public void InitializeChest()
    {
        
        hudController.Instance.CurrentChest = this.transform.GetSiblingIndex();
        ResetChest();
        for(int i = 0 ; i < ChestSlots.Count ; i++)
        {  
            if(GameManager.Instance.ChestList[hudController.Instance.CurrentChest].GetComponent<ChestUI>().ChestInventory[i])
            {
                GameManager.Instance.ChestList[hudController.Instance.CurrentChest].GetComponent<ChestUI>().ChestSlots[i].GetComponent<InventorySlot>().AddItem(ChestInventory[i]);
            }
        }
    }

    public void ResetChest()
    {
        for(int i = 0 ; i < ChestSlots.Count ; i++)
        {  
            ChestSlots[i].GetComponent<InventorySlot>().ClearSlot();
        }
    }

    // Update is called once per frame
    public void AddToChest(int oldIndex, int newIndex)
    {
        Item tempItem = ChestInventory[newIndex];
        ChestInventory[newIndex] = Inventory.Instance.items[oldIndex];
        Inventory.Instance.items[oldIndex] = tempItem;   
    }

    public void GetFromChest(int oldIndex, int newIndex)
    {
        Item tempItem = ChestInventory[oldIndex];
        ChestInventory[oldIndex] = Inventory.Instance.items[newIndex];
        Inventory.Instance.items[newIndex] = tempItem;
    }

    public void MoveInChest(int oldIndex, int newIndex)
    {
        Item tempItem = ChestInventory[oldIndex];
        ChestInventory[oldIndex] = ChestInventory[newIndex];
        ChestInventory[newIndex] = tempItem;
    }
}
