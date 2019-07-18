using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_text_movement : MonoBehaviour {

    [Header("TEXT SETTINGS")]
    public float upwards_speed;
    public float horizontal_speed;
    public float horizontal_offset;
    public float fade_speed;
    public float fade_treshold;

    [HideInInspector]
    public float score;

    Vector3 text_position;
    Text text;
    Outline outline;
    float alpha;

    // MESSAGE SETUP
    void Start()
    {
        text = GetComponent<Text>();

        if(GetComponent<Outline>() != null)
        {
            outline = GetComponent<Outline>();
        }
        else
        {
            outline = null;
        }
        
        alpha = text.color.a;
    }

    // MOVEMENT AND FADING
    void Update()
    {
        GetComponent<RectTransform>().position += transform.right * horizontal_speed * Time.deltaTime;
        GetComponent<RectTransform>().position += transform.up * upwards_speed * Time.deltaTime;

        alpha -= Time.deltaTime * fade_speed;

        if(alpha < fade_treshold)
        {
            fade_speed += 0.1f;
        }
      
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

        if (outline != null)
        {
            outline.effectColor *= new Color(1f, 1f, 1f, alpha);
        }

        if (alpha <= 0)
        {
            Destroy(gameObject);
        }
    }
}
