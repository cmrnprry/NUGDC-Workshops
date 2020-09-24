using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrol, Chase, Attack
}

public class EnemyController : MonoBehaviour
{
    public EnemyState state = EnemyState.Patrol;
    public Transform goal;
    public Transform[] patrolPoints;
    public float sightDistance;

    private Material mat;
    private int point = 0;
    private NavMeshAgent agent;
    private bool hasSeenPlayer = false;




    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mat = GetComponent<Renderer>().material;
    }

    void Patrol()
    {
        agent.destination = patrolPoints[point].position;

        if (hasSeenPlayer == true)
        {
            Debug.Log("I see");
            state = EnemyState.Chase;
        }

        if (transform.position.x == patrolPoints[point].position.x)
        {
            point++;

            if (point >= patrolPoints.Length)
            {
                point = 0;
            }
        }
    }

    void Chase()
    {
        Debug.Log("Chase");
        float distance = Vector3.Distance(transform.position, goal.position);

        if (hasSeenPlayer)
        {
            state = EnemyState.Attack;
            agent.isStopped = true;
        }
        else if (distance >= sightDistance)
        {
            state = EnemyState.Patrol;
        }
        else
        {
            agent.destination = goal.position;
            agent.isStopped = false;
        }
    }

    void Attack()
    {
        Debug.Log("Attack");
        mat.SetColor("_Color", Color.blue);
        float distance = Vector3.Distance(transform.position, goal.position);


        if (!hasSeenPlayer)
        {
            Debug.Log("back to chase");

            mat.SetColor("_Color", Color.red);

            state = EnemyState.Chase;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            hasSeenPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            hasSeenPlayer = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            default:
                Debug.LogError("Enemy has not returned to a state");
                break;
        }

    }
}
