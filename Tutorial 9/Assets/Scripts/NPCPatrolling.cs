using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPatrolling : MonoBehaviour
{
    public enum NPCStates
    {
        Patrol,
        Chase,
        Attack
    }
    public NPCStates CurrentState = NPCStates.Patrol;
    public Vector3[] Points;
    public Transform Player;
    public Bullet Bullet;
    public Material PatrolMaterial;
    public Material ChaseMaterial;
    public Material AttackMaterial;
    public float ChaseUpdateRate = 2f;
    public float ChaseRange = 5f;
    public float AttackRange = 2.5f;
    private NavMeshAgent navMeshAgent;
    private int nextPoint = 0;
    private MeshRenderer meshRenderer;
    private float nextShootTime = 0;
    private float nextChaseTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(Points[nextPoint]);
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {
            case NPCStates.Patrol:
                Patrol();
                break;
            case NPCStates.Chase:
                Chase();
                break;
            case NPCStates.Attack:
                Attack();
                break;
        }

    }

    private void Attack()
    {
        Debug.Log("Attack");

        // Reset Path so that the NPC does not move at players position
        navMeshAgent.ResetPath();

        // Change the material so that we can see when the NPC is in a different state
        meshRenderer.material = AttackMaterial;

        // Set position and rotation
        var position = transform.position + transform.forward;
        transform.LookAt(Player.position);

        // Check if Player is out of the Attarange
        if (Vector3.Distance(transform.position, Player.position) > AttackRange)
        {
            CurrentState = NPCStates.Chase;
        }

        // Apply shoot rate
        if (Time.time > nextShootTime)
        {
            var newBullet = Instantiate(Bullet.gameObject, position, transform.rotation);
            newBullet.GetComponent<Rigidbody>().velocity = transform.forward * Bullet.Speed;
            nextShootTime = Time.time + Bullet.FireRate;
        }
    }

    private void Chase()
    {
        Debug.Log("Chase");

        // Set new destination
        navMeshAgent.SetDestination(Player.position);

        // Change the material so that we can see when the NPC is in a different state
        meshRenderer.material = ChaseMaterial;

        // Check if Player is within attack range
        if (Vector3.Distance(transform.position, Player.position) < AttackRange)
        {
            CurrentState = NPCStates.Attack;
        }

        // Check if Player is out of the range
        if (Vector3.Distance(transform.position, Player.position) > ChaseRange)
        {
            Debug.Log("Out of the range - switching to Patrol state!");
            // Reset previous paths. This will allow NPC to immediatelly continue with patrolling as soon as Player is out of the chase range
            navMeshAgent.ResetPath();
            CurrentState = NPCStates.Patrol;
        }
        nextChaseTime = Time.time + ChaseUpdateRate;
    }

    private void Patrol()
    {
        // Check if Player is in the chase range
        if (Vector3.Distance(transform.position, Player.position) < ChaseRange)
        {
            CurrentState = NPCStates.Chase;
        }
        
        // Check if NPC is moving
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            meshRenderer.material = PatrolMaterial;
            Debug.Log("Patrol");
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
