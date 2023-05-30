using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class People : BaseObject
{
    [SerializeField] protected CircleTimer timer;
    [SerializeField] protected int waitingTime;
    public IEnumerator WaitForBlade()
    {
        timer.StartTimer(waitingTime);
        yield return new WaitForSeconds(waitingTime);
        Destroy(gameObject);
    }
    public void GetBlade(Blade blade)
    {
        StopCoroutine(WaitForBlade());
        timer.InterruptTimer();
        ProcessManager.Instance.whoseBlade.Add(kind, blade);
    }
    public void GetFloatCrystle(FloatCrystle floatCrystle)
    {
        StopCoroutine(WaitForBlade());
        timer.InterruptTimer();
    }
}
