/*using System.Collections;
using System.Collections.Generic;*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsMenu : Miscellaneous
{
	private Button btnSon;
	private Button btnMusique;
	private AudioSource soundCtrl;
	private AudioSource musicCtrl;
	private Scrollbar soundScroll;
	private Scrollbar musicScroll;
	private Text pourcentSon;
	private Text pourcentMusique;
	private float previousSoundVol = 0.0f;
	private float previousMusicVol = 0.0f;
	private static bool tmpOnce = true;
	float lastSoundValue = 0;
	float lastMusicValue = 0;
	// Start is called before the first frame update
	void Start()
	{
		btnSon = FindGoTool("OptionsMenu", "Btn Son").GetComponent<Button>();
		btnMusique = FindGoTool("OptionsMenu", "Btn Musique").GetComponent<Button>();
		
		soundCtrl = GameObject.Find("SoundController").GetComponent<AudioSource>();
		musicCtrl = GameObject.Find("MusicController").GetComponent<AudioSource>();
		soundScroll = FindGoTool("OptionsMenu", "Scrollbar Son").GetComponent<Scrollbar>();
		musicScroll = FindGoTool("OptionsMenu", "Scrollbar Musique").GetComponent<Scrollbar>();
		pourcentSon = FindGoTool("OptionsMenu", "Pourcent Son").GetComponent<Text>();
		pourcentMusique = FindGoTool("OptionsMenu", "Pourcent Musique").GetComponent<Text>();
		defaultMusicSound();
		//Subscribe to the Scrollbar event
		soundScroll.onValueChanged.AddListener(soundScrollbarCallBack);
		lastSoundValue = soundScroll.value;
		musicScroll.onValueChanged.AddListener(musicScrollbarCallBack);
		lastMusicValue = musicScroll.value;
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void HideOptions()
	{
		changeMenu(FindMenu("OptionsMenu"), FindMenu("HomeMenu"));
	}

	public void FlagsToggle() //affiche la langue du toggle enclenche
	{
		// foreach
		if (GameObject.Find("Toggle French").GetComponent<Toggle>().isOn == true)
		{
			Debug.Log("French");
		}
		else if (GameObject.Find("Toggle English").GetComponent<Toggle>().isOn == true)
		{
			Debug.Log("English");
		}
		else if (GameObject.Find("Toggle German").GetComponent<Toggle>().isOn == true)
		{
			Debug.Log("German");
		}
	}

	//---------------------------- Music/Sound Begin ----------------------------//
	public void volumeSound(float value)
	{
		soundCtrl.volume = value;
		pourcentSon.text = Mathf.RoundToInt(value * 100) + "%";
	}

	public void volumeMusic(float value)
	{
		musicCtrl.volume = value;
		pourcentMusique.text = Mathf.RoundToInt(value * 100) + "%";
	}

	public void displayVolumeSound(float value)
	{
		if (value > 0 /* && soundCtrl.isPlaying */)
		{
			btnSon.GetComponentInChildren<Text>().text = "Son 'ON'";
		}
		else
		{
			btnSon.GetComponentInChildren<Text>().text = "Son 'OFF'";
		}

		volumeSound(value);
	}

	public void displayVolumeMusic(float value)
	{
		if (value > 0)
		{
			btnMusique.GetComponentInChildren<Text>().text = "Musique 'ON'";
		}
		else
		{
			btnMusique.GetComponentInChildren<Text>().text = "Musique 'OFF'";
		}

		volumeMusic(value);
	}

	public void defaultMusicSound()
	{
		if (tmpOnce == true)
		{
			soundScroll.numberOfSteps = musicScroll.numberOfSteps = 11; // 0->10 = 11
			soundCtrl.volume = musicCtrl.volume = soundScroll.value = musicScroll.value = 0.2f;
			volumeSound(soundScroll.value);
			volumeMusic(musicScroll.value);
			tmpOnce = !tmpOnce;
		}
	}

	//Will be called when Scrollbar changes
	public void soundScrollbarCallBack(float value)
	{
		/*  if (lastSoundValue > value)
        Debug.Log("Scrolling UP: " + value);
    else
        Debug.Log("Scrolling DOWN: " + value);
*/
		displayVolumeSound(value);
		/* lastSoundValue = value; */
	}

	//Will be called when Scrollbar changes
	public void musicScrollbarCallBack(float value)
	{
		displayVolumeMusic(value);
	}

	/* public void OnDisable()
{
    //Un-Subscribe To Scrollbar Event
    soundScroll.onValueChanged.RemoveListener(soundScrollbarCallBack);
	musicScroll.onValueChanged.RemoveListener(musicScrollbarCallBack);
} */
	public void SwitchSound()
	{
		if (soundScroll.value != 0)
		{
			previousSoundVol = soundCtrl.volume;
			soundScroll.value = 0.0f;
			soundScrollbarCallBack(soundScroll.value);
		}
		else
		{
			soundScroll.value = previousSoundVol;
			soundScrollbarCallBack(previousSoundVol);
		}
	}

	public void SwitchMusic()
	{
		if (musicScroll.value != 0)
		{
			previousMusicVol = musicCtrl.volume;
			musicScroll.value = 0.0f;
			musicScrollbarCallBack(musicScroll.value);
		}
		else
		{
			musicScroll.value = previousMusicVol;
			musicScrollbarCallBack(previousMusicVol);
		}
	}

	// -------------- Music/Sound End -----------------------//
	public void FullScreen()
	{
		Screen.fullScreen = !Screen.fullScreen;
		Debug.Log("Windowed");
	}

	public void Help()
	{
		Application.OpenURL("https://tinyurl.com/SlapDance");
	}
}