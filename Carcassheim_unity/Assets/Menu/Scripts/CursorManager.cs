using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// include fonctions du script via la classe HomeMenu incluant elle meme (ConnectionMenu + Miscellaneous + Monobehaviour)
public class CursorManager : HomeMenu, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	private OptionsMenu option;
	private AccountMenu acc;
	private CreditsMenu cred;
	private StatistiquesMenu stat;
	private Texture2D cursorTexture;
	private CursorMode cursorMode = CursorMode.Auto;
	private Vector2 cursorHotspot = Vector2.zero;
	private Color previousColor;
	private Text btnText;
	private bool tmpBool;
	// Start is called before the first frame update
	void Start()
	{
		// Cursor Texture :
		cursorTexture = Resources.Load("basic_01 BLUE") as Texture2D;
		Cursor.SetCursor(cursorTexture, cursorHotspot, cursorMode);
		// SCRIPT :
		option = gameObject.AddComponent(typeof(OptionsMenu)) as OptionsMenu;
		acc = gameObject.AddComponent(typeof(AccountMenu)) as AccountMenu;
		cred = gameObject.AddComponent(typeof(CreditsMenu)) as CreditsMenu;
		stat = gameObject.AddComponent(typeof(StatistiquesMenu)) as StatistiquesMenu;
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		bool hasText = !GameObject.Find(name).GetComponent<Toggle>() && GameObject.Find(name).GetComponentInChildren<Text>(); //pour bouttons (texte), et non toggle (pas de texte)
		Debug.Log(hasText);
		tmpBool = StrCompare(name, "Btn Jouer") || StrCompare(name, "Btn Statistiques");
		bool tmp = (!getState() && !tmpBool) || getState();
		if (tmp)
		{
			if (hasText)
			{
				btnText = GameObject.Find(name).GetComponentInChildren<Text>();
			}

			switch (name)
			{
				// HomeMenu :
				case "Btn Connexion":
					ShowConnection();
					break;
				case "Btn Jouer":
					if (getState())
						Jouer();
					break;
				case "Btn Statistiques":
					if (getState())
						ShowStatistiques();
					break;
				case "Btn Options":
					ShowOptions();
					break;
				case "Btn Quitter le jeu":
					Quitter();
					break;
				// StatistiquesMenu : 
				case "Btn Retour Stat":
					stat.HideStatistiques();
					break;
				// OptionsMenu :
				case "Btn Retour Opt":
					option.HideOptions();
					break;
				case "Btn Son":
					option.SwitchSound();
					break;
				case "Toggle French":
				case "Toggle English":
				case "Toggle German":
					option.FlagsToggle();
					break;
				case "Btn Musique":
					option.SwitchMusic();
					break;
				case "Btn Fenêtré":
					option.FullScreen();
					break;
				case "Btn Aide":
					option.Help();
					break;
				case "Btn Credits":
					option.ShowCredits();
					break;
				// CreditsMenu :
				case "Btn Retour Credits":
					cred.HideCredits();
					break;
				// ConnectionMenu :
				case "Btn Retour Co":
					HideConnection();
					break;
				case "Btn ForgottenPwdUser":
					ForgottenPwdUser();
					break;
				case "Btn Se Connecter":
					Connect();
					break;
				case "Btn Creer un compte":
					CreateAccount();
					break;
				case "Toggle AfficherMdp":
					HideShowPwd();
					break;
				// AccountMenu
				case "Btn Retour Crea CA":
					acc.HideAccount();
					break;
				case "Btn Creer votre compte":
					acc.CreateAccountConnected();
					break;
				case "Toggle AfficherMdp CA":
					acc.HideShowPwdConf();
					break;
				default:
					return;
			}

			if (hasText)
			{
				if (hasMenuChanged())
				{
					btnText.fontSize -= 3;
					btnText.color = previousColor;
					setMenuChanged(false);
				}
				else
					tryColorText(btnText, Color.blue, "#1e90ff");
			}
			GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
		}
	}
	

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (GameObject.Find(name).GetComponent<Button>())
		{
			btnText = GameObject.Find(name).GetComponent<Button>().GetComponentInChildren<Text>();
			tmpBool = StrCompare(name, "Btn Jouer") || StrCompare(name, "Btn Statistiques");
			if (!getState() && tmpBool)
			{
				tryColorText(btnText, Color.grey, "#808080");
			}
			else
			{
				previousColor = btnText.color;
				tryColorText(btnText, Color.blue, "#1e90ff");
				btnText.fontSize += 3;
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (GameObject.Find(name).GetComponent<Button>())
		{
			bool tmpBool = StrCompare(name, "Btn Jouer") || StrCompare(name, "Btn Statistiques");
			if (getState() || !tmpBool)
			{
				btnText.color = previousColor;
				btnText.fontSize -= 3;
			}
		}
	}
}