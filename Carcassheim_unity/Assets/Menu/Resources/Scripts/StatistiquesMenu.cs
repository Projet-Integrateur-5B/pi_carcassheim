/// <summary>
///     Stats menu
/// </summary>
public class StatistiquesMenu : Miscellaneous
{
	/// <summary>
	///     Go to HomeMenu
	/// </summary>
	public void HideStat()
	{
		HidePopUpOptions();
		ChangeMenu("StatMenu", "HomeMenu");
	}
}