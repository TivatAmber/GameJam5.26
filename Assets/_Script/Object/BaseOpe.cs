using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BaseOpe {
    static public Kind GetKind(GameObject obj)
    {
        BaseObject tmp = obj.GetComponent<BaseObject>();
        if (tmp != null)
        {
            return tmp.Kind;
        }
        return 0;
    }
}
