using UnityEngine;


public enum FastRaccoonStates
{
    moving,
    drinking,
    hyper,
    idle,
    goingDumpster
}

public class FastRaccoonBehavior : MonoBehaviour
{
    
    
    public GameManager gameManager;
    public SpriteRenderer fastRaccoonSR;
    public Sprite raccoonDrinkingSprite;
    public Sprite normalRaccoonSprite;
    public Sprite raccoonHyperSprite;
    Vector3 targetPos; 
    Vector2 dir;
    float speedMultiplier = 0.1f;
    FastRaccoonStates currentState;
    FastRaccoonStates pastState;
    GameObject foundSoda;
    private float hyperTimer;
    private bool hasSoda;
    public Rigidbody2D rb;
    int drinksDrunk = 0;
    bool isDespawning = false;
    float despawnTimer = 3f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pastState = FastRaccoonStates.moving;
        currentState = FastRaccoonStates.idle;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isDespawning)
        {
            despawnTimer -= Time.deltaTime;
            fastRaccoonSR.enabled = false;
            if (despawnTimer <= 0f)
            {
                despawnTimer = 3f;
                drinksDrunk = 0;
                speedMultiplier = 0.1f;
                fastRaccoonSR.enabled = true;
                isDespawning = false;
                currentState = FastRaccoonStates.idle;
            }
            return;
        }
        
        switch(currentState)
        {
            case FastRaccoonStates.moving:
                Debug.Log("FastRaccoon moving");
                fastRaccoonSR.sprite = normalRaccoonSprite;
                //set our position to the next step towards our target
                transform.position = Vector3.MoveTowards(transform.position, targetPos, (0.5f + speedMultiplier) * Time.deltaTime);
                //if we reached the target, stop moving
                if (Vector3.Distance(transform.position, targetPos) < 0.1f)
                {
                    if (hasSoda)
                    {
                        pastState = currentState;
                        currentState = FastRaccoonStates.drinking;
                    }
                    else
                    {
                        pastState = currentState;
                        currentState = FastRaccoonStates.idle;
                    }
                }
                
                break;
            case FastRaccoonStates.drinking:
                Debug.Log("FastRaccoon drinking");
                fastRaccoonSR.sprite = raccoonDrinkingSprite;
                if (currentState != pastState)
                {
                    gameManager.allSodas.Remove(foundSoda);
                    Destroy(foundSoda);
                    drinksDrunk++;
                    if (drinksDrunk >= 10)
                    {
                        pastState = currentState;
                        currentState = FastRaccoonStates.goingDumpster;
                        return;
                    }
                    speedMultiplier += 0.2f;
                    hasSoda = false;
                    pastState =  currentState;
                    Invoke("becomeHyper", 2f);
                }
                break;
            case FastRaccoonStates.hyper:
                Debug.Log("FastRaccoon hyper");
                if (currentState != pastState)
                {
                    hyperTimer = 2f;
                    HyperRandomMove();
                    pastState = currentState;
                    fastRaccoonSR.sprite = raccoonHyperSprite;
                    
                }
                hyperTimer -= Time.deltaTime;
                if (hyperTimer <= 0f)
                {
                    currentState = FastRaccoonStates.idle;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, (4f + speedMultiplier) * Time.deltaTime);
                    if (Vector3.Distance(transform.position, targetPos) < 0.1f)
                    {
                        HyperRandomMove();
                    }
                }
                break;
            case FastRaccoonStates.idle:
                Debug.Log("FastRaccoon idle");
                if (currentState != pastState)
                {
                    rb.linearVelocity = Vector2.zero;
                    RandomMove();
                }
                FindSoda();
                pastState = currentState;
                currentState = FastRaccoonStates.moving;
                break;
            case FastRaccoonStates.goingDumpster:
                Debug.Log("FastRaccoon goingDumpster");
                Vector3 dumpsterPos = new Vector3(-6,3,0);
                transform.position = Vector3.MoveTowards(transform.position, dumpsterPos, (0.5f + speedMultiplier) * Time.deltaTime);
                if (transform.position == dumpsterPos)
                {
                    isDespawning = true;
                }
                break;
        }
    }
    
    void FindSoda()
    {
        float dist = 2000f;
        //tracks the nearest trash
        GameObject closestSoda = null; 
        //loop through the game manager's trash list
        foreach (GameObject soda in gameManager.allSodas)
        {
            if (Vector3.Distance(transform.position, soda.transform.position) < dist)
            {
                //set dist to the new closest distance
                dist = Vector3.Distance(transform.position, soda.transform.position);
                //set the var tracking the closest soda to this soda
                closestSoda = soda;
            }
        }
        //if we found a soda
        if (closestSoda != null)
        {
            targetPos = closestSoda.transform.position;
            pastState = currentState;
            foundSoda = closestSoda;
            currentState = FastRaccoonStates.moving;
            
        }


        
    }
    
    void RandomMove(){
    Vector3 randMove = new Vector3(Random.Range(-3f,3f), Random.Range(-3f, 3f), Random.Range(-3f, 3f));
    targetPos = randMove;
    }
    
    void HyperRandomMove(){
        Vector3 randMove = transform.position + new Vector3(Random.Range(-2f,2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f));
        targetPos = randMove;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "soda")
        {
            foundSoda = other.gameObject;
            hasSoda = true;
        }
    }

    void becomeHyper()
    {
        pastState = currentState;
        currentState = FastRaccoonStates.hyper;
    }
    
    
}
