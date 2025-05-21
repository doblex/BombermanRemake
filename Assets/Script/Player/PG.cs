using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(HealthController))]
public class PG : MonoBehaviour 
{
    Vector2[] directions = new[] {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

    public enum PgType { PLAYER,ENEMY }
    public PgType type;

    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected Grid grid;

    [SerializeField] protected Tilemap MapCollision;
    [SerializeField] protected Tilemap destructibleCollision;

    HealthController healthController;

    protected bool isMoving = false;
    protected Vector3 targetPos;

    private void Awake()
    {
        healthController = GetComponent<HealthController>();
        healthController.onDeath += OnDeath;
    }

    protected virtual void OnDeath()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        Vector3Int cell = grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld(cell);
    }

    protected virtual void Update()
    {
        Movement();
        Checks();
    }

    protected virtual void Checks() { }

    protected void Movement()
    {
        if (isMoving) return;

        Vector2 input = Vector2.zero;

        switch (type)
        {
            case PgType.PLAYER:
                input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                if (input.x != 0) input.y = 0;
                break;
            case PgType.ENEMY:
                input = directions[Random.Range(0, directions.Length)];
                break;
        }

        if (input != Vector2.zero)
        {
            Vector3Int currentCell = grid.WorldToCell(transform.position);
            Vector3Int nextCell = currentCell + new Vector3Int((int)input.x, (int)input.y, 0);

            if (MapCollision.HasTile(nextCell) || destructibleCollision.HasTile(nextCell))
                return;

            targetPos = grid.GetCellCenterWorld(nextCell);
            StartCoroutine(MoveToPosition(targetPos));
        }
    }
    protected IEnumerator MoveToPosition(Vector3 target)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < 1f / moveSpeed)
        {
            transform.position = Vector3.Lerp(startPos, target, elapsedTime * moveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }
}
