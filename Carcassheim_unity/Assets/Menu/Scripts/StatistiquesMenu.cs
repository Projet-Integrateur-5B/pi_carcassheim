public class StatistiquesMenu : Miscellaneous
{
	public void HideStat()
	{
		SetPanelOpen(false);
		Pop_up_Options.SetActive(GetPanelOpen());
		ChangeMenu("StatMenu", "HomeMenu");
	}
}