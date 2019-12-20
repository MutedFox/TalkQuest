using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //Stores the sounds to be added though unity inspector
    public List<AudioClip> sound_clips_;

    //Stores sounds in easily accessible way (via their name)
    private Dictionary<string, AudioClip> sound_dict_;

    //The component to play the sounds from
    private AudioSource audio_source_;

    void Awake()
    {
        audio_source_ = GetComponent<AudioSource>();
        sound_dict_ = new Dictionary<string, AudioClip>();

        //populate sound_dict
        foreach(AudioClip sound_clip in sound_clips_)
        {
            sound_dict_.Add(sound_clip.name, sound_clip);
        }
    }

    //Plays the specified sound via sound_dict_; to be called by objects that want to make noise
    public void PlaySound(string name)
    {
        if(sound_dict_.ContainsKey(name))
            audio_source_.PlayOneShot(sound_dict_[name]);
    }
}
