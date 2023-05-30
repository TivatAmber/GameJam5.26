using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public enum Feature
{
    #region CanInWhich
    CanInFurnace,
    CanInAnvil,
    CanInWaterTank,
    CanInDesk,
    CanInSleep,
    CanInShop,
    #endregion
    #region WhatIsIt
    IsWorkBenches,
    IsFuel,
    IsMaterial,
    IsHardwork,
    IsPeople,
    IsBlade,
    #endregion
    #region Other
    #endregion
}

public enum Kind
{
    #region WorkBenches
    // Start 1
    Furnace = 1,
    Anvil,
    WaterTank,
    Desk,
    Sleep,
    Shop,
    #endregion
    #region Material
    // Start at 101
    IronOre = 101,
    Iron5,
    BladeEmbryo1,

    Health = 105,
    Gold,
    TiredHealth,
    Coal,
    Blade1,
    Blade2,
    Blade3,
    Iron4,
    Iron3,
    Iron2,
    Iron1,
    FloatCrystle, // ¸¡¾§
    Tale, // ´«ÑÔ
    BladeEmbryo2,
    BladeEmbryo3,
    #endregion
    #region People
    // Start at 1001
    Farmer = 1001,
    FlamboyantSamurai,
    Scholar,
    Ronin,
    Shopper,
    #endregion
    #region Memory
    // Start at 2001
    Memory1 = 2001,
    Memory2,
    Memory3,
    Memory4,
    Memory5,
    Memory6,
    Memory7,
    Memory8,
    Memory9,
    Memory10,
    Memory11,
    Memory12,
    Memory13,
    Memory14,
    Memory15,
    Memory16,
    Memory17,
    Memory18,
    Memory19,
    Memory20,
    Memory21,
    Memory22,
    Memory23,
    Memory24,
    Memory25,
    Memory26,
    Memory27,
    Memory28,
    Memory29,
    Memory30,
    Memory31,
    Memory32,
    Memory33,
    #endregion
}
public class BaseObject : MonoBehaviour
{
    public GameObject canvas;
    private List<GameObject> panels = new();
    [SerializeField] protected Kind kind;
    [SerializeField] protected int id;
    [SerializeField] protected List<Feature> tags = new();
    public float temperature;
    public float carbon;
    public float impurities;
    public float crispness;
    public float hardness;
    public int ID
    {
        get { return id; }
        set { id = value; }
    }
    public Kind Kind
    {
        get { return kind; }
        set { kind = value; }
    }

    protected void Start()
    {
        for (int i = 0; i < canvas.transform.childCount; i++) panels.Add(canvas.transform.GetChild(i).gameObject);
        canvas.SetActive(false);
    }
    public bool HasTag(Feature tag)
    {
        return tags.Contains(tag);
    }
    public void AddTag(Feature tag)
    {
        tags.Add(tag);
    }
    public void RemoveTag(Feature tag)
    {
        if (HasTag(tag)) { tags.Remove(tag); }
    }
}

