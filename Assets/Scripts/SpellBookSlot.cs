using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellBookSlot : MonoBehaviour
{
    public Image icon;
    public Button removeButton;
    Spell item;
    public TextMeshProUGUI cdText;
    void Update()
    {
        if(item)
        {
            float remainingCooldown = item.nextCastTime - Time.time;
            if (remainingCooldown > 0)
            {
                cdText.text = $"{remainingCooldown:F1}";
            }
            else
            {
                cdText.text = "";
            }
        }
        
    }

    public void AddItem(Spell newItem)
    {
        item = newItem;

        //icon.sprite = item.icon;
        //icon.enabled = true;
        //removeButton.interactable = true;
    }

    public void ClearSlot ()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    public void UseItem()
    {
        if(item != null)
        {
            item.Use();
        }
    }
}
