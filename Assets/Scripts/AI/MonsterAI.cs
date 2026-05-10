using UnityEngine;
using UnityEngine.AI;
public class MonsterAI : MonoBehaviour
{
    private enum MonsterState
    {
        Idle,
        Patrol,
        Chase,
        Attack
    }
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;
    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waitTimeAtPoint = 2f;
    [Header("Detection")]
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] private float attackRange = 1.5f;
    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    private MonsterState state;
    private int currentPatrolIndex;
    private float waitTimer;
    private void Start()
    {
        state = MonsterState.Patrol;
        agent.speed = patrolSpeed;
    }
    private void Update()
    {
        switch (state)
        {
            case MonsterState.Idle:
                UpdateIdle();
                break;
            case MonsterState.Patrol:
                UpdatePatrol();
                break;
            case MonsterState.Chase:
                UpdateChase();
                break;
            case MonsterState.Attack:
                UpdateAttack();
                break;
        }
    }
    private void UpdateIdle()
    {
        if (CanDetectPlayer())
        {
            EnterChase();
        }
    }
    private void UpdatePatrol()
    {
        if (CanDetectPlayer())
        {
            EnterChase();
            return;
        }
        if (patrolPoints.Length == 0) return;
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        agent.SetDestination(targetPoint.position);
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                waitTimer = 0f;
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            }
        }
    }
    private void UpdateChase()
    {
        agent.SetDestination(player.position);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            state = MonsterState.Attack;
        }
    }
    private void UpdateAttack()
    {
        Debug.Log("Monster attacked player.");
        // Later: trigger death screen, damage, animation, or jumpscare.
    }
    private bool CanDetectPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange)
        {
            return false;
        }
        return true;
    }
    private void EnterChase()
    {
        state = MonsterState.Chase;
        agent.speed = chaseSpeed;
    }
}
