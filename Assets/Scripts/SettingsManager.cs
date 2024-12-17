using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private float musicVolume = 1f;
    private float soundVolume = 1f;

    private bool hardDifficulty;

    private MenuManager menuManager;

    public static SettingsManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Gestion de volumes pour la musique et les (sound effect)
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
    }

    public float GetMusicVolume() { return musicVolume; }

    public void SetSoundVolume(float volume) { soundVolume = volume; }

    public float GetSoundVolume() { return soundVolume; }

    public void setHardDifficulty(bool difficulty)
    {
        hardDifficulty = difficulty;
    }

    public bool GetHardDifficulty()
    {
        return hardDifficulty;
    }

    public void redemarreAnimationsJeu()
    {
        menuManager.demarreJeu();
    }
}
