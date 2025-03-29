using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource ShootingChannel;
    public AudioSource ReloadingChannel;
    
    // Glock18 sound effects
    public AudioClip Glock18Shot;
    public AudioClip Glock18Reload;
    public AudioSource dryFireSoundGlock18;

    // AK47 sound effects
    public AudioClip AK47Shot;
    public AudioClip AK47Reload;
    public AudioSource dryFireSoundAK47;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlayFiringSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Glock18:
                ShootingChannel.PlayOneShot(Glock18Shot); 
                break;
            case WeaponModel.AK47:
                ShootingChannel.PlayOneShot(AK47Shot);
                break;
        }
    }

    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Glock18:
                ReloadingChannel.PlayOneShot(Glock18Reload);
                break;
            case WeaponModel.AK47:
                ReloadingChannel.PlayOneShot(AK47Reload);
                break;
        }
    }

    public void PlayDryFireSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Glock18:
                dryFireSoundGlock18.Play();
                break;
            case WeaponModel.AK47:
                dryFireSoundAK47.Play();
                break;
        }
    }
}
