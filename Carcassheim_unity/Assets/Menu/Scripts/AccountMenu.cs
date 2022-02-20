using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* using System.Text.RegularExpressions; // needed for Regex */
public class AccountMenu : MonoBehaviour
{
	private static ConnectionMenu co;
	private static Miscellaneous ms;
	// Start is called before the first frame update
	void Start()
	{
		// SCRIPT :
		ms = gameObject.AddComponent(typeof(Miscellaneous)) as Miscellaneous;
		co = gameObject.AddComponent(typeof(ConnectionMenu)) as ConnectionMenu;
		if (ms.FindMenu("CreateAccountMenu").activeSelf)
		{
			GameObject.Find("Toggle CA").GetComponent<Toggle>().isOn = false;
			InputField tmpDay = GameObject.Find("InputField Day CA").GetComponent<InputField>();
			tmpDay.characterLimit = 2;
			InputField tmpDay2 = GameObject.Find("InputField Month CA").GetComponent<InputField>();
			tmpDay2.characterLimit = 2;
			InputField tmpDay3 = GameObject.Find("InputField Year CA").GetComponent<InputField>();
			tmpDay3.characterLimit = 4;
		/*  InputField.CharacterValidation =  tmpDay.CharacterValidation.None; */
		/* Regex.Replace(tmpDay.text, @"[^a-zA-Z0-9 ]", ""); */
		}
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void resetWarningTextAM()
	{
		GameObject tmpGO = GameObject.Find("Create Account");
		Text tmpText = tmpGO.GetComponent<Text>();
		ms.tryColor(tmpGO, Color.white, "f4fefe");
		tmpText.text = "Creez votre compte";
	}

	public void HideAccount()
	{
		resetWarningTextAM();
		ms.changeMenu(ms.FindMenu("CreateAccountMenu"), ms.FindMenu("ConnectionMenu"));
	}

	public void HideAccountConnected()
	{
		resetWarningTextAM();
		ms.changeMenu(ms.FindMenu("CreateAccountMenu"), ms.FindMenu("HomeMenu"));
		co.Connected();
	}

	public bool getInputFields()
	{
		bool a = ms.StrCompare(GameObject.Find("InputField Pseudo CA").GetComponent<InputField>().text, "a");
		bool b = ms.StrCompare(GameObject.Find("InputField Email CA").GetComponent<InputField>().text, "b");
		bool c = ms.StrCompare(GameObject.Find("InputField Day CA").GetComponent<InputField>().text, "01");
		bool d = ms.StrCompare(GameObject.Find("InputField Month CA").GetComponent<InputField>().text, "02");
		bool e = ms.StrCompare(GameObject.Find("InputField Year CA").GetComponent<InputField>().text, "0304");
		string tmpPwd = GameObject.Find("InputField Password CA").GetComponent<InputField>().text;
		string tmpPwd2 = GameObject.Find("InputField ConfirmPwd CA").GetComponent<InputField>().text;
		bool f = ms.StrCompare(tmpPwd, "c");
		bool g = ms.StrCompare(tmpPwd2, tmpPwd);
		return a && b && c && d && e && f && g;
	}

	public void CreateAccountConnected()
	{
		bool tmpBool = GameObject.Find("Toggle CA").GetComponent<Toggle>().isOn;
		GameObject tmpGO = GameObject.Find("Create Account");
		Text tmpText = tmpGO.GetComponent<Text>();
		co.setState(tmpBool && getInputFields());
		if (co.getState())
		{
			HideAccountConnected();
		}
		else
		{
			ms.randomIntColor(tmpGO);
			tmpText.text = "Ressaisissez vos informations et acceptez les CGU en cochant la case !";
		}
	}
}