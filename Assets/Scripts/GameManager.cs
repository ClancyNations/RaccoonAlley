using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    
    InputAction leftMouse;
    public GameObject trashPrefab;
    public GameObject foodPrefab;
    public GameObject bajaBlastPrefab;
    
    [SerializeField] public List<GameObject> allFoods = new List<GameObject>();
    [SerializeField] public List<GameObject> allSodas = new List<GameObject>();
    [SerializeField] public List<GameObject> allTrash  = new List<GameObject>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leftMouse = InputSystem.actions.FindAction("Click");
    }

    // Update is called once per frame
    void Update()
    {
        if (leftMouse.WasReleasedThisFrame())
        {
            CheckHover();
        }
    }

    void CheckHover()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        if (hit != null)
        {
               if (hit.CompareTag("dumpster"))
               {
                   Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0.1f, 0.1f, 0));
                   Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0.9f, 0));
                   
                   Vector3 trashSpawnPoint = new Vector3(
                       Random.Range(bottomLeft.x, topRight.x),
                       Random.Range(bottomLeft.y, topRight.y),0
                   );
                   
                   allTrash.Add(Instantiate(trashPrefab, trashSpawnPoint, Quaternion.identity));
               }
        }
    }

}
