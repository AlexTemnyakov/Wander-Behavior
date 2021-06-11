using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class AligatorAnimatorController : MonoBehaviour
{
    public float animationSpeedFactor = 2f;

    private Animator animator;
    private AlligatorBrain alligatorBrain;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        animator.SetFloat("Speed", alligatorBrain.CurrentSpeed);
    }

    private void Initialize()
    {
        animator = GetComponent<Animator>();
        alligatorBrain = GetComponent<AlligatorBrain>();

    }
}
