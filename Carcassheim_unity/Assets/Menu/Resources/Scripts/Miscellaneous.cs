using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/* Convention de nommage : 
 * 
 * Méthodes : Méthode(int a...)
 * Attributs : public int Variable, private int _variable, static s_variable
 * Variables locales : int variable;
 * Classes : Classe1
 * 
 */
public abstract class Miscellaneous : MonoBehaviour
{
	private static bool s_state = false;
	private static bool s_menuHasChanged = false;
	private static GameObject previousMenu = null;
	private static GameObject nextMenu = null;
	private Color colState;
	public GameObject Pop_up_Options;
	public static bool s_isOpenPanel = false;
	void Awake()
	{
		Pop_up_Options = GameObject.Find("SubMenus").transform.Find("Panel Options").gameObject;
		nextMenu = GameObject.Find("HomeMenu"); // Menu courant au lancement du jeu
	}

	// PATCH : 
	public void GetScripts()
	{
		var scripts = Resources.LoadAll<MonoScript>("Scripts");
		int len = scripts.Length;
		foreach (var script in scripts)
		{
			// GetClass method returns the type of the script
			Debug.Log("Script : " + script.GetClass());
		}
	}

	public void HidePopUpOptions()
	{
		SetPanelOpen(false);
		Pop_up_Options.SetActive(GetPanelOpen());
	}

	public void SetPanelOpen(bool b)
	{
		s_isOpenPanel = b;
	}

	public bool GetPanelOpen()
	{
		return s_isOpenPanel;
	}

	public bool GetState()
	{
		return s_state;
	}

	public void SetState(bool b)
	{
		s_state = b;
	}

	public void Connected()
	{
		ColorUtility.TryParseHtmlString("#90EE90", out colState);
		Button tmpStat = GameObject.Find("ShowStat").GetComponent<Button>();
		Button tmpJouer = GameObject.Find("ShowRoomSelection").GetComponent<Button>();
		GameObject.Find("Etat de connexion").GetComponent<Text>().color = colState;
		GameObject.Find("Etat de connexion").GetComponent<Text>().text = "Connecte";
		GameObject.Find("ShowConnection").SetActive(false);
		tmpJouer.interactable = tmpStat.interactable = true;
		tmpJouer.GetComponentInChildren<Text>().color = tmpStat.GetComponentInChildren<Text>().color = Color.white;
	}

	public void SetMenuChanged(bool b)
	{
		s_menuHasChanged = b;
	}

	public bool HasMenuChanged()
	{
		return s_menuHasChanged;
	}

	public GameObject GetPreviousMenu()
	{
		return previousMenu;
	}

	public GameObject GetNextMenu()
	{
		return nextMenu;
	}

	public GameObject FirstActiveChild(GameObject FAGO)
	{
		foreach (Transform child in FAGO.transform)
			if (child.gameObject.activeSelf)
				return child.gameObject;
		return null;
	}

	public GameObject GetCurrentMenu()
	{
		return nextMenu;
	}

	public void ChangeMenu(string close, string goTo)
	{
		s_menuHasChanged = true;
		previousMenu = GameObject.Find(close).gameObject;
		nextMenu = GameObject.Find("SubMenus").transform.Find(goTo).gameObject;
		previousMenu.SetActive(false);
		nextMenu.SetActive(true);
	}

	//Not for passwords -> "" = char
	public string RemoveLastSpace(string mot) // Inputfield
	{
		string modif = mot.TrimEnd();
		return (mot.Length > 1) ? modif : mot;
	}
}