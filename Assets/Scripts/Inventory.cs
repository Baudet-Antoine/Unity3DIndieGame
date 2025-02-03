using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IDataPersistence
{
    public static Inventory Instance;
    
    void Awake()
    {
        Instance = this;
    }

    

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public List<Item> items = new List<Item>();

    public List<Weapon> FastInventory = new List<Weapon>();

    public List<Item> SideInventory = new List<Item>();
    public List<GameObject> FastInventorySlots;

    public int InventorySpace = 20;


    public void LoadData(GameData data)
    {
        items = data.Inventory;
        FastInventory = data.FastInventory;
        SideInventory = data.SideInventory;
    }

    public void SaveData(GameData data)
    {
        data.Inventory = items;
        data.FastInventory = FastInventory;
        data.SideInventory = SideInventory;
    }

    void InitializeFastInventory()
    {
        FastInventory[0] = PlayerController.Instance.CurrentWeapon;
        for(int i = 0 ; i < FastInventorySlots.Count ; i++)
        {  
            if(FastInventory[i])
            {
                FastInventorySlots[i].GetComponent<InventorySlot>().AddItem(FastInventory[i]);
            }
        }
    }
    
    void Start()
    {
        onItemChangedCallback.Invoke();
        InitializeFastInventory();
    }
    public bool Add(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null) 
            {
                items[i] = item; 
                if(onItemChangedCallback != null)
                {
                    onItemChangedCallback.Invoke();
                }
                return true; 
            }
        }

        return false;
    }   

    public void swapMaintoMain(int oldIndex, int newIndex)
    {
        Item tempItem = items[newIndex];
        items[newIndex] = items[oldIndex];
        items[oldIndex] = tempItem;
    }

    public void swapMainToFast(int oldIndex, int newIndex)
    {
        Item tempItem = FastInventory[newIndex];
        FastInventory[newIndex] = (Weapon)items[oldIndex];
        items[oldIndex] = tempItem;
    }

    public void swapFastToFast(int oldIndex, int newIndex)
    {
        Weapon tempItem = FastInventory[newIndex];
        FastInventory[newIndex] = FastInventory[oldIndex];
        FastInventory[oldIndex] = tempItem;
    }

    public void swapFastToMain(int oldIndex, int newIndex)
    {
        Weapon tempItem = FastInventory[oldIndex];
        FastInventory[oldIndex] = (Weapon)items[newIndex];
        items[newIndex] = tempItem;
    }


    public void Remove (Item item)
    {
        items.Remove(item);
        
        if(onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }
}
