using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [SerializeField]
    InputField ip;
    [SerializeField]
    InputField port;

    void Start(){
        ip.text = PlayerPrefs.GetString("ip");
        port.text = PlayerPrefs.GetString("port");
    }
    public void OnStartButtonPushed() {
        try{
            PlayerPrefs.SetString("ip", ip.text);
            PlayerPrefs.SetString("port", port.text);
            GeneralSettings.DistinationIP = ip.text;
            GeneralSettings.DistinationPort = int.Parse(port.text);
        } catch (Exception e) {
            return;
        }

        SceneManager.LoadScene(1);
    }
}
