using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CreateLand : MonoBehaviour
{
    public PlayerBehavior playerBehavior;
    public TextMeshProUGUI goldtext;
    public GameObject landParentPrefab;
    public List<GameObject> neighborLands;
    public List<GameObject> neighborWalls;
    int buyNumber;
    bool isBuy;
    // Start is called before the first frame update
    void Start()
    {
        buyNumber = 10;
        transform.GetChild(0).GetComponent<TextMeshPro>().text = buyNumber.ToString();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (buyNumber!=0)
            {
                isBuy = true;
                StartCoroutine(BuyLand());
            }
            else
            {
                isBuy = false;
            }
        }

      
    }

    IEnumerator BuyLand()
    {
        if (isBuy)
        {
            if (playerBehavior.goldNumber >= buyNumber)
            {
                buyNumber--;
                transform.GetChild(0).GetComponent<TextMeshPro>().text = buyNumber.ToString();
                playerBehavior.goldNumber--;
                goldtext.text = playerBehavior.goldNumber.ToString();
                if (buyNumber == 0)
                {
                    landParentPrefab.transform.DOScale(new Vector3(2, 1, 3), 0.5f).OnComplete(()=> {
                        if (neighborLands.Count>=1)
                        {
                            for (int i = 0; i < neighborLands.Count; i++)
                            {
                                neighborLands[i].SetActive(true);
                            }                   
                        }
                        foreach (var item in neighborWalls)
                        {
                            item.SetActive(false);
                        }
                     
                    });                             
                    transform.GetChild(0).GetComponent<TextMeshPro>().text = "";
                   
                    isBuy = false;
                }
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(BuyLand());
            }
        }   
    
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isBuy = false;
        }
    }
}
