using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutChannel : Channel
{
    public bool OutObject()
    {
        rawImage.texture = tmpImage;
        return GameManager.Instance.OutWorkBenches(target);
    }
}
