using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class AligatorAnimatorController : MonoBehaviour
{
    private Animator animator;
    private AligatorBrain aligatorBrain;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        animator.SetFloat("Speed", aligatorBrain.CurrentSpeed);
    }

    private void Initialize()
    {
        animator = GetComponent<Animator>();
        aligatorBrain = GetComponent<AligatorBrain>();
    }
}
