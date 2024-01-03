using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public AudioClip[] deathSounds; 
    private AudioSource Source;
    public List<GameObject> Enemys = new List<GameObject>();
    public int currentSoundIndex = 11;
    public AudioClip LOOPSON;
    public void AddEnemy(GameObject enemy)
    {
        Source = GetComponent<AudioSource>();
        Enemys.Add(enemy);
    }
    public void EnemyDied(GameObject enemy)
    {
        if (Enemys.Contains(enemy))
        {
            Enemys.Remove(enemy);
            PlayNextDeathSound();
        }
    }
    private void PlayNextDeathSound()
    {
        if (deathSounds.Length > 0)
        {
            if (currentSoundIndex >= 0)
            {
                Source.clip = deathSounds[currentSoundIndex];
                Source.Play();
                currentSoundIndex--;
            }
            else
            {
                Source.clip = LOOPSON;
                Source.Play();
            }
        }
    }
}