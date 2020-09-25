using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControllerCorouintes : MonoBehaviour
{
    //The place that the agent will go to
    public Transform goal;

    //The points the Enemy will travel between
    public Transform[] patrolPoints;

    //The current state of the Enemy
    public EnemyState state = EnemyState.Patrol;

    //boolean to check if the player has been seen
    private bool hasSeenPlayer = false;

    //how far the enemy can see the player
    public float sightDistance = 5f;

    //The object to navigate the NavMesh
    private NavMeshAgent agent;

    //Current point the Enemy is going to
    private int point = 0;

    private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        //Setting the variablet to get the NavMesh Agent data
        agent = GetComponent<NavMeshAgent>();
        mat = GetComponent<Renderer>().material;

        //To start the state machine
        UpdateState();
    }

    IEnumerator Patrol()
    {
        agent.destination = patrolPoints[point].position;

        if (hasSeenPlayer == true)
        {
            state = EnemyState.Chase;
            UpdateState();
            yield break;
        }

        if (transform.position.x == patrolPoints[point].position.x)
        {
            //Wait 2 seconds before moving to the next point
            yield return new WaitForSecondsRealtime(2f);
            point++;

            if (point >= patrolPoints.Length)
            {
                point = 0;
            }
        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(Patrol());
    }

    IEnumerator Chase()
    {
        Debug.Log("Chase");
        float distance = Vector3.Distance(transform.position, goal.position);

        if (hasSeenPlayer)
        {
            state = EnemyState.Attack;
            UpdateState();
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

        yield return new WaitForEndOfFrame();
        StartCoroutine(Chase());
    }

    IEnumerator Attack()
    {
        Debug.Log("Attack");
        mat.SetColor("_Color", Color.blue);

        if (!hasSeenPlayer)
        {
            mat.SetColor("_Color", Color.red);

            state = EnemyState.Chase;
            UpdateState();
        }

        //Wait 2 seconds before "Attacking" again
        yield return new WaitForSecondsRealtime(2f);
        StartCoroutine(Attack());
    }

    // Update is called once per frame
    void UpdateState()
    {
        // agent.destination = goal.position;

        switch (state)
        {
            case EnemyState.Patrol:
                StartCoroutine(Patrol());
                break;
            case EnemyState.Chase:
                StartCoroutine(Chase());
                break;
            case EnemyState.Attack:
                StartCoroutine(Attack());
                break;
            default:
                Debug.LogError("Enemy has not returned to a state");
                break;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("I see the Player");
            hasSeenPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("I do not see the Player");

            hasSeenPlayer = false;
        }
    }
}
