using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour
{

    private List<ItemType> collectedItems = new List<ItemType>();
    private Dictionary<ItemType, int> itemCounts = new Dictionary<ItemType, int>();

    private Material mat;

    public int maxSlots = 7;

    public Action<float, int> speedUpAction;

    public Action<Vector3, Material, HexMetrics.HexOwner> changeMat;

    public List<ItemData> itemDatabase = new List<ItemData>();

    public GameObject comboPlayerEat;

    public GameObject comboPlayerCrash;
    public GameObject comboPlayerCollect;
    public GameObject comboPlayerSpeed;

    void Awake()
    {
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            itemCounts[type] = 0;
        }
        comboPlayerEat = Resources.Load<GameObject>("JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Eerie/CFXR2 WW Enemy Explosion");

        comboPlayerCrash = Resources.Load<GameObject>("JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Electric/CFXR2 Sparks Rain");

        comboPlayerCollect = Resources.Load<GameObject>("JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Fire/CFXR Fire");

        comboPlayerSpeed = Resources.Load<GameObject>("JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Fire/CFXR Fire Breath");

    }


    // Start is called before the first frame update
    void Start()
    {
        mat = gameObject.GetComponent<AIController>().coverMaterial;



    }

    // Update is called once per frame



    public bool AddItem(ItemType itemType, Collider collector)
    {

        // 通过单例访问
        if (collectedItems.Count >= maxSlots)
        {
            //Debug.Log("背包已满！");


            return false;
        }

        collectedItems.Add(itemType);

        itemCounts[itemType]++;


        GameObject newCollected = Instantiate(comboPlayerCollect, collector.transform.position, Quaternion.identity);
        StartCoroutine(DestroyAfterDelay(newCollected, 2f));



        CheckForCombosCrash(itemType, collector.transform.position, collector.gameObject);

        CheckForCombosEat(itemType, collector.transform.position, collector.gameObject);

        return true;




    }

    private void CheckForCombosEat(ItemType itemType, Vector3 itemPosition, GameObject obj)
    {
        int group = (int)itemType / 3;
        int num = (int)itemType % 3;
        bool isTrigger = false;

        if (num == 0)
        {
            ItemType n1 = itemType + 1;
            ItemType n2 = itemType + 2;

            if (itemCounts.ContainsKey(n1) && itemCounts.ContainsKey(n2))
            {
                if (itemCounts[n1] > 0 && itemCounts[n2] > 0)
                {
                    itemCounts[n1] -= 1;
                    itemCounts[n2] -= 1;
                    itemCounts[itemType] -= 1;

                    RemoveThreeItems(itemType, n1, n2);
                    isTrigger = true;

                }






            }
        }
        else if (num == 1)
        {
            ItemType n0 = itemType - 1;
            ItemType n2 = itemType + 1;

            if (itemCounts.ContainsKey(n0) && itemCounts.ContainsKey(n2))
            {
                if (itemCounts[n0] > 0 && itemCounts[n2] > 0)
                {
                    itemCounts[n0] -= 1;
                    itemCounts[n2] -= 1;
                    itemCounts[itemType] -= 1;

                    RemoveThreeItems(itemType, n0, n2);
                    isTrigger = true;

                }
            }


        }
        else
        {
            ItemType n0 = itemType - 2;
            ItemType n1 = itemType - 1;

            if (itemCounts.ContainsKey(n0) && itemCounts.ContainsKey(n1))
            {
                if (itemCounts[n0] > 0 && itemCounts[n1] > 0)
                {
                    itemCounts[n0] -= 1;
                    itemCounts[n1] -= 1;
                    itemCounts[itemType] -= 1;

                    RemoveThreeItems(itemType, n0, n1);
                    isTrigger = true;

                }
            }

        }
        if (!isTrigger)
        {
            return;

        }

        GameObject newEatEffect = Instantiate(comboPlayerEat, itemPosition, Quaternion.identity);
        newEatEffect.transform.localScale *= 10f;
        StartCoroutine(DestroyAfterDelay(newEatEffect, 3f));







        Bomb();



    }


    private void Bomb()
    {
        GameObject missilePrefab = Resources.Load<GameObject>("Prefabs/Bullet/Sphere 1");
        GameObject missile = Instantiate(
            missilePrefab,
            gameObject.transform.position + 10 * Vector3.up,
            Quaternion.identity
        );



        missile.transform.localScale *= 5f;

        List<GameObject> target = new List<GameObject>();
        target.Add(PlayerManager.Instance.PlayerInstance);
        foreach (var ai in AIManager.Instance.AIInstances)
        {
            if (ai != gameObject)
            {
                target.Add(ai);


            }

        }


        int a = UnityEngine.Random.Range(0, 100);
        GameObject obj;
        if (a < 33)
        {
            obj = target[0];

        }
        else if (a < 67)
        {
            obj = target[1];
        }
        else
        {
            obj = target[2];
        }
        missile.GetComponent<HomingMissile>().target = obj;





    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }


    void Update()
    {
        if (collectedItems.Count == maxSlots)
        {
            //Debug.Log("背包已满，无法添加更多道具！");
            speedUpAction?.Invoke(1.75f, 5);
            collectedItems.Clear(); // 清空背包


        }
    }

    // 检查组合效果
    private void CheckForCombosCrash(ItemType itemType, Vector3 itemPosition, GameObject obj)
    {
        if (itemCounts[itemType] >= 3)
        {

            // 触发效果
            TriggerComboEffectCrash(obj, itemPosition);


            // 移除三个相同的道具
            RemoveItemsOfType(itemType, 3);
        }
    }


    private void RemoveThreeItems(ItemType itemType0, ItemType itemType1, ItemType itemType2)
    {
        for (int i = collectedItems.Count - 1; i >= 0; i--)
        {
            if (collectedItems[i] == itemType0)
            {
                collectedItems.RemoveAt(i);


                break;
            }
        }
        for (int i = collectedItems.Count - 1; i >= 0; i--)
        {
            if (collectedItems[i] == itemType1)
            {
                collectedItems.RemoveAt(i);


                break;
            }
        }
        for (int i = collectedItems.Count - 1; i >= 0; i--)
        {
            if (collectedItems[i] == itemType2)
            {
                collectedItems.RemoveAt(i);


                break;
            }
        }

    }

    // 移除指定类型的道具
    private void RemoveItemsOfType(ItemType itemType, int count)
    {
        int removed = 0;

        // 从后往前移除
        for (int i = collectedItems.Count - 1; i >= 0; i--)
        {
            if (collectedItems[i] == itemType)
            {
                collectedItems.RemoveAt(i);
                removed++;

                if (removed >= count) break;
            }
        }

        itemCounts[itemType] -= count;



    }

    // 触发组合效果
    private void TriggerComboEffectCrash(GameObject obj, Vector3 itemPosition)
    {
        //Debug.Log($"触发组合效果: {itemType} x3!");


        if (comboPlayerCrash)
        {
            GameObject newCrash = Instantiate(comboPlayerCrash, itemPosition, Quaternion.identity);
            newCrash.transform.localScale *= 40;
            StartCoroutine(DestroyAfterDelay(newCrash, 3f));



            //Debug.Log("crash");

            HexMetrics.HexOwner temp;
            if (gameObject.CompareTag("AI0"))
            {
                temp = HexMetrics.HexOwner.AI0;
            }
            else if (gameObject.CompareTag("AI1"))
            {
                temp = HexMetrics.HexOwner.AI1;

            }
            else
            {
                temp = HexMetrics.HexOwner.AI2;
            }
            changeMat?.Invoke(itemPosition, mat, temp);





        }


        // 播放特效


        // 根据道具类型执行不同效果


    }



    // 更新背包UI


    public int GetItemCount(ItemType type)
    {
        itemCounts.TryGetValue(type, out int count);
        return count;

    }


}
