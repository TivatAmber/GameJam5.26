using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InChannel : Channel
{
    public void GiveObejct(GameObject obj)
    {
        if (BaseOpe.GetKind(obj) == 0)
        {
            Debug.Log("No Kind");
            return;
        }
        if (filled)
        {
            Debug.Log("This is filled");
            return;
        }
        var a = obj.transform.Find("Icon");
        Texture tmp = null;
        if (a != null) {
            var b = a.Find("RawImage");
            if (b != null)
            {
                var c = b.gameObject.GetComponent<RawImage>();
                if (c != null)
                {
                    tmp = c.texture;
                }
            }
        }
        if (tmp != null) rawImage.texture = tmp;
        if (GameManager.Instance.GiveObject(obj, target))
        {
            whatsIn = BaseOpe.GetKind(obj);
            filled = true;
        }
    }
    public bool ReturnObject()
    {
        rawImage.texture = tmpImage;
        if (whatsIn == 0) return false;
        BaseObject tmp = target.GetComponent<BaseObject>();
        switch (tmp.Kind)
        {
            case Kind.Furnace:
                Furnace furnace = target.GetComponent<Furnace>();
                filled = false;
                return furnace.GiveObject(whatsIn);
            case Kind.Anvil:
                Anvil anvil = target.GetComponent<Anvil>();
                filled = false;
                return anvil.GiveObject(whatsIn);
            case Kind.WaterTank:
                WaterTank waterTank = target.GetComponent<WaterTank>();
                filled = false;
                return waterTank.GiveObject(whatsIn);
            case Kind.Desk:
                Desk desk = target.GetComponent<Desk>();
                filled = false;
                return desk.GiveObject(whatsIn);
            case Kind.Sleep:
                Sleep sleep = target.GetComponent<Sleep>();
                filled = false;
                return sleep.GiveObject(whatsIn);
            case Kind.Shop:
                Shop shop = target.GetComponent<Shop>();
                filled = false;
                return shop.GiveObject(whatsIn);
        }
        return false;
    }
    public void Clear()
    {
        ResetImage();
        filled = false;
        whatsIn = 0;
    }
}
