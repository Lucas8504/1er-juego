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
        candies = new GameObject[xSize, ySize];

        float startX = this.transform.position.x;
        float startY = this.transform.position.y;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject newCandy = Instantiate(currentCandy,
                    new Vector3(startX + (x * offset.x), startY + (y * offset.y), 0),
                    currentCandy.transform.rotation);
                newCandy.name = string.Format("Candy[{0}][{1}]", x, y);

                candies[x, y] = newCandy;


            }
        }
    }
    void Update()
    {
        // This method is called once per frame
        // You can add any logic that needs to be checked or updated every frame here
    }
}

