
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileChecker : MonoBehaviour
{
    [SerializeField] private Tilemap _tileMap;

    private Tile _tile;
    private Vector3Int location;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            location = _tileMap.WorldToCell(mp);

            if (_tileMap.GetTile(location))
            {
                Debug.Log("Tile");
            }
            else
            {
                Debug.Log("No Tile");
            }
        }
    }
}
