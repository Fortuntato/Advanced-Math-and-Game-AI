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
    public float AttachRange = 2.5f;
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
        navMeshAgent.ResetPath();
        meshRenderer.material = AttackMaterial;
        var position = transform.position + transform.forward;
        transform.LookAt(Player.position);
        if (Vector3.Distance(transform.position, Player.position) > AttachRange)
        {
            CurrentState = NPCStates.Chase;
        }
        if (Time.time > nextShootTime)
        {
            var newBullet = Instantiate(Bullet.gameObject, position, transform.rotation);
            newBullet.GetComponent<Rigidbody>().velocity = transform.forward * Bullet.Speed;
            nextShootTime = Time.time + Bullet.FireRate;
        }
    }

    private void Chase()
    {
        //if (Time.time < nextChaseTime)
        //{
        //    return;
        //}
        Debug.Log("Chase");
        navMeshAgent.ResetPath();
        meshRenderer.material = ChaseMaterial;
        navMeshAgent.SetDestination(Player.position);
        if (Vector3.Distance(transform.position, Player.position) < AttachRange)
        {
            CurrentState = NPCStates.Attack;
        }
        if (Vector3.Distance(transform.position, Player.position) > ChaseRange)
        {
            Debug.Log("Out of range!");
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
