using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorBT : CompositeBT
{
    public override NodeStatusBT Execute()
    {
        foreach (NodeBT n in nodes)
        {
            NodeStatusBT status = n.Execute();

            switch (status)
            {
                case NodeStatusBT.SUCCESS:
                case NodeStatusBT.RUNNING:
                    return status;
            }
        }

        return NodeStatusBT.FAILURE;
    }
}
