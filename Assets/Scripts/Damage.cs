using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{

    public bool active = false;
    public GameObject particles;

    public void SetActive()
    {
        active = true;
        particles.SetActive(true);

    }

    public void Disable()
    {
        active = false;
        particles.SetActive(false);
    }
}
