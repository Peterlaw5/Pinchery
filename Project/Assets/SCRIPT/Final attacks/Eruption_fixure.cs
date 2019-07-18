using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eruption_fixure : MonoBehaviour {


    [HideInInspector]
    public float fixure_score_multiplier; // transmitted by the arrow

    [HideInInspector]
    public General_Controls fixure_owner; // transmitted by the arrow

    [HideInInspector]
    public Eruption_attack arrow; // reference to the arrow

    // settings
    [Header("FIXURE SETTINGS"), Tooltip("reference to the ERUPTION GEYSER prefab")]
    public GameObject fixure_geyser;

    [Tooltip("how much time (seconds) the eruption of this fixure will remain on the field before it is switched off ")]
    public float eruption_duration;

    [Tooltip("how much time (seconds) the fixure will remain AFTER its eruption is finished")]
    public float effect_duration;

    [Tooltip("how much time (seconds) is required to erupt the NEXT GEYSER after the BEGINNING of this fixure eruption")]
    public float next_eruption_delay;

    [Tooltip("the Z axis point where to place the fixures in order to put them above the player and the targets")]
    public float eruption_Z_placement;
    
    //misc
    GameObject geyser;
    AudioSource eruption_sound;

    private void Start()
    {
        eruption_sound = GetComponent<AudioSource>();
    }

    public void Erupt()
    {
        eruption_sound.Play();
        geyser = Instantiate(fixure_geyser, new Vector3(transform.position.x, transform.position.y, eruption_Z_placement), Quaternion.identity);
        geyser.GetComponent<Eruption_geyser>().geyser_owner = fixure_owner;
        geyser.GetComponent<Eruption_geyser>().geyser_score_multiplier = fixure_score_multiplier;

        StartCoroutine(EruptionEnds());
        StartCoroutine(NextEruptionDelay());
    }

    //ERUPTION ENDS --------------------------------------------------------------- eruption ends --------------------------------------------------------------------------------------------------
    IEnumerator EruptionEnds()
    {
        yield return new WaitForSeconds(eruption_duration);

        Destroy(geyser); // the geyser is destroyed - animation?
        StartCoroutine(EffectDisappearing());
    }

    //EFFECT DISAPPEARING --------------------------------------------------------------- effect disappearing --------------------------------------------------------------------------------------------------
    IEnumerator EffectDisappearing()
    {
        yield return new WaitForSeconds(effect_duration);

        Destroy(gameObject); // the fixure is destroyed
    }

    //NEXT ERUPTION DELAY --------------------------------------------------------------- next eruption delay --------------------------------------------------------------------------------------------------
    IEnumerator NextEruptionDelay()
    {
        yield return new WaitForSeconds(next_eruption_delay);

        ++arrow.erupted_fixure; 
        
        if(arrow.erupted_fixure < arrow.fixure_counter) // if the next fixure is not null
        {
            arrow.fixures[arrow.erupted_fixure].GetComponent<Eruption_fixure>().Erupt(); // that fixure erupts
        }
        else
        {
            fixure_owner.shadow_is_active = false;
            Destroy(arrow.gameObject); // kill shadow 
        }
    }
}
