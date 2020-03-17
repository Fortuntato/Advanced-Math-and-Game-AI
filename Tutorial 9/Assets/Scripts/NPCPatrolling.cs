using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPatrolling : MonoBehaviour
{
    public Vector3[] Points;
    private NavMeshAgent navMeshAgent;
    private int nextPoint = 0;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(Points[nextPoint]);
    }

    // Update is called once per frame
    void Update()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            Debug.Log("Destination reached");
            nextPoint++;
            if (nextPoint >= Points.Length)
            {
                nextPoint = 0;
            }
            Debug.Log("Now heading to:" + Points[nextPoint]);
            
            navMeshAgent.SetDestination(Points[nextPoint]);
        }
    }
}
