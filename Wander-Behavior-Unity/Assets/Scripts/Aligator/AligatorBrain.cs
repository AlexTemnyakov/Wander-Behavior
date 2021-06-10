using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AligatorBrain : MonoBehaviour
{
    public int seed;
    public float walkingSpeed;
    public float walkingRadius;
    public float maxWaitingTime;
    public float minWaitingTime;

    private System.Random random = null;
    private CompositeBT behaviour;
    private NavMeshAgent agent;
    private float currentSpeed;
    private bool isWaiting;

    private void Awake()
    {

    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        currentSpeed = agent.enabled ? agent.speed : -1f;

        if (isWaiting)
        {
            agent.enabled = false;

            return;
        }

        agent.enabled = true;

        behaviour.Execute();
    }

    public void Initialize()
    {
        random = new System.Random(seed);

        agent = GetComponent<NavMeshAgent>();

        behaviour = new SelectorBT();

        behaviour.AddNode(CreateNodeForWaiting());
        behaviour.AddNode(CreateNodeToSelectNewDestination());
        behaviour.AddNode(CreateNodeToWalk());

        for (int i = 0; i < 100; i++)
        {
            if (SetAgentTarget(NavMeshUtils.CreateRandomPoint(transform.position, 15f, random)))
            {
                break;
            }
        }
    }

    private ActionBT CreateNodeForWaiting()
    {
        return new ActionBT(() =>
        {
            return isWaiting ? NodeStatusBT.SUCCESS : NodeStatusBT.FAILURE;
        });
    }        

    private ActionBT CreateNodeToSelectNewDestination()
    {
        return new ActionBT(() =>
        {
            if (!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial || agent.remainingDistance < 0.1f)
            {
                Stand();

                if (SetAgentTarget(NavMeshUtils.CreateRandomPoint(transform.position, walkingRadius, random)))
                {
                    StartCoroutine(WaitFor((float)random.NextDouble() * (maxWaitingTime - minWaitingTime) + minWaitingTime));
                }

                return NodeStatusBT.SUCCESS;
            }

            return NodeStatusBT.FAILURE;
        });
    }

    private ActionBT CreateNodeToWalk()
    {
        return new ActionBT(() =>
        {
            Walk();

            return NodeStatusBT.SUCCESS;
        });
    }

    private ActionBT CreateNodeToStand()
    {
        return new ActionBT(() =>
        {
            Stand();

            return NodeStatusBT.SUCCESS;
        });
    }    

    private void Walk()
    {
        agent.speed = walkingSpeed;
    }

    private void Stand()
    {
        agent.speed = 0f;
    }

    private bool SetAgentTarget(Vector3 target)
    {
        NavMeshPath path = CalculatePath(transform.position, target);

        if (path != null)
        {
            agent.SetPath(path);

            return true;
        }
        else
        {
            return false;
        }
    }

    private NavMeshPath CalculatePath(Vector3 source, Vector3 target)
    {
        NavMeshHit hit;

        Vector3 sourceOnNavMesh = Vector3.zero;

        Vector3 targetOnNavMesh = Vector3.zero;

        if (NavMesh.SamplePosition(source, out hit, float.MaxValue, NavMesh.AllAreas) && hit.mask != NavMeshUtils.GetAreaMask(NavMeshUtils.crosswalkAreaNumber))
        {
            sourceOnNavMesh = hit.position;

            if (NavMesh.SamplePosition(target, out hit, float.MaxValue, NavMesh.AllAreas) && hit.mask != NavMeshUtils.GetAreaMask(NavMeshUtils.crosswalkAreaNumber))
            {
                targetOnNavMesh = hit.position;

                NavMeshPath path = new NavMeshPath();

                if (NavMesh.CalculatePath(sourceOnNavMesh, targetOnNavMesh, NavMesh.AllAreas, path))
                {
                    return path;
                }
            }
        }

        return null;
    }    

    private IEnumerator WaitFor(float seconds)
    {
        isWaiting = true;
        yield return new WaitForSeconds(seconds);
        isWaiting = false;
    }

    public float CurrentSpeed
    {
        get
        {
            return currentSpeed;
        }
    }
}
