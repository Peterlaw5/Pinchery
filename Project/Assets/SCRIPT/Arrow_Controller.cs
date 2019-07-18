using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Controller : MonoBehaviour {


    #region NEW CODE

    [Header("FX EFFECTS")]
    public GameObject HitFXRed;
    public GameObject HitFXBlue;
    public GameObject HitFXOnWall;
    public GameObject PoofFXTarget;
    public GameObject flying_arrow_effect_P1;
    public GameObject flying_arrow_effect_P2;

    #endregion

    [Header("ARROW COMPONENTS"), Tooltip ("reference to the HOOK 1 in the arrow hierarchy")]
    public GameObject Hook1;
    [Tooltip("reference to the HOOK 2 in the arrow hierarchy")]
    public GameObject Hook2;
    [Tooltip("reference to the HOOK 3 in the arrow hierarchy")]
    public GameObject Hook3;
    [Tooltip("reference to the HIT EFFECT prefab")]
    public GameObject HitFX;

    GameObject effect; // the spawned effect

    [Header("LIFETIME"), Tooltip("how much time (seconds) the arrow remains on the wall")]
    public float on_wall_time;

    // score
   [Space(10), Header("ARROW SCORE SETTINGS"), Tooltip ("how many points are ADDED if the arrow hits 2 targets")]
    public float X2_bonus;
    [Tooltip ("how many points are added if the arrow hits 3 targets")]
    public float X3_bonus;

    [Tooltip("if TRUE: an arrow with N targets will increment the streak by N")]
    public bool TEST_streak_per_target;

    [HideInInspector]
    public int streak_incrementation = 0; // how many units are added to the streak by the arrow

    [Tooltip ("how many points are ADDED per each streak point in the player streak count")]
    public float points_per_streak;

    // misc
    GameObject currentHook; // the hook where to insert the target
    GameObject score_icon;  // the reference to the owner's score icon

    float points_hook1 = 0f; // how many points are present in the hook 1
    float points_hook2 = 0f; // how many points are present in the hook 2
    float points_hook3 = 0f; // how many points are present in the hook 3

    [HideInInspector]
    public float arrow_points = 0f; // how many points are cumulated by the arrow
    [HideInInspector]
    public bool arrow_launched;  // if the arrow is no longer controllable by the player
    [HideInInspector]
    public bool active = false; // if the arrow can or cannot pick up targets
    [HideInInspector]
    public GameObject owner; // the owner of the arrow (can be the player or the opponent)

    [HideInInspector]
    public string[] ArrowCombo = new string[3]; // the list of the IDENTIFIERS of the targets captured by the arrow
    //arrow effect
    public GameObject flying_arrow_effect; // grafic effect
    public GameObject sprite; // sprite that have to been resized
   // public GameObject player;
    Vector3 velocity;
    public float stop_time;
    Transform switcher;

    public AudioSource arrow_sound;
    public AudioSource pin_hitted_sound;
    public AudioSource wall_hitted_sound;
    bool played;


    private void Update()
    {
        if(active && gameObject != null && flying_arrow_effect != null)
        {
            // flying_arrow_effect.SetActive(true); // when throwed activate the effect

            #region NEW CODE

            if (owner.GetComponent<IA_controller>() != null)
                flying_arrow_effect_P2.SetActive(true);
            else
                flying_arrow_effect_P1.SetActive(true);

            #endregion

            if(!played)
            {
                arrow_sound.Play();
                played = true;
            }

        }
    }


    // COLLISION WITH THE WALL AT THE END ---------------------------------------------------------------------------------------------- wall collision ---------------------------
    private void OnCollisionEnter(Collision collision) 
    {
        if(collision.gameObject.tag == "Wall" && active)
        {
            #region NEW CODE
            
            Instantiate(HitFXOnWall, collision.contacts[0].point, Quaternion.identity);

            #endregion
            wall_hitted_sound.Play();

            GetComponent<Rigidbody>().velocity = (new Vector3(0f, 0f, 0f)); // the arrow stops
            active = false; // the arrow is no longer capable of collecting other targets
            arrow_points = points_hook1 + points_hook2 + points_hook3; // final score of the arrow is summed

            // the multi-target bonus is applied
            if (points_hook3 != 0)
            {
                if(points_hook2 != 0)
                {
                    if(points_hook1 != 0) // three thargets hit
                    {
                        arrow_points += X3_bonus; // X3 bonus
                    }
                    else // two targets hit
                    {
                        arrow_points += X2_bonus;
                    }
                }
            }

            // strak bonus application
            if(streak_incrementation != 0)
            {               
                if (owner.GetComponent<General_Controls>().streak_count >= 2) // bonus is applied only if streak is at least X2
                {
                    // spawn "streak! "

                  //  float streak_multiplier = owner.GetComponent<General_Controls>().streak_count;
                  //  arrow_points *= (( Mathf.Round(streak_multiplier / 10) * points_per_streak) + 1); // streak bonus multiplier is applied to the points

                    #region NEW CODE

                    int streak_add = owner.GetComponent<General_Controls>().streak_count;
                    arrow_points += (streak_add * points_per_streak);

                    #endregion

                }
            }
            else
            {
                // spawn "streak broken"
                owner.GetComponent<General_Controls>().streak_count = 0;
                owner.GetComponent<General_Controls>().StreakUpdate(); // streak is updated with a little effect
            }

            StartCoroutine(ArrowDissappearing());
        }
    }

    // COLLISION WITH A TARGET ------------------------------------------------------------------------------------------- target collision ---------------------------------------------
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Food" && active && Hook1.transform.childCount == 0) // if the arrow is active and not full...
        {

            pin_hitted_sound.Play();
            //execute vfx hit

            //effect = Instantiate(HitFX, transform.position, Quaternion.identity);

            #region NEW CODE
            
            if (owner.GetComponent<IA_controller>() != null)
                effect = Instantiate(HitFXBlue, transform.position, Quaternion.identity);
            else
                effect = Instantiate(HitFXRed, transform.position, Quaternion.identity);

            #endregion            

            effect.transform.parent = other.transform;

            // streak incrementation
            owner.GetComponent<General_Controls>().streak_count += 1;
            ++streak_incrementation;
            owner.GetComponent<General_Controls>().StreakUpdate(); // streak is updated with a little effect

            // the streak text is moved near the hit target
            //owner.GetComponent<General_Controls>().streak_text.transform.parent.transform.position = Camera.main.WorldToScreenPoint(other.transform.position);


            Food target = other.GetComponent<Food>();
            // the target dies
            target.Death();
            target.Wings_Animation();
            target.Hit_animation();

            // the point where to hook the target is selected

           
            if (Hook3.transform.childCount == 0) // hook 3 is empty
            {
                Place_new_target(other); // place new target on the top of the arrow

                points_hook3 = other.GetComponent<Food>().score_value;
                ArrowCombo[0] = other.GetComponent<Food>().identifier;

                StartCoroutine(Rallenty());
            }
            else //hook3 is full
            {
                if(Hook2.transform.childCount == 0) // hook 2 is empty
                {
                    Move_Old_To_New(Hook3, Hook2);    // move old target to next position on the arrow
                    Place_new_target(other);

                    points_hook2 = other.GetComponent<Food>().score_value;
                    ArrowCombo[1] = other.GetComponent<Food>().identifier;

                    StartCoroutine(Rallenty());
                }
                else // hook 2 is full
                {
                    if(Hook1.transform.childCount == 0) // hook 1 is empty
                    {
                        Move_Old_To_New(Hook2, Hook1);
                        Move_Old_To_New(Hook3, Hook2);
                        Place_new_target(other);  

                        points_hook1 = other.GetComponent<Food>().score_value;
                        ArrowCombo[2] = other.GetComponent<Food>().identifier;

                        StartCoroutine(Rallenty());
                    }
                }
            }
        }
        
    }
    IEnumerator ArrowDissappearing()
    {
        // the combo is applied to the player
        owner.GetComponent<General_Controls>().player_combo = ArrowCombo;

        // score is applied to the player
        owner.GetComponent<General_Controls>().player_score += arrow_points;

        //the score icon is generated
        score_icon = Instantiate(owner.GetComponent<General_Controls>().score_icon, new Vector3(transform.position.x, transform.position.y, -2f), Quaternion.identity);
        score_icon.GetComponent<Arrow_Score>().score = arrow_points;
        score_icon.transform.parent = null;
        
        yield return new WaitForSeconds(on_wall_time);
        
        #region NEW CODE

        if (Hook1.transform.childCount != 0)
        {
            Instantiate(PoofFXTarget, Hook1.transform.position, Quaternion.identity);
        }

        if (Hook2.transform.childCount != 0)
        {
            Instantiate(PoofFXTarget, Hook2.transform.position, Quaternion.identity);
        }

        if (Hook3.transform.childCount != 0)
        {
            Instantiate(PoofFXTarget, Hook3.transform.position, Quaternion.identity);
        }

        #endregion

        Destroy(gameObject);

    }

    IEnumerator Rallenty()
    {
        if (gameObject != null)
        {
            if (!(gameObject.GetComponent<Rigidbody>().velocity.Equals(new Vector3(0, 1f, 0))))
            {
                velocity = gameObject.GetComponent<Rigidbody>().velocity;
            }
            
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 1f, 0f);
            yield return new WaitForSeconds(stop_time);
            gameObject.GetComponent<Rigidbody>().velocity = velocity;
        }
    }

    private void Place_new_target(Collider new_target) // place new target on the top of the arrow 
    {
        new_target.transform.SetParent(Hook3.transform, true);
        new_target.transform.localPosition = new Vector3(0f, 0f, 0f);
        new_target.enabled = false;
    }

    private void Move_Old_To_New (GameObject old_position, GameObject new_position)  // move old target to next position on the arrow
    {
        switcher = old_position.transform.GetChild(0).transform;
        switcher.transform.SetParent(new_position.transform, true);
        switcher.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

}
