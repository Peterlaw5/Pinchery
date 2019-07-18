using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Final_attacks : MonoBehaviour {

    // generic settings
    [HideInInspector]
    public General_Controls owner; // reference to the player that is the owner of this skill

    //score
    [Header("SCORE SETTINGS"), Tooltip("score multiplier for the targets hit by the skill")]
    public float score_multiplier;

    //misc
    [HideInInspector]
    public bool active;


    //START EFFECT ------------------------------------------------------------------------------ start effect ---------------------------------------------------------------------------------------
    public void StartEffect() 
    {
        // the final attack is activated when the special arrow is launched by the player
        active = true;
    }
}
