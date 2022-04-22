using Assets.System;

public class CreditsMenu : Miscellaneous
{
	public void HideCredits()
	{
		ChangeMenu("CreditsMenu", "OptionsMenu");
		Communication.Instance.isInRoom = 0;
	}
}