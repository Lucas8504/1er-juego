using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{

    private static Color seletedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    private static Candy previousSelected = null;

    private SpriteRenderer spriteRenderer;
    private bool isSelected = false;

    public int id;

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
    }

    private void SelectCandy()
    {
        isSelected = true;
        spriteRenderer.color = seletedColor;
        previousSelected = gameObject.GetComponent<Candy>();
    }

    private void DeselectCandy()
    {
        isSelected = false;
        spriteRenderer.color = Color.white;
        previousSelected = null;
    }

    private void OnMouseDown()
    {
        if (spriteRenderer.sprite == null || BoardManager.sharedInstance.isShifting)
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
                    SwapSprite(previousSelected);
                    previousSelected.DeselectCandy();
                }
               else
                {
                    previousSelected.DeselectCandy();
                    SelectCandy();
                }
                
            }
        }
    }

    public void SwapSprite(Candy newCandy)
    {
        
        if (spriteRenderer.sprite == newCandy.GetComponent<SpriteRenderer>().sprite)
        {
            return;
        }
        Sprite oldCandy = newCandy.spriteRenderer.sprite;
        newCandy.spriteRenderer.sprite = this.spriteRenderer.sprite;
        this.spriteRenderer.sprite = oldCandy;  

        int tempId = newCandy.id;
        newCandy.id = this.id;
        this.id = tempId;
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

        foreach(Vector2 direction in adjacenDirections)
        {
            neighbors.Add(GetNeighbor(direction));
        }
        return neighbors;
    }

    private bool CanSwipe()
    {
        return GetAllNeighbors().Contains(previousSelected.gameObject);
    }
}
