using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Security.Cryptography;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] Objective objectivePrefab;
    [SerializeField] Transform objectiveParent;
    [SerializeField] List<Objective> objectives = new List<Objective>();
    [SerializeField] List<CupType> requiredCupTypes;
    [SerializeField] CupSO cupSO;
    GridSytem gridSytem;
    UIManager uiManager;
    [SerializeField] Animator animator;

    string filePath = "Assets/Levels/level_1.json";

    public static ObjectiveManager Ins;

    private void Awake()
    {
        if (!Ins)
        {
            Ins = this;
        }
        gridSytem = FindObjectOfType<GridSytem>();
        uiManager = FindObjectOfType<UIManager>();
        LoadLevelData(filePath);
    }

    private void Start()
    {
        SpawnObjective();
        animator.Play("new_objective_layout_anim");
    }

    void SpawnObjective()
    {
        var cupSprs = cupSO?.CupSpr;

        if (cupSprs == null) return;
        if (requiredCupTypes.Count == 0) return;

        for (int i = 0; i < 3; i++)
        {
            var cup = requiredCupTypes[i];
            if (cup == null) return;
            var objective = Instantiate(objectivePrefab);
            objective.transform.SetParent(objectiveParent);
            objective.Type = cup;
            objective.SetImg(cupSprs[(int)cup]);
            objectives.Add(objective);
        }

    }
    public void LoadLevelData(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);

            requiredCupTypes = levelData.requiredCupTypes;
        }
        else
        {
            Debug.LogWarning("Level file does not exist: " + filePath);
        }
    }

    public void RemoveRequiredCupType(CupType type)
    {
        var cupType = requiredCupTypes.Find(x => x == type);
        var obj = objectives.Find(x => x.Type == type);
        if (cupType != null)
        {
            obj.Done();
            requiredCupTypes.Remove(cupType);
        }

        if (requiredCupTypes.Count == 0)
        {
            DOVirtual.DelayedCall(0.5f, () =>
            {
                gridSytem.CompleteGame();
                uiManager.CompleteGame();
            });
        }
    }
}
