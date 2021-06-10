using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceBT : CompositeBT
{
    public override NodeStatusBT Execute()
    {
        foreach (NodeBT n in nodes)
        {
            NodeStatusBT status = n.Execute();

            switch (status)
            {
                case NodeStatusBT.FAILURE:
                case NodeStatusBT.RUNNING:
                    return status;
            }
        }

        return NodeStatusBT.SUCCESS;
    }
}
