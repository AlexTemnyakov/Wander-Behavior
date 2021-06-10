using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SucceederBT : DecoratorBT
{
    public SucceederBT(NodeBT __node)
    {
        node = __node;
    }

    public override NodeStatusBT Execute()
    {
        node.Execute();
        switch (node.Execute())
        {
            case NodeStatusBT.SUCCESS:
            case NodeStatusBT.FAILURE:
                return NodeStatusBT.SUCCESS;
        }

        return NodeStatusBT.RUNNING;
    }
}
