using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : MonoBehaviour
{
    [SerializeField] List<GameObject> guides;
    private void Start()
    {
        OrderManager.Instance.canReceiveOrder = false;
        StartCoroutine(ShowGuide());
        for (int i = 0; i < guides.Count; i++)
        {
            guides[i].SetActive(false);
        }
    }
    IEnumerator ShowGuide()
    {
        for (int i = 0; i < guides.Count; i++)
        {
            guides[i].SetActive(true);
            if (i > 1) guides[i-1].SetActive(false);
            yield return new WaitForSeconds(3f);
        }
        for (int i = 0; i < guides.Count; i++) guides[i].SetActive(false);
        OrderManager.Instance.canReceiveOrder = true;
    }
}
