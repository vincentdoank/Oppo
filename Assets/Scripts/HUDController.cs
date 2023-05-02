using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class HUDController : MonoBehaviour
{
    public Button calibrateButton;

    public TMP_InputField roomNameInputField;
    public TMP_Dropdown controlTypeDropdown;

    public Button debugButton;
    public GameObject backgroundObj;

    private void OnEnable()
    {
        calibrateButton.onClick.AddListener(OnCalibrateButtonClicked);
        //debugButton.onClick.AddListener(OnDebugButtonClicked);

        //Array controlTypeArray = Enum.GetValues(typeof(GameManager.ControlType));
        //for (int i = 0; i < controlTypeArray.Length; i++)
        //{
        //    controlTypeDropdown.AddOptions(new List<TMP_Dropdown.OptionData>() {
        //    new TMP_Dropdown.OptionData {
        //        text = GetUpperCaseFirstLetter(controlTypeArray.GetValue(i).ToString())
        //    }});
        //}

        //controlTypeDropdown.AddOptions(new List<TMP_Dropdown.OptionData>() { 
        //    new TMP_Dropdown.OptionData { 
        //        text = GetUpperCaseFirstLetter(GameManager.ControlType.DRAW.ToString())
        //    }, 
        //    new TMP_Dropdown.OptionData { 
        //        text = GetUpperCaseFirstLetter(GameManager.ControlType.KITE.ToString())
        //    },
        //    new TMP_Dropdown.OptionData {
        //        text = GetUpperCaseFirstLetter(GameManager.ControlType.SHAKE.ToString())
        //    }
        //});
        //controlTypeDropdown.onValueChanged.AddListener(OnControlTypeDropDownSelected);

        //backgroundObj.SetActive(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor);

        //backgroundObj.SetActive(Application.platform == RuntimePlatform.Android);
    }

    private void OnDisable()
    {
        debugButton.onClick.RemoveListener(EventManager.onDebugButtonClicked.Invoke);
    }

    private void OnDebugButtonClicked()
    {
        EventManager.onDebugButtonClicked?.Invoke();
    }

    public void OnConnectButtonClicked()
    {
        //NetworkController.Instance.Connect();
        EventManager.onConnectToNetwork?.Invoke();
    }

    public void OnCreateButtonClicked()
    {
        //NetworkController.Instance.CreateRoom();
        EventManager.onCreateRoom?.Invoke();
    }

    public void OnJoinRoomButtonClicked()
    {
        //NetworkController.Instance.JoinRoom(roomNameInputField.text);
        EventManager.onJoinRoom?.Invoke(roomNameInputField.text);
    }

    public void OnExitButtonClicked()
    {
        //NetworkController.Instance.ExitRoom();
        EventManager.onLeaveRoom?.Invoke();
    }

    public void OnCaptureButtonClicked()
    {
        EventManager.onPhotoCaptured?.Invoke();
    }

    public void OnClearLineButtonClicked()
    {
        EventManager.onClearLine?.Invoke();
    }

    public void OnCalibrateButtonClicked()
    {
        EventManager.onCalibrate?.Invoke();
    }

    public void OnControlTypeDropDownSelected(int index)
    {
        EventManager.onControlTypeSelected?.Invoke(index);
        
    }

    private string GetUpperCaseFirstLetter(string text)
    {
        text = text.ToLower();
        return Regex.Replace(text, "^[a-z]", c => c.Value.ToUpper());
    }
}
