using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfigScreen : MonoBehaviour
{
    public TMP_InputField ipInputField;
    public Button connectButton;

    private void Start()
    {
        connectButton.onClick.AddListener(Connect);
        if (PlayerPrefs.HasKey("ip"))
        {
            ipInputField.text = PlayerPrefs.GetString("ip");
        }
    }


    private void Connect()
    {
        PlayerPrefs.SetString("ip", ipInputField.text);
        PlayerPrefs.Save();
        SceneManager.LoadScene("StaticGameplay");
    }
}
