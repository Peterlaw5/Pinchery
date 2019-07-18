using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_Flags : MonoBehaviour
{
    #region NEW CODE

    public GameObject flag;

    ParticleSystem effect;

    bool alreadyPlayed = false;

    private void Start()
    {
        effect = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!flag.activeInHierarchy && alreadyPlayed)
        {
            alreadyPlayed = false;
            effect.Stop();
        }

        if (flag.activeInHierarchy && !alreadyPlayed)
        {
            effect.Play();
            alreadyPlayed = true;
        }
    }

    #endregion

}

