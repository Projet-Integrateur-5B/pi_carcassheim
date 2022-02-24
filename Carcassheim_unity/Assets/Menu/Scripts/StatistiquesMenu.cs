using System;

public class StatistiquesMenu : Miscellaneous
{
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void HideStatistiques()
	{
		changeMenu(FindMenu("StatistiquesMenu"), FindMenu("HomeMenu"));
	}
}
