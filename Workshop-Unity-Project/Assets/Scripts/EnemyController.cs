﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrol, Chase, Attack
}

public class EnemyController : MonoBehaviour
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

        if (!hasSeenPlayer)
        {
            Debug.Log("back to chase");

            mat.SetColor("_Color", Color.red);

            state = EnemyState.Chase;
        }

    }

    // Update is called once per frame
    void Update()
    {
        // agent.destination = goal.position;

        switch (state)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                //Chase();
                break;
            case EnemyState.Attack:
                // Attack();
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
