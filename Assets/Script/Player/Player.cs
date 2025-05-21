using UnityEngine;

public class Player : PG
{   
    [SerializeField] GameObject BombPrefab;

    protected override void Checks()
    {
        PlaceBomb();
    }

    private void PlaceBomb()
    {
        if (isMoving) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Bomb bomb = Instantiate(BombPrefab, transform.position, transform.rotation).GetComponent<Bomb>();
            bomb.Setup(grid, MapCollision, destructibleCollision);
        }
    }
}
