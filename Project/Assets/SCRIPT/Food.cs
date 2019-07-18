using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {

    [Header("TARGET SETTINGS"), Tooltip("The basic score value of this target")]
    public float score_value; // the basic score of this target
    [Tooltip("how fast this target flies")]
    public float flight_speed;

    [HideInInspector]
    public float initial_speed; // a random speed

    public float probability;

    [Header("NAME"), Tooltip("insert an identifier (copy the name)")]
    public string identifier;

    [Header("RELATIVE ICON"), Tooltip("reference to the relative TARGET ICON")]
    public GameObject icon_prefab;

    [Header("DEAD / ALIVE REFERENCES"), Tooltip("reference to the TARGET_alive object in the target hierarchy")]
    public GameObject alive_state;

    [Tooltip("reference to the TARGET_dead object in the target hierarchy")]
    public GameObject dead_state;

    [Tooltip("reference to the TARGET_Hitted object in the target hierarchy")]
    public GameObject hit_state;

    [Tooltip("all the parts you want to detatch from the target when he dies (generally: all but top and bottom)")]
    public GameObject[] wings;
    public float wings_repulsive_force;
    public float wings_rotation_speed;
    bool start_scaling = false;
    float new_scale;
    float real_scaling;

    // TARGET MOVEMENT & estetic animation

    private void Start()
    {
        new_scale = wings[0].transform.localScale.x;
        real_scaling = new_scale * 0.01f;
    }
    void Update ()  // the food moves
    {
        if(transform.parent == null)
        {
            transform.localPosition += transform.right * initial_speed * Time.deltaTime;
        }       
        if(start_scaling==true)
        {
            for(int i = 0; i<wings.Length; i++)
            {
                wings[i].transform.localScale = new Vector3(new_scale, new_scale, 0);
            }
            StartCoroutine(WingsScaler());
        }
    }

    public void Death()
    {
        alive_state.SetActive(false);
        dead_state.SetActive(true);
    }

    public void Wings_Animation()
    {
        start_scaling = true;
        for(int i=0; i<wings.Length; i++)
        {
            if((i+1) % 2 == 0)
            {
                wings[i].GetComponent<Rigidbody>().AddForce(transform.right * (-wings_repulsive_force), ForceMode.Impulse);
                wings[i].GetComponent<Rigidbody>().AddTorque(transform.forward * (wings_rotation_speed), ForceMode.Impulse);
                wings[i].GetComponent<Rigidbody>().useGravity = true;
            }
            else
            {
                wings[i].GetComponent<Rigidbody>().AddForce(transform.right * (wings_repulsive_force), ForceMode.Impulse);
                wings[i].GetComponent<Rigidbody>().AddTorque(transform.forward * (-wings_rotation_speed), ForceMode.Impulse);
                wings[i].GetComponent<Rigidbody>().useGravity = true;
            }
        }
    }

    public void Hit_animation()
    {
       StartCoroutine(ChangeColor());
    }


    //TARGET IS OUTSIDE AND GETS KILLED BY A WALL
    public void OnTriggerEnter(Collider other)  // the food collides with the wall on the other side of the screen and then is destroyed
    {
        if (other.gameObject.tag == "FoodKill")
        {
            Destroy(gameObject);
        }
    }

    IEnumerator WingsScaler()
    {
        if (new_scale > 0)
        {
            new_scale -= real_scaling;
        }
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator ChangeColor()
    {
        hit_state.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        hit_state.SetActive(false);
    }
}
