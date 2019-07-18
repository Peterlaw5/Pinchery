using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eruption_attack : Final_attacks {

    // settings
    [Header("ERUPTION ATTACK SETTINGS"), Tooltip("how fast the eruption arrow flies")]
    public float arrow_force;

    [Tooltip("reference to the ERUPTION FIXURE prefab")]
    public GameObject eruption_fixure;

    [Tooltip("how frequently the launched arrow spawns a terrain fixure")]
    public float fixure_frequency;

    [Tooltip("the Z axis point where to place the fixures in order to put them under the player and the targets")]
    public float fixure_Z_placement;

    [Tooltip("how much time (seconds) you want to wait after the FIRST FIXURE is created for it to erupt")]
    public float initial_eruption_delay;

    // misc
    bool shooted; // the special arrow is shooted by the player

    [HideInInspector]
    public List<GameObject> fixures = new List<GameObject>(); // list of fixures
    
    [HideInInspector]
    public int fixure_counter = 0; // counter for the "fixures" array

    [HideInInspector]
    public int erupted_fixure = 0; // counter for the fixureto erupt

    //sound
    public AudioSource fixure_sound;


    // arrow is pushed and starts to place the fixures on the ground
    private void Update()
    {
        if (active && !shooted)
        {
            GetComponent<Rigidbody>().AddForce(transform.up * arrow_force, ForceMode.Impulse);
            shooted = true;

            StartCoroutine(FixurePlacingPause()); // starts the couner to create the first fixure

            initial_eruption_delay += fixure_frequency;
        
            StartCoroutine(InitialEruptionDelay());
        }
    }

    // COLLISION WITH A WALL ------------------------------------------------------------------------------------------- wall collision ---------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (shooted && active)
        {
            if (other.gameObject.tag == "Wall") // the necro arrow hits a target or the wall and then it generates the necro bomb
            {
                

                GetComponent<Rigidbody>().velocity = (new Vector3(0f, 0f, 0f)); // the arrow stops

                PlaceFixure();

                foreach (Transform child in GetComponentsInChildren<Transform>()) // destroy all the children (removes images and particle effects)
                {
                    if(child.gameObject != this.gameObject)
                    {
                        Destroy(child.gameObject);
                    }                 
                }

                active = false;
            }
        }
    }

    private void PlaceFixure()
    {            
        if(active)
        {
            fixure_sound.Play();
            GameObject fixure;
            fixure = Instantiate(eruption_fixure, new Vector3(transform.position.x, transform.position.y, fixure_Z_placement), transform.rotation);
            fixure.GetComponent<Eruption_fixure>().fixure_owner = owner;
            fixure.GetComponent<Eruption_fixure>().fixure_score_multiplier = score_multiplier;
            fixure.GetComponent<Eruption_fixure>().arrow = this;

            fixures.Add(fixure);

            ++fixure_counter; // array index is incremented
            
            StartCoroutine(FixurePlacingPause());
        }              
    }

    IEnumerator FixurePlacingPause()
    {
        yield return new WaitForSeconds(fixure_frequency);
        PlaceFixure();
    }

    IEnumerator InitialEruptionDelay()
    {
        yield return new WaitForSeconds(initial_eruption_delay);

        fixures[0].GetComponent<Eruption_fixure>().Erupt(); 

    }

}
