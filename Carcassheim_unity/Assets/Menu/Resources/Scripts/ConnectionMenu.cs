using UnityEngine;
using UnityEngine.UI;
using Assets.System;
using ClassLibrary;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;

public class ConnectionMenu : Miscellaneous
{
    private Transform coMenu, CMCI; // Account Menu Container InputField
    private InputField loginCM, passwordCM;
    private GameObject tmpGO;
    private Text tmpText;
    private Toggle hidePwd;

    public List<bool> listAction;
    public Semaphore s_listAction;

    void Start()
    {
        coMenu = GameObject.Find("SubMenus").transform.Find("ConnectionMenu").transform;
        CMCI = coMenu.Find("InputField").transform.Find("InputFieldEndEdit").transform;
        loginCM = CMCI.GetChild(0).GetComponent<InputField>();
        passwordCM = CMCI.GetChild(1).GetComponent<InputField>();
        passwordCM.inputType = InputField.InputType.Password; // Hide password by default
        hidePwd = FindObject(gameObject, "Toggle ShowPwdCM").GetComponent<Toggle>();

        listAction = new List<bool>();
        s_listAction = new Semaphore(1, 1);


        OnMenuChange += OnStart;
        OnMenuChange += ClearAll;
    }

    public void OnStart(string pageName)
    {
        switch (pageName)
        {
            case "ConnectionMenu":
                /* Commuication Async */
                Communication.Instance.StartListening(OnPacketReceived);
                break;

            default:
                /* Ce n'est pas la bonne page */
                /* Stop la reception dans cette class */
                Communication.Instance.StopListening(OnPacketReceived);
                break;
        }
    }

    public void ResetWarningTextCM()
    {
        tmpGO = GameObject.Find("Instructions");
        tmpText = tmpGO.GetComponent<Text>();
        tmpText.color = Color.white;

        //on gere selon la langue
        if(OptionsMenu.langue == 0)
            tmpText.text = "Connectez vous";
        else if (OptionsMenu.langue == 1)
            tmpText.text = "Log in";
        else if (OptionsMenu.langue == 2)
            tmpText.text = "Loggen Sie sich ein";
    }

    public void HideConnection()
    {
        HidePopUpOptions();
        ResetWarningTextCM();
        ChangeMenu("ConnectionMenu", "HomeMenu");
    }

    public void ForgottenPwdUser()
    {

    }

    public void ToggleValueChangedCM(Toggle curT)
    {
        if (curT.name == "Toggle ShowPwdCM")
        {
            if (curT.isOn)
                passwordCM.inputType = InputField.InputType.Standard;
            else
                passwordCM.inputType = InputField.InputType.Password;
            //Changement immédiat sans reclic InputField
            passwordCM.ForceLabelUpdate();
        }
    }

    public void ShowAccount()
    {
        GameObject tmpGO = GameObject.Find("Instructions");
        Text tmpText = tmpGO.GetComponent<Text>();
        tmpText.color = Color.white;

        //on gere selon la langue
        if (OptionsMenu.langue == 0)
            tmpText.text = "Connectez vous";
        else if (OptionsMenu.langue == 1)
            tmpText.text = "Log in";
        else if (OptionsMenu.langue == 2)
            tmpText.text = "Loggen Sie sich ein";
        HidePopUpOptions();
        ChangeMenu("ConnectionMenu", "AccountMenu");
    }

    public void InputFieldEndEdit(InputField inp)
    {
        // Debug.Log("Input submitted" + " : " + inp.text);
    }

    public void Connect()
    {

        Packet packet = new Packet();
        packet.IdMessage = Tools.IdMessage.AccountLogin;
        packet.IdPlayer = 0;
        packet.Data = new[] { RemoveLastSpace(loginCM.text), RemoveLastSpace(passwordCM.text) };

        tmpGO = GameObject.Find("Instructions");
        tmpText = tmpGO.GetComponent<Text>();

        Communication.Instance.SendAsync(packet);
    }

    public void OnPacketReceived(object sender, Packet packet)
    {

        bool res = false;
        if (packet.IdMessage == Tools.IdMessage.AccountLogin)
        {
            if (packet.Error == Tools.Errors.None)
            {
                Communication.Instance.IdClient = packet.IdPlayer;
                res = true;
            }

            s_listAction.WaitOne();
            listAction.Add(res);
            s_listAction.Release();
        }
    }

    private void Update()
    {
        s_listAction.WaitOne();
        int taille = listAction.Count;
        s_listAction.Release();

        if (taille > 0)
        {
            for (int i = 0; i < taille; i++)
            {
                s_listAction.WaitOne();
                SetState(listAction[i]);
                s_listAction.Release();
            }

            s_listAction.WaitOne();
            listAction.Clear();
            s_listAction.Release();

            if (GetState())
            {
                Connected();
                HideConnection();
            }
            else
            {
                tmpGO.GetComponent<Text>().color = Color.yellow;

                //on gere selon la langue
                if (OptionsMenu.langue == 0)
                    tmpText.text = "Ressaisissez votre login et votre mot de passe !";
                else if (OptionsMenu.langue == 1)
                    tmpText.text = "Re-enter your login and password!";
                else if (OptionsMenu.langue == 2)
                    tmpText.text = "Geben Sie Ihren Login und Ihr Passwort erneut ein!";
                
            }
        }
    }
    
    public void ClearAll(string arg)
    {
        loginCM = Clear(loginCM);
        passwordCM = Clear(passwordCM);
        hidePwd.isOn = false;
    }
}