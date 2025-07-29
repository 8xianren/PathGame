using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalRank : MonoBehaviour
{
    public  GameObject[] players;

    public GameObject[] playersName;

    List<List<HexMetrics.HexOwner>> cellOwners;

    public Sprite[] spritesIcon;

    public Sprite[] spriteName;

    public GameObject[] counts;

    int width = 16;

    int height = 16;

    List<int> scores = new List<int> { 0, 0, 0 ,0};
    // Start is called before the first frame update
    void Start()
    {
        cellOwners = FindObjectOfType<HexGrid>().cellOwners;

        
    }


    public void Update()
    {
        StartCoroutine(myfalsh());
    }

    IEnumerator myfalsh()
    {
        Calculate();
        
        yield return new WaitForSeconds(1f);
    }

    // Update is called once per frame
    void Calculate()
    {
        scores[0] = 0;
        scores[1] = 0;
        scores[2] = 0;
        scores[3] = 0;
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                if (cellOwners[i][j] == HexMetrics.HexOwner.Player)
                {
                    scores[0] += 1;
                }
                else if (cellOwners[i][j] == HexMetrics.HexOwner.AI0)
                {
                    scores[1] += 1;
                }
                else if (cellOwners[i][j] == HexMetrics.HexOwner.AI1)
                {
                    scores[2] += 1;
                }
                else if (cellOwners[i][j] == HexMetrics.HexOwner.AI2)

                {
                    scores[3] += 1;
                }
            }
        }

        int[] ranks = scores.GetUniqueRanks(descending: false);

        

        for (int i = 0; i < 4; ++i)
        {
            players[i].GetComponent<Image>().sprite = spritesIcon[ranks[i] - 1];

            playersName[i].GetComponent<Image>().sprite = spriteName[ranks[i] - 1];

            //counts[i].GetComponent<TextMeshProUGUI>().text = scores[ranks[i] - 1].ToString();
        }

        

        
        
    }
}
