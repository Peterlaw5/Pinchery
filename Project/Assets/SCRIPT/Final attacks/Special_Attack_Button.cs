using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Special_Attack_Button : MonoBehaviour {

    #region NEW CODE

    [Header("FX EFFECTS")]
    public GameObject playerEffectDuringCharge;
    public GameObject buttonReadyFX;
    public GameObject buttonPressedFX;

    GameObject playerEffect;
    GameObject buttonReady;
    GameObject buttonEffect;

    #endregion

    public AudioSource button_special_power_click_sound;
    public AudioSource vortex_charging_sound;
    public AudioSource power_is_ready_sound;

    [Header("SPECIAL ATTACK SETTINGS"), Tooltip("reference to the SPECIAL ATTACK prefab of the character")]
    public GameObject special_attack_arrow;

    [Tooltip("how much time is required to load the special attack")]
    public float special_attack_reload_time;

    float special_attack_actual_time;

    public GameObject attack_shadow;

    [HideInInspector]
    public float streak_factor; // based on the skill count

    [HideInInspector]
    public bool ready = false;

    [HideInInspector]
    public bool power_activated;

    [Tooltip("how big the button becomes when the skill is ready. example: 1.5 = 150% of its size")]
    public float button_bump_scale_factor;

    [Tooltip("how fast the button resizes itself after the bump ")]
    public float button_scale_reduction;

    [HideInInspector]
    public General_Controls owner;

    public Image fill_image; // the image that will be filled on top of the button

    // Use this for initialization
    void Start ()
    {
        streak_factor = 1f;
        special_attack_actual_time = 0;
        fill_image.fillAmount = 0;
    }

	// POWER TIME DECREMENT --------------------------------------------------------------------------------------------------- cooldown ---------------------------------------------------
	void Update ()
    {
        if(!power_activated && owner.ready)
        {
            if (special_attack_actual_time < special_attack_reload_time) // cooldown
            {
                special_attack_actual_time += Time.deltaTime; //* streak_factor; // time of the attack is reduced according to the status of the kill streak

                fill_image.fillAmount = (special_attack_actual_time / special_attack_reload_time);
            }
            else if (!ready) // cooldown ends
            {
                special_attack_actual_time = special_attack_reload_time;
                transform.localScale *= button_bump_scale_factor;
                fill_image.fillAmount = 1;

                if (owner.GetComponent<IA_controller>() == null)
                    power_is_ready_sound.Play();

                #region NEW CODE

                if (owner.GetComponent<IA_controller>() == null && buttonReadyFX != null)
                {
                    buttonReady = Instantiate(buttonReadyFX);
                }

                #endregion

                ready = true;
            }
        }

        // bump effect resizing
        if(transform.localScale.z > 1)
        {
            transform.localScale -= Vector3.one * button_scale_reduction;
        }
        else if(transform.localScale.z < 1)
        {
            transform.localScale = Vector3.one;
        }
    }

    

    // SPECIAL ATTACK ACTIVATION - ON CLICK -------------------------------------------------------------------- power activation ---------------------------------------------------------
    public void PowerActivation()
    {
        if(owner.ready && ready && !power_activated && owner.charge < 1)
        {
            power_activated = true;

            #region NEW CODE

            if (playerEffectDuringCharge != null)
            {
                playerEffect = Instantiate(playerEffectDuringCharge, owner.transform.position + new Vector3(0f, 0f, 0.1f), Quaternion.identity);

                if (owner.GetComponent<IA_controller>() == null)
                    vortex_charging_sound.Play();
            }

            if (buttonPressedFX != null)
                buttonEffect = Instantiate(buttonPressedFX);

            #endregion

            if (owner.GetComponent<IA_controller>() == null)
                button_special_power_click_sound.Play();

            Debug.Log(owner.gameObject.name + " activated the special power: " + gameObject.name);
            //owner.shadow_is_active = true;
            StartCoroutine(PowerArrowInfusion());                    
        }      
    }

    IEnumerator PowerArrowInfusion()
    {
        yield return new WaitUntil(() => owner.arrow_ready == true);

        owner.LoadSpecialAttack(); // the player loads the special attack
        
        ready = false;
    }

    // POWER IS USED ---------------------------------------------------------------------------------------------- power is used -------------------------------------------------------
    public void PowerIsLaunched()
    {
        #region NEW CODE

        if (playerEffect != null)
        {
            if (owner.GetComponent<IA_controller>() == null)
                vortex_charging_sound.Stop();

            Destroy(playerEffect);
        }

        #endregion

        special_attack_actual_time = 0; // cooldown time is reset
        fill_image.fillAmount = 0f;

        owner.shadow_is_active = true;
        power_activated = false;
    }
}
