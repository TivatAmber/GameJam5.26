using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using UnityEngine;

public class ProcessManager : Singleton<ProcessManager>
{
    [SerializeField] private bool[] memorys;
    private bool[] doingMemorys;
    private bool[] givenMemorys;
    private bool[] workBenches;
    private bool[] doingWorkBenches;
    private bool[] givenWorkBenches;
    private bool[] people;
    private bool[] doingPeople;
    private bool[] givenPeople;
    private bool needUpdate;
    private bool betweenSixAndSeven;

    #region Check
    private bool farmmerVsRonin = false;
    private bool poorEnd = false;
    private bool farmerKilled = false;
    private bool giveBlade = false;
    private bool win = false;
    private bool flower = false;
    private bool failToFight = false;
    private bool trueEnd = false;
    #endregion

    [SerializeField] private CircleTimer[] timers;
    public Dictionary<Kind, Blade> whoseBlade = new();
    Memory[] mems;
    private void Start()
    {
        memorys = new bool[33 + 1];
        doingMemorys = new bool[33 + 1];
        givenMemorys = new bool[33 + 1];
        workBenches = new bool[6 + 1];
        doingWorkBenches = new bool[6 + 1];
        givenWorkBenches = new bool[6 + 1];
        people = new bool[5 + 1];
        doingPeople = new bool[5 + 1];
        givenPeople = new bool[5 + 1];

        for (int i = 0; i < memorys.Length; i++) { memorys[i] = false; }
        for (int i = 0; i < givenMemorys.Length; i++) { givenMemorys[i] = false; }
        for (int i = 0; i < workBenches.Length; i++) { workBenches[i] = false; }
        for (int i = 0; i < givenWorkBenches.Length; i++) { givenWorkBenches[i] = false; }
        for (int i = 0;i < people.Length; i++) { people[i] = false; }
        for (int i = 0; i < givenPeople.Length; i++) { givenPeople[i] = false; }

        memorys[1] = doingMemorys[1]  = true;
        workBenches[5] = givenWorkBenches[5] = true;
        needUpdate = true;
        betweenSixAndSeven = false;

        #region Test
        //workBenches[1] = true;
        //memorys[12] = true;
        #endregion
    }
    private void Update()
    {
        if (poorEnd)
        {
            if (memorys[13] && !givenMemorys[13])
            {
                PrefabManager.Instance.GetObject(Kind.Memory13);
                givenMemorys[13] = true;
                if (!doingMemorys[14])
                    StartCoroutine(ChangeMemorys(14, 5));
            }
            if (memorys[14] && !givenMemorys[14])
            {
                PrefabManager.Instance.GetObject(Kind.Memory14);
                givenMemorys[14] = true;
            }
            return; // 穷死了
        }
        if (failToFight)
        {
            if (memorys[33] && !givenMemorys[33])
            {
                PrefabManager.Instance.GetObject(Kind.Memory33);
                givenMemorys[33] = true;
            }
            return;
        }
        if (trueEnd)
        {
            return;
        }
        Debug.Log(whoseBlade.Count);
        #region 新手教程
        if (memorys[1] && !givenMemorys[1])
        {
            if (needUpdate) mems = FindObjectsOfType<Memory>();
            foreach(Memory mem in mems)
            {
                if (mem.Kind == Kind.Memory1)
                {
                    givenMemorys[1] = true;
                    needUpdate = false;
                    if (!doingMemorys[2]) 
                        StartCoroutine(ChangeMemorys(2, 5));
                }
            }
        }
        if (memorys[2] && !givenMemorys[2])
        {
            PrefabManager.Instance.GetObject(Kind.Memory2);
            givenMemorys[2] = true;
            if (!doingMemorys[3]) 
                StartCoroutine(ChangeMemorys(3, 5));
            Sleep[] sleeps = FindObjectsOfType<Sleep>();
            foreach (Sleep sleep in sleeps)
            {
                Destroy(sleep.gameObject);
            }
        }
        if (memorys[3] && !givenMemorys[3])
        {
            PrefabManager.Instance.GetObject(Kind.Memory3);
            givenMemorys[3] = true;
            if (!doingWorkBenches[1]) 
                StartCoroutine(ChangeWorkBenches(1, 5));
        }
        if (workBenches[1] && !givenWorkBenches[1])
        {
            PrefabManager.Instance.GetObject(Kind.Furnace);
            givenWorkBenches[1] = true;
            if (!doingMemorys[4]) 
                StartCoroutine(ChangeMemorys(4, 5));
        }
        if (memorys[4] && !givenMemorys[4])
        {
            PrefabManager.Instance.GetObject(Kind.Memory4);
            givenMemorys[4] = true;
            if (!doingWorkBenches[2])
                StartCoroutine(ChangeWorkBenches(2, 5));
        }
        if (workBenches[2] && !givenWorkBenches[2])
        {
            PrefabManager.Instance.GetObject(Kind.Anvil);
            givenWorkBenches[2] = true;
            if (!doingMemorys[5])
                StartCoroutine(ChangeMemorys(5, 1));
            needUpdate = true;
        }
        if (memorys[5] && !givenMemorys[5])
        {
            if (needUpdate) mems = FindObjectsOfType<Memory>();
            foreach(Memory mem in mems)
            {
                if (mem.Kind == Kind.Memory5)
                {
                    givenMemorys[5] = true;
                    needUpdate = false;
                    if (!doingMemorys[6])
                        StartCoroutine(ChangeMemorys(6, 5));
                }
            }
        }
        if (memorys[6] && !givenMemorys[6])
        {
            PrefabManager.Instance.GetObject(Kind.Memory6);
            givenMemorys[6] = true;
        }
        if (givenMemorys[6] && !betweenSixAndSeven)
        {
            // One Health to TiredHealth
            var tmp = FindObjectsOfType<Health>();
            if (tmp.Length > 0)
            {
                Destroy(tmp[0].gameObject);
                PrefabManager.Instance.GetObject(Kind.TiredHealth);
                if (!doingWorkBenches[3])
                    StartCoroutine(ChangeWorkBenches(3, 5));
                betweenSixAndSeven = true;
            }
        }
        if (workBenches[3] && betweenSixAndSeven && !givenWorkBenches[3])
        {
            PrefabManager.Instance.GetObject(Kind.WaterTank);
            givenWorkBenches[3] = true;
            if (!doingMemorys[7])
                StartCoroutine(ChangeMemorys(7, 3));
        }
        if (memorys[7] && !givenMemorys[7]) {
            PrefabManager.Instance.GetObject(Kind.Memory7);
            givenMemorys[7] = true;
            if (!doingPeople[1])
                StartCoroutine (ChangePeople(1, 3));
            if (!doingWorkBenches[4])
                StartCoroutine(ChangeWorkBenches(4, 3));
            if (!doingWorkBenches[6])
                StartCoroutine(ChangeWorkBenches(6, 3));
        }
        if (workBenches[4] && !givenWorkBenches[4])
        {
            PrefabManager.Instance.GetObject(Kind.Desk);
            givenWorkBenches[4] = true;
        }
        if (workBenches[6] && !givenWorkBenches[6])
        {
            PrefabManager.Instance.GetObject(Kind.Shop);
            givenWorkBenches[6] = true;
        }
        #endregion

        #region 农夫
        if (people[1] && !givenPeople[1])
        {
            PrefabManager.Instance.GetObject(Kind.Farmer);
            givenPeople[1] = true;
            if (!doingMemorys[8])
                StartCoroutine(ChangeMemorys(8, 8));
            if (!doingPeople[5])
                StartCoroutine(ChangePeople(5, 270));
        }
        if (memorys[8] && !givenMemorys[8])
        {
            PrefabManager.Instance.GetObject(Kind.Memory8);
            givenMemorys[8] = true;
            if (!doingMemorys[9])
                StartCoroutine(ChangeMemorys(9, 1));
        }
        if (memorys[9] && !givenMemorys[9])
        {
            PrefabManager.Instance.GetObject(Kind.Memory9);
            givenMemorys[9] = true;
            if (!doingMemorys[10])
                StartCoroutine(ChangeMemorys(10, 5));
        }
        if (memorys[10] && !givenMemorys[10])
        {
            PrefabManager.Instance.GetObject(Kind.Memory10);
            givenMemorys[10] = true;
            if (!doingMemorys[11])
                StartCoroutine(ChangeMemorys(11, 1));
        }
        if (memorys[11] && !givenMemorys[11])
        {
            PrefabManager.Instance.GetObject(Kind.Memory11);
            PrefabManager.Instance.GetObject(Kind.IronOre);
            givenMemorys[11] = true;
            if (!doingMemorys[12])
                StartCoroutine(ChangeMemorys(12, 1));
        }
        if (memorys[12] && !givenMemorys[12])
        {
            PrefabManager.Instance.GetObject(Kind.Memory12);
            givenMemorys[12] = true;
            Farmer[] farmers = FindObjectsOfType<Farmer>();
            foreach (Farmer farmer in farmers)
            {
                StartCoroutine(farmer.WaitForBlade());
            }
        }
        if (whoseBlade.ContainsKey(Kind.Farmer))
        {
            people[2] = true;
            if (whoseBlade[Kind.Farmer].hardness < 1.0f || whoseBlade[Kind.Farmer].crispness > 2.0f)
            {
                poorEnd = true;
                memorys[13] = true;
                return;
            }
        }
        if (!poorEnd && whoseBlade.ContainsKey(Kind.Farmer) && whoseBlade.ContainsKey(Kind.Ronin))
        {
            farmmerVsRonin = true;
            if (whoseBlade[Kind.Ronin].hardness - 1.5f > whoseBlade[Kind.Farmer].hardness)
            {
                farmerKilled = true;
                memorys[15] = true;
                Farmer[] farmers = FindObjectsOfType<Farmer>();
                foreach (Farmer farmer in farmers)
                {
                    Destroy(farmer.gameObject);
                }
                whoseBlade.Remove(Kind.Farmer);
            } 
            else
            {
                memorys[29] = true;
            }
        }
        if (farmmerVsRonin)
        {
            if (farmerKilled)
            {
                #region farmerKilled
                if (memorys[15] && !givenMemorys[15])
                {
                    PrefabManager.Instance.GetObject(Kind.Gold);
                    PrefabManager.Instance.GetObject(Kind.Gold);
                    PrefabManager.Instance.GetObject(Kind.Memory15);
                    givenMemorys[15] = true;
                }
                #endregion
            }
            else
            {
                #region farmerNotKilled
                if (memorys[29] && !givenMemorys[29])
                {
                    PrefabManager.Instance.GetObject(Kind.Memory29);
                    givenMemorys[29] = true;
                    people[3] = true;
                }
                #endregion
            }
        }
        #endregion

        #region 内家门客
        if (people[2] && !givenPeople[2])
        {
            PrefabManager.Instance.GetObject(Kind.IronOre);
            PrefabManager.Instance.GetObject(Kind.Coal);
            PrefabManager.Instance.GetObject(Kind.FlamboyantSamurai);
            givenPeople[2] = true;
            if (!doingMemorys[17])
                StartCoroutine(ChangeMemorys(17, 5));
        }
        if (memorys[17] && !givenMemorys[17])
        {
            PrefabManager.Instance.GetObject(Kind.Memory17);
            givenMemorys[17] = true;
            if (!doingMemorys[18])
                StartCoroutine(ChangeMemorys(18, 5));
        }
        if (memorys[18] && !givenMemorys[18])
        {
            PrefabManager.Instance.GetObject(Kind.Memory18);
            givenMemorys[18] = true;
            if (!doingMemorys[19])    
                StartCoroutine (ChangeMemorys(19, 5));
        }
        if (memorys[19] && !givenMemorys[19])
        {
            PrefabManager.Instance.GetObject(Kind.Memory19);
            givenMemorys[19] = true;
            FlamboyantSamurai[] FlamboyantSamurais = FindObjectsOfType<FlamboyantSamurai>();
            foreach (FlamboyantSamurai FlamboyantSamurai in FlamboyantSamurais)
            {
                StartCoroutine(FlamboyantSamurai.WaitForBlade());
            }
            if (!doingMemorys[20])
                StartCoroutine(ChangeMemorys(20, 60));
        }
        if (memorys[20] && !givenMemorys[20])
        {
            PrefabManager.Instance.GetObject(Kind.Memory20);
            givenMemorys[20] = true;
            if (!doingPeople[4])
                StartCoroutine(ChangePeople(4, 60));
        }
        #endregion

        #region 浪人
        if (people[4] && !givenPeople[4])
        {
            PrefabManager.Instance.GetObject(Kind.Ronin);
            givenPeople[4] = true;
            if (!doingMemorys[22])
                StartCoroutine(ChangeMemorys(22, 5));
        }
        if (memorys[22] && !givenMemorys[22])
        {
            PrefabManager.Instance.GetObject(Kind.Memory22);
            givenMemorys[22] = true;
            if (!doingMemorys[21])
                StartCoroutine(ChangeMemorys(21, 1));
        }
        if (memorys[21] && !givenMemorys[21])
        {
            PrefabManager.Instance.GetObject(Kind.IronOre);
            PrefabManager.Instance.GetObject(Kind.Memory21);
            givenMemorys[21] = true;
            if (!doingMemorys[23])
                StartCoroutine(ChangeMemorys(23, 2));
        }
        if (memorys[23] && !givenMemorys[23])
        {
            PrefabManager.Instance.GetObject(Kind.Memory23);
            givenMemorys[23] = true;
        }
        #endregion

        #region 志士
        if (people[3] && !givenPeople[3])
        {
            PrefabManager.Instance.GetObject(Kind.IronOre);
            PrefabManager.Instance.GetObject(Kind.Scholar);
            givenPeople[3] = true;
            if (!doingMemorys[30])
                StartCoroutine(ChangeMemorys(30, 10));
        }
        if (memorys[30] && !givenMemorys[30])
        {
            PrefabManager.Instance.GetObject(Kind.Memory30);
            givenMemorys[30] = true;
            if (!doingMemorys[31])
                StartCoroutine(ChangeMemorys(31, 10));
        }
        if (memorys[31] && !givenMemorys[31])
        {
            PrefabManager.Instance.GetObject(Kind.Memory31);
            givenMemorys[31] = true;

            Scholar[] scholars = FindObjectsOfType<Scholar>();
            foreach (Scholar scholar in scholars)
            {
                StartCoroutine(scholar.WaitForBlade());
            }
        }

        // TODO Test Shopper
        if (givenPeople[3] && !giveBlade)
        {
            if (whoseBlade.ContainsKey(Kind.Scholar))
            {
                giveBlade = true;
            }
        }

        if (givenPeople[3] && givenPeople[4] && givenPeople[2])
        {
            if (!win)
            {
                float ronin, scholar, flamboyantSamurai;
                if (whoseBlade.ContainsKey(Kind.Ronin)) ronin = whoseBlade[Kind.Ronin].hardness;
                else ronin = 3.0f;
                if (whoseBlade.ContainsKey(Kind.Scholar)) scholar = whoseBlade[Kind.Scholar].hardness;
                else scholar = 1.0f;
                if (whoseBlade.ContainsKey(Kind.FlamboyantSamurai)) flamboyantSamurai = whoseBlade[Kind.FlamboyantSamurai].hardness;
                else flamboyantSamurai = 1.0f;
                if (ronin + scholar > flamboyantSamurai * 5)
                {
                    win = true;
                    memorys[32] = true;
                } 
                else
                {
                    failToFight = true;
                    memorys[33] = true;
                }
            }
            else
            {
                if (memorys[32] && givenMemorys[32])
                {
                    PrefabManager.Instance.GetObject(Kind.Memory32);
                    givenMemorys[32] = true;
                    if (!doingMemorys[16])
                        StartCoroutine(ChangeMemorys(16, 10));
                }
                if (memorys[16] && !givenMemorys[16])
                {
                    flower = true;
                    PrefabManager.Instance.GetObject(Kind.Memory16);
                    givenMemorys[16] = true;
                    BaseObject[] objs = FindObjectsOfType<BaseObject>();
                    foreach(BaseObject obj in objs)
                    {
                        if (obj is Furnace || obj is Blade) continue;
                        Destroy(obj.gameObject);
                    }
                }
                if (flower)
                {
                    Blade[] objs = FindObjectsOfType<Blade>();
                    if (objs.Length == 0)
                    {
                        trueEnd = true;
                    }
                }
            }
        }
        
        #endregion

        #region 商人
        if (people[5] && !givenPeople[5])
        {
            PrefabManager.Instance.GetObject(Kind.Shopper);
            givenPeople[5] = true;
            if (!doingMemorys[26])
                StartCoroutine(ChangeMemorys(26, 1));
        }
        if (memorys[26] && !givenMemorys[26])
        {
            PrefabManager.Instance.GetObject(Kind.Memory26);
            givenMemorys[26] = true;
            if (!doingMemorys[28])
                StartCoroutine(ChangeMemorys(28, 5));
        }
        if (memorys[28] && !givenMemorys[28])
        {
            PrefabManager.Instance.GetObject(Kind.Memory28);
            givenMemorys[28] = true;
            if (!doingMemorys[27])
                StartCoroutine(ChangeMemorys(27, 5));
        }
        if (memorys[27] && !givenMemorys[27])
        {
            PrefabManager.Instance.GetObject(Kind.Memory27);
            givenMemorys[27] = true;
        }
        #endregion
    }
    IEnumerator ChangeMemorys(int id, int time)
    {
        foreach (CircleTimer timer in timers)
        {
            if (!timer.Working && !doingMemorys[id])
            {
                doingMemorys[id] = true;
                timer.StartTimer(time);
            }
        }
        yield return new WaitForSeconds(time);
        memorys[id] = true;
    }
    IEnumerator ChangeWorkBenches(int id, int time)
    {
        foreach (CircleTimer timer in timers)
        {
            if (!timer.Working && !doingWorkBenches[id])
            {
                timer.StartTimer(time);
                doingWorkBenches[id] = true;
            }
        }
        yield return new WaitForSeconds(time);
        workBenches[id] = true;
    }
    IEnumerator ChangePeople(int id, int time)
    {
        foreach (CircleTimer timer in timers)
        {
            if (!timer.Working && !doingPeople[id])
            {
                timer.StartTimer(time);
                doingPeople[id] = true;
            }
        }
        yield return new WaitForSeconds(time);
        people[id] = true;
    }
}
