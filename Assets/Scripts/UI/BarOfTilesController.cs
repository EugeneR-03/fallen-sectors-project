using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BarOfTilesController : MonoBehaviour
{
    private GameObject[] _tiles;

    private void Awake()
    {
        _tiles = GetComponentsInChildren<Image>(true)
                    .Select(x => x.gameObject)
                    .Where(x => x.name.StartsWith("BarTile"))
                    .ToArray();
    }

    private void SetActiveBarTile(int tileIndex, bool isActive)
    {
        _tiles[tileIndex].SetActive(isActive);
    }

    public void SetActiveBarTiles(int tilesCount)
    {
        for (int i = 0; i < _tiles.Length; i++)
        {
            if (i < tilesCount)
            {
                SetActiveBarTile(i, true);
            }
            else
            {
                SetActiveBarTile(i, false);
            }
        }
    }
}