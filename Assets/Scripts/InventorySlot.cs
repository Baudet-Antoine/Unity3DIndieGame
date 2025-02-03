using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public GameObject icon;
    public Button removeButton;
    public Item item;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        if (transform.GetChild(0).childCount == 0)
        {
            draggableItem.parentAfterDrag = transform.GetChild(0);
        }
        else
        {
            transform.GetChild(0).gameObject.transform.GetChild(0).SetParent(draggableItem.oldParent);
            draggableItem.parentAfterDrag = transform.GetChild(0);

        }
    }


    public void ReturnToInitialPosition(DraggableItem draggableItem)
    {
        draggableItem.transform.SetParent(draggableItem.parentAfterDrag);
        draggableItem.transform.position = draggableItem.parentAfterDrag.position;
    }
    public void AddItem(Item newItem)
    {
        icon = Instantiate(newItem.icon, transform.GetChild(0));

        item = newItem;
    }

    public void ClearSlot()
    {
        item = null;
        Destroy(icon);
        icon = null;
    }

    public void OnRemoveButton(Transform oldParent)
    {
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0f, 0.5f), Random.Range(-0.5f, 0.5f));
        GameObject Item = Instantiate(item.ModelGround, PlayerController.Instance.gameObject.transform.position + randomOffset, Quaternion.identity);
        
        Inventory.Instance.FastInventory[oldParent.parent.GetSiblingIndex()] = null;
            
        Destroy(icon);
    }

    public void UseItem()
    {
        if(item != null)
        {
            item.Use();
        }
    }
}
