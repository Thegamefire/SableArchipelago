using System.Collections.Generic;
using UnityEngine;

namespace com.thegamefire.sablearchipelago;

partial class AreaBlocker
{
    private static readonly Dictionary<string, HashSet<AreaBlockerLocation>> WallLocations = new()
    {
        ["TerrainChunk -8,-2"] =
        [
            new(Area.Redsee, Area.Hakoa, -3743.7f, -523.8f, 89.4f, 476.6f)
        ],
        ["TerrainChunk -7,-2"] =
        [
            new(Area.Redsee, Area.Hakoa, -3254.3f, -498.4f, 85f, 504.7f)
        ],
        ["TerrainChunk -6,-2"] =
        [
            new(Area.Redsee, Area.Hakoa, -2746.8f, -476.6f, 89.9f, 528.6f)
        ],
        ["TerrainChunk -5,-3"] =
        [
            new(Area.Badlands, Area.Hakoa, -2212.9f, -1104, 16.2f, 269.7f),
            new(Area.Badlands, Area.Hakoa, -2123.8f, -1313.5f, 303, 301.1f),
        ],
        ["TerrainChunk -5,-2"] =
        [
            new(Area.Badlands, Area.Hakoa, -2275.1f, -510.4f, 290.6f, 217f),
            new(Area.Badlands, Area.Hakoa, -2175.3f, -761.2f, 0, 428.4f),
        ],
        ["TerrainChunk -5,-1"] =
        [
            new(Area.Redsee, Area.Hakoa, -2446.4f, -458.3f, 63.3f, 84.1f),
            new(Area.Hakoa, Area.Badlands, -2391.2f, -458.2f, 314.1f, 51.8f),
            new(Area.Redsee, Area.Badlands, -2345.5f, -208.7f, 15.5f, 483f)
        ],
        ["TerrainChunk -5,0"] =
        [
            new(Area.Redsee, Area.Badlands, -2349.2f, 274.1f, 344.8f, 520.5f)
        ],
        ["TerrainChunk -5,1"] =
        [
            new(Area.Redsee, Area.Badlands, -2417.6f, 603f, 0f, 158.1f),
            new(Area.Redsee, Area.Badlands, -2211.5f, 719.2f, 80.3f, 424.9f)
        ],
        ["TerrainChunk -4,-4"] =
        [
            new(Area.Hakoa, Area.Badlands, -1862, -1595.1f, 357, 237.2f),
            new(Area.Hakoa, Area.Badlands, -1688.6f, -1728.4f, 95.8f, 334),
            new(Area.Hakoa, Area.Badlands, -1511.8f, -1862.7f, 354, 238.5f)
        ],
        ["TerrainChunk -4,-3"] =
        [
            new(Area.Hakoa, Area.Badlands, -1932.8f, -1436.7f, 122.3f, 155)
        ],
        ["TerrainChunk -4,1"] =
        [
            new(Area.Redsee, Area.Badlands, -1896f, 890.1f, 38.2f, 346.3f)
        ],
        ["TerrainChunk -4,2"] =
        [
            new(Area.Redsee, Area.Badlands, -1691.2f, 1274.5f, 21.5f, 539.5f)
        ],
        ["TerrainChunk -4,3"] =
        [
            new(Area.Redsee, Area.Badlands, -1546.6f, 1542.2f, 70.8f, 100.2f)
        ],
        ["TerrainChunk -3,-6"] =
        [
            new(Area.Hakoa, Area.Badlands, -1226.5f, -2516.9f, 284.9f, 289.9f),
            new(Area.Hakoa, Area.Badlands, -1046.4f, -2755, 348, 411.1f),
        ],
        ["TerrainChunk -3,-5"] =
        [
            new(Area.Hakoa, Area.Badlands, -1402.5f, -2021.4f, 293.6f, 209.7f),
            new(Area.Hakoa, Area.Badlands, -1337.4f, -2271.5f, 8.1f, 424.5f)
        ],
        ["TerrainChunk -3,3"] =
        [
            new(Area.Redsee, Area.Badlands, -1399.4f, 1544.4f, 278.3f, 209.2f)
        ],
        ["TerrainChunk -3,2"] =
        [
            new(Area.Redsee, Area.Badlands, -1148.7f, 1444.2f, 300f, 341.5f)
        ],
        ["TerrainChunk -2,-9"] =
        [
            new(Area.Hakoa, Area.Badlands, -825.5f, -4091.9f, 9.9f, 237.3f),
            new(Area.Hakoa, Area.Badlands, -771.5f, -4414.6f, 340.2f, 439.7f)
        ],
        ["TerrainChunk -2,-8"] =
        [
            new(Area.Hakoa, Area.Badlands, -819.3f, -3724.9f, 356.8f, 503.8f)
        ],
        ["TerrainChunk -2,-7"] =
        [
            new(Area.Hakoa, Area.Badlands, -918.7f, -3214.6f, 341.8f, 545.9f)
        ],
        ["TerrainChunk -2,1"] =
        [
            new(Area.Sansee, Area.Badlands, -616.2f, 744.2f, 338f, 607.5f)
        ],
        ["TerrainChunk -2,2"] =
        [
            new(Area.Sansee, Area.Badlands, -799.3f, 1157.6f, 332.1f, 300.9f),
            new(Area.Redsee, Area.Badlands, -934.3f, 1323.5f, 118.1f, 152.4f),
            new(Area.Redsee, Area.Sansee, -887f, 1407.6f, 351f, 240f)
        ],
        ["TerrainChunk -2,3"] =
        [
            new(Area.Redsee, Area.Sansee, -881f, 1773.8f, 5f, 509f)
        ],
        ["TerrainChunk -2,4"] =
        [
            new(Area.Redsee, Area.Sansee, -680.7f, 2246.1f, 38.2f, 586f)
        ],
        ["TerrainChunk -2,6"] =
        [
            new(Area.Redsee, Area.Sansee, -520.9f, 3305f, 354f, 437f)
        ],
        ["TerrainChunk -2,7"] =
        [
            new(Area.Redsee, Area.Sansee, -520.1f, 3719.6f, 6.7f, 397.7f)
        ],
        ["TerrainChunk -1,-1"] =
        [
            new(Area.Sansee, Area.Badlands, -167.4f, -154.1f, 316.8f, 494.6f)
        ],
        ["TerrainChunk -1,0"] =
        [
            new(Area.Sansee, Area.Badlands, -419.2f, 244.1f, 339.1f, 468.5f)
        ],
        ["TerrainChunk -1,4"] =
        [
            new(Area.Redsee, Area.Sansee, -478f, 2501.1f, 41.2f, 65.9f)
        ],
        ["TerrainChunk -1,5"] =
        [
            new(Area.Redsee, Area.Sansee, -320f, 2692.3f, 39.4f, 431.9f),
            new(Area.Redsee, Area.Sansee, -281f, 2941.9f, 310.3f, 260f)
        ],
        ["TerrainChunk -1,6"] =
        [
            new(Area.Redsee, Area.Sansee, -440.9f, 3058.2f, 298.7f, 139.9f)
        ],
        ["TerrainChunk -1,7"] =
        [
            new(Area.Redsee, Area.Sansee, -454.6f, 3970.3f, 36.8f, 139.3f)
        ],
        ["TerrainChunk -1,8"] =
        [
            new(Area.Redsee, Area.Sansee, -206.1f, 4134.4f, 61.9f, 476.5f)
        ],
        ["TerrainChunk 0,8"] =
        [
            new(Area.Redsee, Area.Sansee, 250f, 4255.5f, 87.4f, 503.3f)
        ],
        ["TerrainChunk 0,-9"] =
        [
            new(Area.Badlands, Area.TheWash, 149.3f, -4346.7f, 5.5f, 754.4f)
        ],
        ["TerrainChunk 0,-8"] =
        [
            new(Area.Badlands, Area.TheWash, 127.1f, -3833.9f, 337.2f, 302),
            new(Area.Badlands, Area.TheWash, 243.2f, -3583.9f, 57, 413.3f)
        ],
        ["TerrainChunk 0,-7"] =
        [
            new(Area.Badlands, Area.TheWash, 462.1f, -3341.4f, 19.6f, 277.7f)
        ],
        ["TerrainChunk 0,-2"] =
        [
            new(Area.Sansee, Area.Badlands, 325.3f, -631.2f, 311.7f, 468.7f)
        ],
        ["TerrainChunk 0,-1"] =
        [
            new(Area.Sansee, Area.Badlands, 76.4f, -405, 313.5f, 206.6f)
        ],
        ["TerrainChunk 1,-7"] =
        [
            new(Area.Badlands, Area.TheWash, 603.6f, -3069.3f, 33.8f, 342.8f)
        ],
        ["TerrainChunk 1,-6"] =
        [
            new(Area.Badlands, Area.TheWash, 741.7f, -2700, 10.8f, 464.2f)
        ],
        ["TerrainChunk 1,-5"] =
        [
            new(Area.Badlands, Area.TheWash, 893.8f, -2269.3f, 28f, 464.1f)
        ],
        ["TerrainChunk 1,-2"] =
        [
            new(Area.Sansee, Area.Badlands, 749.6f, -831.8f, 280.2f, 509.4f)
        ],
        ["TerrainChunk 1,8"] =
        [
            new(Area.Redsee, Area.Sansee, 751.3f, 4280.3f, 87f, 502.5f)
        ],
        ["TerrainChunk 2,-5"] =
        [
            new(Area.Badlands, Area.TheWash, 1252.7f, -2045.4f, 84.9f, 505.7f)
        ],
        ["TerrainChunk 2,-3"] =
        [
            new(Area.Sansee, Area.Badlands, 1443.2f, -1003.8f, 309.3f, 147.8f)
        ],
        ["TerrainChunk 2,-2"] =
        [
            new(Area.Sansee, Area.Badlands, 1193.5f, -917.1f, 281.8f, 395.2f)
        ],
        ["TerrainChunk 2,8"] =
        [
            new(Area.Redsee, Area.SodicWaste, 1000.1f, 4410.7f, 0f, 234.3f),
            new(Area.Sansee, Area.SodicWaste, 1250.6f, 4287f, 91.5f, 505f)
        ],
        ["TerrainChunk 2,9"] =
        [
            new(Area.Redsee, Area.SodicWaste, 1000.1f, 4777.3f, 0f, 505f)
        ],
        ["TerrainChunk 3,-4"] =
        [
            new(Area.Badlands, Area.TheWash, 1752.3f, -1884.3f, 60.9f, 570.8f)
        ],
        ["TerrainChunk 3,-3"] =
        [
            new(Area.Sansee, Area.Badlands, 1749.8f, -1145.3f, 290.8f, 537.6f)
        ],
        ["TerrainChunk 3,8"] =
        [
            new(Area.Sansee, Area.SodicWaste, 1753.7f, 4185.3f, 110.4f, 545f)
        ],
        ["TerrainChunk 4,-4"] =
        [
            new(Area.Badlands, Area.TheWash, 2255.2f, -1649.3f, 69.2f, 543.9f)
        ],
        ["TerrainChunk 4,-3"] =
        [
            new(Area.Sansee, Area.Badlands, 2274.5f, -1110.2f, 64.6f, 608.7f)
        ],
        ["TerrainChunk 4,7"] =
        [
            new(Area.Sansee, Area.SodicWaste, 2287.2f, 3841.1f, 309.6f, 570.3f)
        ],
        ["TerrainChunk 4,8"] =
        [
            new(Area.Sansee, Area.SodicWaste, 2038.3f, 4056.5f, 318.8f, 91.2f)
        ],
        ["TerrainChunk 5,-2"] =
        [
            new(Area.Sansee, Area.Badlands, 2908.8f, -936.2f, 302.7f, 258.2f),
            new(Area.Sansee, Area.Badlands, 2675.7f, -923.3f, 65.9f, 276.6f)
        ],
        ["TerrainChunk 5,-3"] =
        [
            new(Area.Badlands, Area.TheWash, 2753.5f, -1390.8f, 56.6f, 593.2f)
        ],
        ["TerrainChunk 5,0"] =
        [
            new(Area.Ewer, Area.Sansee, 2806.6f, 511.5f, 287.8f, 137.3f),
        ],
        ["TerrainChunk 5,6"] =
        [
            new(Area.Sansee, Area.SodicWaste, 2822.3f, 3410f, 301.8f, 438.9f)
        ],
        ["TerrainChunk 5,7"] =
        [
            new(Area.Sansee, Area.SodicWaste, 2571.2f, 3592.6f, 316.0f, 187.0f)
        ],
        ["TerrainChunk 6,-3"] =
        [
            new(Area.Badlands, Area.TheWash, 3007.4f, -1117, 4.7f, 226.4f),
            new(Area.Sansee, Area.TheWash, 3339.2f, -924.2f, 76.3f, 668.1f)
        ],
        ["TerrainChunk 6,3"] =
        [
            new(Area.Sansee, Area.SodicWaste, 3459.2f, 1926.0f, 335.6f, 211.1f)
        ],
        ["TerrainChunk 6,4"] =
        [
            new(Area.Sansee, Area.SodicWaste, 3420.2f, 2273.7f, 1f, 505.0f)
        ],
        ["TerrainChunk 6,5"] =
        [
            new(Area.Sansee, Area.SodicWaste, 3319.3f, 2776.2f, 337.2f, 544.3f)
        ],
        ["TerrainChunk 6,6"] =
        [
            new(Area.Sansee, Area.SodicWaste, 3111.1f, 3160.7f, 322.5f, 338.0f)
        ],
        ["TerrainChunk 7,3"] =
        [
            new(Area.Sansee, Area.SodicWaste, 3753.9f, 1720.8f, 113.6f, 550.0f)
        ],
        ["TerrainChunk 8,1"] =
        [
            new(Area.Sansee, Area.SodicWaste, 4413.4f, 968f, 13.0f, 113.5f),
            new(Area.Sansee, Area.SodicWaste, 4451f, 0, 289.3f, 109.3f)
        ],
        ["TerrainChunk 8,2"] =
        [
            new(Area.Sansee, Area.SodicWaste, 4279.8f, 1383.5f, 312.2f, 417.7f),
            new(Area.Sansee, Area.SodicWaste, 4430.1f, 1133.8f, 2.0f, 222.0f)
        ],
        ["TerrainChunk 8,3"] =
        [
            new(Area.Sansee, Area.SodicWaste, 4065.4f, 1567.3f, 306.0f, 148.4f)
        ],
        ["TerrainChunk 9,1"] =
        [
            new(Area.Sansee, Area.SodicWaste, 4712.9f, 851.5f, 97.4f, 425.1f),
        ],
    };
}

public readonly struct AreaBlockerLocation
{
    public readonly Area Area1;
    public readonly Area Area2;
    public readonly float PosX;
    public readonly float PosZ;
    public readonly float Rotation;
    public readonly float Length;

    public AreaBlockerLocation(
        Area area1,
        Area area2,
        float posX,
        float posZ,
        float rotation,
        float length)
    {
        Area1 = area1;
        Area2 = area2;
        PosX = posX;
        PosZ = posZ;
        Rotation = rotation;
        Length = length;
    }
}