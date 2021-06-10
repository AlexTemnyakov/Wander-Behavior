using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBT : NodeBT
{
    private Func<NodeStatusBT> action;

    public ActionBT(Func<NodeStatusBT> __action)
    {
        action = __action;
    }

    public override NodeStatusBT Execute()
    {
        return action.Invoke();
    }
}
