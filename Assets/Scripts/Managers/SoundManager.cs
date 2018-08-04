using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    /* 0枪声 1走路 2奔跑
     * 
     * 
     */

    public static SoundManager instance;

    public AudioSource effectSource;        //音效声源
    public AudioSource musicSource;          //bgm声源
    public AudioClip[] effects;         //预备音效
    public AudioClip[] musics;     //预备背景音
    public AudioClip effect;            //当前音效
    public AudioClip music;        //当前bgm

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    public void SetEffect(int index)
    {
        effect = effects[index];
    }
    public void SetBackGround(int index)
    {
        music = musics[index];
    }
    public void PlayEffect()
    {
        effectSource.clip = effect;
        effectSource.Play();
    }
    public void PlayMusic()
    {
        musicSource.clip = music;
        musicSource.Play();
    }
    public void StopEffect()
    {
        effectSource.Stop();
    }
    public void SpeedEffect(float pitch)
    {
        effectSource.pitch = pitch;
    }
}
