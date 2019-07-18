using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class    IA_controller : General_Controls {

    [Space(10), Header("IA_ACTIVATION"), Tooltip("use this to DEACTIVATE the IA player")]
    public bool STOP_IA;


    [Space(10), Header("ARROW SETTINGS")]
    public float arrow_force; // the speed given to the arrow
    public float reload_time; // fire rate

    [Space(10), Header("PLAYER MOVEMENTS SETTINGS")]
    public float load_distance;
    public float turn_distance;
    [Space(10)]
    public float left_angle;
    public float right_angle;
    public bool TEST_reset_direction;
    
    // IA variables
    bool is_indecise = true;
    bool is_waiting = false;
    bool charged = false;
    public Animator ia_animator;

    public float aim_min_time;
    public float aim_max_time;
    float aim_time;

    public float rotation_speed;
    float target_angle;
    bool target_angle_selected;

    // special attack
    GameObject spec_atk; // reference to the created button that launches the special attack
    public float final_min_time; // time for the IA to decide when to activate the final attack after it is ready
    public float final_max_time;

    float final_time;
    bool is_final_indecise;

    [Space(10)]
    public GameObject arrow;
    Vector3 object_pos;
    Vector3 mouse_pos;

    float angle;

    // Use this for initialization
    void Start()
    {

        // creation of the button for the special attack in the UI
        spec_atk = Instantiate(special_attack_prefab, special_attack_button_spawner.transform);
        spec_atk.GetComponentInChildren<Special_Attack_Button>().owner = this;

        foreach (GameObject flag in Flags)
        {
            flag.gameObject.SetActive(false);
        }

        angle = 0;

        charge = 0f;

        player_score = 0f;
    }

    // IA MOVEMENT
    void Update()
    {
        // scale reduction of the streak combo
        if (streak_text.transform.parent.GetComponent<RectTransform>().localScale.x > 1)
        {
            streak_text.transform.parent.localScale = (streak_text.transform.parent.localScale) - (streak_scale_reduction * Vector3.one);
        }
        else if (streak_text.transform.parent.GetComponent<RectTransform>().localScale.x < 1)
        {
            streak_text.transform.parent.localScale = Vector3.one;
        }

        if (!ready && streak_count > 0)
        {
            streak_count = 0;
            StreakUpdate();
        }

        // IA KILL SWITCH
        if (ready && !STOP_IA)
        {
            if (!arrow_ready && !is_waiting)
            {
                Load_Arrow();
            }

            //Prepare a new shot
            if (arrow_ready && is_indecise)
            {
                if(!target_angle_selected)
                {
                    target_angle = Random.Range(-right_angle, left_angle);
                    target_angle_selected = true;
                }               

                if(angle == target_angle)
                {
                    target_angle_selected = false;
                }
                else if (angle > target_angle)
                {
                    if((angle - rotation_speed) < target_angle)
                    {
                        angle = target_angle;
                    }
                    else
                    {
                        angle -= rotation_speed;
                    }
                        
                }
                else if(angle < target_angle)
                {
                    if ((angle + rotation_speed) > target_angle)
                    {
                        angle = target_angle;
                    }
                    else
                    {
                        angle += rotation_speed;
                    }
                }
                                      
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Clamp(angle, -right_angle, left_angle))); //rotation clamped by angles
            }           
            else if (arrow_ready && !is_indecise) //condition to release the arrow 
            {
                Shoot_Arrow();
                charge = 0f;
                ia_animator.GetComponent<Animator>().SetFloat("Charge", charge);
                
                StartCoroutine(ArrowDissappearing());
            }

        }
        // IA animation
        if(!charged && arrow_ready)
        {
                StartCoroutine(ReloadingAnimation());
                ia_animator.GetComponent<Animator>().SetFloat("Charge", charge);

                if(charge >= 1)
                {
                charge = 1;
                    charged = true;                
                }
        }


        // IA power activation
        if( !is_final_indecise && spec_atk.GetComponent<Special_Attack_Button>().ready)
        {
            StartCoroutine(FinalActivation());
            is_final_indecise = true;
        }
    }



    private void Shoot_Arrow() //THE ARROW IS RELEASED
    {
        if (current_arrow != null && charged) 
        {
            
            current_arrow.transform.parent = null;

            if (charged_special) // if attack is special
            {
                current_arrow.GetComponent<Final_attacks>().StartEffect(); // effect of the final attack is set active

                spec_atk.GetComponent<Special_Attack_Button>().power_activated = false;
                spec_atk.GetComponent<Special_Attack_Button>().PowerIsLaunched();

                charged_special = false;
            }
            else // normal arrow that flies
            {
                current_arrow.GetComponent<Arrow_Controller>().active = true;
                current_arrow.GetComponent<Rigidbody>().AddForce(transform.up * arrow_force, ForceMode.Impulse);
            }

            current_arrow = null;            
            arrow_ready = false;

            is_indecise = true;
        }
    }

    private void Load_Arrow() // THE ARROW IS LOADED
    {
        current_arrow = Instantiate(arrow, new Vector3(0f, 100f, 0f), transform.rotation);  // arrow creation
        current_arrow.GetComponent<Arrow_Controller>().owner = gameObject;
        current_arrow.transform.parent = arrow_hand.transform;
        current_arrow.transform.localPosition = new Vector3(0f, 1f, 0f);
        arrow_ready = true;

        StartCoroutine(AimingTime());
    }
    

    IEnumerator ArrowDissappearing()  // Current arrow is destroyed after a certain time
    {
        is_waiting = true;

        yield return new WaitForSeconds(reload_time);
        charged = false;
        is_waiting = false;
    }

    //AIMING TAKES TIME....
    IEnumerator AimingTime()
    {
        aim_time = (Random.Range(aim_min_time, aim_max_time));

        yield return new WaitForSeconds(aim_time);

        is_indecise = false;
    }

    //TO DECIDE WHEN ACTIVATE THE FINAL SKILL IF READY TAKES TIME....
    IEnumerator FinalActivation()
    {
        final_time = (Random.Range(final_min_time, final_max_time));

        yield return new WaitForSeconds(final_time);

        spec_atk.GetComponent<Special_Attack_Button>().PowerActivation();
        is_final_indecise = false;
    }

    // RELOAD ANIMATION
    IEnumerator ReloadingAnimation()
    {
        charge += 0.05f; // hardcoded!
        yield return new WaitForSeconds(0.5f);        
    }
}
