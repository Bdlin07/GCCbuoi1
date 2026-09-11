using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] Vector2Int grid = new Vector2Int(5, 4);
    [SerializeField] float size = 1;
    [SerializeField] float sp = 5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        float a = grid.x * size + (grid.x - 1) * sp;
        float b = grid.y * size + (grid.y - 1) * sp;
        float c = size + sp;

        for (int i = 0; i < grid.x; i++)
        {
            for (int j = 0; j < grid.y; j++)
            {
                float x = -a / 2 + size / 2 + i * c;
                float y = -b / 2 + size / 2 + j * c;
                float z = 0;

                Gizmos.DrawWireCube(
                    new Vector3(x, y, z),
                    new Vector3(size, size, 0)
                );
            }
        }
    }
}