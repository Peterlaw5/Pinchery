using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class General_Controls : MonoBehaviour {

    // abstract: parent of PLAYER CONTROLS and IA CONTROLLER

    [HideInInspector]
    public string[] player_combo = new string[3]; // the list of items collected by the arrow of the player

    [HideInInspector]
    public float player_score; // the score of the player

    // general references
    [Header("GENERAL REFERENCES"), Tooltip("reference to the PLAYER SCORE ICON prefab")]
    public GameObject score_icon;

    [Tooltip("reference to the PLAYER COMBO ICON prefab")]
    public GameObject combo_icon;

    [Tooltip("reference to the PLAYER QUEST ICON prefab")]
    public GameObject quest_icon;

    [Tooltip("reference to the PLAYER STREAK COUNTER TEXT in the streak box of the CANVAS hierarchy")]
    public Text streak_text;

    [Tooltip("reference to the PLAYER STREAK FINAL TEXT prefab")]
    public GameObject streak_final_text;

    [Tooltip("reference to the PLAYER STREAK MISS TEXT prefab")]
    public GameObject streak_miss_text;

    [Tooltip("reference to the PLAYER STREAK WOW TEXT prefab")]
    public GameObject streak_wow_text;

    [Tooltip("offset for the wow messages")]
    public Vector3 wow_message_vertical_offset;

    [System.Serializable]
    public struct Wow_Text // the structure of the kill streak messages
    {
        public string message;
        public int streak_number;
    }
    [Header("STREAK MESSAGES"), Tooltip("insert what to say and how many streaks you have to get to achieve it")]
    public Wow_Text[] Streak_messages;

    [Tooltip("reference to the 3 FLAGS that indicates that the player collected a quest item")]
    public GameObject[] Flags;

    [Tooltip("reference to the ARROW HAND in the player's spine archer's bones hierarchy")]
    public GameObject arrow_hand;

    // special attack
    [Header("SPECIAL ATTACK"), Tooltip("reference to the character's special attack prefab")]
    public GameObject special_attack_prefab;

    [Tooltip("reference to the Special attack button spawner in the canvas hierarchy")]
    public GameObject special_attack_button_spawner;

    // combo streak settings
    [Header("COMBO REPRESENTATION STREAK SETTINGS"), Tooltip("how much the streak text is scaled when combo is updated - example: (1.5) = scaled by 150% ")]
    public float streak_bump_scale_factor;
    [Tooltip("how fast the scale of the streak is brought back to normal (amount reduced every frame)")]
    public float streak_scale_reduction;

    // misc
    [HideInInspector]
    public string[] player_quest = new string[3]; // the copy of the current quest, but for each single player in order to uncheck the collected item when hit

    [HideInInspector]
    public int collected_combo; // how many items the player has collected

    [HideInInspector]
    public int streak_count; // the streak of the player

    [HideInInspector]
    public GameObject current_arrow;  // the arrow currently in use for this player

    [HideInInspector]
    public bool arrow_ready = false; // the player loaded the new arrow

    [HideInInspector]
    public bool ready; // can be controlled

    [HideInInspector]
    public float charge; // charge animations mixing parameter

    [HideInInspector]
    public bool charged_special = false; // the special skill has been activated but not launched

    [HideInInspector]
    public float streak_at_activation; // strak count at the moment of the activation of the special skill
        
    public bool shadow_is_active; // the dark shadow effect 


    // CHECK COMBO -------------------------------------------------------------------------------------------------- check combo -----------------------------------------------------------------
    public void CheckMyCombo()
    {
        for(int i = 0; i < player_combo.Length; ++i)
        {
            if (player_combo[i] != null)
            {
                for(int x = 0; x < player_quest.Length; ++x)
                {
                    if(player_quest[x] != null && player_combo[i] == player_quest[x]) // an item has been found
                    {
                        Flags[x].SetActive(true); // his flag is turned on

                        this.player_quest[x] = null; // the item is no longer necessary for the quest

                        ++ collected_combo; // when 3 items are collected, the combo is complete

                        break;
                    }
                }                
            }
        }

        for (int i = 0; i < player_combo.Length; ++i) // clear the combo list after the check is finished
        {
            player_combo[i] = null;
        }
        
    }

    public void StreakUpdate()
    {
        if(streak_count >= 2)
        {
            streak_text.transform.parent.gameObject.SetActive(true);

            //streak_text.text = "x " + (Mathf.Floor( streak_count / 10 ) +1 ).ToString() + "."+ ( streak_count % 10 ).ToString();

            #region NEW CODE

            streak_text.text = "+ " + streak_count;

            #endregion


            streak_text.transform.parent.localScale = Vector3.one; // scale reset
            streak_text.transform.parent.localScale *= streak_bump_scale_factor; // scale set - bump effect

            // spawning of the WOW message
            if(streak_wow_text != null) // IA does not spawn wow messages
            {
                for (int i = Streak_messages.Length -1 ; i >= 0; --i)
                {
                    if (streak_count == Streak_messages[i].streak_number)
                    {
                        GameObject message = Instantiate(streak_wow_text, streak_text.transform.parent.transform.parent.transform); // the message is created
                        message.transform.position = streak_text.transform.position + wow_message_vertical_offset;
                        message.GetComponent<Text>().text = Streak_messages[i].message;
                        break;
                    }
                }
            }         
        }
        else
        {
            streak_text.transform.parent.gameObject.SetActive(false);
        }        
    }

    // LOAD SPECIAL ATTACK -------------------------------------------------------------------------------  load special attack --------------------------------------------------------
    public void LoadSpecialAttack()
    {
        if (current_arrow != null) // check arrow recently released
        {
            Destroy(current_arrow);            
        }

        // the special attack arrow is instantiated
        current_arrow = Instantiate(special_attack_prefab.GetComponent<Special_Attack_Button>().special_attack_arrow, new Vector3(0f, 100f, 0f), transform.rotation); // special arrow is spawned

        current_arrow.GetComponent<Final_attacks>().owner = this;

        current_arrow.transform.parent = arrow_hand.transform;

        current_arrow.transform.localPosition = new Vector3(0f, 1f, 0f);

        // streak count is put in memory at the moment of the launch of the skill
        if (streak_count > 1)
        {
            streak_at_activation = streak_count; //(1 + (Mathf.Floor(streak_count / 10))); // not reset?

            // spawn "infused" text
        }
        else
        {
            streak_at_activation = 0;
        }

        // streak is reset
        // streak_count = 0;
        // StreakUpdate();


        charged_special = true;

        
    }

}
