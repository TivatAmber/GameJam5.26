using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleTimer : MonoBehaviour
{
    [Header("Set in Inspector")]
    [SerializeField] private Transform m_Image;
    [SerializeField] private Transform m_Text;
    [SerializeField] private int targetProcess = 10;
    private float currentAmout = 0;
    private bool working;
    public float CurrentAmout
    {
        get { return currentAmout; }
    }
    public bool Working
    {
        get { return working; }
    }
    void Update()
    {
        if (currentAmout < targetProcess)
        {
            currentAmout += Time.deltaTime;
            if (currentAmout > targetProcess)
            {
                currentAmout = targetProcess;
            }
            m_Text.GetComponent<TMPro.TextMeshProUGUI>().text = ((int)currentAmout).ToString() + "s";
            m_Image.GetComponent<Image>().fillAmount = currentAmout / targetProcess;
        }
        else
        {
            working = false;
            gameObject.SetActive(false);
        }
    }
    public void StartTimer(int maxTime)
    {
        gameObject.SetActive(true);
        targetProcess = maxTime;
        currentAmout = 0.0f;
        working = true;
    }
    public void InterruptTimer()
    {
        gameObject.SetActive(false);
        working = false;
    }
}
