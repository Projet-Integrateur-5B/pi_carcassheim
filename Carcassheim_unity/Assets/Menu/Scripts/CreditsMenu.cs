using UnityEngine;
using UnityEngine.UI;

public class CreditsMenu : Miscellaneous
{
	void Start()
	{
	}

	void Update()
	{
	}

	public void HideCredits()
	{
		ChangeMenu("CreditsMenu", "OptionsMenu");
	}
}