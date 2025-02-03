using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent; 
    Inventory inventory;
    

    InventorySlot[] slots;
    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.Instance;
        inventory.onItemChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    // Update is called once per frame 
    void Update()
    {
        
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count && inventory.items[i] != null)
            {
                if (slots[i].icon == null || !slots[i].icon.activeSelf)
                {
                    slots[i].AddItem(inventory.items[i]);
                }
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }




}
