using UnityEngine;
using UnityEngine.UI;

public class StatistiquesMenu : Miscellaneous
{
	void Start()
	{
	}

	public void HideStat()
	{
		ChangeMenu("StatMenu", "HomeMenu");
	}
}