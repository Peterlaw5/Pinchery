using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necrobomb_attack : Final_attacks{

    // settings
    [Header("NECROBOMB ATTACK SETTINGS"), Tooltip("how fast the necro arrow flies")]
    public float arrow_force;

    [Tooltip("reference to the NECRO BOMB EXPLOSION prefab")]
    public GameObject necro_bomb;
    public AudioSource Necro_explosion;
    // misc
    bool shooted;

    // ARROW MOVEMENT ------------------------------------------------------------------------------------------------------------- arrow movement ------------------------------------------------
    private void Update()
    {
        if(active && !shooted)
        {
            GetComponent<Rigidbody>().AddForce(transform.up * arrow_force, ForceMode.Impulse);
            shooted = true;
        }
    }

    // COLLISION WITH A TARGET OR A WALL ------------------------------------------------------------------------------------------- target collision ---------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if(shooted)
        {
            if (other.gameObject.tag == "Food" || other.gameObject.tag == "Wall") // the necro arrow hits a target or the wall and then it generates the necro bomb
            {
                Necro_explosion.Play();

                GetComponent<Rigidbody>().velocity = (new Vector3(0f, 0f, 0f)); // the arrow stops

                GameObject bomb;
                bomb = Instantiate(necro_bomb, transform.position, Quaternion.identity); // necrobomb is created
                bomb.GetComponent<Necrobomb_bomb>().bomb_score_multiplier = score_multiplier;
                bomb.GetComponent<Necrobomb_bomb>().bomb_owner = owner;

                Destroy(gameObject);
            }
        }
        
    }
}
