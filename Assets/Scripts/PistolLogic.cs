using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PistolLogic : MonoBehaviour
{
    private float _nombreMunitionsMaximals;
    private float _nombreMunitionsDansChargeur;

    private float degatsParBalle;

    private GameObject boutPistolet;

    [SerializeField]
    GameObject shotPrefab;

    // Sounds
    AudioSource soundSource;

    [SerializeField]
    AudioClip emptyCharger;

    [SerializeField]
    AudioClip reloadSound;

    [SerializeField]
    AudioClip gunshot;

    // Start is called before the first frame update
    void Start()
    {
        soundSource = GetComponentInChildren<AudioSource>();
        
        // instancier variables selon le type d'arme
        if (CompareTag("SMG"))
        {
            degatsParBalle = 10f;
            _nombreMunitionsMaximals = 30f;
        }
        else if (CompareTag("Pistol"))
        {
            degatsParBalle = 25f;
            _nombreMunitionsMaximals = 10f;
        }
        else if (CompareTag("AR"))
        {
            degatsParBalle = 30;
            _nombreMunitionsMaximals = 20;
        }

        _nombreMunitionsDansChargeur = _nombreMunitionsMaximals;

        boutPistolet = transform.Find("SortieDeBalle").gameObject;

        // Activation lumière si il n'est pas pris
        if (gameObject.transform.parent == null)
        {
            GetComponent<Light>().enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float getDegatsParBalle()
    {
        return degatsParBalle;
    }

    public float getNbMunitions()
    {
        return _nombreMunitionsDansChargeur;
    }

    public float getNbBallesMaximals()
    {
        return _nombreMunitionsMaximals;
    }

    public void recharge(float recharge)
    {
        _nombreMunitionsDansChargeur = recharge;
    }

    public void tirer()
    {
        if (_nombreMunitionsDansChargeur > 0)
        {
            _nombreMunitionsDansChargeur -= 1;

            Instantiate(shotPrefab, boutPistolet.transform.position, Quaternion.identity);
            makeGunshotSound();
        }
        else
            makeEmptyChargerSound();
     }

    // Sons de l'arme
    public void makeEmptyChargerSound()
    {
        AudioClip clip = emptyCharger;
        soundSource.clip = clip;
        soundSource.volume = SettingsManager.instance.GetSoundVolume();
        soundSource.Play();
    }

    public void makeReloadingSound()
    {
        AudioClip clip = reloadSound;
        soundSource.clip = clip;
        soundSource.volume = SettingsManager.instance.GetSoundVolume();
        soundSource.Play();
    }

    public void makeGunshotSound()
    {
        AudioClip clip = gunshot;
        soundSource.clip = clip;
        soundSource.volume = SettingsManager.instance.GetSoundVolume();
        soundSource.Play();
    }
}
