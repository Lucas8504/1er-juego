using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BoardManager: MonoBehaviour
{
    public static BoardManager sharedInstance;
    public List<Sprite> prefabs = new List<Sprite>();   
    public GameObject currentCandy;
    public int xSize , ySize;

    private GameObject[,] candies;

    private bool isShifting { get; set; }

    void Start()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Vector2 offset = currentCandy.GetComponent<BoxCollider2D>().size;
        CreateInitialBoard(offset);
    }

    private void CreateInitialBoard( Vector2 offset)

    {

    }

    void Update()
    {
        // This method is called once per frame
        // You can add any logic that needs to be checked or updated every frame here
    }
}

