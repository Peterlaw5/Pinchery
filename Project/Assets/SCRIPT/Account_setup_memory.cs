using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Account_setup_memory : MonoBehaviour {

    public static int selected_character = 0;
    public static int selected_stage = 0;

    public void Select_character (int character)
    {
        selected_character = character;
        print(selected_character);
    }

    public void Select_stage (int stage)
    {
        selected_stage = stage;
        print(selected_stage);
    }

    
}
