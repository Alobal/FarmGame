using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class Sound : MonoBehaviour
    {

        [SerializeField] private AudioSource source;
        // Start is called before the first frame update
        void Start()
        {

        }

        public void Init(AudioConfig config)
        {
            source.clip = config.sound_clip;
            source.volume = config.sound_volumn;
            source.pitch=config.GetRandomPitch();
        }

        public void  Play()
        {
            source.Play();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}