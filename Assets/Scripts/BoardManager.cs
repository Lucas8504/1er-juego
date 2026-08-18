using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager sharedInstance;
    public List<PieceDefinition> prefabs = new List<PieceDefinition>();
    public GameObject currentCandy;
    public int xSize, ySize;

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

    private void CreateInitialBoard(Vector2 offset)
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
                {
                    idx = Random.Range(0, prefabs.Count);
                } while ((x > 0 && idx == candies[x - 1, y].GetComponent<Candy>().id) ||
                        (y > 0 && idx == candies[x, y - 1].GetComponent<Candy>().id));

                Candy candy = newCandy.GetComponent<Candy>();
                candy.SetPiece(prefabs[idx]);

                newCandy.transform.parent = this.transform;
                candies[x, y] = newCandy;
            }
        }
    }
    

    public IEnumerator FindNullCandies()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (candies[x, y].GetComponent<Candy>().piece == null)
                {
                    yield return StartCoroutine(MakeCandiesFall(x, y));
                }
            }
        }

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                candies[x, y].GetComponent<Candy>().FindAllMatches();
            }
        }
    }

    private IEnumerator MakeCandiesFall(int x, int yStart, float shiftDelay = 0.05f)
    {
        isShifting = true;
        List<Candy> candyColumn = new List<Candy>();
        int nullCandies = 0;

        for (int y = yStart; y < ySize; y++)
        {
            Candy candy = candies[x, y].GetComponent<Candy>();
            if (candy.piece == null)
            {
                nullCandies++;
            }
            candyColumn.Add(candy);
        }

        for (int i = 0; i < nullCandies; i++)
        {
            GUIManager.sharedInstance.AddScore(10);

            yield return new WaitForSeconds(shiftDelay);
            for (int j = 0; j < candyColumn.Count - 1; j++)
            {
                candyColumn[j].SetPiece(candyColumn[j + 1].piece);
                candyColumn[j + 1].SetPiece(GetNewCandy(x, ySize - 1));
            }
        }

        if (nullCandies > 0)
        {
            GUIManager.sharedInstance.IncreaseMultiplier();
        }

        isShifting = false;
    }

    private PieceDefinition GetNewCandy(int x, int y)
    {
        List<int> adjacentIds = new List<int>();

        if (x > 0)
            adjacentIds.Add(candies[x - 1, y].GetComponent<Candy>().id);
        if (x < xSize - 1)
            adjacentIds.Add(candies[x + 1, y].GetComponent<Candy>().id);
        if (y > 0)
            adjacentIds.Add(candies[x, y - 1].GetComponent<Candy>().id);

        List<PieceDefinition> possibleCandies = new List<PieceDefinition>();
        for (int i = 0; i < prefabs.Count; i++)
        {
            if (!adjacentIds.Contains(prefabs[i].id))
                possibleCandies.Add(prefabs[i]);
        }

        return possibleCandies[Random.Range(0, possibleCandies.Count)];
    }
}
