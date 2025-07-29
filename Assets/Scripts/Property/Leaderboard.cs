using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private List<PlayerInfo> players = new List<PlayerInfo>();

    private List<List<HexMetrics.HexOwner>> cellOwners;

    public int width = 16;

    public int height = 16;

    public void UpdateLeaderboard()
    {
        // 1. 按分数降序排序（分数高的在前）
        var sortedPlayers = players.OrderByDescending(p => p.score).ToList();

        // 2. 重新排列UI元素位置
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            // 修改UI元素的Sibling Index（控制渲染/布局顺序）
            sortedPlayers[i].playerImage.transform.SetSiblingIndex(i);

            // 可选：动态移动位置（如果需要动画效果）
            // StartCoroutine(MoveToPosition(sortedPlayers[i].playerImage, i));
        }
    }

    // 更新特定玩家的分数（调用此方法后需手动调用UpdateLeaderboard）
    public void UpdatePlayerScore(Image target, int newScore)
    {
        players.FirstOrDefault(p => p.playerImage == target).score = newScore;
    }

    public void Update()
    {
        StartCoroutine(myfalsh());
    }

    IEnumerator myfalsh()
    {
        Calculate();
        UpdateLeaderboard();
        yield return new WaitForSeconds(1f);
    }

    private void Calculate()
    {
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                if (cellOwners[i][j] == HexMetrics.HexOwner.Player)
                {
                    players[0].score += 1;
                }
                else if (cellOwners[i][j] == HexMetrics.HexOwner.AI0)
                {
                    players[1].score += 1;
                }
                else if (cellOwners[i][j] == HexMetrics.HexOwner.AI1)
                {
                    players[2].score += 1;
                }
                else
                {
                    players[3].score += 1;
                }
            }
        }
    }

    public void Start()
    {
        cellOwners = FindObjectOfType<HexGrid>().cellOwners;
    }
}

[System.Serializable]
public class PlayerInfo
{
    public Image playerImage; // 对应的UI图标
    public int score;         // 玩家分数
}
