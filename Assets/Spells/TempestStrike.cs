using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tempest Strike", menuName = "Inventory/Spell/Tempest Strike")]
public class TempestStrike : Spell
{
    public float slowAmount = 30; // 30% de ralentissement
    public float damagePerSecond = 10f;
    public float effectDuration = 5f;
    public float effectRadius = 10f; // Rayon de l'effet
    public GameObject vortexEffect; // Préfabriqué de l'effet visuel du vortex
    public AudioClip tempestSound;  // Son de l'effet
    public float growSpeed = 2f;    // Vitesse à laquelle le vortex grandit

    // Lancement du sort
    public override void CastSpell(GameObject target)
    {
        if (isFirstAttack || Time.time >= nextCastTime)
        {
            Vector3 spawnPosition = target.transform.position; // Créer le vortex à la position de la cible
            spawnPosition.y = 0; // S'assurer que le vortex est à une hauteur de 0
            GameObject vortex = Instantiate(vortexEffect, spawnPosition, Quaternion.identity); // Lancer l'effet visuel
            
            // Commencer avec une échelle de zéro (ou très petite) pour simuler l'émergence
            vortex.transform.localScale = Vector3.zero;

            // Lancer la coroutine pour appliquer les dégâts et agrandir le vortex
            PlayerController.Instance.StartCoroutine(ApplyTempestEffect(vortex, spawnPosition));

            nextCastTime = Time.time + cooldown; // Définir le prochain cast possible
        }
    }

    private IEnumerator ApplyTempestEffect(GameObject vortex, Vector3 centerPosition)
    {
        float elapsedTime = 0f;
        List<EnemyController> affectedEnemies = new List<EnemyController>();

        // Récupérer tous les ennemis dans la zone du vortex au début
        Collider[] hitColliders = Physics.OverlapSphere(centerPosition, effectRadius, LayerMask.GetMask("Enemy"));

        // Boucle de durée du sort
        while (elapsedTime < effectDuration)
        {
            // Agrandir progressivement le vortex vers sa taille finale
            if (vortex.transform.localScale.x < effectRadius)
            {
                // Ajuster la vitesse de croissance en fonction du temps
                vortex.transform.localScale += Vector3.one * growSpeed * Time.deltaTime;
            }

            // Vérifier à nouveau les colliders pour s'assurer que nous ne manquons pas d'ennemis qui entrent ou sortent
            hitColliders = Physics.OverlapSphere(centerPosition, effectRadius, LayerMask.GetMask("Enemy"));

            foreach (Collider collider in hitColliders)
            {
                EnemyController enemy = collider.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    // Si l'ennemi n'est pas déjà affecté, appliquer le ralentissement
                    if (!affectedEnemies.Contains(enemy))
                    {
                        affectedEnemies.Add(enemy); // Ajouter l'ennemi à la liste des affectés
                        enemy.ApplySlow(slowAmount); // Appliquer le ralentissement
                    }

                    // Appliquer les dégâts sur la durée
                    enemy.TakeDamage(damagePerSecond * Time.deltaTime, Color.white);
                }
            }

            // Vérifier les ennemis qui ne sont plus dans la zone d'effet
            foreach (EnemyController enemy in affectedEnemies.ToArray())
            {
                if (enemy && Vector3.Distance(enemy.transform.position, centerPosition) > effectRadius)
                {
                    enemy.SpeedReset(); // Rétablir la vitesse normale
                    affectedEnemies.Remove(enemy); // Retirer de la liste des affectés
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Attendre le prochain frame
        }

        // Effets terminés, détruire le vortex
        foreach (EnemyController enemy in affectedEnemies)
        {
            enemy.SpeedReset(); // Rétablir la vitesse normale des ennemis restants
        }

        Destroy(vortex);
    }
}
