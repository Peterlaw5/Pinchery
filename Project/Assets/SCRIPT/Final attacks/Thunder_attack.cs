using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder_attack : Final_attacks {

    // settings
    [Header("THUNDER ATTACK SETTINGS"), Tooltip("how much time (seconds) the thunder will remain active on the field once launched")]
    public float thunder_effect_duration;

    [Tooltip("reference to the THUNDER AREA prefab")]
    public GameObject thunder_area;
    public AudioSource thunder_sound;
    // misc
    bool shooted;

    // ARROW MOVEMENT ------------------------------------------------------------------------------------------------------------- arrow movement ------------------------------------------------
    private void Update()
    {
        if (active && !shooted)
        {
            thunder_sound.Play();

            GameObject thunder;
            thunder = Instantiate(thunder_area, transform.position, transform.rotation);
            thunder.GetComponent<Thunder_area>().area_owner = owner;
            thunder.GetComponent<Thunder_area>().area_score_multiplier = score_multiplier;

            shooted = true;

            Destroy(gameObject); // arrow is destroyed when launched
        }
    }
}
