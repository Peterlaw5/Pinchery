using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder_area : MonoBehaviour {

    #region NEW CODE

    [Header("FX")]
    public GameObject targetDeathFX;

    #endregion

    [HideInInspector]
    public General_Controls area_owner; // transmitted by the arrow

    [HideInInspector]
    public float area_score_multiplier; // transmitted by the arrow

    // settings
    [Header("THUNDER AREA SETTINGS"), Tooltip("how much time (seconds) the thunder will remain on the field after the launch of the arrow")]
    public float effect_duration;

    // timer is started to delete the area effect
    private void Start()
    {
        StartCoroutine(EffectDisappearing());
    }

    // COLLISION WITH A TARGET ------------------------------------------------------------------------------------------- target collision ---------------------------------------------
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Food") // if the colliders of the attack hit a target after the attack has been released
        {
            DestroyTarget(other);
        }
    }

    void DestroyTarget(Collider target)
    {
        float add_score;

        add_score = Mathf.Floor(target.GetComponent<Food>().score_value * area_score_multiplier);
        add_score += area_owner.streak_at_activation;
        #region NEW CODE 

        Instantiate(targetDeathFX, target.ClosestPoint(transform.position), Quaternion.identity);

        #endregion

        area_owner.player_score += add_score;

        //the score icon is generated
        GameObject score_icon;
        score_icon = Instantiate(area_owner.score_icon, new Vector3(target.transform.position.x, target.transform.position.y, -2f), Quaternion.identity);
        score_icon.GetComponent<Arrow_Score>().score = add_score;
        score_icon.transform.parent = null;

        // FINAL ATTACK CAN COMPLETE QUEST
        area_owner.player_combo[0] = target.GetComponent<Food>().identifier;

        Destroy(target.gameObject);
    }

    //EFFECT DISAPPEARING --------------------------------------------------------------- effect disappearing --------------------------------------------------------------------------------------------------
    IEnumerator EffectDisappearing()
    {
        yield return new WaitForSeconds(effect_duration);
        area_owner.shadow_is_active = false;
        Destroy(gameObject); // kill shadow 
    }

}
