using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{

    [CreateAssetMenu(fileName = "AudioConfigListSO", menuName = "Audio/AudioConfigListSO")]
    public class AudioConfigListSO : ScriptableObject
    {
        public List<AudioConfig> data;


        public AudioConfig Get(SoundName name)
        {
            return data.Find(x => x.sound_name == name);
        }
    }

    [Serializable]
    public class AudioConfig
    {
        public SoundName sound_name;
        public AudioClip sound_clip;

        public Vector2 sound_pitch_range;
        [Range(0.1f,1f)]
        public float sound_volumn ;

        public float GetRandomPitch()
        {
            return UnityEngine.Random.Range(sound_pitch_range.x, sound_pitch_range.y);
        }
    }
}
