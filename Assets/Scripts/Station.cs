using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{

    public bool active = false;

    public void SetActive()
    {
        active = true;

    }

    public void Disable()
    {
        active = false;
    }

}
