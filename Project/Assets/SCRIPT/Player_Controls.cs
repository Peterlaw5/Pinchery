using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Controls : General_Controls {

    // INSERIRE COMPILAZIONE CONDIZIONALE - PC / ANDROID ? 

    //for graphic animation
    [Header("GRAPHIC SETTINGS"), Tooltip("reference to the SPINE MECANIM ARCHER in the player hierarchy")]
    public GameObject bow_animation;
   
    GameObject spec_atk; // reference to the created button that launches the special attack

    // shooting markers indicators
    [Header("BOW-CHARGE MARKERS SETTINGS"), Tooltip("reference to the RIGHT MARKER in player hierarchy")] 
    public GameObject right_marker;

    [Tooltip("reference to the LEFT MARKER in player hierarchy")]
    public GameObject left_marker;

    [Tooltip("the normal material for the markers (PROVVISORIO)")]
    public Sprite normal_marker_sprite;

    [Tooltip("the highlighted material for the markers (when they are joined) (PROVVISORIO)")]
    public Sprite highlighted_marker_sprite;

    [Tooltip("the distance the markers moves before changing material - according to the marker initial position in scene")]
    public float marker_movement_distance;

    Vector3 right_marker_initial_position;
    Vector3 left_marker_initial_position;

    // player shot settings
    [Header("SHOT SETTINGS"), Tooltip("how fast the arrow flies")]
    public float arrow_force;

    [Tooltip("how much time the arrow remains in game before the next arrow is loaded")]
    public float reload_time;

    [Space(10), Header("PLAYER MOVEMENTS SETTINGS"), Tooltip("how far do you have to drag before the arrow can be shot correctly")]
    public float load_distance;

    [Tooltip ("how far do you have to drag before the direction of the player will start changing")]
    public float turn_distance;

    // player area
    [Header("PLAYER INTERACTION AREA") , Tooltip("define the radius and the position of the interactable area of the player")]
    public float player_area;

    [Tooltip("positive: offsets to the right")]
    public float player_area_horizontal_offset;
    [Tooltip("positive: offsets upwards")]
    public float player_area_vertical_offset;

    Vector3 player_area_position; // the position of the player area
   
    float mouse_distance; // calculated to determinate the touchable player area

    // player rotation clamp
    [Header("ROTATION CLAMP"), Tooltip ("left angle clamp (degrees)")]
    public float left_angle;

    [Tooltip("right angle clamp (degrees)")]
    public float right_angle;

    // references
    [Space(10), Tooltip ("reference to the ARROW PREFAB")]
    public GameObject arrow;

    [Tooltip ("reference to the TOUCH MARKER in the player hierarchy")]
    public GameObject touch_marker;

    bool bow_grabbable = false; // the player finger is above the player when starting to load the arrow

    bool grab_bow;   // the player grabbed the bow

    Vector3 touch_marker_pos;     // position of the touch marker
    Vector3 mouse_pos;              // position of the mouse (screen)
    Vector3 mouse_world_pos;          // position of the mouse (world)
    Vector3 mouse_start_position;       // position of the clic (world)
  
    // orientation
    float angle;

    // Initialization
    void Start()
    {
        // creation of the button for the special attack in the UI
        spec_atk = Instantiate(special_attack_prefab, special_attack_button_spawner.transform);
        spec_atk.GetComponentInChildren<Special_Attack_Button>().owner = this;

        //hides the touchpoint
        touch_marker.transform.GetChild(0).gameObject.SetActive(false);

        // initial position of the markers is memorized
        right_marker_initial_position = right_marker.transform.localPosition;
        left_marker_initial_position = left_marker.transform.localPosition;

        player_area_position = new Vector3(transform.position.x + player_area_horizontal_offset, transform.position.y + player_area_vertical_offset, transform.position.z);

        foreach (GameObject flag in Flags)
        {
            flag.gameObject.SetActive(false);
        }

        Load_Arrow();
        
        player_score = 0f;
    }

    // Update is called once per frame
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
        
        if(!ready && streak_count > 0)
        {
            streak_count = 0;
            StreakUpdate();
        }

        if (ready)
        {
            // MOUSE AND TOUCH MARKER POSITION CAPTURE --------------------------------------------------------------------------- mouse & markers --------------------------------------------------------
            mouse_pos = Input.mousePosition;
            mouse_world_pos = Camera.main.ScreenToWorldPoint(mouse_pos);
            mouse_world_pos.z = transform.position.z;

            touch_marker_pos = Camera.main.WorldToScreenPoint(touch_marker.transform.position);

            mouse_pos.z = touch_marker_pos.z;
            mouse_pos.x = mouse_pos.x - touch_marker_pos.x;
            mouse_pos.y = mouse_pos.y - touch_marker_pos.y;

            mouse_distance = Vector3.Distance(player_area_position, mouse_world_pos);
            
            
            // BOW CAN BE GRABBED ---------------------------------------------------------------------------------------------- bow grabbable ----------------------------------------------------------------
            if(Input.GetAxis("Fire1") <= 0 && grab_bow == false)
            {
                if (mouse_distance <= player_area)
                {
                    bow_grabbable = true;
                }
                else
                {
                    bow_grabbable = false;
                }
            }

            //BOW ANIMATION ------------------------------------------------------------------------------------------------- bow shot conditions --------------------------------------------------------
            if (grab_bow && (mouse_start_position.y - mouse_world_pos.y > 0))
            {
                charge = Vector3.Distance( new Vector3(0f, mouse_start_position.y, 0f), new Vector3(0f, mouse_world_pos.y, 0f)) / load_distance;

                if(charge > 1)
                {
                    charge = 1;
                }

                // markers material change (PROVVISORIO)
                if(charge >= 1) // markers are highlithed
                {
                    right_marker.GetComponent<SpriteRenderer>().sprite = highlighted_marker_sprite;
                    left_marker.GetComponent<SpriteRenderer>().sprite = highlighted_marker_sprite;
                }
                else // markers are not nighlighted
                {
                    right_marker.GetComponent<SpriteRenderer>().sprite = normal_marker_sprite;
                    left_marker.GetComponent<SpriteRenderer>().sprite = normal_marker_sprite;
                }

                // position of the markers are related to the charge of the bow
                right_marker.transform.localPosition = right_marker_initial_position - new Vector3(charge * marker_movement_distance, 0f, 0f);
                left_marker.transform.localPosition = left_marker_initial_position + new Vector3(charge * marker_movement_distance, 0f, 0f);

                bow_animation.GetComponent<Animator>().SetFloat("Charge", charge);
            }
            else
            {
                // position of the markers is reset
                right_marker.transform.localPosition = right_marker_initial_position;
                left_marker.transform.localPosition = left_marker_initial_position;

                bow_animation.GetComponent<Animator>().SetFloat("Charge", 0f);
            }

            //Prepare a new shot
            if (Input.GetAxis("Fire1") > 0)
            {
                if (arrow_ready && grab_bow)
                {
                    right_marker.SetActive(true);
                    left_marker.SetActive(true);

                    //rotazione dell'arciere
                    if (Vector3.Distance(mouse_start_position, mouse_world_pos) >= turn_distance && mouse_world_pos.y < mouse_start_position.y) // try fix !
                    {
                        angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;


                        touch_marker.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Clamp((angle + 90), -right_angle, left_angle))); //rotation clamped by angles
                        transform.rotation = touch_marker.transform.rotation;
                    }
                }
                //the bow is grabbed
                else if (bow_grabbable && arrow_ready && !grab_bow)
                {
                    grab_bow = true;
                    bow_grabbable = false;
                    
                    mouse_start_position = mouse_world_pos;
                    touch_marker.transform.position = mouse_start_position;
                }
            }
     
            //condition to release the arrow 
            else if (arrow_ready && grab_bow && (Input.GetAxis("Fire1") <= 0) && charge >= 1 && (mouse_start_position.y - mouse_world_pos.y > 0))
            {
                Shoot_Arrow();

                ResetAnimation();
                
                grab_bow = false;                
            }

            //condition for wrong shoot
            else if ((arrow_ready && grab_bow &&  (Input.GetAxis("Fire1") <= 0) && charge <  1))
            {
                grab_bow = false;

                ResetAnimation();                
            }
        }
        else // not ready
        {
            ResetAnimation();
        }
    }

    // LOAD ARROW ------------------------------------------------------------------------------------------------- load arrow --------------------------------------------------------
    private void Load_Arrow() 
    {
        // The standard arrow is instantiated
        current_arrow = Instantiate(arrow, new Vector3(0f, 100f, 0f), transform.rotation);  // arrow creation
        current_arrow.GetComponent<Arrow_Controller>().owner = gameObject;
        current_arrow.transform.parent = arrow_hand.transform;
        current_arrow.transform.localPosition = new Vector3(0f, 1f, 0f);
       
        arrow_ready = true;
    }

    //THE ARROW IS RELEASED ----------------------------------------------------------------------------------- shoot arrow ---------------------------------------------------------------------
    private void Shoot_Arrow()
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

        StartCoroutine(RestTime());           
    }

    // RESET ANIMATION -------------------------------------------------------------------------------------------------- reset animation ------------------------------------------------------------
    private void ResetAnimation()
    {
        //markers are reset and charge of the bow is reset too
        right_marker.SetActive(false);
        left_marker.SetActive(false);

        right_marker.transform.localPosition = right_marker_initial_position;
        left_marker.transform.localPosition = left_marker_initial_position;
        right_marker.GetComponent<SpriteRenderer>().sprite = normal_marker_sprite;
        left_marker.GetComponent<SpriteRenderer>().sprite = normal_marker_sprite;

        bow_animation.GetComponent<Animator>().SetFloat("Charge", 0f);
        charge = 0;
    }

    // the arrow is launched and after a certain time is destroyed
    IEnumerator RestTime() // PROVVISORIO
    {        
        yield return new WaitForSeconds(reload_time);
                
        Load_Arrow();
    }
}
