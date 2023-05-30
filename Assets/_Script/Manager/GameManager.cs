using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject nowGameObject;
    private GameObject[] nowPanels;
    private GameObject tmpGameObject;

    private float higherZ = -1;
    private float lowerZ = 0;

    private int lstFrame;
    private int nowFrame;

    private void Start()
    {
        nowPanels = new GameObject[32];
        lstFrame = 0;
    }
    private void Update()
    {
        nowFrame += 1;
        if (OrderManager.Instance.nowOrder == Order.LeftShort)
        {
            if (nowGameObject == null)
            {
                if (!GetObject(out nowGameObject, 6)) {
                    if (!GetObject(out nowGameObject, 7)) {
                        if (GetObject(out tmpGameObject, 8))
                        {
                            Channel cha = tmpGameObject.GetComponent<Channel>();
                            if (nowFrame - lstFrame > 10)
                            {
                                lstFrame = nowFrame;
                                if (cha is OutChannel) HandleOutChannel(tmpGameObject);
                                else if (cha is InChannel) HandleWorkBenches(cha);
                            }
                        }
                    }
                }
            }
        }

        if (OrderManager.Instance.nowOrder == Order.LeftPressing || OrderManager.Instance.nowOrder == Order.LeftLong)
        {
            if (nowGameObject != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 dir = ray.direction * (1 / ray.direction.z);
                nowGameObject.transform.position = ray.origin + dir * (higherZ - ray.origin.z);
            }
        }

        if (OrderManager.Instance.nowOrder == Order.LeftLeave)
        {
            if (nowGameObject != null)
            {
                if (GetObject(out tmpGameObject, 8))
                {
                    GiveObject(nowGameObject, tmpGameObject);
                }
                nowGameObject.transform.position = nowGameObject.transform.position + new Vector3(0, 0, lowerZ - higherZ);
                nowGameObject = null;
            }

            if (OrderManager.Instance.pressingFrames <= 45)
            {
                for (int i = 6; i < 8; i++)
                {
                    if (nowPanels[i] != null && !GetObject(out tmpGameObject))
                    {
                        nowPanels[i].SetActive(false);
                        nowPanels[i] = null;
                    }
                    else
                    {
                        if (nowPanels[i] == null && GetObject(out tmpGameObject, i))
                        {
                            nowPanels[i] = tmpGameObject.GetComponent<BaseObject>().canvas;
                            if (nowPanels[i] != null) nowPanels[i].SetActive(true);
                        }
                    }
                }
            }
        }
    }
    private bool HandleOutChannel(GameObject nowChannel)
    {
        OutChannel tmpa = nowChannel.GetComponent<OutChannel>();
        if (tmpa == null) return false;
        return tmpa.OutObject();
    }
    private bool GetObject(out GameObject hitrecord, int layerMask = -1)
    {
        hitrecord = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (layerMask == -1 && Physics.Raycast(ray, out hitInfo, 1000)) hitrecord = hitInfo.collider.gameObject;
        if (Physics.Raycast(ray, out hitInfo, 1000, 1 << layerMask)) hitrecord = hitInfo.collider.gameObject;
        return hitrecord != null;
    }
    public bool GiveObject(GameObject a, GameObject b)
    {
        if (a == null || b == null) return false;
        BaseObject tmpa = a.GetComponent<BaseObject>(), tmpb = b.GetComponent<BaseObject>();
        InChannel tmpc = b.GetComponent<InChannel>();
        if (tmpa == null || (tmpb == null && tmpc == null)) return false;

        if (tmpb != null) {
            if (tmpb.Kind == Kind.Furnace && tmpa.HasTag(Feature.CanInFurnace)) {
                Furnace furnace = b.GetComponent<Furnace>();
                return furnace.ReceiveObject(a);
            }
            else if (tmpb.Kind == Kind.Sleep && tmpa.HasTag(Feature.CanInSleep))
            {
                Sleep sleep = b.GetComponent<Sleep>();
                return sleep.ReceiveObject(a);
            }
            else if (tmpb.Kind == Kind.Anvil && tmpa.HasTag(Feature.CanInAnvil))
            {
                Anvil anvil = b.GetComponent<Anvil>();
                return anvil.ReceiveObject(a);
            }
            else if (tmpb.Kind == Kind.Desk && tmpa.HasTag(Feature.CanInDesk))
            {
                Desk desk = b.GetComponent<Desk>();
                return desk.ReceiveObject(a);
            }
            else if (tmpb.Kind == Kind.WaterTank && tmpa.HasTag(Feature.CanInWaterTank))
            {
                WaterTank waterTank = b.GetComponent<WaterTank>();
                return waterTank.ReceiveObject(a);
            }
            else if (tmpb.Kind == Kind.Shop && tmpa.HasTag(Feature.CanInShop))
            {
                Shop shop = b.GetComponent<Shop>();
                return shop.ReceiveObject(a);
            }
        }
        if (tmpc != null)
        {
            tmpc.GiveObejct(a);
        }
        
        return false;
    }
    public bool HandleWorkBenches(Channel channel)
    {
        InChannel cha = channel.GetComponent<InChannel>();
        if (cha == null) return false;
        return cha.ReturnObject();
    }
    public bool OutWorkBenches(GameObject WorkBenches)
    {
        BaseObject tmp = WorkBenches.GetComponent<BaseObject>();
        switch (tmp.Kind)
        {
            case Kind.Furnace:
                Furnace furnace = WorkBenches.GetComponent<Furnace>();
                return furnace.OutObject();
            case Kind.Anvil:
                Anvil anvil = WorkBenches.GetComponent<Anvil>();
                return anvil.OutObject();
            case Kind.WaterTank:
                WaterTank waterTank = WorkBenches.GetComponent<WaterTank>();
                return waterTank.OutObject();
            case Kind.Desk:
                Desk desk = WorkBenches.GetComponent<Desk>();
                return desk.OutObject();
            case Kind.Sleep:
                Sleep sleep = WorkBenches.GetComponent<Sleep>();
                return sleep.OutObject();
            case Kind.Shop:
                Shop shop = WorkBenches.GetComponent<Shop>();
                return shop.OutObject();
        }
        return false;
    }
    private bool FindInterface<T>(GameObject gameObject, out T interfaceRecord)
        where T: class
    {
        interfaceRecord = default(T);
        MonoBehaviour[] tmp = gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour obj in tmp)
        {
            if (obj is T)
            {
                interfaceRecord = obj as T;
                return true;
            }
        }
        return false;
    }
}
