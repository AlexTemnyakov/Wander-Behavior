using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelBT : CompositeBT
{
    public override NodeStatusBT Execute()
    {
        bool runningChild = false;

        foreach (NodeBT n in nodes)
        {
            NodeStatusBT status = n.Execute();

            switch (status)
            {
                case NodeStatusBT.SUCCESS:
                    continue;
                case NodeStatusBT.FAILURE:
                    return NodeStatusBT.FAILURE;
                case NodeStatusBT.RUNNING:
                    runningChild = true;
                    continue;
            }
        }

        return runningChild ? NodeStatusBT.RUNNING : NodeStatusBT.SUCCESS;
    }
}
