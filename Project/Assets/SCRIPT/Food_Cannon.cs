using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food_Cannon : MonoBehaviour {

    [Header("FOOD CANNON SETTINGS")]
    public float min_fire_time;
    public float max_fire_time;

    [Space(10)]
    public Match_Manager match_manager;

    [Space(10) , Header("LIST OF SHOOTABLE PREFAB TARGETS")]
    public GameObject[] FoodList;

    public bool ready = false;
    bool started = false;
    
    //random food generation
    float randomTime;
    float random_number;
    int selected_food;
    GameObject randomFood;

    float low_range = 0;
    float high_range = 0;

    private void Update()
    {
        if( match_manager.GetComponent<Match_Manager>().ready && !started)
        {
            LoadFood();
            ready = true;
            started = true;
        }
        else if (match_manager.GetComponent<Match_Manager>().ready == false && ready == true)
        {
            ready = false;
            started = false;
        }
    }

    // THE RANDOM SPAWN TIME IS SET, A NEW FOOD IS PREPARED --------------------------------------------------------------------------------- Load Food -----------------------------------------------
    private void LoadFood()
    {
            randomTime = Random.Range(min_fire_time, max_fire_time);
            StartCoroutine(SpawnFood());     
    }

    // A NEW TARGET IS GENERATED IN RELATION OF ITS PROBABILITY ----------------------------------------------------------------------------- Spawn Food ----------------------------------------------
    IEnumerator SpawnFood()
    {
        yield return new WaitForSeconds(randomTime); // the cannon waits a random time before preparing the new target to shoot

        random_number = Random.Range(0f, 100f);

        for(int i = 0; i < FoodList.Length; ++i)
        {         
            if(i == 0)
            {
                low_range = 0;
                high_range = FoodList[i].GetComponent<Food>().probability;
            }
            else
            {
                low_range = high_range;
                high_range += FoodList[i].GetComponent<Food>().probability;
            }
            if (random_number >= low_range && random_number < high_range)
            {
                // prendi la iesima pallina
                selected_food = i;
                break;
            }
        }

        randomFood = Instantiate(FoodList[selected_food], transform.position, Quaternion.identity);

        if (transform.rotation.z != 0) // if it is not 0°, then it must be 180°
        {
            randomFood.GetComponent<Food>().initial_speed = Mathf.Abs(randomFood.GetComponent<Food>().flight_speed) * (-1) ;
        }
        else
        {
            randomFood.GetComponent<Food>().initial_speed = Mathf.Abs(randomFood.GetComponent<Food>().flight_speed);
        }

        randomFood.transform.parent = null;

        

        yield return new WaitUntil(() => ready == true);

        LoadFood(); // a new food is loaded        
    }
}
