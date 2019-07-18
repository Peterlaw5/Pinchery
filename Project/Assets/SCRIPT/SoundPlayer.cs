using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {

    public AudioSource acquisto_negato;
    public AudioSource bottone_conferma;
    public AudioSource cambio_opzioni_menu; 
    public AudioSource menu_music;
   
 
    public void PlayAcquistoNegato()
    {
        acquisto_negato.Play();
    }

    public void PlayBottoneConferma()
    {
        bottone_conferma.Play();
    }

    public void PlayCambioOpzioniMenu()
    {
        cambio_opzioni_menu.Play();
    }

    public void PlayMenuMusic()
    {
        menu_music.Play();
    }
}
