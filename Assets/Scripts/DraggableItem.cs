using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag; 
    public Transform oldParent;
    public Image image;
    private Transform canvas;

    void Awake()
    {
        canvas = GameObject.Find("GlobalCanvas").transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        oldParent = transform.parent;
        parentAfterDrag = transform.parent; 
        transform.SetParent(canvas); 
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; 
    }

    void ReturnInitialPosition()
    {
        GameObject tempImage = parentAfterDrag.parent.GetComponent<InventorySlot>().icon;
        parentAfterDrag.parent.GetComponent<InventorySlot>().icon = oldParent.parent.GetComponent<InventorySlot>().icon;
        oldParent.parent.GetComponent<InventorySlot>().icon = tempImage;


        Item tempItem = parentAfterDrag.parent.GetComponent<InventorySlot>().item;
        parentAfterDrag.parent.GetComponent<InventorySlot>().item = oldParent.parent.GetComponent<InventorySlot>().item;
        oldParent.parent.GetComponent<InventorySlot>().item = tempItem;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!EventSystem.current.IsPointerOverGameObject(eventData.pointerId))
        {
            parentAfterDrag.parent.GetComponent<InventorySlot>().OnRemoveButton(oldParent);
            return;
        }

        if (parentAfterDrag != null)
        {
            transform.SetParent(parentAfterDrag);

            if (parentAfterDrag.childCount > 1)
            {
                transform.SetSiblingIndex(parentAfterDrag.childCount - 1);
            }
            else
            {
                transform.SetSiblingIndex(0);
            }

            transform.localPosition = Vector3.zero;
        }

        
        

        InventorySlot[] allSlots = parentAfterDrag.parent.transform.parent.GetComponentsInChildren<InventorySlot>();

        int newSlotIndex = -1;
        int oldSlotIndex = -1;

        // Parcours tous les slots et trouve celui où l'item a été déposé
        for (int i = 0; i < allSlots.Length; i++)
        {
            if (allSlots[i].transform.GetChild(0) == parentAfterDrag)
            {
                newSlotIndex = i;
            }
            if (allSlots[i].transform.GetChild(0) == oldParent)
            {
                oldSlotIndex = i;
            }
            if(newSlotIndex != -1 && oldSlotIndex != -1)
            {
                break;
            }
        }

        GameObject tempImage = oldParent.parent.GetComponent<InventorySlot>().icon;
        oldParent.parent.GetComponent<InventorySlot>().icon = parentAfterDrag.parent.GetComponent<InventorySlot>().icon;
        parentAfterDrag.parent.GetComponent<InventorySlot>().icon = tempImage;

        Item tempItem = oldParent.parent.GetComponent<InventorySlot>().item;
        oldParent.parent.GetComponent<InventorySlot>().item = parentAfterDrag.parent.GetComponent<InventorySlot>().item;
        parentAfterDrag.parent.GetComponent<InventorySlot>().item = tempItem;

        if(oldSlotIndex == -1)
        {
            oldSlotIndex = oldParent.transform.parent.GetSiblingIndex();
        }



        if(this.gameObject.transform.parent.transform.parent.tag == "MainInventorySlot" && oldParent.gameObject.transform.parent.tag == "MainInventorySlot")
        {
            Inventory.Instance.swapMaintoMain(oldSlotIndex,newSlotIndex);
        }
        else if(this.gameObject.transform.parent.transform.parent.tag == "FastInventorySlot"  && oldParent.gameObject.transform.parent.tag == "FastInventorySlot")
        {
            Inventory.Instance.swapFastToFast(oldSlotIndex,newSlotIndex);
        }
        else if(this.gameObject.transform.parent.transform.parent.tag == "FastInventorySlot"  && oldParent.gameObject.transform.parent.tag == "MainInventorySlot")
        {
            Inventory.Instance.swapMainToFast(oldSlotIndex,newSlotIndex);
        }
        else if(this.gameObject.transform.parent.transform.parent.tag == "MainInventorySlot"  && oldParent.gameObject.transform.parent.tag == "FastInventorySlot")
        {
            Inventory.Instance.swapFastToMain(oldSlotIndex,newSlotIndex);
        }
        else if(this.gameObject.transform.parent.transform.parent.tag == "SideInventorySlot")
        {
            Debug.Log("Not available Yet");
            transform.SetParent(oldParent);
            transform.position = oldParent.position;
        }




        
        else if(this.gameObject.transform.parent.transform.parent.tag == "ChestSlot"  && oldParent.gameObject.transform.parent.tag == "MainInventorySlot")
        {
            GameManager.Instance.ChestList[hudController.Instance.CurrentChest].GetComponent<ChestUI>().AddToChest(oldSlotIndex,newSlotIndex);
        }
        else if(this.gameObject.transform.parent.transform.parent.tag == "MainInventorySlot"  && oldParent.gameObject.transform.parent.tag == "ChestSlot")
        {
            GameManager.Instance.ChestList[hudController.Instance.CurrentChest].GetComponent<ChestUI>().GetFromChest(oldSlotIndex,newSlotIndex);
        }
        else if(this.gameObject.transform.parent.transform.parent.tag == "ChestSlot"  && oldParent.gameObject.transform.parent.tag == "ChestSlot")
        {
            GameManager.Instance.ChestList[hudController.Instance.CurrentChest].GetComponent<ChestUI>().MoveInChest(oldSlotIndex,newSlotIndex);
        }


        image.raycastTarget = true;
    }
}
