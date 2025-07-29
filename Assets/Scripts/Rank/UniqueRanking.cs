using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using System.Linq;

public static class UniqueRanking
{
    /// <summary>
    /// 获取列表中每个元素的唯一排名
    /// </summary>
    /// <param name="list">输入列表</param>
    /// <param name="descending">true为降序（默认，数值大者胜），false为升序</param>
    /// <typeparam name="T">可比较类型</typeparam>
    /// <returns>原始数组下标对应的排名（从1开始）</returns>
    public static int[] GetUniqueRanks<T>(this List<T> list, bool descending = true) 
        where T : IComparable<T>
    {
        if (list == null)
            return Array.Empty<int>();
        
        if (list.Count == 0)
            return Array.Empty<int>();

        // 创建带原始索引的元素集合
        var items = list.Select((value, index) => new { Value = value, Index = index })
                        .ToList();

        // 排序处理
        var sortedItems = descending 
            ? items.OrderByDescending(x => x.Value)
                  .ThenBy(x => x.Index)  // 值相同时，原始索引小的排名靠前
                  .ToList()
            : items.OrderBy(x => x.Value)
                  .ThenBy(x => x.Index)  // 值相同时，原始索引小的排名靠前
                  .ToList();

        // 分配唯一排名
        int[] ranks = new int[list.Count];
        for (int rank = 0; rank < sortedItems.Count; rank++)
        {
            ranks[sortedItems[rank].Index] = rank + 1;  // 排名从1开始
        }

        return ranks;
    }
}