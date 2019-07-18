using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#region NEW CODE

public enum Champ // FOR TUNING
{
    Thunder,
    Necro,
    Fire
}

#endregion

public class Match_Manager : MonoBehaviour {
   
    #region NEW CODE

    [Header("FX EFFECTS")]
    public ParticleSystem startQuestEffect;
    public ParticleSystem questCompletedP1;
    public ParticleSystem questCompletedP1a;
    public ParticleSystem questCompletedP2;
    public ParticleSystem questCompletedP2b;
    public GameObject P1WinFX;
    public GameObject P2WinFX;

    [Header("SET CHAMPS")]
    public Champ playerChamp;
    public Champ opponentChamp;

    #endregion

    // THIS OBJECT IS RESPONSIBLE FOR THE MANAGEMENT OF THE TIME, THE SCORE, THE QUEST DISPLAY and the SPAWN PROBABILITY OF EACH BALL IN RELATION OF THE QUEST

    //timer
    [Header("MATCH TIMER SETTINGS"), Tooltip ("how much time (seconds) the match lasts")]
    public float match_duration;
    [Tooltip("the reference to the TIMER TEXT in the TIMER section of the canvas hierarchy")]
    public Text timer_text;
    [Tooltip("how long is the final countdown. example: if 3 - then is -match will end in 3... 2... 1... match over! - ")]
    public float final_countdown;
    [Tooltip("the reference to the FINAL COUNTDOWN TEXT in the canvas hierarchy")]
    public Text final_countdown_text;

    //score
    [Space(10), Header("PLAYER SCORE DISPLAY SETTINGS"), Tooltip ("reference to PLAYER")]
    public Player_Controls player_1;

    [Tooltip ("reference to the PLAYER 1 SCORE TEXT on the canvas hierarchy")]
    public Text P1_score_text;

    [Tooltip("reference to the PLAYER 1 STREAK BOX on the canvas hierarchy")]
    public GameObject P1_streak_box;

    [Tooltip("references to the 3 P1 flags in the canvas quest display hierarchy")]
    public GameObject[] P1_flags = new GameObject[3];

    [Space(10), Tooltip("reference to OPPONENT")]
    public IA_controller player_2;

    [Tooltip("reference to the PLAYER 2 SCORE TEXT on the canvas hierarchy")]
    public Text P2_score_text;

    [Tooltip("reference to the PLAYER 2 STREAK BOX on the canvas hierarchy")]
    public GameObject P2_streak_box;

    [Tooltip("references to the 3 P2 flags in the canvas quest display hierarchy")]
    public GameObject[] P2_flags = new GameObject[3];

    //ui
    [Header("UI"), Tooltip("the bar that scrolls during the quest")]
    public Image quest_bar;
    [Tooltip("offset for the - quest completed - message")]
    public float quest_text_offset;

    [Tooltip ("IF YOU WANT TO SEE THE BAR WITHOUT A QUEST")]
    public bool TEST_show_bar;

    [Tooltip("reference to the quest alert text")]
    public GameObject quest_alert_text;
    [Tooltip ("reference to the quest_alert_spawner empty object in the match manager hierarchy")]
    public GameObject quest_alert_spawner;
    bool alert_spawned = false;

    [Space(10), Tooltip("reference to the three SLOTS in each banner of the quest display")]
    public GameObject[] QuestSlots = new GameObject[3];

    [Tooltip("the three BANNERS of the quest tracker")]
    public GameObject[] QuestBanners = new GameObject[2];

    [Space(10), Tooltip("reference to the CANNON PREFAB")]
    public Food_Cannon cannon_prefab;

    [Tooltip("this must be filled with ALL THE CANNONS in the level")]
    public GameObject[] match_cannons;

    GameObject[] IconList; // list of all the icons of the food types i can shoot with the cannon


    // quests
    [Space(10) , HideInInspector]
    public string[] QuestList = new string[3]; // contains the materials (PROVVISORIO) of the three items in the quest display slots
    //string[] QuestListString = new string[3];      // contains the names of the materials (PROVVISORIO) of the three items in the quest display slots


    // quest pool structure
    [System.Serializable]
    public struct Quest // the structure of the quest
    {
        [Header("DURATION"), Space(10), Tooltip("how much time (seconds) the quest will remain active")]
        public float wave_duration;

        [Header("PROBABILITIES"), Tooltip("based on the order of the CANNON PREFAB FOOD LIST -> element 0 value = probability of the cannon food list element 0  -> SUM MUST BE 100!")]
        public float[] targets_probability;

        [Space(10), Tooltip ("check this TRUE if you want to create a new quest with the parameters below --> if FALSE, set the PROBABILITY STATUS DURATION as timer for this wave")]
        public bool quest_active;

        [Header("QUEST"),Space(10),Tooltip("insert the prefab of the target you want in the quest (the food - not the icon)")]
        public GameObject[] quest_items; 

        [Header("REWARD"),Tooltip ("insert how many points you want to give to the player who completes the quest")]
        public int quest_reward;
      
        [Header("RATE OF FIRE"), Tooltip("minimum time to fire")]
        public float min_fire_time;

        [Tooltip("maximum time to fire")]
        public float max_fire_time;
    }

    [Tooltip("how many seconds the warning is spawned before the quest")]
    public float quest_alert_time;

    [Space(10), Header("WAVE / QUEST EVENT LIST"), Tooltip("NUMBER OF QUESTS - fill each field of each quest!")]
    public Quest[] match_quests; // the list of quests

    int current_quest = 0; // shows the current quest 
    public float current_quest_time;

    // initial countdown
    [Header ("INITIAL COUNTDOWN SETTINGS")]
    public Text countdown_text;

    [Space(10), Tooltip("the time you have to wait before the countdown starts")]
    public float entry_time_countdown;

    [Tooltip("how long is the initial countdown? 3 = 3...2...1...GO!")]
    public int initial_countdown; // i secondi del countdown iniziale

    [Range(0f,1f), Tooltip("the fraction of time when the countdown text is visible: 0 = invisible 1 = always there")]
    public float show_countdown_time;

    [HideInInspector]
    public bool ready = false;

    [Tooltip("the font size of the numbers of the initial countdown")]
    public int initial_countdown_size;

    [Tooltip("what to say at the end of the countdown: GO! VAI! PLAY! - mind the font size of the message in order to make the message fit in the canvas")]
    public string initial_message;

    [Tooltip("the initial font size of the message at the end of the coun down: make it fit!")]
    public int initial_message_size;

    [Range(0,10), Tooltip("how much the font size is reduced every frame. 0 = no reduction")] 
    public int text_reduction;

    bool started = false;

    [Header("END OF THE MATCH REFERENCES"), Tooltip ("reference to the MATCH OVER TEXT in the canvas hierarchy")]   
    public GameObject match_over_text;

    [Tooltip("reference to the END GAME MENU in the canvas hierarchy")]
    public GameObject EndMenu;

    [Tooltip("reference to the PLAYER 1 WINS text in the canvas hierarchy")]
    public GameObject p1win;

    [Tooltip("reference to the PLAYER 2 WINS text in the canvas hierarchy")]
    public GameObject p2win;


    bool shadow_already_in_scene = false; // TO NOT PLACE TO SHADOW IN SCENE

    [Tooltip("reference to the SHADOW in the match manager hierarchy")]
    public GameObject shadow;

    // Initial instantiation
    [ HideInInspector]
    public GameObject account_setup_memory; // the GLOBAL MEMORY FROM THE MENU SCENE with player and background in memory
  
    [Header("GAME INITIALIZATION"), Tooltip("reference to the P1_SPAWNER object in the hierarchy")]
    public GameObject P1_spawner;

    [Tooltip("the list of all the players in the game")]
    public GameObject[] Players_prefab = new GameObject[3];

    [Space(10), Tooltip("reference to the P2_SPAWNER object in the hierarchy")]
    public GameObject P2_spawner;

    [Tooltip("the list of all the Opponents(IA) in the game")]
    public GameObject[] Opponents_prefab = new GameObject[3];

    [Space(10), Tooltip("reference to the BACKGROUND SPAWNER object in hierarchy")]
    public GameObject background_spawner;

    [Tooltip("the list of all the background in the game")]
    public GameObject[] Backgrounds_prefab = new GameObject[2];

    [Space(10), Tooltip("reference to the P1 ATTACK BUTTON SPAWNER in the canvas hierarchy")]
    public GameObject P1_attack_button_spawner;

    [Tooltip("reference to the P2 ATTACK BUTTON SPAWNER in the canvas hierarchy")]
    public GameObject P2_attack_button_spawner;

    bool match_initialized; // players have been instantiated


    //Audio variab
    public AudioSource main_soundtrack; //
    public AudioSource quest_sound_spawned; //
    public AudioSource quest_sound_completed; //
    public AudioSource match_start_sound; //
    public AudioSource initial_countdown_bip;//
    public AudioSource match_end_sound;//
    public AudioSource final_countdown_bip;//
    public AudioSource victory_sound;
    public AudioSource defeat_sound;

    // SET BASIC PROBABILITY FOR EACH TARGET  ----------------------------------------------------------------------- awake ---------------------------------------------------------------  
    private void Awake()
    {
        // initial spawn probability
        for (int i = 0; i < cannon_prefab.FoodList.Length; ++i)
        {
            cannon_prefab.FoodList[i].GetComponent<Food>().probability = match_quests[current_quest].targets_probability[i];
        }

        // hiding quest banners
        foreach (GameObject banner in QuestBanners) // at the start, every slot of the quest is deactivated
        {
            banner.SetActive(false);
        }

        match_over_text.SetActive(false);
        EndMenu.SetActive(false);
        p1win.SetActive(false);
        p2win.SetActive(false);

        StartCoroutine(MatchCountdownInitial());  // initial time for the start of the countdown

        // instantiate players
        if(/*account_setup_memory.activeInHierarchy &&*/ !match_initialized)
        {
            match_initialized = true;
            // get component della memory e prendi i vari asset

            // get the array, instantiate the i <- (memory) element of the array

            GameObject P1_prefab; // PLAYER 1
            
             P1_prefab = Instantiate(Players_prefab[Account_setup_memory.selected_character], P1_spawner.transform.position, Quaternion.identity);

           

            player_1 = P1_prefab.transform.GetChild(0).GetComponent<Player_Controls>(); // PLAYER 1 IS ASSIGNED
            player_1.special_attack_button_spawner = P1_attack_button_spawner;
            player_1.streak_text = P1_streak_box.transform.GetChild(0).GetComponent<Text>();

            for(int i = 0; i < P1_flags.Length; ++i)
            {
                player_1.Flags[i] = P1_flags[i];
            }

            GameObject P2_prefab; // PLAYER 2 - opponent is randomic every match

            P2_prefab = Instantiate(Opponents_prefab[Random.Range(0,Opponents_prefab.Length)], P2_spawner.transform.position, Quaternion.identity);

           

            player_2 = P2_prefab.transform.GetChild(0).GetComponent<IA_controller>(); // PLAYER 2 IS ASSIGNED
            player_2.special_attack_button_spawner = P2_attack_button_spawner;
            player_2.streak_text = P2_streak_box.transform.GetChild(0).GetComponent<Text>();

            for (int i = 0; i < P2_flags.Length; ++i)
            {
                player_2.Flags[i] = P2_flags[i];
            }

            // BACKGROUND
            Instantiate(Backgrounds_prefab[Account_setup_memory.selected_stage], background_spawner.transform.position, Quaternion.identity); // BACKGROUND IS CREATED
        }

    }

    // UPDATE
    void Update ()
    {
        //INITIAL PROBABILITY FOR TARGETS - CLEAR QUEST SLOTS ---------------------------------------------------------------- posticipated start --------------------------------------------------------     
        if(ready && !started)
        {
            started = true;
            IconList = new GameObject[cannon_prefab.FoodList.Length];
            for (int i = 0; i < IconList.Length; ++i)
            {
                IconList[i] = cannon_prefab.FoodList[i];
            }

            // create the  initial wave
            InitializeWave();
        }
        
        // initial countdown text reduction
        if(initial_countdown > 0 && !started)
        {
            if(countdown_text.fontSize >= 0)
            {
                countdown_text.fontSize -= text_reduction;
            }                
            else
            {
                countdown_text.fontSize = 0;
                countdown_text.text = "";
            }               
        }


        //TIMER --------------------------------------------------------------------------------------------------------------- timer management ---------------------------------------------

        //cooldown
        if (ready)
        {

            if (match_duration > 0)
            {
                match_duration -= Time.deltaTime; // a second is reduced
                timer_text.text = match_duration.ToString("F0");

                // quest bar fill amount management
                if(current_quest_time > 0)
                {
                    current_quest_time -= Time.deltaTime;

                    if(TEST_show_bar || match_quests[current_quest].quest_active)
                    {
                        quest_bar.fillAmount = current_quest_time / match_quests[current_quest].wave_duration;
                    }
                    else
                    {
                        quest_bar.fillAmount = 0f;
                    }                    
                }
                
                // final countdown
                if(match_duration <= final_countdown)
                {
                    if(final_countdown_text.gameObject.activeInHierarchy == false)
                    {
                        final_countdown_text.gameObject.SetActive(true);
                    }

                    final_countdown_text.text =   Mathf.CeilToInt(match_duration).ToString();

                }
            }
            else
            {
                final_countdown_text.gameObject.SetActive(false);

                Game_Over();
            }

            // PLAYER SCORES REPRESENTATION --------------------------------------------------------------------------- score text ---------------------------------------------------------------

            P1_score_text.text = player_1.player_score.ToString();
            P2_score_text.text = player_2.player_score.ToString();

            // PLAYER COMBO -------------------------------------------------------------------------------------------- player combo ---------------------------------------------------------------------    

            if (QuestBanners[2].activeInHierarchy) // if the quest slots are active
            {
                if (player_1.player_combo[0] != null)
                {
                    CheckCombo(player_1);
                }

                if (player_2.player_combo[0] != null)
                {
                    CheckCombo(player_2);
                }
            }

            // QUEST ALERT --------------------------------------------------------------------------------------------- quest alert -------------------------------------------------
            if( !(current_quest + 1 >= match_quests.Length) &&  match_quests[current_quest + 1].quest_active)
            {
                if (current_quest_time <= quest_alert_time && !alert_spawned)
                {
                    // ALERT IS SPAWNED
                    Instantiate(quest_alert_text, quest_alert_spawner.transform.position, Quaternion.identity);
                    alert_spawned = true;
                }

            }
        }

        // FINAL SHADOW ------------------------------------------------------------------------------------------------------ final attack shadow --------------------------------------------

        if((player_1.shadow_is_active == true || player_2.shadow_is_active == true) && shadow_already_in_scene == false) // if one of the player activates the FINAL and no shadow is present
        {
            shadow.GetComponent<Final_Shadow>().ShadowFadeIn();
            shadow_already_in_scene = true; // shadow appears           
        }
        if ((player_1.shadow_is_active == false && player_2.shadow_is_active == false) && shadow_already_in_scene)
        {           
            shadow.GetComponent<Final_Shadow>().ShadowFadeOut();
            shadow_already_in_scene = false;
        }
    }

    // A NEW QUEST IS GENERATED ------------------------------------------------------------------ initialize Wave -----------------------------------------------------------------------------------------
    private void InitializeWave()
    {
        //Debug.Log("NEW QUEST GENERATED");

        // NEW RATE OF FIRE
        foreach (GameObject cannon in match_cannons)
        {
            cannon.GetComponent<Food_Cannon>().min_fire_time = match_quests[current_quest].min_fire_time;
            cannon.GetComponent<Food_Cannon>().max_fire_time = match_quests[current_quest].max_fire_time;
        }

        // NEW TARGET PROBABILITIES
        for (int i = 0; i < cannon_prefab.FoodList.Length; ++i) // each TARGET PROBABILITY is set according the the quest pool (targets probability)
        {
            cannon_prefab.FoodList[i].GetComponent<Food>().probability = match_quests[current_quest].targets_probability[i];
        }

        // timer of the quest for alert
        current_quest_time = match_quests[current_quest].wave_duration;
        alert_spawned = false;

        // QUEST CREATION
        if(match_quests[current_quest].quest_active)
        {
            #region NEW CODE

            startQuestEffect.Play();

            #endregion
            quest_sound_spawned.Play();

            foreach (GameObject banner in QuestBanners)
            {
                banner.SetActive(true);
            }

            // the icons of the quest display are created
            for (int i = 0; i < QuestList.Length; ++i)
            {
                GameObject new_icon = Instantiate(match_quests[current_quest].quest_items[i].GetComponent<Food>().icon_prefab, transform.position, Quaternion.identity); // relative icon is created
                new_icon.transform.SetParent(QuestSlots[i].transform);  // the icon of the target is binded to the quest slot in the UI
                new_icon.transform.position = QuestSlots[i].transform.position;
                new_icon.transform.localScale = new Vector3(0.7f, 0.6f, 1f);

                QuestList[i] = new_icon.GetComponent<Quest_Icon>().icon_identifier; // icon IDENTIFIER is associated to the quest
            }

            // the quest is copied into the memory of each player so they can compare it individually
            for (int i = 0; i < QuestList.Length; ++i)
            {
                player_1.player_quest[i] = QuestList[i];
            }
            for (int i = 0; i < QuestList.Length; ++i)
            {
                player_2.player_quest[i] = QuestList[i];
            }

            StartCoroutine(WaveTimer());
        }

        else // have no quest
        {
            StartCoroutine(WaveTimer());
        }        
    }

    // CHECK COMBO ----------------------------------------------------------------------------------------- check combo ------------------------------------------------------------------------------------
    private void CheckCombo(General_Controls player)
    {    

            player.CheckMyCombo(); // the player controls if his arrow contains elements required by the quest

            if (player.collected_combo >= QuestList.Length) // if all the items in the combo have been collected, the combo is complete
            {
                Reward(player); // reward the player
            }
    }

    // THE REWARD FOR THE COMPLETED QUEST IS ASSIGNED TO THE PLAYER -------------------------------------------------------------------- quest reward ------------------------------------
    private void Reward(General_Controls player)
    {
        Debug.Log("REWARD is assigned to: " + player.gameObject.name);

        #region NEW CODE


        if (player.GetComponent<IA_controller>() != null)
        {
            questCompletedP2.Play();
            questCompletedP2b.Play();
        }
        else
        {
            questCompletedP1.Play();
            questCompletedP1a.Play();
        }

        #endregion
        quest_sound_completed.Play();
        player.player_score += match_quests[current_quest].quest_reward; // quest reward is assigned to the player

        for (int i = 0; i < player.player_combo.Length; i++)
        {
            player.player_combo[i] = null;            
            QuestList[i] = null;
        }

        //the combo icon is generated to show the value of the quest
        GameObject combo_icon;
        combo_icon = Instantiate(player.GetComponent<General_Controls>().combo_icon, transform.position, Quaternion.identity);     
        combo_icon.GetComponent<Arrow_Score>().score = match_quests[current_quest].quest_reward;    
        combo_icon.transform.parent = null;

        //the quest icon is generated to enfatize the player who completed the quest
        Instantiate(player.quest_icon, transform.position, Quaternion.identity);


        StopAllCoroutines(); // PROVVISORIO --> perchè non posso fermare la singola coroutine?

        RemoveQuest(); // removes the current quest

        NextWave(); // prepares the next wave
    }

    // THE OLD QUEST IS REMOVED ------------------------------------------------------------------------------------------ remove quest ---------------------------------------------------------------
    private void RemoveQuest()
    {
        quest_bar.fillAmount = 0;

        for (int i = 0; i < QuestList.Length; ++i) // the slots are cleared and the quest list is reset
        {
            if(QuestSlots[i].transform.childCount > 0)
            {
                Destroy(QuestSlots[i].transform.GetChild(0).gameObject);
            } 
            
            QuestBanners[i].SetActive(false);
            QuestList[i] = null;
            player_1.player_combo[i] = null;
            player_2.player_combo[i] = null;
            player_1.Flags[i].gameObject.SetActive(false);
            player_2.Flags[i].gameObject.SetActive(false);
            player_1.collected_combo = 0;
            player_2.collected_combo = 0;
        }

        // quest flags are reset
        foreach (GameObject flag in player_1.Flags)
        {
            flag.SetActive(false);
        }
        foreach (GameObject flag in player_2.Flags)
        {
            flag.SetActive(false);
        }
    }

    // INITIAL COUNTDOWN -----------------------------------------------------------------------------------------------------------start match ---------------------------------------------
    private void StartMatch()
    {
        if (initial_countdown > -1)
        {
            StartCoroutine(MatchCountdown());
        }
        else
        {
            
            main_soundtrack.Play();
            player_1.ready = true;
            player_2.ready = true;
            ready = true;
        }
    }

    private void NextWave()
    {
        ++current_quest;
               
        InitializeWave();        
    }

    // QUEST IS  "ACTIVE" AND AFTER THE TIMER, QUEST IS "INACTIVE" ---------------------------------------------------------------- Quest timer -----------------------------------------------
    IEnumerator WaveTimer()
    {
        float quest_time;  // the time a quest is active

        quest_time = match_quests[current_quest].wave_duration; // quest duration is set in the EDITOR

        yield return new WaitForSeconds(quest_time); // after this time, the quest is deactivated

        RemoveQuest(); // removes the quest

        yield return new WaitUntil(() => ready == true); // if match manager is not ready, (match ended) the netx wave must not be generated

        NextWave();
    }


    // INITIAL WAIT TIME BEFORE THE START OF THE INITIAL COUNTDOWN ----------------------------------------------------------- Initial Match Countdown -------------------------------------------------------
    IEnumerator MatchCountdownInitial()
    {
        yield return new WaitForSeconds(entry_time_countdown);       
        StartMatch();
    }

    // DISPLAYS THE NUMBER OF THE INITIAL COUNTDOWN EVERY SECOND
    IEnumerator MatchCountdown()
    {
        StartCoroutine(MatchCountdownText());
        if (initial_countdown > 0)
        {
            countdown_text.text = (initial_countdown).ToString();
            countdown_text.fontSize = initial_countdown_size;
            initial_countdown_bip.Play();
        }
        else
        {
            countdown_text.fontSize = initial_message_size;
            countdown_text.text = initial_message;
            Debug.Log("LET THE MATCH BEGIN!");
            match_start_sound.Play();
        }

        if (match_duration <= final_countdown && match_duration >= 0)
        {
            final_countdown_bip.Play();
        }


         yield return new WaitForSeconds(1);

        --initial_countdown;
       
        StartMatch();
    }
    // HIDES THE NUMBER OF THE COUNTDOWN AFTER A CERTAIN TIME
    IEnumerator MatchCountdownText()
    {       
        yield return new WaitForSeconds(show_countdown_time);
        countdown_text.text = "";
    }



    //TIME OUT ------------------------------------------------------------------------------------------------------ End of the game ---------------------------------------------------------
    private void Game_Over()
    {
        // game stops

        ready = false;
        player_1.ready = false;
        player_2.ready = false;

        // cannons stops
        foreach (GameObject cannon in match_cannons)
        {
            cannon.GetComponent<Food_Cannon>().ready = false;
        }


        StartCoroutine(EndMenuDelay());
    }

    // AFTER THE TIME OUT ------------------------------------------------------------------------------------------ End match menu spawn delay --------------------------------------------------------
    IEnumerator EndMenuDelay()
    {
        float end_menu_delay;
        float on_wall_time_1 = player_1.arrow.GetComponent<Arrow_Controller>().on_wall_time;
        float on_wall_time_2 = player_2.arrow.GetComponent<Arrow_Controller>().on_wall_time;
        if (on_wall_time_1 >= on_wall_time_2)
        {
            end_menu_delay = on_wall_time_1 + 0.5f;
        }
        else
        {
            end_menu_delay = on_wall_time_2 + 0.5f;
        }

        // match over spawns
        match_over_text.SetActive(true);

        main_soundtrack.volume = 0.5f;
        match_end_sound.Play();

        yield return new WaitForSeconds(end_menu_delay);

        // match over disappears
        match_over_text.SetActive(false);

        // "retry" and "go to menu" buttons appears 
        EndMenu.SetActive(true);

        // winning player gets the score
        if (player_1.player_score > player_2.player_score)
        {
            p1win.SetActive(true);

            victory_sound.Play();           

            #region NEW CODE 

            Instantiate(P1WinFX);

            #endregion

        }
        else
        {
            p2win.SetActive(true);

            defeat_sound.Play();

            #region NEW CODE

            Instantiate(P2WinFX);

            #endregion

        }
    }
}
