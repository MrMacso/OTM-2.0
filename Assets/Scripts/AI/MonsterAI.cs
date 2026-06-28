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
    private bool hasAttacked;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    private void Start()
    {
        if (agent == null)
        {
            Debug.LogWarning($"{nameof(MonsterAI)} on {name} has no NavMeshAgent assigned.");
            enabled = false;
            return;
        }

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            player = playerObject != null ? playerObject.transform : null;
        }

        if (player == null)
        {
            Debug.LogWarning($"{nameof(MonsterAI)} on {name} has no player target assigned.");
            enabled = false;
            return;
        }

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

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            state = MonsterState.Idle;
            return;
        }

        Transform targetPoint = patrolPoints[currentPatrolIndex];

        if (targetPoint == null)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            return;
        }

        agent.SetDestination(targetPoint.position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
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
        if (hasAttacked)
        {
            return;
        }

        hasAttacked = true;
        Debug.Log("Monster attacked player.");
    }

    private bool CanDetectPlayer()
    {
        if (player == null)
        {
            return false;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer <= detectionRange;
    }

    private void EnterChase()
    {
        state = MonsterState.Chase;
        agent.speed = chaseSpeed;
    }
}
