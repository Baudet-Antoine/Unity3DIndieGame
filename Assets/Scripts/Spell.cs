using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Inventory/Spell")]
public class Spell : Item
{
    public bool isPassive;
    public bool isFirstAttack = true;
    public bool isTargeted;
    public int manaCost;
    public float cooldown;
    public float range;
    public float duration;

    public GameObject visualEffect; 
    public GameObject impactEffect; 

    public float nextCastTime;
    public string passiveDescription;
    public string activeDescription;

    void Start()
    {
        // Initialisation possible ici si nécessaire
    }

    virtual public void CastSpell(GameObject target)
    {
        
    }

    private void ShowVisualEffect(Vector3 targetPosition)
    {
        if (visualEffect != null)
        {
            Instantiate(visualEffect, targetPosition, Quaternion.identity);
        }
    }

    private void ApplySpellEffect(GameObject target)
    {
        if (impactEffect != null)
        {
            Instantiate(impactEffect, target.transform.position, Quaternion.identity);
        }
    }

    public void ActivatePassive()
    {
        if (isPassive)
        {
        }
    }
}
