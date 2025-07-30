using System;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneManager : MonoBehaviour
{
    public static CutSceneManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    [Header("컷데이터 세팅")]
    public CutSceneData  currentCutSceneData;
    public CutSceneFuntions cutSceneFunctions; 
    public bool isTest;
    private float timer = 0f;
    private int currentCutIndex = 0;
    
    private Dictionary<string, GameObject> activeActors = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> activeBigGroupActors = new Dictionary<string, GameObject>();
    
    public void Start()
    {
        if (isTest == true)
        {
            InitStartCutScene();
        }
    }

    public void InitStartCutScene()
    {
        if (currentCutSceneData != null)
        {
            // 싱글 액터 관리
            foreach (var actorData in currentCutSceneData.actors)
            {
                GameObject actorInstance = Instantiate(actorData.actPrefab, actorData.position, Quaternion.identity);
                actorInstance.name = actorData.name; 
                activeActors[actorData.name] = actorInstance;
            }
            
            // 멀티 액터 관리
            foreach (var bigGroupActorData in currentCutSceneData.bigGroupActors)
            {
                GameObject groupParent = new GameObject(bigGroupActorData.name);
                groupParent.transform.position = bigGroupActorData.position; 
                activeBigGroupActors[bigGroupActorData.name] = groupParent;

                foreach (var actorInGroupData in bigGroupActorData.actors)
                {
                    Vector3 actorWorldPosition = actorInGroupData.position; 

                    GameObject actorInstance = Instantiate(actorInGroupData.actPrefab, actorWorldPosition, Quaternion.identity);
                    actorInstance.name = actorInGroupData.name;
                    actorInstance.transform.SetParent(groupParent.transform); // 부모 GameObject의 자식으로 설정
                    activeActors[actorInGroupData.name] = actorInstance; // 개별 Actor로도 접근 가능하도록 추가
                }
            }
        }
    }
    
    
    
}