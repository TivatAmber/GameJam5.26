using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] GameObject information;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] int timeInAndOut = 100;
    [SerializeField] bool wating = true;
    [SerializeField] bool end = false;
    [SerializeField] AudioSource audioSource;
    private void Start()
    {
        information.SetActive(false);
        wating = true;
        end = false;
        timeInAndOut = 100;
        Debug.Log(Time.timeScale);
    }
    private void Update()
    {
        //Debug.Log(Time.time);
        if (Input.anyKey && wating)
        {
            wating = false;
            information.gameObject.SetActive(true);
            StartCoroutine(ShowInformation());
        }

        if (end)
        {
            SceneManager.LoadScene(1);
        }
    }
    IEnumerator ShowInformation()
    {
        audioSource.Play();
        for (int i = 0; i <= timeInAndOut; i++)
        {
            //Debug.Log("Fir" + i.ToString());
            canvasGroup.alpha = 1.0f * i / timeInAndOut;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(2.0f);
        for (int i = timeInAndOut; i >= 0 ; i--)
        {
            canvasGroup.alpha = 1.0f * i / timeInAndOut;
            yield return new WaitForFixedUpdate();
        }

        end = true;
        yield break;
    }
}
