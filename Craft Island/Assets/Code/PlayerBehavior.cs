using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class PlayerBehavior : MonoBehaviour
{
    
    Animator anim;
    public List<Collider> currentTreeParent = new List<Collider>();
    List<GameObject> erasedObjects = new List<GameObject>();
    int index;
    bool chop;
    int logNumber;
   
    public int goldNumber;
    int firstBuildNumber;
    int firstBuildNumber2;
    public int producedWoodNumber;
    bool isBuild;
    bool isBuild2;
    bool isProduce;
    
  
    
    public Transform stackPoint;
    public Transform shop;
    public GameObject logPrefab;
    public GameObject treePrefab;
    public GameObject goldPrefab;
    
  
    public  Mesh buildMesh;
    public  TextMeshProUGUI logText;
    
    public  TextMeshProUGUI goldText;
    public  TextMeshPro firstBuildtext;
    public  TextMeshPro firstBuildtext2;
    public  TextMeshPro produceWoodtext;
    

   

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        firstBuildNumber = 10;
        firstBuildtext.text = firstBuildNumber.ToString();
        firstBuildNumber2 = 10;
        firstBuildtext2.text = firstBuildNumber2.ToString();
        goldNumber = 50;
        goldText.text = goldNumber.ToString();
        

    }

    // Update is called once per frame
    void Update()
    {
        if (chop)
        {
            float distance;
            float minDistance = 10000;
            Collider nearestTree = null;
            foreach (var item in currentTreeParent)
            {
                distance = Vector3.Distance(item.transform.position, transform.position);
                if (distance < minDistance)
                {
                    nearestTree = item;
                    minDistance = distance;
                }
                transform.LookAt(item.transform);
            }
        }
       
        
    }

    
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("TreeParent"))
        {
            currentTreeParent.Add(other);
            chop = true;
            anim.SetBool("Chop", true);
        }
        if (other.CompareTag("Log")) // Collect Log
        {
            other.GetComponent<Rigidbody>().isKinematic = true;
            other.transform.DOMove(stackPoint.position, 0.3f).OnComplete(() => SetLogPos(other));
            other.GetComponent<SphereCollider>().enabled = false;
            other.GetComponent<CapsuleCollider>().enabled = false;
            logNumber++;
            logText.text = logNumber.ToString();
        }
        if (other.CompareTag("TradeWood"))
        {
            if (!isBuild && stackPoint.childCount>10)
            {
                StartCoroutine(BuildGoldShop(other));
            }            
        }
        if (other.CompareTag("BaseWoodBuilding"))
        {
            if (!isBuild && stackPoint.childCount > 10)
            {
                StartCoroutine(BuildWood(other));
            }
        }
        if (other.CompareTag("GetGold"))
        {           
            StartCoroutine(GetGold());
        }
        if (other.CompareTag("ProduceWood"))
        {
            if (producedWoodNumber >= 1)
            {
               CollectWoodFromBuilding(other);
            }
        }
        
    }

    IEnumerator ProduceWood()
    {
        
        if (isProduce && producedWoodNumber<=99)
        {
           producedWoodNumber++;
            produceWoodtext.text = producedWoodNumber.ToString();
        }
        yield return new WaitForSeconds(1);
        StartCoroutine(ProduceWood());
    }

    void CollectWoodFromBuilding(Collider woodBuilding)
    {
       
        for (int i = 0; i < producedWoodNumber; i++)
        {

            GameObject wood = Instantiate(logPrefab, woodBuilding.transform.position + new Vector3(0, 4, -3), Quaternion.identity);
            wood.transform.parent = stackPoint;
            wood.transform.DOLocalJump(Vector3.zero, 2, 1, 0.5f);
            
        }
        producedWoodNumber = 0;

    }
    IEnumerator SpawnGold()
    {
        
        for (int i = 0; i < 5; i++)
        {
            
            GameObject gold = Instantiate(goldPrefab, shop.transform.position + new Vector3(0, 4, -3), Quaternion.identity);
            gold.transform.parent = transform;
            gold.transform.DOLocalJump(Vector3.zero, 2, 1, 0.5f).OnComplete(()=> Destroy(gold.gameObject));
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator GetGold()
    {
       
        if (stackPoint.childCount>=10)
        {
            logNumber-=10;
            logText.text = logNumber.ToString();
            goldNumber += 5;
            goldText.text = goldNumber.ToString();
            for (int i = 0; i < 10; i++)
            {
                Destroy(stackPoint.GetChild(stackPoint.childCount-1-i).gameObject);
            }
            StartCoroutine(SpawnGold());
        }
        yield return new WaitForSeconds(1f);
        if (stackPoint.childCount >= 10)
            StartCoroutine(GetGold());
    }
    IEnumerator BuildWood(Collider other)
    {
        
        if (!isBuild2)
        {
            if (stackPoint.childCount > 0 && firstBuildNumber2 > 0)
            {
                firstBuildNumber2--;
                logNumber--;
                firstBuildtext2.text = firstBuildNumber2.ToString();
                logText.text = logNumber.ToString();
                Destroy(stackPoint.GetChild(stackPoint.childCount - 1).gameObject);
            }
            if (firstBuildNumber2 < 1)
            {
                other.GetComponent<MeshFilter>().sharedMesh = buildMesh;
                other.gameObject.tag = "ProduceWood";
                other.transform.GetChild(0).gameObject.SetActive(false);
                other.transform.GetChild(2).gameObject.SetActive(true);
                other.transform.GetChild(3).gameObject.SetActive(true);
                other.transform.GetChild(3).GetComponent<TextMeshPro>().text = "0 / 100";
                index = stackPoint.childCount;
                isBuild2 = true;
                isProduce = true;
                StartCoroutine(ProduceWood());

            }
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(BuildWood(other));
        }
      

       
    }
    IEnumerator BuildGoldShop(Collider other)
    {
        
        if (!isBuild)
        {
            if (stackPoint.childCount > 0 && firstBuildNumber > 0)
            {
                firstBuildNumber--;
                logNumber--;
                firstBuildtext.text = firstBuildNumber.ToString();
                logText.text = logNumber.ToString();
                Destroy(stackPoint.GetChild(stackPoint.childCount - 1).gameObject);
            }
            if (firstBuildNumber <= 1)
            {
                other.GetComponent<MeshFilter>().sharedMesh = buildMesh;
                other.gameObject.tag = "GetGold";
                other.transform.GetChild(0).gameObject.SetActive(false);
                index = stackPoint.childCount;
                isBuild = true;
            }
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(BuildGoldShop(other));
        }
       
      
    }

    private void SetLogPos(Collider other)
    {
        
        float Ypos = Mathf.Floor(index / 3) * 0.2f;
        float Xpos = (index % 3) * 0.3f - 0.3f;
        other.transform.parent = stackPoint;
        other.transform.localPosition = new Vector3(Xpos, Ypos, 0);
        other.transform.localEulerAngles = stackPoint.localEulerAngles + Vector3.forward * 90;
        index++;
        

    }

    public void Hit()
    {

        if (chop)
        {
            
            foreach (Collider item in currentTreeParent)
            {
                SpawnLog(item);
                if (item.transform.childCount == 1)
                {
                    item.transform.GetChild(0).gameObject.SetActive(false);
                    item.transform.GetChild(0).transform.parent = null;
                }

                if (item.transform.childCount > 1)
                {
                    int number = Random.Range(1, item.transform.childCount);
                    item.transform.GetChild(number).GetComponent<Rigidbody>().isKinematic = false;
                    item.transform.GetChild(number).tag = "FallenParts";
                    item.transform.GetChild(number).transform.parent = null;
                }
                if (item.transform.childCount < 1 )
                {
                    chop = false;
                    anim.SetBool("Chop", false);
                    item.gameObject.GetComponent<Collider>().enabled = false;
                    StartCoroutine(SpawnTree(item));
                    erasedObjects.Add(item.gameObject);
                }
            }
            foreach (var item in erasedObjects)
            {
                currentTreeParent.Remove(item.GetComponent<Collider>());
                
            }
        }
        StartCoroutine(RemoveFallenParts());
    }

    IEnumerator RemoveFallenParts()
    {
        
        yield return new WaitForSeconds(2);
        GameObject[] fallenParts= GameObject.FindGameObjectsWithTag("FallenParts");
        foreach (var item in fallenParts)
        {
            Destroy(item,1);
        }
    }

    IEnumerator SpawnTree(Collider tree)
    {
        
        yield return new WaitForSeconds(10);
        Instantiate(treePrefab, tree.transform.position, Quaternion.identity);
       
    }
    void SpawnLog(Collider currentTreeParent)
    {
        GameObject log›nstance = Instantiate(logPrefab, currentTreeParent.transform.position+new Vector3(0.3f,3,0), Quaternion.identity);
        log›nstance.GetComponent<Rigidbody>().AddForce(Vector3.up * 300);

        GameObject log›nstance2 = Instantiate(logPrefab, currentTreeParent.transform.position + new Vector3(-0.3f, 3, 0), Quaternion.identity);
        log›nstance2.GetComponent<Rigidbody>().AddForce(Vector3.up * 300);

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TreeParent"))
        {
            chop = false;
            anim.SetBool("Chop", false);
            foreach (var item in currentTreeParent)
            {
                currentTreeParent.Remove(item.GetComponent<Collider>());
            }
        }

        if (other.CompareTag("GetGold"))
        {
            index = stackPoint.childCount;
        }
        if (other.CompareTag("ProduceWood"))
        {
            index = stackPoint.childCount;
        }


    }



}
