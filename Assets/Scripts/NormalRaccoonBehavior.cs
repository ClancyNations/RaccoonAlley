using UnityEngine;
using System.Collections.Generic;


public enum normalRaccStates{
    moving,
    searching,
    resting,
    goingDumpster
}
public class NormalRaccoonBehavior : MonoBehaviour
{
    
    public GameManager gameManager;
    public SpriteRenderer normalRaccoonSR;
    public Sprite raccoonSearchingSprite;
    public Sprite normalRaccoonSprite;
    public Sprite raccoonRestingSprite;
    
    
    Vector3 targetPos; 
    Vector2 dir;
    bool isMoving;
    bool isSearching;
    bool isResting;
    private GameObject foundTrash;
    normalRaccStates  currentState;
    normalRaccStates pastState;
    int trashSearched = 0;
    bool isDespawning = false;
    float despawnTimer = 3f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = normalRaccStates.resting;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isDespawning)
        {
            despawnTimer -= Time.deltaTime;
            normalRaccoonSR.enabled = false;
            if (despawnTimer <= 0f)
            {
                despawnTimer = 3f;
                trashSearched = 0;
                normalRaccoonSR.enabled = true;
                isDespawning = false;
                currentState = normalRaccStates.resting;
            }
            return;
        }
        
        switch(currentState)
        {
            case normalRaccStates.moving:
                Debug.Log("normalRaccMoving");
                isResting = false;
                normalRaccoonSR.sprite = normalRaccoonSprite;
                //set our position to the next step towards our target
                transform.position = Vector3.MoveTowards(transform.position, targetPos, 1f * Time.deltaTime);
                //if we reached the target, stop moving
                if (Vector3.Distance(transform.position, targetPos) < 0.1f)
                {
                    isMoving = false;
                    isSearching = true;
                    pastState = currentState;
                    currentState = normalRaccStates.searching;
                    break;
                }
                break;
            case normalRaccStates.searching:
                Debug.Log("normalRaccSearching");
                normalRaccoonSR.sprite = raccoonSearchingSprite;
                if (currentState != pastState)
                {
                    Invoke("TrashSearching", 0f);
                    trashSearched++;
                    if (trashSearched >= 15)
                    {
                        pastState = currentState;
                        currentState = normalRaccStates.goingDumpster;
                        return;
                    }
                    Invoke("Rest", 2f);
                    pastState = currentState;
                }
                break;
            case normalRaccStates.resting:
                Debug.Log("normalRaccResting");
                normalRaccoonSR.sprite = raccoonRestingSprite;
                FindTrash();
                break;
            case normalRaccStates.goingDumpster:
                Debug.Log("normalRaccGoingDumpster");
                Vector3 dumpsterPos = new Vector3(-6,3,0);
                transform.position = Vector3.MoveTowards(transform.position, dumpsterPos, 1f * Time.deltaTime);
                if (transform.position == dumpsterPos)
                {
                    isDespawning = true;
                }
                break;

        }
        
        
    }


    void FindTrash()
    {
        float dist = 2000f;
        //tracks the nearest trash
        GameObject closestTrash = null; 
        //loop through the game manager's trash list
        foreach (GameObject trash in gameManager.allTrash)
        {
            if (Vector3.Distance(transform.position, trash.transform.position) < dist)
            {
                //set dist to the new closest distance
                dist = Vector3.Distance(transform.position, trash.transform.position);
                //set the var tracking the closest trash to this trash
                closestTrash = trash;
            }
        }
        //if we found a trash
        if (closestTrash != null)
        {
            targetPos = closestTrash.transform.position;
            foundTrash = closestTrash;
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (isMoving)
        {
            currentState = normalRaccStates.moving;
        }
        
    }

    void Rest()
    {
        isSearching = false;
        isResting = true;
        transform.position = Vector3.MoveTowards(transform.position, transform.position, 1.5f * Time.deltaTime);
        currentState = normalRaccStates.resting;
    }

    void TrashSearching()
    {
        gameManager.allTrash.Remove(foundTrash);
        Destroy(foundTrash);
        int trashType = Random.Range(0, 2);
        if (trashType == 0)
        {
            gameManager.allSodas.Add(Instantiate(gameManager.bajaBlastPrefab, targetPos, Quaternion.identity));
        }
        else if (trashType == 1)
        {
            gameManager.allFoods.Add(Instantiate(gameManager.foodPrefab,  targetPos, Quaternion.identity));
        }
        
    }
    
    
}
