using UnityEngine;
using UnityEngine.UI; /* using System.Text.RegularExpressions; // needed for Regex */
using Assets.System;
using ClassLibrary;
using System.Collections.Generic;
using System.Threading;

/// <summary>
///     Account menu.
/// </summary>
public class AccountMenu : Miscellaneous
{
    private Transform accMenu, AMCI; // Account Menu Container InputField
    private InputField pseudoCA, emailCA, passwordCA, confirmPwdCA;
    [SerializeField] StringValidator dayCA, monthCA, yearCA;
    private static bool boolCGU = false;
    public List<bool> listAction;
    public Semaphore s_listAction;
    private GameObject tmpGO;
    private Text tmpText;

    static int created = 0;
    int id_Object = 0;

    [SerializeField] TMPro.TMP_Text errorText;

    /// <summary>
    /// Start is called before the first frame update <see cref = "AccountMenu"/> class.
    /// </summary>
    void Start()
    {
        id_Object = created++;
        Debug.Log("xd " + id_Object);
        // INITIALISATION
        accMenu = GameObject.Find("SubMenus").transform.Find("AccountMenu").transform;
        AMCI = accMenu.transform.Find("InputField").Find("InputFieldEndEdit").transform;
        Debug.Log(AMCI);
        AMCI.Find("InputField Day CA").GetComponent<InputField>().characterLimit = 2;
        AMCI.Find("InputField Month CA").GetComponent<InputField>().characterLimit = 2;
        AMCI.Find("InputField Year CA").GetComponent<InputField>().characterLimit = 4;
        pseudoCA = AMCI.Find("InputField Pseudo CA").GetComponent<InputField>();
        Debug.Log("pseudo " + pseudoCA);
        emailCA = AMCI.Find("InputField Email CA").GetComponent<InputField>();
        Debug.Log("email " + emailCA);
        passwordCA = AMCI.Find("InputField Password CA").GetComponent<InputField>();
        Debug.Log("password " + passwordCA);
        confirmPwdCA = AMCI.Find("InputField ConfirmPwd CA").GetComponent<InputField>();
        Debug.Log("confirms " + confirmPwdCA);
        passwordCA.inputType = confirmPwdCA.inputType = InputField.InputType.Password; // Hide password by default
        listAction = new List<bool>();
        s_listAction = new Semaphore(1, 1);
        /*
		tmpGO = GameObject.Find("Create Account");
		tmpText = tmpGO.GetComponent<Text>();
		*/
        errorText.outlineWidth = 0.3f;
        errorText.outlineColor = new Color32(255, 128, 0, 255);
    }

    void OnEnable()
    {
        Debug.Log("HALLO");
        OnMenuChange += OnStart;
    }

    void OnDisable()
    {
        Debug.Log("OSKOUR");
        OnMenuChange -= OnStart;
    }



    /// <summary>
    /// OnStart is called when the menu is changed to this one <see cref = "AccountMenu"/> class.
    /// </summary>
    /// <param name = "pageName">Page name.</param>
    public void OnStart(string pageName)
    {
        switch (pageName)
        {
            case "AccountMenu":
                /* Commuication Async */
                Communication.Instance.StartListening(OnPacketReceived);
                ClearAll(null);
                break;
            default:
                /* Ce n'est pas la bonne page */
                /* Stop la reception dans cette class */
                errorText.gameObject.SetActive(false);
                Communication.Instance.StopListening(OnPacketReceived);
                break;
        }
    }

    /// <summary>
    /// Reset Warning on text fields <see cref = "AccountMenu"/> class.
    /// </summary>
    public void ResetWarningTextAM()
    {
        /*
            tmpText.color = Color.white;
            tmpText.text = "Creez votre compte";
            */
    }

    /// <summary>
    /// Hide Account menu when connected to the server <see cref = "AccountMenu"/> class.
    /// </summary>
    public void HideAccount()
    {
        ResetWarningTextAM();
        HidePopUpOptions();
        ChangeMenu("AccountMenu", "ConnectionMenu");
    }

    /// <summary>
    /// Hide Account when connected to the server <see cref = "AccountMenu"/> class.
    /// </summary>
    public void HideAccountConnected()
    {
        ResetWarningTextAM();
        HidePopUpOptions();
        ChangeMenu("AccountMenu", "HomeMenu");
        Connected();
    }

    /// <summary>
    /// Toggle Password visibility <see cref = "AccountMenu"/> class.
    /// </summary>
    /// <param name = "curT">Current Toggle</param>
    public void ToggleValueChangedAM(Toggle curT)
    {
        if (curT.name == "Toggle ShowPwdAcc")
        {
            if (curT.isOn)
                passwordCA.inputType = confirmPwdCA.inputType = InputField.InputType.Standard;
            else
                passwordCA.inputType = confirmPwdCA.inputType = InputField.InputType.Password;
            //Changement immédiat sans reclic InputField
            passwordCA.ForceLabelUpdate();
            confirmPwdCA.ForceLabelUpdate();
        }

        if (curT.name == "Toggle CGU")
            boolCGU = !boolCGU;
    }

    /// <summary>
    /// Get the input value of the input fields password and confirm password <see cref = "AccountMenu"/> class.
    /// </summary>
    /// <returns>The value of the input fields password and confirm password</returns>
    public bool GetInputFields()
    {
        string tmpPwd = RemoveLastSpace(passwordCA.text);
        string tmpPwd2 = RemoveLastSpace(confirmPwdCA.text);
        bool valid_pass = string.Equals(tmpPwd2, tmpPwd);
        bool is_empty = RemoveLastSpace(tmpPwd).Length == 0;
        bool name_is_empty = RemoveLastSpace(pseudoCA.text).Length == 0;
        bool mail_is_empty = RemoveLastSpace(emailCA.text).Length == 0;

        int day = dayCA.endValue(), month = monthCA.endValue(), year = yearCA.endValue();
        System.DateTime date;
        bool valid_date = System.DateTime.TryParse("" + day + "/" + month + "/" + year, out date);
        if (name_is_empty)
        {
            switch (OptionsMenu.langue)
            {
                case 1:
                    errorText.SetText("Please write your login.");
                    break;
                case 2:
                    errorText.SetText("Please");
                    break;
                default:
                    errorText.SetText("Merci d'écrire votre nom d'utilisateur");
                    break;
            }
            errorText.gameObject.SetActive(true);
        }

        else if (mail_is_empty)
        {
            switch (OptionsMenu.langue)
            {
                case 1:
                    errorText.SetText("Please write your mail");
                    break;
                case 2:
                    errorText.SetText("Please");
                    break;
                default:
                    errorText.SetText("Merci d'écrire votre e-mail");
                    break;
            }
            errorText.gameObject.SetActive(true);
        }

        else if (!valid_date)
        {
            switch (OptionsMenu.langue)
            {
                case 1:
                    errorText.SetText("Please write a valid date.");
                    break;
                case 2:
                    errorText.SetText("Please");
                    break;
                default:
                    errorText.SetText("Merci d'écrire une date valide");
                    break;
            }
            errorText.gameObject.SetActive(true);
        }

        else if (is_empty)
        {
            switch (OptionsMenu.langue)
            {
                case 1:
                    errorText.SetText("Please write a password.");
                    break;
                case 2:
                    errorText.SetText("Please");
                    break;
                default:
                    errorText.SetText("Merci d'écrire un mot de passe");
                    break;
            }
            errorText.gameObject.SetActive(true);
        }

        else if (!valid_pass)
        {
            switch (OptionsMenu.langue)
            {
                case 1:
                    errorText.SetText("The passwords doesn't match.");
                    break;
                case 2:
                    errorText.SetText("Passwörter stimmen nicht überein");
                    break;
                default:
                    errorText.SetText("Les mots de passes ne correspondent pas");
                    break;
            }
            errorText.gameObject.SetActive(true);
        }
        else if (!boolCGU)
        {
            switch (OptionsMenu.langue)
            {
                case 1:
                    errorText.SetText("Please accept the CGU.");
                    break;
                case 2:
                    errorText.SetText("Vielen Dank, dass Sie die CGU akzeptiert haben.");
                    break;
                default:
                    errorText.SetText("Merci d'accepter les CGU.");
                    break;
            }
            errorText.gameObject.SetActive(true);
        }
        return valid_pass && boolCGU && !is_empty && !mail_is_empty && !name_is_empty && valid_date;
    }

    /// <summary>
    /// Create the account <see cref = "AccountMenu"/> class.
    /// </summary>
    public void CreateAccount()
    {
        if (GetInputFields())
        {
            Packet packet = new Packet();
            packet.IdMessage = Tools.IdMessage.AccountSignup;
            packet.IdPlayer = 0;
            packet.Data = new[] { RemoveLastSpace(pseudoCA.text), RemoveLastSpace(passwordCA.text), RemoveLastSpace(emailCA.text), GameObject.Find("InputField Year CA").GetComponent<InputField>().text + "/" + GameObject.Find("InputField Month CA").GetComponent<InputField>().text + "/" + GameObject.Find("InputField Day CA").GetComponent<InputField>().text };
            Communication.Instance.SendAsync(packet);
        }
        else
        {
            Debug.Log("CHAMPS PAS OK");
        }
    }

    /// <summary>
    // CGU Link
    /// </summary>
    public void CGU()
    {
        //Application.OpenURL("https://tinyurl.com/Kakyoin-and-Polnareff");
    }

    /// <summary>
    /// OnPacketReceived is called when a packet is received <see cref = "AccountMenu"/> class.
    /// </summary>
    /// <param name = "sender">Sender.</param>
    /// <param name = "packet">Packet.</param>
    public void OnPacketReceived(object sender, Packet packet)
    {
        bool res = false;
        if (packet.IdMessage == Tools.IdMessage.AccountLogin)
        {
            if (packet.Error == Tools.Errors.None)
            {
                res = true;
            }

            s_listAction.WaitOne();
            listAction.Add(res);
            s_listAction.Release();
        }
    }

    /// <summary>
    /// Update every frame <see cref = "AccountMenu"/> class.
    /// </summary>
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
            Debug.Log(GetState());
            if (GetState())
            {
                HideAccountConnected();
                Connected();
            }
            else
            {
                switch (OptionsMenu.langue)
                {
                    case 1:
                        errorText.SetText("The account couldn't be created.");
                        break;
                    case 2:
                        errorText.SetText("");
                        break;
                    default:
                        errorText.SetText("Le compte n'a pas pu être créé.");
                        break;
                }
                errorText.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Clean all input fields <see cref = "AccountMenu"/> class.
    /// </summary>
    public void ClearAll(string arg)
    {
        pseudoCA = Clear(pseudoCA);
        emailCA = Clear(emailCA);
        passwordCA = Clear(passwordCA);
        confirmPwdCA = Clear(confirmPwdCA);
    }
}