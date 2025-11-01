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

    public bool isShifting { get; set; }

    private Candy selectedCandy;

    public const int MinCandiesToMatch = 2; 

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

        int idx = -1;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject newCandy = Instantiate(currentCandy,
                    new Vector3(startX + (x * offset.x),
                                startY + (y * offset.y),
                                0),
                    currentCandy.transform.rotation
                    );
                newCandy.name = string.Format("Candy[{0}][{1}]", x, y);

                do
                { idx = Random.Range(0, prefabs.Count);
                }while( (x>0 && idx == candies[x-1,y].GetComponent<Candy>().id) ||
                        (y > 0 && idx == candies[x, y - 1].GetComponent<Candy>().id) );

                Sprite sprite = prefabs[idx];
                newCandy.GetComponent<SpriteRenderer>().sprite = sprite;
                newCandy.GetComponent<Candy>().id = idx;

                newCandy.transform.parent = this.transform;
                candies[x, y] = newCandy;


            }
        }
    }
    private List<GameObject> FindMatch(Vector2 direction)
    {
         List<GameObject> matchingCandies = new List<GameObject>();

    }
}

 