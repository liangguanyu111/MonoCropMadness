using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class character : MonoBehaviour
{
    public const int groundLayer = 7;

    public NavMeshAgent agent;
    private Animator an;


    public UnityAction aniTodo;
    public UnityAction actionTodo;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        an = this.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.layer == groundLayer)
                {
                    NavMeshHit meshHit;
                    if (NavMesh.SamplePosition(hit.point, out meshHit, 1.0f, NavMesh.AllAreas))
                    {
                        agent.isStopped = false;
                        an.SetBool("Move", true);
                        an.SetBool("Idle", true);
                        agent.SetDestination(hit.point);
                    }
                }
            }
        }
        if (an.GetBool("Move") &&agent.destination!=null && Vector3.Distance(this.transform.position, agent.destination) <= agent.stoppingDistance) 
        {
                          
            if (aniTodo != null)
            {
                aniTodo.Invoke();
                aniTodo = null;
            }
            an.SetBool("Idle", false);
            an.SetBool("Move", false);
            agent.isStopped = true;
        }
    }


    public void SetAni(string trigger) 
    {
        an.SetTrigger(trigger);
    }

    public void Action()
    {
        if (actionTodo != null)
        {
            an.SetBool("Idle", true);
            actionTodo.Invoke();
            actionTodo = null;
        }
    }


    IEnumerator action()
    {
        yield return new WaitForSecondsRealtime(4.0f);
        if (actionTodo != null)
        {
            actionTodo.Invoke();
            actionTodo = null;
            an.SetBool("Idle", true);
        }
    }
}
