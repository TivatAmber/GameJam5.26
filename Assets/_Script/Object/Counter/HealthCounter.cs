using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    private void Update()
    {
        text.text = FindObjectsOfType<Health>().Length.ToString();
    }
}
