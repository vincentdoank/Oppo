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
            CrowdPerson.GetComponentInChildren<Animation>()?.CrossFade("idle_1", 0.2f);
        }
    }

    public void CrowdClap()
    {
        foreach (GameObject CrowdPerson in Crowd)
        {
            CrowdPerson.GetComponentInChildren<Animation>()?.CrossFade("clapping_1", 0.2f);
        }
    }

    public void CrowdCheer()
    {
        foreach (GameObject CrowdPerson in Crowd)
        {
            CrowdPerson.GetComponentInChildren<Animation>()?.CrossFade("cheering_1", 0.2f);
        }
    }

    public void CrowdWave()
    {
        foreach (GameObject CrowdPerson in Crowd)
        {
            CrowdPerson.GetComponentInChildren<Animation>()?.CrossFade("wave_1", 0.2f);
        }
    }

    public void CrowdSit()
    {
        foreach (GameObject CrowdPerson in Crowd)
        {
            CrowdPerson.GetComponentInChildren<Animation>()?.CrossFade("sitting_1", 0.2f);
        }
    }

    public void CrowdRandom()
    {
        foreach (GameObject CrowdPerson in Crowd)
        {
            if (CrowdPerson.GetComponentInChildren<Animation>())
            {
                AnimationClip clip = Animations[Random.Range(0, Animations.Length)];
                Animation animation = CrowdPerson.GetComponentInChildren<Animation>();
                animation.clip = clip;
                animation.CrossFade(clip.name, 0.2f);
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
            CrowdCheer();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            CrowdWave();
        }
    }

}