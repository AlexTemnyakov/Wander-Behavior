using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Artificial Intelligence for the aligator.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class AlligatorBrain : MonoBehaviour
{
    [Header("Random Number Generator", order = 0)]
    [Tooltip("Value to initialize the random number generator.")]
    public int seed = 0;

    [Space(30, order = 1)]

    [Header("Walking Area", order = 2)]
    [Tooltip("Maximum distance from the start point.")]
    public float walkingRadius = 10;

    [Space(30)]

    [Header("Walking Speed", order = 3)]
    [Tooltip("Minimum walking speed.")]
    public float minWalkingSpeed = 0.2f;
    [Tooltip("Maximum walking speed.")]
    public float maxWalkingSpeed = 1.0f;

    [Space(30)]

    [Header("Idle time", order = 4)]
    [Header("When the aligator reaches the target point, it stands for some time.", order = 5)]
    [Tooltip("Minimum waiting time.")]
    public float minWaitingTime = 0.0f;
    [Tooltip("Maximum waiting time.")]
    public float maxWaitingTime = 10.0f;

    [Space(30)]

    [Header("Debugging", order = 6)]
    [Range(0, 2)]
    [Tooltip("Draw the sphere at the destination point in the editor. 0 - don't draw. 1 - draw always. 2 - draw when the object is selected.")]
    public byte drawSphereAtDestination = 0;
    [SerializeField]
    private float selectedTimeToWait;
    [SerializeField]
    private float selectedWalkingSpeed;

    private System.Random random = null;

    private CompositeBT behaviour = null;

    private NavMeshAgent agent = null;

    private bool isWaiting = false;    

    private Vector3 startPosition;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        behaviour.Execute();
    }

    private void Initialize()
    {
        startPosition = transform.position;

        random = new System.Random(seed);

        agent = GetComponent<NavMeshAgent>();
        agent.ResetPath();

        behaviour = new SelectorBT();

        behaviour.AddNode(CreateNodeForWaiting());
        behaviour.AddNode(CreateNodeToSelectNewDestination());
        behaviour.AddNode(CreateNodeToWalk());
    }

    private ActionBT CreateNodeForWaiting()
    {
        return new ActionBT(() =>
        {
            if (isWaiting)
            {
                Stand();

                return NodeStatusBT.SUCCESS;
            }
            else
                return NodeStatusBT.FAILURE;
        });
    }        

    private ActionBT CreateNodeToSelectNewDestination()
    {
        return new ActionBT(() =>
        {
            if (!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial || agent.remainingDistance < 0.1f)
            {
                Stand();

                selectedWalkingSpeed = Mathf.Lerp(minWalkingSpeed, maxWalkingSpeed, (float)random.NextDouble());

                if (SetAgentTarget(NavMeshUtils.CreateRandomPoint(startPosition, walkingRadius, random)))
                {
                    selectedTimeToWait = Mathf.Lerp(minWaitingTime, maxWaitingTime, (float)random.NextDouble());
                    StartCoroutine(WaitFor(selectedTimeToWait));
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

    private void Walk()
    {
        agent.speed = selectedWalkingSpeed;
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

        if (NavMesh.SamplePosition(source, out hit, float.MaxValue, NavMesh.AllAreas))
        {
            sourceOnNavMesh = hit.position;

            if (NavMesh.SamplePosition(target, out hit, float.MaxValue, NavMesh.AllAreas))
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

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (agent != null && drawSphereAtDestination == 1)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(agent.destination, 0.15f);
        }
#endif
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        if (agent != null && drawSphereAtDestination == 2)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(agent.destination, 0.15f);
        }
#endif
    }

    public float CurrentSpeed
    {
        get
        {
            return !isWaiting ? agent.speed : -1f;
        }
    }
}
