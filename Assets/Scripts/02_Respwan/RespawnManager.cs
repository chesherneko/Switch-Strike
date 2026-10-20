using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoSingleton<RespawnManager>
{
    private static bool needRespawn = false;
    private static int sceneBuildIndex = -1;
    private static int savePointIdx = 0;

    [SerializeField] private List<SavePoint> savePoints;
    
    private void Start()
    {
        int prevBuildIndex = sceneBuildIndex;
        int buildIndex = SceneManager.GetActiveScene().buildIndex;

        sceneBuildIndex = buildIndex;

        if (prevBuildIndex != buildIndex)
        {
            needRespawn = false;
            savePointIdx = 0;
            return;
        }

        if (needRespawn == false) return;

        needRespawn = false;

        SavePoint savePoint = savePoints[savePointIdx];
        Player.Local.transform.position = savePoint.SpawnPoint;
    }

    [Button]
    public void Respawn()
    {
        needRespawn = true;
        SceneManager.LoadSceneAsync(sceneBuildIndex);
    }
    
    public void SetCurrentSavePoint(SavePoint savePoint)
    {
        if (savePoints.Contains(savePoint) == false)
            return;

        int idx = savePoints.IndexOf(savePoint);

        savePointIdx = idx > savePointIdx ? 
            idx :
            savePointIdx;
    }
}
