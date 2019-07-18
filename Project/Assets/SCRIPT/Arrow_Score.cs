﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrow_Score : MonoBehaviour {

    [Header("ARROW SCORE TEXT SETTINGS")]
    public float upwards_speed;
    public float horizontal_speed;
    public float horizontal_offset;
    public float fade_speed;

    string score_message;

    [HideInInspector]
    public float score;


	// MESSAGE SETUP
	void Start ()
    {
        transform.localPosition += transform.right * horizontal_offset; // initial offset is applied to the start position

        if (score != 0)
        {
            score_message = "+" + score.ToString();
        }
        else
        {
            score_message = "MISS";
        }
        
        GetComponent<TextMesh>().text = score_message;

        StartCoroutine(FadeText(fade_speed, GetComponent<TextMesh>()));
	}
	
    // MOVEMENT OF THE TEXT
	void Update ()
    {
        transform.localPosition += transform.right * horizontal_speed * Time.deltaTime;
        transform.localPosition += transform.up * upwards_speed * Time.deltaTime;     
    }

    public IEnumerator FadeText(float fade_factor, TextMesh text) // 
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);

        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / fade_factor));
            yield return null;
        }

        Destroy(gameObject);
    }
}
