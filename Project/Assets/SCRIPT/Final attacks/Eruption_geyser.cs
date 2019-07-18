using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eruption_geyser : MonoBehaviour {

    #region NEW CODE

    [Header("FX")]
    public GameObject targetDeathFX;

    #endregion

    [HideInInspector]
    public float geyser_score_multiplier; // transmitted by the arrow

    [HideInInspector]
    public General_Controls geyser_owner; // transmitted by the arrow

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

        add_score = Mathf.Floor (target.GetComponent<Food>().score_value * geyser_score_multiplier);
        add_score += geyser_owner.streak_at_activation;
        #region NEW CODE

        Instantiate(targetDeathFX, target.ClosestPoint(transform.position), Quaternion.identity);

        #endregion

        geyser_owner.player_score += add_score;

        //the score icon is generated
        GameObject score_icon;
        score_icon = Instantiate(geyser_owner.score_icon, new Vector3(target.transform.position.x, target.transform.position.y, -2f), Quaternion.identity);
        score_icon.GetComponent<Arrow_Score>().score = add_score;
        score_icon.transform.parent = null;

        // FINAL ATTACK CAN COMPLETE QUEST
        geyser_owner.player_combo[0] = target.GetComponent<Food>().identifier;

        Destroy(target.gameObject);
    }
}
