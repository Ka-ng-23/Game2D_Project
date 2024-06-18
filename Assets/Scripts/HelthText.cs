using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HelthText : MonoBehaviour
{
    //pixels per second
    public Vector3 moveSpeed = new Vector3(0, 80, 0);
    public float timeToFade = 1f;


    RectTransform textTransform;
    TextMeshProUGUI textMeshPro;

    private float timeElapsed;
    private Color startColor;

    private void Awake()
    {
        textTransform = GetComponent<RectTransform>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
        startColor = textMeshPro.color;
    }

    private void Update()
    {
        textTransform.position += moveSpeed * Time.deltaTime;

        timeElapsed += Time.deltaTime;


        if (timeElapsed < timeToFade)
        {
            float fadeAlpha = startColor.a * (1 - (timeElapsed / timeToFade));
            textMeshPro.color = new Color(startColor.r, startColor.b, startColor.g, fadeAlpha);
        }
        else
        {
            Destroy(gameObject);
        }
    }


}
