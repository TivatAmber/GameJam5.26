using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    private void Update()
    {
        text.text = FindObjectsOfType<Gold>().Length.ToString();
    }
}
