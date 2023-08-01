using Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSound : MonoBehaviour
{
    [SerializeField] private SoundName foot_sound_name;


    private void Awake()
    {
        
    }
    public void PlayFootSound()
    {
        ObjectPoolManager.instance.GetSound(foot_sound_name);
    }
}
