using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLogic : MonoBehaviour
{
    private float nombreCollisions = 0f;

    [SerializeField]
    GameObject explosionPrefab;

    // Sounds
    private AudioSource soundSource;

    [SerializeField]
    AudioClip GrenadeExplosionSound;

    // Start is called before the first frame update
    void Start()
    {
        soundSource = GetComponentInChildren<AudioSource>();

        // Enlever lumi�re de rep�rage si le joueur a lanc� la grenade
        if (LevelManager.instance.getGrenadeTire())
        {
            Light lumiere = GetComponent<Light>();

            lumiere.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Explosion lors de n'importe quelle d�tection
    private void OnCollisionEnter(Collision collision)
    {
        nombreCollisions += 1;

        if (LevelManager.instance.getGrenadeTire() && nombreCollisions == 1)
        {
            SphereCollider zoneExplosion = GameObject.Find("ZoneExplosion").GetComponent<SphereCollider>();
            zoneExplosion.enabled = true;

            Collider[] colliders = Physics.OverlapSphere(zoneExplosion.transform.position, zoneExplosion.radius);

            // Rep�rage de tout ce qui est dans la zone d'explosion
            foreach (Collider col in colliders)
            {
                Transform parentObject = col.transform.parent;

                if (parentObject != null)
                {
                    if (parentObject.gameObject.transform.parent != null)
                    {
                        // D�g�ts aux ennemis
                        if (parentObject.gameObject.transform.parent.gameObject.name.Equals("Ennemis"))
                        {
                            EnnemyLogic ennemy = parentObject.gameObject.GetComponent<EnnemyLogic>();
                            ennemy.PrendreDegats(100f);
                        }
                    }
                }
            }

            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            makeGrenadeExplosionSound();

            // D�marrer la coroutine pour d�truire l'objet apr�s la fin du son
            StartCoroutine(DestroyAfterSound());
        }
    }

    // Coroutine pour attendre la fin du son avant de d�truire l'objet
    private IEnumerator DestroyAfterSound()
    {
        yield return new WaitForSeconds(GrenadeExplosionSound.length);
        Destroy(gameObject);
    }

    // Son d'explosion
    public void makeGrenadeExplosionSound()
    {
        AudioClip clip = GrenadeExplosionSound;
        soundSource.clip = clip;
        soundSource.volume = SettingsManager.instance.GetSoundVolume();
        soundSource.Play();
    }
}
