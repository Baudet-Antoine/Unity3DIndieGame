using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBook : MonoBehaviour
{
    public static SpellBook instance;
    public SpellBookSlot[] spellSlots; 

    void Awake()
    {
        instance = this;
    }

    

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public List<Spell> spells;

    public int SpellBookSpace = 20;
    
    void Start()
    {
        for (int i = 0; i < spells.Count; i++)
        {
            spells[i].nextCastTime = Time.time;
            if (i < spellSlots.Length)
            {
                spellSlots[i].AddItem(spells[i]); 
            }
        }
    }

   void Update()
    {
        if(PlayerController.Instance.onCombat)
        {
            Vector3 targetPosition = GetMousePosition();

            if (spells.Count > 0 && Input.GetKeyDown("1"))
            {
                TryCastSpell(0, targetPosition); // Sort associé à la touche "1"
            }
            if (spells.Count > 1 && Input.GetKeyDown("2"))
            {
                TryCastSpell(1, targetPosition); // Sort associé à la touche "2"
            }
            if (spells.Count > 2 && Input.GetKeyDown("3"))
            {
                TryCastSpell(2, targetPosition); // Sort associé à la touche "3"
            }
            if (spells.Count > 3 && Input.GetKeyDown("4"))
            {
                TryCastSpell(3, targetPosition); // Sort associé à la touche "4"
            }
        }
        
    }

    // Fonction pour vérifier si le sort peut être lancé et le lancer
    void TryCastSpell(int spellIndex, Vector3 targetPosition)
    {
        Spell spell = spells[spellIndex];
        

        // Vérifier le cooldown du sort
        if (spell.isFirstAttack || Time.time >= spell.nextCastTime)
        {
            if(spell.isTargeted)
            {
                spell.CastSpell(PlayerController.Instance.FindClosestEnemyNearPosition(targetPosition)); // Passer la position de la souris
            }
            else
            {
                GameObject emptyObject = new GameObject("EmptySpellTarget"); 
                Destroy(emptyObject,spell.duration);
                emptyObject.transform.position = targetPosition;  
                spell.CastSpell(emptyObject);
            }
            spell.nextCastTime = Time.time + spell.cooldown; // Appliquer le cooldown après le lancement du sort
            spell.isFirstAttack = false;
        }
        else
        {
        }
    }

    // Méthode pour obtenir la position où la souris pointe sur le terrain
    Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point; // Retourne la position sur le terrain
        }

        return Vector3.zero; // Retourne une valeur par défaut si rien n'est touché
    }



    

    public bool Add (Spell spell)
    {
        if(spells.Count >= SpellBookSpace)
        {
           return false;
        }
        
        spells.Add (spell);
        if(onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
        
        return true;
    }
    
    public void Remove (Spell spell)
    {
        spells.Remove(spell);
        
        if(onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }
}
