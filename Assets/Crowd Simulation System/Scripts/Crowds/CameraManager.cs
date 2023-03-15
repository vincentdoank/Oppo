using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour

{

    public bool ZoomedIn;
    public bool ZoomedOut;
    public void Start()

    {

        ZoomedIn = true;

    }

    public void ZoomIn()

    {

        if (!ZoomedIn)

        {

            if (!GetComponent<Animation>().IsPlaying("camera_1_zoom_out"))

            {

                GetComponent<Animation>().Play("camera_1_zoom_in");

                ZoomedIn = true;
                ZoomedOut = false;

            }

        }

    }

    public void ZoomOut()

    {

        if (!ZoomedOut)

        {

            if (!GetComponent<Animation>().IsPlaying("camera_1_zoom_in"))

            {

                GetComponent<Animation>().Play("camera_1_zoom_out");

                ZoomedIn = false;
                ZoomedOut = true;

            }

        }

    }

}