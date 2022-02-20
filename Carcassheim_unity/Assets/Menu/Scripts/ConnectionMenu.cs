using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConnectionMenu : MonoBehaviour
{
	private HomeMenu home;
	private Miscellaneous ms;
	private static bool State = false;
	// Start is called before the first frame update
	void Start()
	{
		// SCRIPT :
		ms = gameObject.AddComponent(typeof(Miscellaneous)) as Miscellaneous;
		home = gameObject.AddComponent(typeof(HomeMenu)) as HomeMenu;
	}

	// Update is called once per frame
	void Update()
	{
	}

	public bool getState()
	{
		return State;
	}

	public void setState(bool b)
	{
		State = b;
	}

	public void resetWarningTextCM()
	{
		GameObject tmpGO = GameObject.Find("Instructions");
		Text tmpText = tmpGO.GetComponent<Text>();
		ms.tryColor(tmpGO, Color.white, "f4fefe");
		tmpText.text = "Connectez vous";
	}

	public void HideConnection()
	{
		resetWarningTextCM();
		ms.changeMenu(ms.FindMenu("ConnectionMenu"), ms.FindMenu("HomeMenu"));
	}

	public void ForgottenPwdUser()
	{
		Application.OpenURL("https://tinyurl.com/Kakyoin-and-Polnareff");
	}

	public void Connected()
	{
		Color newCol;
		Button tmpStat = GameObject.Find("Btn Statistiques").GetComponent<Button>();
		Button tmpJouer = GameObject.Find("Btn Jouer").GetComponent<Button>();
		ms.tryColor(GameObject.Find("Etat de connexion"), Color.green, "#90EE90");
		GameObject.Find("Etat de connexion").GetComponent<Text>().text = "Connecte";
		GameObject.Find("Etat de connexion").transform.position = new Vector3(1250, 475, 0);
		GameObject.Find("Btn Connexion").SetActive(false);
		tmpJouer.interactable = true;
		tmpStat.interactable = true;
		ColorUtility.TryParseHtmlString("#f4fefe", out newCol);
		tmpJouer.GetComponentInChildren<Text>().color = newCol;
		tmpStat.GetComponentInChildren<Text>().color = newCol;
		Debug.Log("Connecté");
	}

	public void Connect()
	{
		bool a = ms.StrCompare(GameObject.Find("InputField Email/Login").GetComponent<InputField>().text, "Hello");
		bool b = ms.StrCompare(GameObject.Find("InputField Password").GetComponent<InputField>().text, "World");
		State = a && b;
		GameObject tmpGO = GameObject.Find("Instructions");
		Text tmpText = tmpGO.GetComponent<Text>();
		if (State)
		{
			HideConnection();
			Connected();
		}
		else
		{
			ms.randomIntColor(tmpGO);
			tmpText.text = "Ressaissiez votre login et votre mot de passe !";
		}
	}

	public void CreateAccount()
	{
		GameObject tmpGO = GameObject.Find("Instructions");
		Text tmpText = tmpGO.GetComponent<Text>();
		ms.tryColor(tmpGO, Color.white, "f4fefe");
		tmpText.text = "Connectez vous";
		ms.changeMenu(ms.FindMenu("ConnectionMenu"), ms.FindMenu("CreateAccountMenu"));
	}
}