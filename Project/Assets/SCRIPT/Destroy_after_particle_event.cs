using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Destroy_after_particle_event : MonoBehaviour
{
    #region NEW CODE

    void OnEnable()
    {
        StartCoroutine(CheckIfFinished());
    }

    IEnumerator CheckIfFinished()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();

        while (true && ps != null)
        {
            yield return new WaitForSeconds(0.5f);

            if (!ps.IsAlive(true))
            {
                Destroy(gameObject);

                break;
            }
        }
    }

    #endregion

}
