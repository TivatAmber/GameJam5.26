using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronOre : BaseObject, ISmelt
{
    #region ISmelt

    void ISmelt.ChangeTemperature(float delta)
    {
        temperature += delta;
    }
    #endregion
}
