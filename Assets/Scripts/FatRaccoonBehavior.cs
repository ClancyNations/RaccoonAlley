using UnityEngine;


public enum FatRaccoonStates
{
    eating,
    moving,
    resting,
    goingDumpster
}
public class FatRaccoonBehavior : MonoBehaviour
{
    public GameManager gameManager;
    public SpriteRenderer fatRaccoonSR;
    public Sprite raccoonEatingSprite;
    public Sprite normalRaccoonSprite;
    public Sprite raccoonRestingSprite;
    Vector3 targetPos; 
    Vector2 dir;
    FatRaccoonStates currentState;
    FatRaccoonStates pastState;
    GameObject foundFood;
    int foodEaten = 0;
    bool isDespawning = false;
    float despawnTimer = 4f;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = FatRaccoonStates.resting;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDespawning)
        {
            despawnTimer -= Time.deltaTime;
            fatRaccoonSR.enabled = false;
            if (despawnTimer <= 0f)
            {
                despawnTimer = 4f;
                foodEaten = 0;
                fatRaccoonSR.enabled = true;
                isDespawning = false;
                currentState = FatRaccoonStates.resting;
            }
            return;
        }
        switch (currentState)
        {
            case FatRaccoonStates.eating:
                Debug.Log("eating");
                fatRaccoonSR.sprite = raccoonEatingSprite;
                if (currentState != pastState)
                {
                    gameManager.allFoods.Remove(foundFood);
                    Destroy(foundFood);
                    foodEaten++;
                    if (foodEaten >= 5)
                    {
                        pastState = currentState;
                        currentState = FatRaccoonStates.goingDumpster;
                        return;
                    }
                    pastState =  currentState;
                    Invoke("rest", 2f);
                    
                }
                break;
            case FatRaccoonStates.moving:
                Debug.Log("fatRaccMoving");
                fatRaccoonSR.sprite = normalRaccoonSprite;
                //set our position to the next step towards our target
                transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.3f * Time.deltaTime);
                //if we reached the target, stop moving
                if (Vector3.Distance(transform.position, targetPos) < 0.1f)
                {
                    pastState = currentState;
                    currentState = FatRaccoonStates.eating;
                    break;
                }
                break;
            case FatRaccoonStates.resting:
                Debug.Log("resting");
                fatRaccoonSR.sprite = raccoonRestingSprite;
                FindFood();
                break;
            case FatRaccoonStates.goingDumpster:
                Debug.Log("goingDumpster");
                Vector3 dumpsterPos = new Vector3(-6,3,0);
                transform.position = Vector3.MoveTowards(transform.position, dumpsterPos, 0.3f * Time.deltaTime);
                if (transform.position == dumpsterPos)
                {
                    isDespawning = true;
                }
                break;
                
        }
        
    }

    void rest()
    {
        currentState = FatRaccoonStates.resting;
    }

    void FindFood()
    {
        float dist = 2000f;
        //tracks the nearest trash
        GameObject closestFood = null; 
        //loop through the game manager's trash list
        foreach (GameObject food in gameManager.allFoods)
        {
            if (Vector3.Distance(transform.position, food.transform.position) < dist)
            {
                //set dist to the new closest distance
                dist = Vector3.Distance(transform.position, food.transform.position);
                //set the var tracking the closest soda to this soda
                closestFood = food;
            }
        }
        //if we found a soda
        if (closestFood != null)
        {
            targetPos = closestFood.transform.position;
            pastState = currentState;
            foundFood = closestFood;
            currentState = FatRaccoonStates.moving;
            
        }
    }
    
    
}
