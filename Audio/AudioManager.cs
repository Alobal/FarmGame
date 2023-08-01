using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        public List<SceneAudioConfig> scene_configs;
        public AudioConfigListSO audio_configs;

        public AudioSource ambient_source;
        public AudioSource music_source;

        public AudioMixer audio_mixer;
        public AudioMixerSnapshot snapshot_normal;
        public AudioMixerSnapshot snapshot_amibent_only;
        public AudioMixerSnapshot snapshot_mute;

        static float fade_span = 5f;//音频渐入渐出时间 s
        // Start is called before the first frame update
        void Start()
        {
            SwitchSceneAudio("Field1");
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            TransitionManager.AfterSceneLoad += OnAfterSceneLoad;
        }


        private void OnDisable()
        {
            TransitionManager.AfterSceneLoad -= OnAfterSceneLoad;

        }

        private void OnAfterSceneLoad(object sender, AfterSceneLoadEventArgs args)
        {
            SwitchSceneAudio(args.target_scene);
        }

        private  void SwitchSceneAudio(string scene_name)
        {
            if (scene_configs.Find(x => x.scene_name == scene_name) is SceneAudioConfig scene_audio)
            {
                AudioConfig ambient= audio_configs.Get(scene_audio.ambient);
                AudioConfig music= audio_configs.Get(scene_audio.music);
                SourcePlay(ambient_source, ambient);
                snapshot_amibent_only.TransitionTo(0);
                StartCoroutine(UtilityMethods.WaitDoCR(() => snapshot_normal.TransitionTo(fade_span),fade_span));
                SourcePlay(music_source, music);
            }

        }
        
        private void SourcePlay(AudioSource source,AudioConfig config)
        {
            if (config == null)
            {
                source.Stop();
                return;
            }
            source.clip = config.sound_clip;
            source.pitch=config.GetRandomPitch();
            string mixer_param = source == music_source ? "MusicVolume" : "AmbientVolume";
            audio_mixer.SetFloat(mixer_param, LinearVolumeToDB(config.sound_volumn));
            //DOTween.To(() => source.volume, x => source.volume = x, audio.sound_volumn, fade_span);
            if(source.isActiveAndEnabled)
            {
                source.Play();
            }
        }

        private float LinearVolumeToDB(float linear_volume)
        {
            return linear_volume != 0 ? 20.0f * Mathf.Log10(linear_volume) : -60;
        }

        private float DBVolumeToLinear(float db_volume)
        {
            return Mathf.Pow(10.0f, db_volume / 20.0f);
        }
    }

    [Serializable]
    public class SceneAudioConfig
    {
        public string scene_name;
        //用于在AudioConfigListSO中查询引用
        public SoundName ambient;
        public SoundName music;
    }

}
