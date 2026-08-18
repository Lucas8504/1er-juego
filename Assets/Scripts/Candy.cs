using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{

    private static Color seletedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    private static Candy previousSelected = null;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isSelected = false;

    public int id;
    public PieceDefinition piece;

    private Vector2[] adjacenDirections = new Vector2[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void SetPiece(PieceDefinition newPiece)
    {
        piece = newPiece;
        if (newPiece != null)
        {
            id = newPiece.id;
            spriteRenderer.sprite = newPiece.sprite;
            if (animator != null && newPiece.animatorOverride != null)
            {
                animator.runtimeAnimatorController = newPiece.animatorOverride;
            }
        }
        else
        {
            id = -1;
            spriteRenderer.sprite = null;
        }
    }

    private void SelectCandy()
    {
        // Solo se puede seleccionar si hay movimientos disponibles
        if (GUIManager.sharedInstance.MovesCounter > 0)
        {
            isSelected = true;
            spriteRenderer.color = seletedColor;
            previousSelected = gameObject.GetComponent<Candy>();
        }
    }

    private void DeselectCandy()
    {
        isSelected = false;
        spriteRenderer.color = Color.white;
        previousSelected = null;
    }

    private void OnMouseDown()
    {
        if (piece == null || BoardManager.sharedInstance.isShifting)
        {
            return;
        }
        if (isSelected)
        {
            DeselectCandy();
        }
        else
        {
            if (previousSelected == null)
            {
                SelectCandy();
            }
            else
            {
                if (CanSwipe())
                {
                    GUIManager.sharedInstance.ResetMultiplier();

                    SwapPiece(previousSelected);
                    previousSelected.FindAllMatches();
                    previousSelected.DeselectCandy();
                    FindAllMatches();

                    ;
                    GUIManager.sharedInstance.MovesCounter--;
                   
                }
                else
                {
                    previousSelected.DeselectCandy();
                    SelectCandy();
                }

            }
        }
    }

    public void SwapPiece(Candy other)
    {
        if (this.piece == other.piece)
        {
            return;
        }

        PieceDefinition tempPiece = this.piece;
        this.SetPiece(other.piece);
        other.SetPiece(tempPiece);
    }

    private GameObject GetNeighbor(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    private List<GameObject> GetAllNeighbors()
    {
        List<GameObject> neighbors = new List<GameObject>();

        foreach (Vector2 direction in adjacenDirections)
        {
            neighbors.Add(GetNeighbor(direction));
        }
        return neighbors;
    }

    private bool CanSwipe()
    {
        return GetAllNeighbors().Contains(previousSelected.gameObject);
    }

    private List<GameObject> FindMatch(Vector2 direction)
    {
        List<GameObject> matchingCandies = new List<GameObject>();

        //Consulta de vecinos.
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction);

        while (hit.collider != null && hit.collider.GetComponent<Candy>().id == this.id)
        {
            matchingCandies.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, direction);
        }

       

        return matchingCandies;

    }

    private bool ClearMatch(Vector2[] directions)
    {
        List<GameObject> matchingCandies = new List<GameObject>();
        foreach (Vector2 direction in directions)
        {
            matchingCandies.AddRange(FindMatch(direction));
        }

        if (matchingCandies.Count >= BoardManager.MinCandiesToMatch)
        {
            foreach (GameObject candy in matchingCandies)
            {
                candy.GetComponent<Candy>().SetPiece(null);
            }

            return true;
        }
        else
        {
            return false;
        } 

    }

    public void FindAllMatches()
    {
        if (piece == null)
        {
            return;
        }
        
        bool hMatch = ClearMatch(new Vector2[2] 
        {
            Vector2.left, Vector2.right 
        });

        bool vMatch = ClearMatch(new Vector2[2]
        {
            Vector2.up, Vector2.down
        });

        if (hMatch || vMatch)
        {
            SetPiece(null);
            StopCoroutine(BoardManager.sharedInstance.FindNullCandies());
            StartCoroutine(BoardManager.sharedInstance.FindNullCandies());
            
        }
       
    }

}
