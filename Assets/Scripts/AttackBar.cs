using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackBar : MonoBehaviour
{
    public Image bar;
    private bool isFirstAttack = true; // Détecte si c'est la première attaque

    void Update()
    {
        UpdateAttackBar();
    }

    void UpdateAttackBar()
    {
        Weapon currentWeapon = PlayerController.Instance.CurrentWeapon;

        if (Time.time >= currentWeapon.nextFireTime)
        {
            if (isFirstAttack && Input.GetButtonDown("Fire1")) // Vérifier si c'est la première attaque et que le joueur tire
            {
                isFirstAttack = false; // Marquer la première attaque comme effectuée
            }
        }
        // Si c'est la première attaque, la barre reste pleine
        if (isFirstAttack)
        {
            bar.fillAmount = 1f;
        }
        else{
            float attackPercent = 1 - ((currentWeapon.nextFireTime - Time.time) / currentWeapon.fireRate);
        attackPercent = Mathf.Clamp01(attackPercent);
        bar.fillAmount = attackPercent;
    
        }

        // Si l'attaque est prête (le temps est supérieur à nextFireTime)
        
    }
}
