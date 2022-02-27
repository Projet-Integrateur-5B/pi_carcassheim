using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreditsMenu : Miscellaneous
{
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void HideCredits()
	{
		changeMenu(FindMenu("CreditsMenu"), FindMenu("OptionsMenu"));
	}
}