using UnityEngine;
using UnityEngine.UIElements;

public class OneWayWall : MonoBehaviour
{
   private BoxCollider2D WallCollider;
    void Start()
    {
        WallCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            WallCollider.isTrigger = false;
            Debug.Log("Player entered one-way wall trigger");
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Box"))
        {
            Debug.Log("Puzzle Completed!");
        }
    }
}
