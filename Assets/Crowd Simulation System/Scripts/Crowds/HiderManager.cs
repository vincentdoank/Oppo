using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiderManager : MonoBehaviour

{

    public GameObject Sign1;

    void Update()

    {

        if (GetComponent<Animation>().IsPlaying("wave_1"))

        {

            Sign1.SetActive(true);

        }

        else

        {

            Sign1.SetActive(false);

        }

    }

}
