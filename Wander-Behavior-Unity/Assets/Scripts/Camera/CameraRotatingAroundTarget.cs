using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

/// <summary>
/// Script for the camera that rotates around an object.
/// </summary>
public class CameraRotatingAroundTarget : MonoBehaviour
{
    [Header("Target Object", order = 0)]
    public Transform target = null;

    [Space(30)]

    [Header("Distances To Target Object", order = 1)]
    public float minDistanceToTarget = 0.5f;
    public float maxDistanceToTarget = 5.0f;
    public float currentDistanceToTarget = 2.0f;

    [Space(30)]

    [Header("Angles", order = 2)]
    public float verticalAngleInDegrees = -45.0f;
    public float horizontalAngleInDegrees = 0.0f;

    [Space(30)]

    [Header("Speeds", order = 3)]
    public float rotationSpeed = 0.1f;
    public float zoomingSpeed = 1.2f;
    public float interpolationSpeed = 0.5f;

    private Vector3? lastMousePosition;

    private void Start()
    {
        SetPositionAndRotation(false);
    }

    private void LateUpdate()
    {
        ProcessInput();
        SetPositionAndRotation();
    }

    private void SetPositionAndRotation(bool interpolate = true)
    {
        Quaternion targetOffsetRotation = Quaternion.Euler(verticalAngleInDegrees, horizontalAngleInDegrees, 0);
        Vector3 targetOffset = targetOffsetRotation * Vector3.forward;
        Vector3 newPosition = target.position + targetOffset;

        transform.position = interpolate ? Vector3.Lerp(transform.position, newPosition, interpolationSpeed) : newPosition;

        Vector3 dirToTarget = (target.position - transform.position).normalized;

        transform.position = target.position - dirToTarget * currentDistanceToTarget;

        transform.LookAt(target.position);
    }

    private void ProcessInput()
    {
        var mousePosition = Input.mousePosition;
        var rightMouseButtonClicked = Input.GetMouseButton(1);
        var mouseWheelEps = 0.0001f;
        var mouseWheelValue = Input.GetAxis("Mouse ScrollWheel");
        var mouseWheelScrolled = Mathf.Abs(mouseWheelValue) > mouseWheelEps;

        if (rightMouseButtonClicked)
        {
            if (lastMousePosition.HasValue)
            {
                float yDiff = lastMousePosition.Value.y - mousePosition.y;
                verticalAngleInDegrees += yDiff * rotationSpeed;
                verticalAngleInDegrees = Mathf.Clamp(verticalAngleInDegrees, -80.0f, 80.0f);

                float xDiff = lastMousePosition.Value.x - mousePosition.x;
                horizontalAngleInDegrees += xDiff * rotationSpeed;
                horizontalAngleInDegrees = CorrectAngle(horizontalAngleInDegrees);
            }

            lastMousePosition = mousePosition;
        }
        else
            lastMousePosition = null;

        if (mouseWheelScrolled)
        {
            currentDistanceToTarget -= mouseWheelValue * zoomingSpeed;

            currentDistanceToTarget = Mathf.Clamp(currentDistanceToTarget, minDistanceToTarget, maxDistanceToTarget);
        }
    }

    private float CorrectAngle(float value)
    {
        if (value < -360.0f)
            return 360.0f - (-360.0f - value);
        else if (value > 360.0f)
            return -360.0f + (value - 360.0f);
        return value;
    }
}