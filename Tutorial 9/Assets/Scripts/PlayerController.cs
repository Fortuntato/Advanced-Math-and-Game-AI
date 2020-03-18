using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    public GameObject Pointer;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;

            if(Physics.Raycast(ray, out rayHit))
            {
                Debug.Log("Mouse position: " + Input.mousePosition);
                navMeshAgent.SetDestination(rayHit.point);
                Pointer.transform.position = rayHit.point;
            }
        }
    }
}
