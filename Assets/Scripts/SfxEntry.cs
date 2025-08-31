using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SfxDef {
    public string key;                // "ui_click", "card_flip" ...
    public AudioClip[] clips;         // 여러 개면 랜덤
    [Range(0f,1f)] public float volume = 1f;
    public Vector2 pitch = new Vector2(1f, 1f); // min,max
}

[RequireComponent(typeof(AudioSource))]
public class SfxEntry : MonoBehaviour
{
    public static SfxEntry I;                   // 선택: 편의용
    public List<SfxDef> list = new();

    [Range(0f,1f)] public float masterVolume = 1f; // 씬 공통 SFX 볼륨
    AudioSource src;
    Dictionary<string,SfxDef> map;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        src.playOnAwake = false; src.loop = false; src.spatialBlend = 0f;
        masterVolume = PlayerPrefs.GetFloat("SfxVol", masterVolume);
        src.volume = masterVolume;

        map = new Dictionary<string, SfxDef>();
        foreach (var d in list) if (!map.ContainsKey(d.key)) map.Add(d.key, d);

        if (I == null) I = this;     // 간단히 전역 접근하고 싶을 때만 사용
    }

    public void SetVolume01(float v){
        masterVolume = Mathf.Clamp01(v);
        src.volume = masterVolume;
        PlayerPrefs.SetFloat("SfxVol", masterVolume);
    }

    public void PlayKey(string key){
        if (!map.TryGetValue(key, out var d) || d.clips == null || d.clips.Length==0) { Debug.LogWarning($"SFX key not found: {key}"); return; }
        var clip = d.clips[Random.Range(0, d.clips.Length)];
        float oldPitch = src.pitch;
        float p = (d.pitch.x==d.pitch.y)? d.pitch.x : Random.Range(d.pitch.x, d.pitch.y);
        src.pitch = p;
        // AudioSource.volume(=masterVolume) × volumeScale(=d.volume)
        src.PlayOneShot(clip, d.volume);
        src.pitch = oldPitch;
    }

    // 인스펙터 OnClick에서 문자열 못 넘길 때 쓰는 프록시
    public void PlayKey_Inspector(string k){ PlayKey(k); }

}
