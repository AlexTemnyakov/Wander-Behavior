using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverterBT : DecoratorBT
{
    public InverterBT(NodeBT __node)
    {
        node = __node;
    }

    public override NodeStatusBT Execute()
    {
        switch (node.Execute())
        {
            case NodeStatusBT.SUCCESS:
                return NodeStatusBT.FAILURE;
            case NodeStatusBT.FAILURE:
                return NodeStatusBT.SUCCESS;
        }

        return NodeStatusBT.RUNNING;
    }
}
