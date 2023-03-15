using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour

{

    public GameObject Reference2D;
    public GameObject Reference3D;

    public bool IsSingleCrowd;
    public bool IsCombinedCrowd;

    public void Update()

    {

        if (IsSingleCrowd)

        {

            if (GetComponentInChildren<Animation>().IsPlaying("idle_1"))

            {

                GetComponentInChildren<Animator>().SetBool("waving", false);
                GetComponentInChildren<Animator>().SetBool("cheering", false);
                GetComponentInChildren<Animator>().SetBool("clapping", false);
                GetComponentInChildren<Animator>().SetBool("sitting", false);
                GetComponentInChildren<Animator>().SetBool("idle", true);

            }

            else if (GetComponentInChildren<Animation>().IsPlaying("clapping_1"))

            {

                GetComponentInChildren<Animator>().SetBool("waving", false);
                GetComponentInChildren<Animator>().SetBool("cheering", false);
                GetComponentInChildren<Animator>().SetBool("clapping", true);
                GetComponentInChildren<Animator>().SetBool("sitting", false);
                GetComponentInChildren<Animator>().SetBool("idle", false);

            }

            else if (GetComponentInChildren<Animation>().IsPlaying("cheering_1"))

            {

                GetComponentInChildren<Animator>().SetBool("waving", false);
                GetComponentInChildren<Animator>().SetBool("cheering", true);
                GetComponentInChildren<Animator>().SetBool("clapping", false);
                GetComponentInChildren<Animator>().SetBool("sitting", false);
                GetComponentInChildren<Animator>().SetBool("idle", false);

            }

            else if (GetComponentInChildren<Animation>().IsPlaying("wave_1"))

            {

                GetComponentInChildren<Animator>().SetBool("waving", true);
                GetComponentInChildren<Animator>().SetBool("cheering", false);
                GetComponentInChildren<Animator>().SetBool("clapping", false);
                GetComponentInChildren<Animator>().SetBool("sitting", false);
                GetComponentInChildren<Animator>().SetBool("idle", false);

            }

            else if (GetComponentInChildren<Animation>().IsPlaying("sitting_1"))

            {

                GetComponentInChildren<Animator>().SetBool("waving", false);
                GetComponentInChildren<Animator>().SetBool("cheering", false);
                GetComponentInChildren<Animator>().SetBool("clapping", false);
                GetComponentInChildren<Animator>().SetBool("sitting", true);
                GetComponentInChildren<Animator>().SetBool("idle", false);

            }

        }

        if (IsCombinedCrowd)

        {

            if (Reference3D.GetComponentInChildren<Animation>().IsPlaying("idle_1"))

            {

                Reference2D.GetComponentInChildren<Animator>().SetBool("waving", false);
                Reference2D.GetComponentInChildren<Animator>().SetBool("cheering", false);
                Reference2D.GetComponentInChildren<Animator>().SetBool("clapping", false);
                Reference2D.GetComponentInChildren<Animator>().SetBool("sitting", false);
                Reference2D.GetComponentInChildren<Animator>().SetBool("idle", true);

            }

            else if (Reference3D.GetComponentInChildren<Animation>().IsPlaying("clapping_1"))

            {

                Reference2D.GetComponentInChildren<Animator>().SetBool("waving", false);
                Reference2D.GetComponentInChildren<Animator>().SetBool("cheering", false);
                Reference2D.GetComponentInChildren<Animator>().SetBool("clapping", true);
                Reference2D.GetComponentInChildren<Animator>().SetBool("sitting", false);
                Reference2D.GetComponentInChildren<Animator>().SetBool("idle", false);

            }

            else if (Reference3D.GetComponentInChildren<Animation>().IsPlaying("cheering_1"))

            {

                Reference2D.GetComponentInChildren<Animator>().SetBool("waving", false);
                Reference2D.GetComponentInChildren<Animator>().SetBool("cheering", true);
                Reference2D.GetComponentInChildren<Animator>().SetBool("clapping", false);
                Reference2D.GetComponentInChildren<Animator>().SetBool("sitting", false);
                Reference2D.GetComponentInChildren<Animator>().SetBool("idle", false);

            }

            else if (Reference3D.GetComponentInChildren<Animation>().IsPlaying("wave_1"))

            {

                Reference2D.GetComponentInChildren<Animator>().SetBool("waving", true);
                Reference2D.GetComponentInChildren<Animator>().SetBool("cheering", false);
                Reference2D.GetComponentInChildren<Animator>().SetBool("clapping", false);
                Reference2D.GetComponentInChildren<Animator>().SetBool("sitting", false);
                Reference2D.GetComponentInChildren<Animator>().SetBool("idle", false);

            }

            else if (Reference3D.GetComponentInChildren<Animation>().IsPlaying("sitting_1"))

            {

                Reference2D.GetComponentInChildren<Animator>().SetBool("waving", false);
                Reference2D.GetComponentInChildren<Animator>().SetBool("cheering", false);
                Reference2D.GetComponentInChildren<Animator>().SetBool("clapping", false);
                Reference2D.GetComponentInChildren<Animator>().SetBool("sitting", true);
                Reference2D.GetComponentInChildren<Animator>().SetBool("idle", false);

            }

        }

    }

}