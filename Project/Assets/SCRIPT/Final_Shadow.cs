using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Final_Shadow : MonoBehaviour {

    SpriteRenderer shadow_image;

    [Tooltip("how much you want to darken the field; 1 = 100% black - 0 = no effect")]
    public float target_alpha;

    bool shadow_fade_in;

    public float fade_in_speed;

    [HideInInspector]
    public bool shadow_fade_out;

    public float fade_out_speed;

    private void Start()
    {
        shadow_image = gameObject.GetComponent<SpriteRenderer>();

        shadow_image.color *= new Color(1, 1, 1, 0); // alpha at start is reset
    }

    // match manager comands to fade in
    public void ShadowFadeIn()
    {
        shadow_fade_in = true;
    }
    
    // match manager comands to fade out
    public void ShadowFadeOut()
    {
        shadow_fade_out = true;
    }

    private void Update()
    {
        if (shadow_fade_in) // fade in
        {
            if (shadow_image.color.a < target_alpha)
            {
                shadow_image.color += new Color(0, 0, 0, (fade_in_speed * Time.deltaTime));
            }
            else
            {
                shadow_image.color = new Color(shadow_image.color.r, shadow_image.color.g, shadow_image.color.b, target_alpha);
                shadow_fade_in = false;
            }
        }
        else if (shadow_fade_out) // fade out
        {
            if (shadow_image.color.a > 0)
            {
                shadow_image.color -= new Color(0, 0, 0, (fade_out_speed * Time.deltaTime));
            }
            else
            {
                shadow_image.color = new Color(shadow_image.color.r, shadow_image.color.g, shadow_image.color.b, 0);
                shadow_fade_out = false;
            }
        }
    }
}
