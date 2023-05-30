using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iron : BaseObject, ISmelt
{
    #region ISmelt
    public void ChangeTemperature(float delta)
    {
        temperature += delta;
    }
    #endregion
}
