using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeDefined
{
    public int dist;
    public bool self;
    public bool straight;
    public bool piercing;
    public bool basicRange;
    public List<int> allowedDirections;
    
    public delegate bool IsTileValidDel(Vector3Int tile);
    public IsTileValidDel IsTileValid;

    public RangeDefined(int r, bool s, bool p, bool br, List<int> ad, IsTileValidDel itv, bool slf) {
        dist = r;
        straight = s;
        piercing = p;
        basicRange = br;
        allowedDirections = ad;
        IsTileValid = itv;
        self = slf;
    }

    public RangeDefined(int r, bool s, bool p, bool br, List<int> ad, IsTileValidDel itv) : this(r, s, p, br, ad, itv, false) { }
    public RangeDefined(int r, bool s, bool p, bool br) : this(r, s, p, br, new List<int>(), null, false) { }
    public RangeDefined(int r, bool s, bool p, bool br, bool slf) : this(r, s, p, br, new List<int>(), null, slf) { }
    public RangeDefined(int r, bool s, bool p, bool br, IsTileValidDel itv, bool slf) : this(r, s, p, br, new List<int>(), itv, slf) { }
    public RangeDefined(int r, bool br) : this(r, false, false, br, new List<int>(), null, false) { }
    public RangeDefined(int r, bool s, bool p) : this(r, s, p, false, new List<int>(), null, false) { }
    public RangeDefined(int r, bool s, bool p, IsTileValidDel itv) : this(r, s, p, false, new List<int>(), itv, false) { }
    public RangeDefined(int r) : this(r, false, false, false, new List<int>(), null, false) { }
}
