using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necrobomb_bomb : MonoBehaviour
{
    #region NEW CODE

    [Header("FX")]
    public GameObject targetDeathFX;

    #endregion

    [HideInInspector]
    public float bomb_score_multiplier; // transmitted by the arrow

    [HideInInspector]
    public General_Controls bomb_owner; // transmitted by the arrow

    //settings
    [Header("NECROBOMB EXPLOSION SETTINGS"), Tooltip("the initial scale of the bomb")]
    public float initial_scale;

    [Tooltip("the scale the bomb will reach")]
    public float final_scale;

    [Tooltip("how much the bomb increments its radius every frame")]
    public float radius_incrementation_speed;

    [Tooltip("how much time (seconds) the bomb will remain on stage AFTER it reached the final scale")]
    public float effect_duration;

    //misc
    bool scale_reached;

    void Start()
    {
        transform.localScale = Vector3.one * initial_scale;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale.x < final_scale)
        {
            transform.localScale += (Vector3.one * (Time.deltaTime/radius_incrementation_speed));

        }
        else if (!scale_reached)
        {
            transform.localScale = (Vector3.one * final_scale);
            //effect.localScale = (Vector3.one) * ((1 / transform.localScale.x) * 0.6f);

            scale_reached = true;
            StartCoroutine(EffectDisappearing());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Food") // if the colliders of the attack hit a target after the attack has been released
        {
            DestroyTarget(other);
        }
    }

    // DESTROY TARGET ---------------------------------------------------------------- destroy target ------------------------------------------------------------------------------------------------------
    void DestroyTarget(Collider target)
    {
        float add_score;

        add_score = Mathf.Floor(target.GetComponent<Food>().score_value * bomb_score_multiplier);
        add_score += bomb_owner.streak_at_activation;

        #region NEW CODE

        Instantiate(targetDeathFX, target.ClosestPoint(transform.position), Quaternion.identity);

        #endregion

        bomb_owner.player_score += add_score;

        //the score icon is generated
        GameObject score_icon;
        score_icon = Instantiate(bomb_owner.score_icon, new Vector3(target.transform.position.x, target.transform.position.y, -2f), Quaternion.identity);
        score_icon.GetComponent<Arrow_Score>().score = add_score;
        score_icon.transform.parent = null;

        // FINAL ATTACK CAN COMPLETE QUEST
        bomb_owner.player_combo[0] = target.GetComponent<Food>().identifier;

        Destroy(target.gameObject);
    }

    //EFFECT DISAPPEARING --------------------------------------------------------------- effect disappearing --------------------------------------------------------------------------------------------------
    IEnumerator EffectDisappearing()
    {
        yield return new WaitForSeconds(effect_duration);
        bomb_owner.shadow_is_active = false;
        Destroy(gameObject); // kill shadow 
    }
}
