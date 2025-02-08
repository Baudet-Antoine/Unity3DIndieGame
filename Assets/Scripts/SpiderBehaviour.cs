using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBehaviour : MonoBehaviour
{

    private GameObject Model;
    public float stopDistance = 2f;
    public float attackInterval = 1f;

    private GameObject target;
    private float distanceToPlayer;
    private bool isAttacking = false;
    private Coroutine attackCoroutine;
    private Animator animator;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    private EnemyController enemyController;

    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = gameObject.transform.parent.GetComponent<UnityEngine.AI.NavMeshAgent>();
        enemyController = GetComponent<EnemyController>();
        Model = enemyController.Model;
    }

    void Update()
    {
        MoveTowardsPlayer();
    }
    
    void MoveTowardsPlayer()
    {
        target = enemyController.FindClosestPlayer();

        if (target != null)
        {
            distanceToPlayer = Vector3.Distance(gameObject.transform.parent.transform.position, target.transform.position);
            // Si le joueur est à portée d'attaque
            if (distanceToPlayer <= stopDistance)
            {
                animator.SetBool("isRunning", false);
                navMeshAgent.isStopped = true;
                // Commencer l'attaque si ce n'est pas déjà fait
                if (!isAttacking)
                {
                    StartAttack();
                }

                // Rotation de l'ennemi pour faire face au joueur
                Vector3 direction = (target.transform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.Slerp(Model.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
                Model.transform.rotation = lookRotation;
            }
            else // Si le joueur est trop loin
            {
                animator.SetBool("isRunning", true);
                // Configurer la destination du NavMeshAgent
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(target.transform.position);
                navMeshAgent.speed = enemyController.speed;

                // Arrêter l'attaque si le joueur est trop éloigné
                if (isAttacking)
                {
                    StopAttack();
                }

                // Rotation de l'ennemi pour faire face au joueur tout en se déplaçant
                Vector3 direction = (target.transform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.Slerp(Model.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
                Model.transform.rotation = lookRotation;
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
            navMeshAgent.isStopped = true;
        }
    }


    void StartAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }
            attackCoroutine = StartCoroutine(AttackPlayer());
        }
    }

    private IEnumerator AttackPlayer()
    {
        animator.SetBool("isRunning", false);
        while (isAttacking)
        {
            animator.ResetTrigger("Attack1");
            animator.SetTrigger("Attack1");
            yield return new WaitForSeconds(attackInterval);
        }
    }

    private void Attack1()
    {
        if (target != null)
        {
            target.GetComponent<PlayerController>().TakeDamage(enemyController.AttackDamage, Color.white);
        }
    }

    void StopAttack()
    {
        if (isAttacking)
        {
            isAttacking = false;
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }
        }
    }

}
