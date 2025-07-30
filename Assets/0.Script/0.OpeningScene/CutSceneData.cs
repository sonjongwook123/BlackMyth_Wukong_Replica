using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "CutSceneData", menuName = "CutScene/New Data")]
public class CutSceneData : ScriptableObject
{
    public List<BigGroupActor> bigGroupActors;
    public List<Actor> actors;
    public float fullTime;
    public List<cutData> cutList;
}

[Serializable]
public class cutData
{
    public float cutTime;
    public float startTime;
    public float endTime;
    public List<subTitleData> subTitleList;
    public List<CutAudioClip> clips;
    public List<ActingData> actingList;
    public List<CameraData> cameraDataList;
    public List<MusicData> musicList;
}

[Serializable]
public class MusicData
{
    public float time;
    public Enums.Bgm bgm;
}

[Serializable]
public class CameraData
{
    public Vector3 position;
    public Vector3 rotation;
    public float time;
}

[Serializable]
public class VfxObject
{
    public GameObject prefab;
    public Vector3 position;
    public Vector3 rotation;
    public float time;
}

[Serializable]
public class subTitleData
{
    public float showTime;
    public string subTitle;
}

public enum ActingType
{
    Move,
    Explosion,
    Emotion,
    Attack
}

[Serializable]
public class ActingData
{
    public string actorName;
    public ActingType actingType;
}

[Serializable]
public class CutAudioClip
{
    public float playTime;
    public AudioClip clip;
}

[Serializable]
public enum DestroyType
{
    NoDestroy,
    WillDestroy
}

[Serializable]
public enum AudioType
{
    Voice,
    Emotion
}

[Serializable]
public enum AudioVoiceType
{
    Talk,
    Attack,
    Damaged,
}

[Serializable]
public enum AudioEmotionType
{
    Happy,
    Angry,
    Sad,
    Ambress
}

[Serializable]
public abstract class ActingAble
{
    public string name;
    public Vector3 position;
}

[Serializable]
public class Actor: ActingAble
{
    public GameObject actPrefab;
    public List<ActorAudioClip> audioClips;
}

[Serializable]
public class BigGroupActor: ActingAble
{
    public List<Actor> actors;
}

[Serializable]
public class ActorAudioClip
{
    public AudioType  audioType;
    public AudioVoiceType audioVoiceType;
    public AudioEmotionType audioEmotionType;
    public AudioClip audioClip;
}

[Serializable]
public class ActorAudioData
{
    public float time;
    public AudioType  audioType;
    public AudioVoiceType audioVoiceType;
    public AudioEmotionType audioEmotionType;
}