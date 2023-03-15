using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    public GameObject[] Crowd;
    public AnimationClip[] Animations;

    public static CrowdManager Instance { get; private set; }

    public void Start()
    {
        Instance = this;
        Crowd = GameObject.FindGameObjectsWithTag("Crowd");
        CrowdRandom();
    }

    public void CrowdIdle()
    {
        foreach (GameObject CrowdPerson in Crowd)
        {
            if (CrowdPerson.GetComponentInChildren<Animation>())
            {
                CrowdPerson.GetComponentInChildren<Animation>().Play("idle_1");
            }
        }
    }

    public void CrowdClap()
    {
        foreach (GameObject CrowdPerson in Crowd)
        {
            if (CrowdPerson.GetComponentInChildren<Animation>())
            {
                CrowdPerson.GetComponentInChildren<Animation>().Play("clapping_1");
            }
        }
    }

    public void CrowdCheer()
    {
        foreach (GameObject CrowdPerson in Crowd)
        {
            if (CrowdPerson.GetComponentInChildren<Animation>())
            {
                CrowdPerson.GetComponentInChildren<Animation>().Play("cheering_1");
            }
        }
    }

    public void CrowdWave()
    {
        foreach (GameObject CrowdPerson in Crowd)
        {
            if (CrowdPerson.GetComponentInChildren<Animation>())
            {
                CrowdPerson.GetComponentInChildren<Animation>().Play("wave_1");
            }
        }
    }

    public void CrowdSit()
    {
        foreach (GameObject CrowdPerson in Crowd)
        {
            if (CrowdPerson.GetComponentInChildren<Animation>())
            {
                CrowdPerson.GetComponentInChildren<Animation>().Play("sitting_1");
            }
        }
    }

    public void CrowdRandom()
    {
        foreach (GameObject CrowdPerson in Crowd)
        {
            if (CrowdPerson.GetComponentInChildren<Animation>())
            {
                CrowdPerson.GetComponentInChildren<Animation>().clip = Animations[Random.Range(0, Animations.Length)];
                CrowdPerson.GetComponentInChildren<Animation>().Play();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            CrowdClap();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            CrowdIdle();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CrowdRandom();
        }
    }

}