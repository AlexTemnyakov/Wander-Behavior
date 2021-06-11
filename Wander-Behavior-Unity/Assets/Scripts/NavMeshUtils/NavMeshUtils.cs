using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshUtils : MonoBehaviour
{
    public static int GetAreaMask(int areaNumber)
    {
        return 1 << areaNumber;
    }

    public static Vector3 CreateRandomPoint(Vector3 basePosition, float radius, System.Random random = null)
    {
        float angle;
        Vector3 vector;
        Vector3 targetPoint;

        if (random == null)
        {
            angle = UnityEngine.Random.value * 360.0f;
        }
        else
        {
            angle = (float)random.NextDouble() * 360.0f;
        }

        vector = Quaternion.Euler(0, angle, 0) * (new Vector3(1, 0, 1)).normalized * radius;
        targetPoint = basePosition + vector;

        return targetPoint;
    }

    public static Vector3 CreateRandomPoint(float radius, System.Random random = null)
    {
        return CreateRandomPoint(Vector3.zero, radius, random);
    }

    public static int GetAreaMaskOfPoint(Vector3 point, float eps = 0.5f)
    {
        NavMeshHit navMeshHit;

        if (NavMesh.SamplePosition(point, out navMeshHit, eps, NavMesh.AllAreas))
        {
            return navMeshHit.mask;
        }
        else
        {
            return -1;
        }
    }
}
