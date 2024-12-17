using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.DeviceSimulation;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    private CharacterController _characterController;
    private Vector3 _playerInput;

    private float _playerSpeed = 3f;

    private float munitionsMaximales = 100f;
    private float _munitions = 16f;

    private PistolLogic _pistoletJoueur;

    private float pointDeVie = 100f;

    private bool isPlayerAlive = true;

    private string[] collectionsPourGagner = new string[3];

    private bool contientGrenade = false;

    [SerializeField]
    GameObject grenadePrefab;

    [SerializeField]
    float throwForce;

    private float distanceDevant = 1f;

    [SerializeField]
    GameObject smokePrefab;

    private string[] toutesLesArmes = new string[3];
    private int indexGunMains = 0;

    [SerializeField]
    private GameObject[] armes;

    //SMG
    private float fireRate = 600f;
    private float nextFireTime;

    // Sounds
    AudioSource soundSource;

    [SerializeField]
    AudioClip ramasseObjet;

    [SerializeField]
    AudioClip changeGun;

    // Start is called before the first frame update
    void Start()
    {
        // Début du jeu avec uniquement le pistolet
        _characterController = GetComponent<CharacterController>();
        _pistoletJoueur = GetComponentInChildren<PistolLogic>();

        toutesLesArmes[0] = _pistoletJoueur.gameObject.tag;

        soundSource = GetComponentInChildren<AudioSource>();

        armes[1].SetActive(false);
        armes[2].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerAlive)
        {
            if (LevelManager.instance.getIsGamePaused())
                return;

            _playerInput = Vector3.zero;
            _playerInput.x = Input.GetAxis("Horizontal");
            _playerInput.z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * _playerInput.x + transform.forward * _playerInput.z;

            _characterController.Move(move * Time.deltaTime * _playerSpeed);

            if (Input.GetKeyDown(KeyCode.LeftShift))
                _playerSpeed *= 2;

            if (Input.GetKeyUp(KeyCode.LeftShift))
                _playerSpeed = 3f;

            // Tirer - Clic Gauche Selon pistoler ou AR
            if (Input.GetButtonDown("Fire1") && (_pistoletJoueur.CompareTag("Pistol") || _pistoletJoueur.CompareTag("AR")))
            {
                if (_pistoletJoueur.CompareTag("AR"))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        gestionTirSelonGun();
                    }
                }
                else
                    gestionTirSelonGun();
            }

            // Tirer avec le SMG en main
            if (Input.GetButton("Fire1") && _pistoletJoueur.CompareTag("SMG") && Time.time >= nextFireTime && _pistoletJoueur.getNbMunitions() > 0)
            {
                gestionTirSelonGun();

                nextFireTime = Time.time + 60f / fireRate;
            }

            // Gestion de recharge
            if (Input.GetKeyDown(KeyCode.R))
            {
                float nombreDeBallesAMettre = _pistoletJoueur.getNbBallesMaximals() - _pistoletJoueur.getNbMunitions();

                if (nombreDeBallesAMettre == 0)
                {
                    return;
                }
                else if (_munitions >= nombreDeBallesAMettre)
                {
                    _munitions -= nombreDeBallesAMettre;
                    _pistoletJoueur.recharge(_pistoletJoueur.getNbBallesMaximals());
                    _pistoletJoueur.makeReloadingSound();
                }
                else if (_munitions > 0)
                {
                    _pistoletJoueur.recharge(_pistoletJoueur.getNbMunitions() + _munitions);
                    _munitions = 0;
                    _pistoletJoueur.makeReloadingSound();
                }

            }

            // Gestion du lancement de la grenade selon force avec l'affichage
            if (Input.GetKeyDown(KeyCode.G) && contientGrenade)
            {
                Vector3 cameraPosition = Camera.main.transform.position;
                Quaternion cameraRotation = Camera.main.transform.rotation;

                Vector3 throwDirection = cameraRotation * Vector3.forward;

                throwDirection.Normalize();

                Vector3 spawnPosition = cameraPosition + throwDirection * distanceDevant;

                GameObject grenadeInstance = Instantiate(grenadePrefab, spawnPosition, Quaternion.identity);

                Rigidbody grenadeRigidbody = grenadeInstance.GetComponent<Rigidbody>();

                grenadeRigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);

                contientGrenade = false;
                LevelManager.instance.GrenadeTirer();
            }

            // Gestion changements d'armes
            if (Input.GetKeyDown(KeyCode.E))
            {
                changerProchaineArme();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                changerGunArriere();
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                changerProchaineArme();
            }
            else if (scroll < 0f)
            {
                changerGunArriere();
            }
        }
    }


    void changerProchaineArme()
    {
        int indexOriginel = indexGunMains;
        for (int i = indexGunMains + 1; i < toutesLesArmes.Length + indexGunMains; i++)
        {
            int index = i % toutesLesArmes.Length;

            if (toutesLesArmes[index] != null)
            {
                indexGunMains = index;
                ChangementArme(indexGunMains, indexOriginel);
                break;
            }
        }
    }

    void changerGunArriere()
    {
        int indexOriginel = indexGunMains;
        for (int i = indexGunMains - 1 + toutesLesArmes.Length; i > indexGunMains; i--)
        {
            int index = i % toutesLesArmes.Length;

            if (toutesLesArmes[index] != null)
            {
                indexGunMains = index;
                ChangementArme(indexGunMains, indexOriginel);
                break;
            }
        }
    }

    private void ChangementArme(int nouvIndex, int ancienIndex)
    {
        if (nouvIndex >= 0 && nouvIndex < armes.Length)
        {
            armes[nouvIndex].SetActive(true);
        }
        if (ancienIndex >= 0 && ancienIndex < armes.Length)
        {
            armes[ancienIndex].SetActive(false);
        }

        _pistoletJoueur = armes[nouvIndex].GetComponent<PistolLogic>();

        LevelManager.instance.changementArmeJoueur(_pistoletJoueur);
        makeChangeGunSound();
    }


    public void gestionTirSelonGun()
    {
        if (_pistoletJoueur.getNbMunitions() > 0)
        {
            _pistoletJoueur.tirer();

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            bool hitMade = Physics.Raycast(ray, out hit);
            if (hitMade)
            {
                EnnemyLogic enemy = hit.transform.GetComponentInParent<EnnemyLogic>();
                if (enemy != null)
                {
                    // Frapper ennemi avec une balle
                    if (hit.collider.name.Equals("Tête"))
                        enemy.PrendreDegats(_pistoletJoueur.getDegatsParBalle() * 4);
                    else if (hit.collider.name.Equals("BrasGauche") ||
                             hit.collider.name.Equals("BrasDroit") ||
                             hit.collider.name.Equals("JambeGauche") ||
                             hit.collider.name.Equals("JambeDroit"))
                        enemy.PrendreDegats(_pistoletJoueur.getDegatsParBalle() / 4);
                    else
                        enemy.PrendreDegats(_pistoletJoueur.getDegatsParBalle());
                }
                else
                {
                    // Balle raté
                    LevelManager.instance.makeMissedGunshotSound();

                    Instantiate(smokePrefab, hit.point, Quaternion.identity);
                }
            }
            else
            {
                LevelManager.instance.makeMissedGunshotSound();
            }
        }
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z);
    }

    // Gestion prise d'un objet
    private void OnTriggerEnter(Collider other)
    {
        Transform parentTrigger = other.transform.parent;

        if (other.gameObject.name.Equals("ZonePrendreGrenade"))
        {
            contientGrenade = true;
            LevelManager.instance.JoueurContientGrenade(parentTrigger.gameObject);
            makeItemPickedUpSound();
        }
        else if (parentTrigger.name.Equals("Document_01"))
        {
            collectionsPourGagner[0] = "Document_01";
            LevelManager.instance.cocherElement(parentTrigger.gameObject);
            makeItemPickedUpSound();
        }
        else if (parentTrigger.name.Equals("Document_02"))
        {
            collectionsPourGagner[1] = "Document_02";
            LevelManager.instance.cocherElement(parentTrigger.gameObject);
            makeItemPickedUpSound();
        }
        else if (parentTrigger.name.Equals("Key"))
        {
            collectionsPourGagner[2] = "Key";
            LevelManager.instance.cocherElement(parentTrigger.gameObject);
            makeItemPickedUpSound();
        }
        else if (parentTrigger.name.Equals("WinningDoors"))
        {
            LevelManager.instance.joueurAtteintZoneVictoire(collectionsPourGagner);
        }
        else if (other.gameObject.name.Equals("ZoneRamassageGun"))
        {
            if (parentTrigger.gameObject.CompareTag("SMG") && !toutesLesArmes.Contains("SMG"))
            {
                toutesLesArmes[1] = "SMG";
                Destroy(parentTrigger.gameObject);
                LevelManager.instance.SMGPickesUpAppear();
                makeItemPickedUpSound();
            }
            else if (parentTrigger.gameObject.CompareTag("AR") && !toutesLesArmes.Contains("AR"))
            {
                toutesLesArmes[2] = "AR";
                Destroy(parentTrigger.gameObject);
                LevelManager.instance.ARPickesUpAppear();
                makeItemPickedUpSound();
            }
        }
        else if (parentTrigger.gameObject.CompareTag("Munitions"))
        {
            if (_munitions != munitionsMaximales)
            {
                _munitions += 10;

                if (_munitions > munitionsMaximales)
                    _munitions = munitionsMaximales;

                Destroy(parentTrigger.gameObject);
            }
        }
    }

    // Entré dans la zone d'un ennemi
    private void OnTriggerStay(Collider other)
    {
        Transform parentTrigger = other.transform.parent;

        if (other.gameObject.name.Equals("ZoneDetectable"))
        {
            EnnemyLogic enemy = parentTrigger.GetComponent<EnnemyLogic>();
            if (enemy != null)
            {
                enemy.onPlayerDetected();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Transform parentTrigger = other.transform.parent;

        if (parentTrigger.name.Equals("WinningDoors"))
        {
            LevelManager.instance.sortieZoneVictoire();
        }
    }

    public float getNbMunitions()
    {
        return _munitions;
    }

    public float getNombreDePointsVie()
    {
        return pointDeVie;
    }

    public void touche(float nombreDegats)
    {
        pointDeVie -= nombreDegats;

        if (pointDeVie <= 0)
            isPlayerAlive = false;
    }

    public bool getIsPlayerAlive()
    {
        return isPlayerAlive;
    }

    // Sons  fait par le joueur
    public void makeItemPickedUpSound()
    {
        AudioClip clip = ramasseObjet;
        soundSource.clip = clip;
        soundSource.volume = SettingsManager.instance.GetSoundVolume();
        soundSource.Play();
    }

    public void makeChangeGunSound()
    {
        AudioClip clip = changeGun;
        soundSource.clip = clip;
        soundSource.volume = SettingsManager.instance.GetSoundVolume();
        soundSource.Play();
    }
}