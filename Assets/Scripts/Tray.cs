using DG.Tweening;
using DG.Tweening.Core.Enums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class Tray : MonoBehaviour
{
    Vector3 offset;
    bool isDragging = false;
    bool isClick = true;
    GridSytem gridSystem;
    GridItem gridItem;
    Vector3 originalPos;
    float zOffset = 1.5f;
    Vector3 defaultPos;
    Node lastNode;
    Vector3 lastPosition;
    float angle = 20f;
    [SerializeField] List<Cup> listCup = new List<Cup>();
    List<Vector3> listPositionCup;
    [SerializeField] Transform cupGroup;
    [SerializeField] Cup cupPrefab;
    [SerializeField] BoxCloser boxCloserPrefab;

    public List<Cup> ListCup { get => listCup; set => listCup = value; }
    public List<Vector3> ListPositionCup { get => listPositionCup; set => listPositionCup = value; }

    private void Start()
    {
        gridSystem = FindObjectOfType<GridSytem>();
        SetUpPositionCup();
        SpawnCups();
    }

    void SetUpPositionCup()
    {
        ListPositionCup = new List<Vector3>();
        ListPositionCup.Add(new Vector3(-0.66f, 0f, 0.3f));
        ListPositionCup.Add(new Vector3(0f, 0f, 0.3f));
        ListPositionCup.Add(new Vector3(0.66f, 0f, 0.3f));
        ListPositionCup.Add(new Vector3(-0.66f, 0f, -0.39f));
        ListPositionCup.Add(new Vector3(0f, 0f, -0.39f));
        ListPositionCup.Add(new Vector3(0.66f, 0f, -0.39f));
    }

    private void OnMouseDown()
    {
        if (!isClick) return;

        originalPos = transform.position;
        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;

        Vector3 newPosition = transform.position;
        newPosition.z += zOffset;
        transform.position = newPosition;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 newPosition = GetMouseWorldPosition() + offset;
            newPosition.y = originalPos.y;
            newPosition.z += zOffset;
            transform.position = newPosition;

            Node nearestNode = gridSystem.FindNearestNode(transform.position);
            TiltTrayOnDirection(newPosition);
            HandleNodeFade(nearestNode);
        }
    }

    private void OnMouseUp()
    {
        if (!isClick) return;

        isDragging = false;
        HandleNodeCollision();
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.y = 0f;
        return worldPosition;
    }

    void TiltTrayOnDirection(Vector3 currentPosition)
    {
        // Tính toán sự thay đổi vị trí
        Vector3 dir = currentPosition - lastPosition;

        // Khai báo biến để lưu góc quay mục tiêu
        Vector3 targetRotation = Vector3.zero;

        // Xác định sự thay đổi lớn nhất để nghiêng theo hướng chính
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
        {
            // Nếu thay đổi theo trục x lớn hơn
            if (dir.x < 0) // left
            {
                targetRotation.z = -angle;
            }
            else if (dir.x > 0) // right
            {
                targetRotation.z = angle;
            }
        }
        else
        {
            // Nếu thay đổi theo trục z lớn hơn hoặc bằng
            if (dir.z < 0) // down
            {
                targetRotation.x = angle;
            }
            else if (dir.z > 0) // top
            {
                targetRotation.x = -angle;
            }
        }

        transform.DOKill();

        transform.DORotate(targetRotation, 0.2f);

        lastPosition = currentPosition;
    }

    void HandleNodeCollision()
    {
        Node nearestNode = gridSystem.FindNearestNode(transform.position);

        if (nearestNode == null)
        {
            transform.DOKill();
            transform.localPosition = defaultPos;
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            return;
        }
        if (!nearestNode.GridItem.Tray && nearestNode.GridItem.GridItemType != GridItemType.Box)
        {
            nearestNode.GridItem.Tray = this;
            nearestNode.Tray = this;
            gridItem = nearestNode.GridItem;
            transform.SetParent(nearestNode.GridItem.transform);
            transform.localPosition = Vector3.zero;
            transform.DOKill();
            nearestNode.GridItem.FadeOut();
            transform.DORotate(Vector3.zero, 0f);
            isClick = false;
            CheckDirectionTray();
            gridSystem.RemoveTray(this);
            DOTween.Sequence()
            .AppendInterval(0.4f)
            .AppendCallback(DeleteTray);
        }
        else
        {
            transform.DOKill();
            transform.localPosition = defaultPos;
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
    public void SetDefaultPos(Vector3 position)
    {
        defaultPos = position;
    }
    void HandleNodeFade(Node nearestNode)
    {

        if (nearestNode != null)
        {
            if (lastNode != nearestNode)
            {
                if (lastNode != null)
                {
                    lastNode.GridItem.FadeOut();
                }
                if (nearestNode.GridItem.Tray != null) return;
                if (nearestNode.GridItem.GridItemType == GridItemType.Box) return;
                nearestNode.GridItem.FadeIn();
                lastNode = nearestNode;
            }
        }
        else
        {
            if (lastNode != null)
            {
                lastNode.GridItem.FadeOut();
                lastNode = null;
            }
            else
            {
                //Debug.Log("null");
            }
        }
    }

    public void CheckDirectionTray()
    {
        Dictionary<CupType, Tray> cupTypeTrayMap = new Dictionary<CupType, Tray>();
        HashSet<Tray> processedTrays = new HashSet<Tray>();
        for (int i = 0; i < gridItem.Neighbors.Count; i++)
        {
            Tray neighborTray = gridItem?.Neighbors[i]?.Tray;
            if (neighborTray != null)
            {
                CupType cupType = GetCupTypeCountMax(neighborTray.ListCup);
                Tray trayMaxCupType = FindTrayWithMaxCups(cupType, processedTrays);
                if (!cupTypeTrayMap.ContainsKey(cupType))
                {
                    cupTypeTrayMap[cupType] = trayMaxCupType;
                    processedTrays.Add(trayMaxCupType);
                }
            }
        }

        foreach (var entry in cupTypeTrayMap)
        {
            TransferCupsFromNeighbors(entry.Key, entry.Value);
            TransferCupsFromCurrentTray(entry.Key, entry.Value);
        }

        CupType currentCupType = GetCupTypeCountMax(ListCup);
        Tray currentTrayMaxCupType = FindTrayWithMaxCups(currentCupType, processedTrays);
        if (!cupTypeTrayMap.ContainsKey(currentCupType))
        {
            cupTypeTrayMap[currentCupType] = currentTrayMaxCupType;
            processedTrays.Add(currentTrayMaxCupType);
        }


        foreach (var entry in cupTypeTrayMap)
        {
            TransferCupsFromNeighbors(entry.Key, entry.Value);
            TransferCupsFromCurrentTray(entry.Key, entry.Value);
        }

        foreach (var entry in cupTypeTrayMap)
        {
            MoveCupsToTray(entry.Key, entry.Value);
        }


    }

    void DeleteTray()
    {
        if (listCup.Count == 0)
        {
            gridItem.Node.Tray = null;
            gridItem.Tray = null;
            var boxCollider = GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }
            transform.DOScale(0.01f, 0.6f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
        for (int i = 0; i < 4; i++)
        {
            var tray = gridItem?.Neighbors[i]?.Tray;
            var cups = gridItem?.Neighbors[i]?.Tray?.ListCup;
            if (tray == null) continue;
            if (cups == null) continue;
            if (cups.Count == 0)
            {
                var neighborBoxCollider = tray.GetComponent<BoxCollider>();
                if (neighborBoxCollider != null)
                {
                    neighborBoxCollider.enabled = false;
                }
                tray.gridItem.Node.Tray = null;
                tray.gridItem.Tray = null;
                tray.transform.DOScale(0.01f, 0.6f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    tray.gameObject.SetActive(false);
                });
            }
        }
    }
    // Tìm Tray nào có Cup theo Type nhiều nhất
    public Tray FindTrayWithMaxCups(CupType cupType, HashSet<Tray> HasTray)
    {
        Tray maxTray = null;
        int maxCount = 0;

        for (int i = 0; i < gridItem.Neighbors.Count; i++)
        {
            Tray neighborsTray = gridItem?.Neighbors[i]?.Tray;
            if (neighborsTray && !HasTray.Contains(neighborsTray))
            {
                int count = GetCupCountOfType(neighborsTray.ListCup, cupType);
                if (count > maxCount)
                {
                    count = maxCount;
                    maxTray = neighborsTray;
                }
            }
        }

        if (!maxTray)
        {
            maxTray = this;
        }

        return maxTray;
    }

    int GetCupCountOfType(List<Cup> listCup, CupType cupType)
    {
        int count = 0;
        for (int i = 0; i < listCup.Count; i++)
        {
            if (listCup[i].Type == cupType)
            {
                count++;
            }
        }
        return count;
    }

    public CupType GetCupTypeCountMax(List<Cup> listCup)
    {
        Dictionary<CupType, int> CupTypeCounts = new Dictionary<CupType, int>();
        CupType cupType = CupType.None;
        int maxCount = 0;
        for (int i = 0; i < listCup.Count; i++)
        {
            var cup = listCup[i];
            if (CupTypeCounts.ContainsKey(cup.Type))
            {
                CupTypeCounts[cup.Type]++;
            }
            else
            {
                CupTypeCounts[cup.Type] = 1;
            }
        }

        foreach (var cupTypeCount in CupTypeCounts)
        {
            if (cupTypeCount.Value > maxCount)
            {
                maxCount = cupTypeCount.Value;
                cupType = cupTypeCount.Key;
            }
        }
        return cupType;
    }

    void TransferCupsFromNeighbors(CupType cupType, Tray targetTray)
    {
        if (ListCup.Any(x => x.Type == cupType))
        {
            for (int i = 0; i < gridItem.Neighbors.Count; i++)
            {
                Tray neighborTray = gridItem?.Neighbors[i]?.Tray;
                if (targetTray != neighborTray && neighborTray)
                {
                    List<Cup> cupMove = neighborTray.ListCup.FindAll(x => x.Type == cupType);
                    
                    int countType = targetTray.ListCup.FindAll(x => x.Type == cupType).Count;
                    int cupsNeed = 6 - countType;

                    // Lấy đủ số cup còn thiếu
                    var cupsToTransfer = cupMove.Take(cupsNeed).ToList();

                    foreach (var cup in cupsToTransfer)
                    {
                        targetTray.ListCup.Add(cup);
                        neighborTray.ListCup.Remove(cup);
                    }

                    
                }
            }

        }
    }

    void TransferCupsFromCurrentTray(CupType cupType, Tray targetTray)
    {
        List<Cup> currentTrayCups = ListCup.FindAll(x => x.Type == cupType);
        if (ListCup != targetTray.ListCup)
        {
            int countType = targetTray.ListCup.FindAll(x => x.Type == cupType).Count;
            int cupsNeed = 6 - countType;
            List<Cup> differentCup = targetTray.ListCup.FindAll(x => x.Type != cupType);
            var cupsToTransfer = currentTrayCups.Take(cupsNeed).ToList();

            foreach (var cup in cupsToTransfer)
            {
                targetTray.ListCup.Add(cup);
                ListCup.Remove(cup);
            }
            if (targetTray.ListCup.FindAll(x => x.Type == cupType).Count >= 6)
            {
                foreach (var cup in differentCup)
                {
                    ListCup.Add(cup);
                    targetTray.ListCup.Remove(cup);
                }
            }
        }
    }

    public void MoveCupsToTray(CupType cupType, Tray targetTray)
    {
        if (!targetTray) return;

        float delayTime = 0f;
        for (int i = 0; i < targetTray.ListCup.Count; i++)
        {
            var cup = targetTray.ListCup[i];
            var pos = targetTray.ListPositionCup[i];
            cup.transform.SetParent(targetTray.cupGroup);
            cup.transform.DOKill();
            cup.transform.DOLocalMove(pos, 0.2f).SetDelay(delayTime);
            cup.transform.DORotate(Vector3.zero, 0.2f);
            delayTime += 0.05f;
        }

        DOTween.Sequence()
            .AppendInterval(0.8f)
            .AppendCallback(() =>
            {
            if (CheckCorrectCups(targetTray, cupType))
            {
                    for (int i = 0; i < targetTray.gridItem.Neighbors.Count; ++i)
                    {
                        var grid = targetTray?.gridItem?.Neighbors[i];

                        if (grid == null || grid.Box == null) continue;

                        if (grid.GridItemType == GridItemType.Box)
                        {
                            grid.GridItemType = GridItemType.Empty;
                            grid.Box.transform.DOScale(1.2f, 0.4f).SetEase(Ease.InBack);
                            grid.PlayParticleBox();
                        }
                    }

                    targetTray.gridItem.PlayParticle();
                    targetTray.GetComponent<BoxCollider>().enabled = false;
                    targetTray.gridItem.Tray = null;
                    targetTray.gridItem.Node.Tray = null;
                    DOTween.Sequence()
                        .AppendCallback(() =>
                        {
                            float delayTime = 0f;
                            for (int i = 0; i < targetTray.ListCup.Count; i++)
                            {
                                var cup = targetTray.ListCup[i];
                                DOVirtual.DelayedCall(delayTime, () =>
                                {
                                    cup.gameObject.SetActive(true);
                                    cup.LidAnimation();
                                });
                                delayTime += 0.12f;
                            }
                        })
                        .AppendInterval(0.55f)
                        .Append(targetTray.transform.DOLocalMoveY(2f, 0.4f))
                        .AppendCallback(() =>
                        {
                            var box = Instantiate(boxCloserPrefab);
                            box.transform.SetParent(targetTray.transform);
                            box.transform.DOLocalMove(Vector3.zero, 0f);
                            box.gameObject.SetActive(true);
                            box.Play();
                            box.transform.DOLocalMoveY(0.7f, 0.7f).OnComplete(() =>
                            {
                                targetTray.transform.DOMoveX(15f, 1.7f).SetEase(Ease.OutBack);
                            });
                            ObjectiveManager.Ins.RemoveRequiredCupType(cupType);

                        })
                    ;
                }
            })
            ;
    }

    public bool CheckCorrectCups(Tray tray, CupType cupType)
    {
        int countCup = tray.ListCup.FindAll(x => x.Type == cupType).Count;

        if (countCup >= 6)
        {
            return true;
        }

        return false;
    }

    void SpawnCups()
    {
        var randomCups = Random.Range(1, 3);
        var cupTypes = gridSystem.GetRequiredCupTypes();
        for (int i = 0; i < randomCups; i++)
        {
            var cup = Instantiate(cupPrefab, cupGroup);
            var pos = listPositionCup[i];
            cup.transform.DOLocalMove(pos, 0.3f);
            cup.Type = cupTypes[Random.Range(0, cupTypes.Count)];
            cup.ActiveCupType();
            listCup.Add(cup);
        }
    }
}
