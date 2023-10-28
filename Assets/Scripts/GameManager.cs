using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public int maxUser = 3;

    public LineBrush lineBrush;
    //public KiteController[] kites;
    //public WebCamera webCamera;
    public FootballController footballController;
    public SelectableControlType controlTypeController;

    public HUDController hud;
    public UnityEvent onServer;
    public UnityEvent onClient;

    //public float elapsedTime = 0;
    //public float resetTime = 0.5f;

    public bool IsServer { get; private set; }
    public bool IsConnected { get; private set; }

    public enum ControlType
    {
        KITE = 0,
        SHAKEDRAW = 1,
        DRAW = 2,
        PICTURE = 3
    }
    public ControlType controlType;

    public static GameManager Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //Debug.unityLogger.logEnabled = false;
        
        //controlType = ControlType.SHAKEDRAW;
        
        
        Settings.networkType = Settings.NetworkType.NetCode;

        //if (Application.platform == RuntimePlatform.WindowsEditor)
        //{
        //    IsServer = true;
        //}

        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            IsServer = true;
        }

        EventManager.onControlTypeSelected += OnControlTypeSelected;
        //EventManager.onConnectToNetwork += CheckControlType;
        //EventManager.onLeaveRoom += ShowMainMenu;

        //HideCameraTexture();

        Debug.LogWarning("isServer : " + IsServer);
    }

    private void OnDestroy()
    {
        EventManager.onControlTypeSelected -= OnControlTypeSelected;
        //EventManager.onConnectToNetwork -= CheckControlType;
        EventManager.onLeaveRoom -= ShowMainMenu;
    }

    private void ShowMainMenu()
    {
        if (!IsServer)
        {
            controlTypeController.gameObject.SetActive(true);
            controlTypeController.tweenFade.PlayBackward(null);
            //HideCameraTexture();
        }
    }

    public void CheckControlType()
    {
        //NetworkController.Instance.ipText.text = "Control Type : " + controlType.ToString();
        if (controlType == ControlType.PICTURE)
        {
            EventManager.onUseCameraSelected?.Invoke();
        }
        else
        {
            //HideCameraTexture();
        }
    }

    //private void Update()
    //{
    //    if (curDebugClickCount > 0)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        if (elapsedTime >= resetTime)
    //        {
    //            elapsedTime = 0;
    //            curDebugClickCount = 0;
    //        }
    //    }

    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        curDebugClickCount += 1;
    //        if (curDebugClickCount >= debugClick)
    //        {
    //            EventManager.onLeaveRoom?.Invoke();
    //        }
    //    }

    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Debug.Log("space pressed");
    //        EventManager.sendNetworkMessage?.Invoke(JsonUtility.ToJson(new EventMessageData { methodName = "OnPhoneShaked", parameters = null }));
    //    }
    //}

    //private void OnDebugCountAdded()
    //{
    //    curDebugClickCount += 1;
    //    if (curDebugClickCount >= debugClick)
    //    {
    //        curDebugClickCount = 0;
    //        if (IsServer)
    //        {
    //            IsServer = false;
    //            onClient?.Invoke();
    //            ShowControlTypeDropdown();
    //        }
    //        else
    //        {
    //            IsServer = true;
    //            onServer?.Invoke();
    //            HideControlTypeDropdown();
    //        }
    //    }
    //}

    //public void ChangeServerDebugColor(UnityEngine.UI.Image image)
    //{
    //    image.color = serverColor;
    //    Debug.Log("change color : " + serverColor);
    //}

    //public void ChangeClientDebugColor(UnityEngine.UI.Image image)
    //{
    //    image.color = clientColor;
    //    Debug.Log("change color : " + clientColor);
    //}

    private void OnControlTypeSelected(int index)
    {
        controlType = (ControlType)index;
    }

    public void ShowControlTypeDropdown()
    {
        //hud.controlTypeDropdown.gameObject.SetActive(true);
    }

    public void HideControlTypeDropdown()
    {
        //hud.controlTypeDropdown.gameObject.SetActive(false);
    }

    //public void ShowCameraTexture()
    //{
    //    webCamera.photoImage.transform.parent.gameObject.SetActive(true);
    //}

    //public void HideCameraTexture()
    //{
    //    webCamera.photoImage.transform.parent.gameObject.SetActive(false);
    //    webCamera.capturedPhotoImage.gameObject.SetActive(false);
    //}

    public void SetDrawingLine(int index, List<Vector3> points)
    {
        lineBrush.SetLine(index, points);
    }

    //public void SetKiteAngle(float angle)
    //{
    //    foreach (KiteController kite in kites)
    //    {
    //        kite.SetAngle(angle);
    //    }
    //}

    //public void SetPicture(byte[] photo)
    //{
    //    webCamera.SetPhoto(photo);
    //}

    //public void SetPicture(byte[] photo, int width, int height)
    //{
    //    webCamera.SetPhoto(photo, width, height);
    //}

    //public void SetPicture(string encodedPhoto)
    //{
    //    webCamera.SetPhoto(encodedPhoto);
    //}

    //public void SetPicture(string encodedPhoto, int width, int height)
    //{
    //    webCamera.SetPhoto(encodedPhoto, width, height);
    //}

    public void UpdateGoalKeeperPosition(Vector3 position, Vector3 handPosition)
    {
        footballController.UpdateGoalKeeperPosition(position, handPosition);
    }

    public void UpdateFootballPosition(Vector3 position, Vector3 eulerAngle)
    {
        footballController.UpdateFootballPosition(position, eulerAngle);
    }

    public void Shoot()
    {
        footballController.Shoot();
    }

    public ulong GetClientId()
    {
        if (Settings.networkType == Settings.NetworkType.NetCode)
        {
            return WTI.NetCode.NetworkController.Instance == null ? 0 : WTI.NetCode.NetworkController.Instance.GetClientId();
        }
        else
        {
            return WTI.PUN.NetworkController.Instance == null ? 0 : (ulong)WTI.PUN.NetworkController.Instance.GetClientId();
        }
    }
}
