using UnityEngine;
using UnityEngine.Tilemaps;

public class Bomb : MonoBehaviour
{
    [SerializeField] int explosionRadius = 2;
    [SerializeField] float cellDimensionMultiplayer = 0.48f;
    [SerializeField] float explosionDuration = 1f;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float explosionDelay = 2f;

    [SerializeField] LayerMask layer;

    Grid grid;
    Tilemap obstacle;
    Tilemap breakable;

    Vector3Int[] directions = new[] {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };

    bool[,] explosionGrid;
    float timer = 0;

    public void Setup(Grid _grid, Tilemap _obstacle, Tilemap _breakable)
    {
        grid = _grid;
        obstacle = _obstacle;
        breakable = _breakable;

        Vector3Int center = grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld(center);

        explosionGrid = new bool[2 * explosionRadius + 1, 2 * explosionRadius + 1];
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= explosionDelay)
        {
            Explode();
            Destroy(gameObject); // Optional: destroy bomb object
        }
    }

    private void Explode()
    {
        Vector3Int center = grid.WorldToCell(transform.position);
        int centerIndex = explosionRadius;

        explosionGrid[centerIndex, centerIndex] = true;

        foreach (var dir in directions)
        {
            for (int i = 1; i <= explosionRadius; i++)
            {
                Vector3Int cell = center + dir * i;
                int dx = dir.x * i;
                int dy = dir.y * i;

                if (obstacle.HasTile(cell))
                    break;

                explosionGrid[centerIndex + dx, centerIndex + dy] = true;
            }

            RaycastHit2D hit = Physics2D.Raycast(transform.position, ((Vector2Int)dir),explosionRadius * cellDimensionMultiplayer, layer);
            Debug.DrawLine(transform.position, hit.point, Color.yellow,5f);
            if(hit.collider != null)
            {
                HealthController healthController;

                if (hit.collider.gameObject.TryGetComponent<HealthController>(out healthController))
                {
                    healthController.DoDamage(1);          
                }
            }

        }

        ProcessExplosion(center);
    }

    void ProcessExplosion(Vector3Int centerCell)
    {
        int size = explosionRadius;
        for (int x = -size; x <= size; x++)
        {
            for (int y = -size; y <= size; y++)
            {
                int gridX = x + size;
                int gridY = y + size;

                if (explosionGrid[gridX, gridY])
                {
                    Vector3Int cell = centerCell + new Vector3Int(x, y, 0);

                    if (breakable.HasTile(cell))
                        breakable.SetTile(cell, null);

                    GameObject clone = Instantiate(explosionPrefab, grid.GetCellCenterWorld(cell), Quaternion.identity);
                    Destroy(clone, clone.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

                }
            }
        }
    }
}
