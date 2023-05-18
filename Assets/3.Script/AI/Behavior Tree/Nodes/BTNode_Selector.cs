using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTNode_Selector : BTNodeBase
{
    //���� �� ���� ��� ����
    protected override bool ContinueEvaluatingIfChildFailed()
    {
        return true;
    }
    //���� �� ����
    protected override bool ContinueEvaluatingIfChildSucceeded()
    {
        return false;
    }

    //���� ������Ʈ
    protected override void OnTickedAllChildren()
    {
        LastStatus = _children[_children.Count - 1].LastStatus;
    }
}
