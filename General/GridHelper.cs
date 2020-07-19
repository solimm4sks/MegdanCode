using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class GridHelper
{
    public static Grid grid;
    public static Tilemap groundTilemap;
    public static Tilemap waterTilemap;

    private static Vector3Int v3ispecial = new Vector3Int(-999, -999, 0);
    public static Color standableTileColor = new Color(120f / 255, 117f / 255, 76f / 255, 1f); //highlight move color
    public static Color spawnTileColor = new Color(36f / 255, 122f / 255, 242f / 255, 1f); //highlight spawn color

    public static void SetGrid(Grid g) {
        grid = g;
    }

    public static void SetGroundTilesColor(List<Vector3Int> tiles, Color color) {
        foreach (var tile in tiles)
            groundTilemap.SetColor(tile, color);
    }


    public static List<IAttackable> IAttackableHitFilter(List<RaycastHit2D> hits) {
        List<IAttackable> targets = new List<IAttackable>();
        foreach (var hit in hits) {
            IAttackable target = hit.transform.parent.GetComponent<IAttackable>();
            if (target != null)
                targets.Add(target);
        }
        return targets;
    }

    public static List<IAttackable> IAttackablesInTiles(List<Vector3Int> tiles) {
        return IAttackableHitFilter(RaycastTilesNoEmptyHits(tiles));
    }

    public static int TileDistance(Vector3Int t1, Vector3Int t2) {
        if (t1 == t2)
            return 0;

        List<Vector3Int> adjs = new List<Vector3Int>(GetAdjacentTiles(t1));
        if (TileInList(t2, adjs))
            return 1;
        List<Vector3Int> nearby = new List<Vector3Int>(GetNearbyTiles(t1));
        if (TileInList(t2, nearby))
            return 2;
        List<Vector3Int> dist3 = new List<Vector3Int>(GetDist3Tiles(t1));
        if (TileInList(t2, dist3))
            return 3;
        return 4;
    }

    public static bool TileExists(Vector3Int tile) {
        return groundTilemap.HasTile(tile);
    }

    public static bool IsTileOnRim(Vector3Int tile) {
        for(int i = 0; i < 6; ++i) {
            Vector3Int adj = GetTileInDirection(tile, i);
            if(!groundTilemap.HasTile(adj))
                return true;
        }
        return false;
    }

    public static int GetAdjDirection(Vector3Int t1, Vector3Int t2) {
        if (!IsTileAdjacent(t1, t2))
            return -1;

        for (int i = 0; i < 6; ++i) {
            if (t2 == GetTileInDirection(t1, i))
                return i;
        }

        return -1;
    }

    public static bool IsTileAdjacent(Vector3Int t1, Vector3Int t2) {
        if (GetTileInDirection(t1, 0) == t2 ||
            GetTileInDirection(t1, 1) == t2 ||
            GetTileInDirection(t1, 2) == t2 ||
            GetTileInDirection(t1, 3) == t2 ||
            GetTileInDirection(t1, 4) == t2 ||
            GetTileInDirection(t1, 5) == t2) {
            return true;
        }
        return false;
    }

    public static List<RaycastHit2D> RaycastTile(Vector3Int tile) {
        return new List<RaycastHit2D>(Physics2D.RaycastAll(grid.CellToWorld(tile), Vector2.zero));
    }

    public static List<RaycastHit2D> RaycastTiles(List<Vector3Int> tiles) {
        List<RaycastHit2D> result = new List<RaycastHit2D>();
        foreach(var tile in tiles) {
            RaycastHit2D[] hits = Physics2D.RaycastAll(grid.CellToWorld(tile), Vector2.zero);
            foreach(var hit in hits)
                if(hit)
                    result.Add(hit);
        }
        return result;
    }

    public static List<RaycastHit2D> RaycastTilesNoEmptyHits(List<Vector3Int> tiles) {
        List<RaycastHit2D> result = new List<RaycastHit2D>();
        foreach (var tile in tiles) {
            RaycastHit2D[] hits = Physics2D.RaycastAll(grid.CellToWorld(tile), Vector2.zero);
            foreach(var hit in hits)
            if(hit)
                result.Add(hit);
        }
        return result;
    }

    public static Vector3Int GetTileInDirection(Vector3Int t, int dir) { ///up, top right, bot right, bot, bot left, top left = 0, 1, 2, 3, 4, 5
        switch (dir) {
            case 0:
                t.x += 1;
                return t;
            case 1:
                t.x += Mathf.Abs(t.y % 2); t.y += 1;
                return t;
            case 2:
                t.x -= (Mathf.Abs(t.y) % 2) ^ 1; t.y += 1;
                return t;
            case 3:
                t.x -= 1;
                return t;
            case 4:
                t.x -= (Mathf.Abs(t.y) % 2) ^ 1; t.y -= 1;
                return t;
            case 5:
                t.x += (Mathf.Abs(t.y) % 2); t.y -= 1;
                return t;

            default:
                Debug.LogError("Wrong direction (GridHelper.GetTileInDirection)");
                return v3ispecial; ///error, wrong dir
        }
    }

    public static List<Vector3Int> AllTiles() {
        List<Vector3Int> tiles = new List<Vector3Int>();

        for (int x = groundTilemap.cellBounds.xMin; x < groundTilemap.cellBounds.xMax; x++) {
            for (int y = groundTilemap.cellBounds.yMin; y < groundTilemap.cellBounds.yMax; y++) {
                Vector3Int localPlace = new Vector3Int(x, y, 0);
                tiles.Add(localPlace);
            }
        }

        List<Vector3Int> tmp = new List<Vector3Int>(tiles);
        foreach(var tile in tmp) {
            if(!TileExists(tile))
                tiles.Remove(tile);                
        }

        return tiles;
    }

    public static List<Vector3Int> TilesInRange(Vector3Int startPos, RangeDefined range) {
        if (range.basicRange) {
            Debug.LogError("Invalid request in GridHelper.TilesInRange. You should request range.basicRange from a PlayerManager");
            return new List<Vector3Int>();
        }

        if (range.allowedDirections == null) {
            range.allowedDirections = new List<int>();
        }

        if (range.allowedDirections.Count == 0) {
            for (int i = 0; i < 6; ++i) {
                range.allowedDirections.Add(i);
            }
        }

        if(range.dist == -1) { // i want to do this even if (range.straight == true) accidenatly
            List<Vector3Int> tiles = TilesInDistance(startPos, -1, range.self);
            return FilterInvalidTiles(tiles, range.IsTileValid);
        }

        if (range.straight) {
            List<Vector3Int> tiles = new List<Vector3Int>();
            foreach (int dir in range.allowedDirections) {
                List<Vector3Int> temp = TilesInDirection(startPos, dir, range.dist, range.piercing, range.IsTileValid);
                foreach (var tile in temp) {
                    tiles.Add(tile);
                }
            }
            if (range.self)
                tiles.Add(startPos);
            return tiles; //filtering must be done in TilesInDirection, so no need to do it here
        }

        List<Vector3Int> tiles1 = TilesInDistance(startPos, range.dist, range.self);
        return FilterInvalidTiles(tiles1, range.IsTileValid);
    }

    public static List<Vector3Int> FilterInvalidTiles(List<Vector3Int> tiles, RangeDefined.IsTileValidDel IsTileValid) {
        List<Vector3Int> temp = new List<Vector3Int>(tiles);
        foreach (var tile in temp)
            if (IsTileValid != null && IsTileValid != CanAttackThrough && !IsTileValid(tile)) //Can Attack through only makes sense if it is accounted for while making the range, doesnt make sense just to exclude it
                tiles.Remove(tile);
        return tiles;
    }

    public static List<Vector3Int> TilesInBasic(PlayerManager pm) {
        return pm.TilesInBasic();
    }

    public static int MAX_TILEDIST_ON_MAP = 4;
    public static List<Vector3Int> TilesInDistance(Vector3Int start, int dist, bool self = false) { //make this recurzive
        List<Vector3Int> tiles;
        switch (dist) {
            case -1:
                tiles = AllTiles();
                break;
            case 0:
                return new List<Vector3Int>() { start };
            case 1:
                tiles = new List<Vector3Int>(GetAdjacentTiles(start));
                break;
            case 2:
                tiles = new List<Vector3Int>(GetNearbyTiles(start));
                break;
            case 3:
                tiles = new List<Vector3Int>(GetDist3Tiles(start));
                break;
            default:
                Debug.LogError("GridHelper.TilesInDistance: Non-Implemented Distance, defaulting to AllTiles()");
                tiles = AllTiles();
                break;
        }

        if (self) 
            if (!tiles.Contains(start))
                tiles.Add(start);
        else 
            tiles.Remove(start);

        return tiles;
    }

    public static List<Vector3Int> GetDist3Tiles(Vector3Int start) {
        Vector3Int[] nearbyTiles = GetNearbyTiles(start);
        HashSet<Vector3Int> tiles = new HashSet<Vector3Int>(nearbyTiles);
        foreach (var tile in nearbyTiles) {
            Vector3Int[] adjTiles = GetAdjacentTiles(tile);
            foreach (var adjTile in adjTiles) {
                if (adjTile != start)
                    tiles.Add(adjTile);
            }
        }

        return new List<Vector3Int>(tiles);
    }

    public static List<Vector3Int> TilesInDirection(Vector3Int start, int dir, int dist, bool pierce, RangeDefined.IsTileValidDel IsTileValid) {
        if (IsTileValid == null) {
            IsTileValid = CanStandOn;
        }

        List<Vector3Int> tiles = new List<Vector3Int>();
        Vector3Int current = start;
        for (int i = 0; i < dist; ++i) {
            Vector3Int next = GetTileInDirection(current, dir);
            if (IsTileValid == CanAttackThrough)
                tiles.Add(next);
            if (!IsTileValid(next) && !pierce)
                break;
            if (IsTileValid != CanAttackThrough)
                tiles.Add(next);

            current = next;
        }

        return tiles;
    }

    public static List<Vector3Int> TilesInDirection(Vector3Int start, int dir, int dist, bool pierce) {
        return TilesInDirection(start, dir, dist, pierce, null);
    }

    public static List<Vector3Int> TilesInDirection(Vector3Int start, int dir, int dist) { //i can pierce :p
        return TilesInDirection(start, dir, dist, true, null);
    }


    /*
    //THIS CODE NEEDS TO BE FIXED AT SOME POINT
    public static List<Vector3Int> TilesInDistance(Vector3Int start, int dist) { //doesnt work;
        return new List<Vector3Int>();

        Dictionary<Vector3Int, bool> visited = new Dictionary<Vector3Int, bool>();
        GetTilesInRangeRek(start, range);

        void GetTilesInRangeRek(Vector3Int v, int left) {
            visited[v] = true;
            if (left == 0)
                return;
            Vector3Int[] adjs = GetAdjacentTiles(v);
            for (int i = 0; i < adjs.Length; ++i) {
                if (!visited.ContainsKey(adjs[i])) {
                    GetTilesInRangeRek(adjs[i], left - 1);
                }
            }
        }

        List<Vector3Int> res = new List<Vector3Int>();
        foreach (var x in visited.Keys) {
            if (x != start)
                res.Add(x);
        }

        return res;
    }
    */

    public static Vector3Int[] GetNearbyTiles(Vector3Int tile) {
        Vector3Int[] nearbyTiles = new Vector3Int[18];
        Vector3Int[] adjTiles = GetAdjacentTiles(tile);

        for (int i = 0; i < 6; ++i)
            nearbyTiles[i] = adjTiles[i];

        int prvTile = -1, dir = 0;
        for (int i = 6; i < 18; ++i) {
            prvTile += i % 2 == 0 ? 1 : 0; dir += i % 2 == 1 ? 1 : 0;
            nearbyTiles[i] = GetTileInDirection(nearbyTiles[prvTile], dir % 6); //dont ask, it works
        }
        return nearbyTiles;
    }

    public static List<Vector3Int> ChargeTilesInDistance(Vector3Int start, int crange) {
        List<Vector3Int> result = new List<Vector3Int>();
        for (int i = 0; i < 6; ++i) {
            Vector3Int current = GetChargeTile(start, i, crange);
            if(current != start)
                result.Add(current);
        }

        return result;
    }

    public static Vector3Int GetChargeTile(Vector3Int start, int dir, int crange, ref int acDist) { //also "returns" the actual distance
        acDist = 0;
        Vector3Int res = start;
        while (crange > 0 && CanStandOn(GetTileInDirection(res, dir))) {
            crange--;
            res = GetTileInDirection(res, dir);
            acDist++;
        }
        return res;
    }

    public static Vector3Int GetChargeTile(Vector3Int start, int dir, int crange) {
        int temp = 0;
        return GetChargeTile(start, dir, crange, ref temp);
    }

    public static Vector3Int GetShootTile(Vector3Int start, int dir, int crange) {
        Vector3Int res = start;
        while (crange > 0 && CanAttackThrough(GetTileInDirection(res, dir))) {
            crange--;
            res = GetTileInDirection(res, dir);
        }
        return res;
    }

    public static List<Vector3Int> AllStandableTiles(PlayerManager pm) {
        List<Vector3Int> tiles = new List<Vector3Int>();

        for (int x = groundTilemap.cellBounds.xMin; x < groundTilemap.cellBounds.xMax; x++) {
            for (int y = groundTilemap.cellBounds.yMin; y < groundTilemap.cellBounds.yMax; y++) {
                Vector3Int localPlace = new Vector3Int(x, y, 0);
                if (CanStandOn(localPlace)) {
                    tiles.Add(localPlace);
                }
            }
        }

        return tiles;
    }

    public static Vector3Int[] GetAdjacentTiles(Vector3Int t) {
        Vector3Int[] list = new Vector3Int[6];
        for (int i = 0; i < 6; ++i)
            list[i] = GetTileInDirection(t, i);
        return list;
    }

    public static bool CanAttackThrough(Vector3Int tile) {
        return CanStandOn(tile); //the same for now
    }

    public static bool CanStandOn(Vector3Int tile) {
        List<RaycastHit2D> hits = RaycastTile(tile);
        foreach (var hit in hits) {
            if (hit) {
                ITerrain terrain = hit.transform.parent.GetComponent<ITerrain>();
                if (terrain != null) {
                    return false;
                }
            }
        }

        if (groundTilemap.HasTile(tile))
            return true;
        else
            return false;
    }

    public static bool TileOccupied(Vector3Int tile) {
        List<RaycastHit2D> hits = RaycastTile(tile);
        foreach (var hit in hits) {
            IOccupyTile occupyTile = hit.transform.parent.GetComponent<IOccupyTile>();
            if (occupyTile != null) {
                if (!occupyTile.ignoreIfPlayerNotOwner)
                    return true;

                if (PhotonNetwork.LocalPlayer != occupyTile.photonView.Owner)
                    return false;
                else
                    return true;
            }
        }
        return false;
    }

    

    public static bool TileInList(Vector3Int tile, List<Vector3Int> tiles) {
        foreach (var t in tiles) {
            if (tile == t)
                return true;
        }
        return false;
    }

    public static void SetAllTileFlags() {
        for (int x = groundTilemap.cellBounds.xMin; x < groundTilemap.cellBounds.xMax; x++) {
            for (int y = groundTilemap.cellBounds.yMin; y < groundTilemap.cellBounds.yMax; y++) {
                Vector3Int localPlace = new Vector3Int(x, y, 0);
                groundTilemap.SetTileFlags(localPlace, TileFlags.None);
            }
        }
    }
}
