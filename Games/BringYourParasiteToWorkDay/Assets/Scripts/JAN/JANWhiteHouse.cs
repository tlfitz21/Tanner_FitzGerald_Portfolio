using UnityEngine;
using UnityEngine.Rendering;

// Builds the miniature West Wing from cubes the first time JanitorScene plays.
// North is +Z. The janitor starts in the lobby facing the north door.
// A ramp on the east climbs to a mezzanine, a second ramp drops back into the
// west-wing corridor, and that corridor ends at the Oval Office.
// A west annex and an east wing loop back into the same corridor.
public static class JANWhiteHouse
{
    static readonly Color Wall = new Color(0.83f, 0.79f, 0.71f);
    static readonly Color Wood = new Color(0.46f, 0.29f, 0.16f);
    static readonly Color CarpetRed = new Color(0.55f, 0.12f, 0.14f);
    static readonly Color CarpetBlue = new Color(0.16f, 0.24f, 0.48f);
    static readonly Color Cream = new Color(0.91f, 0.87f, 0.73f);
    static readonly Color Gold = new Color(0.74f, 0.58f, 0.22f);
    static readonly Color Desk = new Color(0.36f, 0.22f, 0.12f);
    static readonly Color Cabinet = new Color(0.24f, 0.33f, 0.28f);

    const float WallThick = 0.45f;
    public const float Span = 0.85f;
    const float GroundTop = 5.6f;
    const float Ceiling = 5.9f;
    const float Loft = 6.2f;
    const float UpperTop = 10f;
    const float UpperCeiling = 10.3f;

    public static void Build(Transform root)
    {
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.42f, 0.39f, 0.35f);

        GameObject placed = GameObject.Find("White House");
        Transform house = placed != null ? placed.transform : null;
        if (house == null)
        {
            house = new GameObject("White House").transform;
            house.SetParent(root, false);
            BuildMap(house);
        }

        Populate(house);
    }

    public static void BuildMap(Transform house)
    {
        Lobby(house);
        WestWing(house);
        WestAnnex(house);
        EastWing(house);
        UpperFloor(house);
        OvalOffice(house);
    }

    static void Lobby(Transform house)
    {
        Slab("Lobby Floor", -32f, 32f, 0f, 48f, 0f, 0.35f, CarpetRed, house);
        Slab("Lobby Ceiling", -32f, 16f, 0f, 48f, Ceiling, 0.3f, Wall, house);
        Slab("East Bay Ceiling", 16f, 32f, 0f, 22f, Ceiling, 0.3f, Wall, house);
        Slab("Stairwell Ceiling", 16f, 32f, 10f, 36f, UpperCeiling, 0.3f, Wall, house);
        WallX("South Wall", -32f, 32f, 0f, 0f, GroundTop, Wall, house);
        WallZ("West Wall", -32f, 0f, 18f, 0f, GroundTop, Wall, house);
        WallZ("West Wall", -32f, 26f, 48f, 0f, GroundTop, Wall, house);
        WallZ("East Wall Upper", 32f, 0f, 128f, GroundTop, UpperTop, Wall, house);
        WallZ("East Wall", 32f, 0f, 40f, 0f, GroundTop, Wall, house);
        WallZ("East Wall", 32f, 46f, 80f, 0f, GroundTop, Wall, house);
        WallZ("East Wall", 32f, 88f, 114f, 0f, GroundTop, Wall, house);
        WallZ("East Wall", 32f, 122f, 128f, 0f, GroundTop, Wall, house);
        WallX("North Wall Left", -32f, -5f, 48f, 0f, GroundTop, Wall, house);
        WallX("North Wall Right", 5f, 16f, 48f, 0f, GroundTop, Wall, house);
        WallX("North Wall Upper", 16f, 32f, 48f, 0f, 5.8f, Wall, house);
        Ramp("Grand Stair", new Vector3(24f, 0.2f, 12f), new Vector3(24f, Loft, 36f), 5.5f, Wood, house);
        float stairSide = 5.5f * 0.5f + (WallThick * 0.5f + 0.05f) / Span;
        WallZ("Stair West", 24f - stairSide, 12f, 36f, 0f, UpperTop, Wall, house);
        WallZ("Stair East", 24f + stairSide, 12f, 36f, 0f, UpperTop, Wall, house);
        Column(-14f, 18f, GroundTop, house);
        Column(14f, 18f, GroundTop, house);
        Column(-14f, 34f, GroundTop, house);
        Column(14f, 34f, GroundTop, house);
        DeskAt(new Vector3(-20f, 0f, 16f), house);
        CabinetAt(new Vector3(8f, 0f, 22f), house);
        Sign("ENTRANCE HALL", new Vector3(0f, 3.2f, 0.4f), 0f, house);
        Lamp(new Vector3(0f, 4.4f, 24f), new Color(1f, 0.93f, 0.82f), 38f, house);
        Lamp(new Vector3(-18f, 4.4f, 10f), new Color(1f, 0.93f, 0.82f), 24f, house);
    }

    static void WestWing(Transform house)
    {
        Slab("Corridor Floor", -8.4f, 8.4f, 47.6f, 128.4f, 0f, 0.35f, CarpetBlue, house);
        Slab("Corridor Ceiling", -8.4f, 8.4f, 48f, 128f, Ceiling, 0.3f, Wall, house);

        WallZ("Corridor West", -8f, 48f, 60f, 0f, GroundTop, Wall, house);
        WallZ("Corridor West", -8f, 68f, 98f, 0f, GroundTop, Wall, house);
        WallZ("Corridor West", -8f, 106f, 120f, 0f, GroundTop, Wall, house);
        WallZ("Corridor West", -8f, 126f, 128f, 0f, GroundTop, Wall, house);
        WallZ("Corridor East", 8f, 48f, 64f, 0f, GroundTop, Wall, house);
        WallZ("Corridor East", 8f, 76f, 110f, 0f, GroundTop, Wall, house);

        Office("Office A", -32f, -8f, 50f, 78f, 60f, 68f, true, house, 54f, 62f);
        Office("Office B", -32f, -8f, 86f, 120f, 98f, 106f, true, house, 108f, 116f, -26f, -18f);
        Office("Office C", 16f, 32f, 58f, 102f, 66f, 74f, false, house);
        HallToOfficeC(house);
        RampLanding(house);

        DeskAt(new Vector3(0f, 0f, 84f), house);
        CabinetAt(new Vector3(3f, 0f, 92f), house);
        Sign("WEST WING", new Vector3(0f, 3.4f, 47.6f), 180f, house);
        Lamp(new Vector3(0f, 4.6f, 70f), new Color(1f, 0.93f, 0.82f), 30f, house);
        Lamp(new Vector3(0f, 4.6f, 100f), new Color(1f, 0.93f, 0.82f), 28f, house);
        Lamp(new Vector3(0f, 4.6f, 120f), new Color(1f, 0.93f, 0.82f), 24f, house);
    }

    static void Office(string name, float x0, float x1, float z0, float z1, float doorZ0, float doorZ1, bool doorOnEast, Transform house, float farDoorZ0 = 0f, float farDoorZ1 = 0f, float northDoorX0 = 0f, float northDoorX1 = 0f)
    {
        Slab(name + " Floor", x0, x1, z0, z1, 0f, 0.35f, Wood, house);
        float wallTop = doorOnEast ? GroundTop : 5.75f;
        if (doorOnEast)
            Slab(name + " Ceiling", x0, x1, z0, z1, Ceiling, 0.3f, Wall, house);

        float west = Mathf.Min(x0, x1);
        float east = Mathf.Max(x0, x1);
        if (doorOnEast)
        {
            if (farDoorZ1 > farDoorZ0)
            {
                WallZ(name + " West", west, z0, farDoorZ0, 0f, wallTop, Wall, house);
                WallZ(name + " West", west, farDoorZ1, z1, 0f, wallTop, Wall, house);
            }
            else
                WallZ(name + " West", west, z0, z1, 0f, wallTop, Wall, house);
        }

        float doorX = doorOnEast ? east : west;
        WallZ(name + " Door Side", doorX, z0, doorZ0, 0f, wallTop, Wall, house);
        WallZ(name + " Door Side", doorX, doorZ1, z1, 0f, wallTop, Wall, house);
        WallX(name + " South", x0, x1, z0, 0f, wallTop, Wall, house);
        if (northDoorX1 > northDoorX0)
        {
            WallX(name + " North", x0, northDoorX0, z1, 0f, wallTop, Wall, house);
            WallX(name + " North", northDoorX1, x1, z1, 0f, wallTop, Wall, house);
        }
        else
            WallX(name + " North", x0, x1, z1, 0f, wallTop, Wall, house);
        Lamp(new Vector3((x0 + x1) * 0.5f, 4.2f, (z0 + z1) * 0.5f), new Color(1f, 0.9f, 0.75f), 26f, house);
    }

    static void WestAnnex(Transform house)
    {
        Slab("Press Floor", -50f, -32f, 8f, 48f, 0f, 0.35f, Wood, house);
        Slab("Press Ceiling", -50f, -32f, 8f, 48f, Ceiling, 0.3f, Wall, house);
        WallZ("Press West", -50f, 8f, 48f, 0f, GroundTop, Wall, house);
        WallX("Press South", -50f, -32f, 8f, 0f, GroundTop, Wall, house);
        WallX("Press North", -50f, -44f, 48f, 0f, GroundTop, Wall, house);
        WallX("Press North", -36f, -32f, 48f, 0f, GroundTop, Wall, house);
        DeskAt(new Vector3(-42f, 0f, 18f), house);
        Sign("PRESS ROOM", new Vector3(-41f, 3.3f, 10f), 0f, house);
        Lamp(new Vector3(-41f, 4.4f, 30f), new Color(1f, 0.93f, 0.82f), 26f, house);

        Slab("Service Floor", -50f, -32f, 48f, 120f, 0f, 0.35f, CarpetBlue, house);
        Slab("Service Ceiling", -50f, -32f, 48f, 120f, Ceiling, 0.3f, Wall, house);
        WallZ("Service West", -50f, 48f, 120f, 0f, GroundTop, Wall, house);
        WallZ("Service East", -32f, 48f, 54f, 0f, GroundTop, Wall, house);
        WallZ("Service East", -32f, 78f, 86f, 0f, GroundTop, Wall, house);
        Sign("WEST GALLERY", new Vector3(-41f, 3.3f, 50f), 0f, house);
        Lamp(new Vector3(-41f, 4.4f, 70f), new Color(1f, 0.93f, 0.82f), 26f, house);
        Lamp(new Vector3(-41f, 4.4f, 104f), new Color(1f, 0.93f, 0.82f), 26f, house);

        Slab("Cross Hall Floor", -50f, -7.6f, 120f, 128f, 0f, 0.35f, CarpetBlue, house);
        Slab("Cross Hall Ceiling", -50f, -7.6f, 120f, 128f, Ceiling, 0.3f, Wall, house);
        WallZ("Cross Hall West", -50f, 120f, 128f, 0f, GroundTop, Wall, house);
        Lamp(new Vector3(-28f, 4.4f, 124f), new Color(1f, 0.93f, 0.82f), 24f, house);

        Slab("Cabinet Floor", -50f, -32f, 128f, 160f, 0f, 0.35f, Cream, house);
        Slab("Cabinet Ceiling", -50f, -32f, 128f, 160f, Ceiling, 0.3f, Wall, house);
        WallZ("Cabinet West", -50f, 128f, 160f, 0f, GroundTop, Wall, house);
        WallX("Cabinet South", -50f, -46f, 128f, 0f, GroundTop, Wall, house);
        WallX("Cabinet South", -38f, -32f, 128f, 0f, GroundTop, Wall, house);
        WallX("Cabinet North", -50f, -32f, 160f, 0f, GroundTop, Wall, house);
        DeskAt(new Vector3(-41f, 0f, 148f), house);
        Sign("CABINET ROOM", new Vector3(-41f, 3.3f, 132f), 0f, house);
        Lamp(new Vector3(-41f, 4.4f, 146f), new Color(1f, 0.93f, 0.82f), 28f, house);
    }

    static void EastWing(Transform house)
    {
        Slab("Map Floor", 32f, 50f, 36f, 56f, 0f, 0.35f, Wood, house);
        Slab("Map Ceiling", 32f, 50f, 36f, 56f, Ceiling, 0.3f, Wall, house);
        WallZ("Map East", 50f, 36f, 56f, 0f, GroundTop, Wall, house);
        WallX("Map South", 32f, 50f, 36f, 0f, GroundTop, Wall, house);
        WallX("Map North", 32f, 44f, 56f, 0f, GroundTop, Wall, house);
        DeskAt(new Vector3(40f, 0f, 46f), house);
        Sign("MAP ROOM", new Vector3(41f, 3.3f, 38f), 0f, house);
        Lamp(new Vector3(41f, 4.4f, 48f), new Color(1f, 0.93f, 0.82f), 22f, house);

        Slab("East Hall Floor", 44f, 50f, 56f, 72f, 0f, 0.35f, CarpetBlue, house);
        Slab("East Hall Ceiling", 44f, 50f, 56f, 72f, Ceiling, 0.3f, Wall, house);
        WallZ("East Hall West", 44f, 56f, 72f, 0f, GroundTop, Wall, house);
        WallZ("East Hall East", 50f, 56f, 72f, 0f, GroundTop, Wall, house);
        Lamp(new Vector3(47f, 4.4f, 64f), new Color(1f, 0.93f, 0.82f), 16f, house);

        Slab("Dining Floor", 32f, 50f, 72f, 100f, 0f, 0.35f, CarpetRed, house);
        Slab("Dining Ceiling", 32f, 50f, 72f, 100f, Ceiling, 0.3f, Wall, house);
        WallZ("Dining East", 50f, 72f, 100f, 0f, GroundTop, Wall, house);
        WallX("Dining South", 32f, 44f, 72f, 0f, GroundTop, Wall, house);
        WallX("Dining North", 32f, 40f, 100f, 0f, GroundTop, Wall, house);
        DeskAt(new Vector3(40f, 0f, 86f), house);
        Sign("STATE DINING", new Vector3(41f, 3.3f, 76f), 0f, house);
        Lamp(new Vector3(41f, 4.4f, 88f), new Color(1f, 0.93f, 0.82f), 28f, house);

        Slab("East Return Floor", 32f, 50f, 100f, 128f, 0f, 0.35f, CarpetBlue, house);
        Slab("East Return Ceiling", 32f, 50f, 100f, 128f, Ceiling, 0.3f, Wall, house);
        WallZ("East Return East", 50f, 100f, 128f, 0f, GroundTop, Wall, house);
        WallX("East Return North", 32f, 50f, 128f, 0f, GroundTop, Wall, house);
        Lamp(new Vector3(42f, 4.4f, 114f), new Color(1f, 0.93f, 0.82f), 24f, house);
    }

    static void HallToOfficeC(Transform house)
    {
        Slab("Side Hall Floor", 7.6f, 16.2f, 62f, 78f, 0f, 0.35f, Wood, house);
        Slab("Side Hall Ceiling", 7.6f, 16.2f, 62f, 78f, Ceiling, 0.3f, Wall, house);
        WallX("Side Hall South", 8f, 16f, 62f, 0f, GroundTop, Wall, house);
        WallX("Side Hall North", 8f, 16f, 78f, 0f, GroundTop, Wall, house);
    }

    static void RampLanding(Transform house)
    {
        Slab("Landing Floor", 8f, 32f, 110f, 128f, 0f, 0.35f, Wood, house);
        Slab("Landing Ceiling West", 8f, 20.5f, 110f, 128f, Ceiling, 0.3f, Wall, house);
        Slab("Landing Ceiling East", 27.5f, 32f, 110f, 128f, Ceiling, 0.3f, Wall, house);
        Slab("Landing Ceiling North", 8f, 32f, 118f, 128f, Ceiling, 0.3f, Wall, house);
        Slab("Ramp Well Ceiling", 20.5f, 27.5f, 110f, 118f, UpperCeiling, 0.3f, Wall, house);
        WallX("Landing South Left", 8f, 21.2f, 110f, 0f, UpperTop, Wall, house);
        WallX("Landing South Right", 26.8f, 32f, 110f, 0f, UpperTop, Wall, house);
    }

    static void UpperFloor(Transform house)
    {
        Slab("Mezzanine", 16f, 32f, 36f, 104f, Loft, 0.4f, CarpetBlue, house);
        Slab("Mezzanine North West", 16f, 21.2f, 104f, 106f, Loft, 0.4f, CarpetBlue, house);
        Slab("Mezzanine North East", 26.8f, 32f, 104f, 106f, Loft, 0.4f, CarpetBlue, house);
        Slab("Mezzanine Ceiling", 16f, 32f, 36f, 106f, UpperCeiling, 0.3f, Wall, house);
        WallZ("Mezzanine West", 16f, 36f, 106f, Loft, UpperTop, Wall, house);
        WallX("Overlook Rail Left", 16f, 21f, 36.2f, Loft, Loft + 0.95f, Gold, house);
        WallX("Overlook Rail Right", 27f, 32f, 36.2f, Loft, Loft + 0.95f, Gold, house);
        WallX("Mezzanine North Left", 16f, 21.2f, 106f, Loft, UpperTop, Wall, house);
        WallX("Mezzanine North Right", 26.8f, 32f, 106f, Loft, UpperTop, Wall, house);
        Ramp("Down Ramp", new Vector3(24f, Loft, 104f), new Vector3(24f, 0.15f, 122f), 5.6f, Wood, house);
        float rampSide = 5.6f * 0.5f + (WallThick * 0.5f + 0.05f) / Span;
        WallZ("Ramp West", 24f - rampSide, 104f, 122f, 0f, UpperTop, Wall, house);
        WallZ("Ramp East", 24f + rampSide, 104f, 122f, 0f, UpperTop, Wall, house);
        CabinetAt(new Vector3(29f, Loft, 70f), house);
        DeskAt(new Vector3(22f, Loft, 84f), house);
        Lamp(new Vector3(24f, 9.2f, 55f), new Color(1f, 0.93f, 0.8f), 28f, house);
        Lamp(new Vector3(20f, 9.2f, 88f), new Color(1f, 0.93f, 0.8f), 28f, house);
        LoftRoom(house);
    }

    static void LoftRoom(Transform house)
    {
        WallX("Loft Room South", 24f, 32f, 64f, Loft, UpperTop, Wall, house);
        WallX("Loft Room North", 24f, 32f, 92f, Loft, UpperTop, Wall, house);
        WallZ("Loft Room West", 24f, 64f, 74f, Loft, UpperTop, Wall, house);
        WallZ("Loft Room West", 24f, 80f, 92f, Loft, UpperTop, Wall, house);
        Sign("LINCOLN", new Vector3(28f, 8.1f, 68f), 0f, house);
        Lamp(new Vector3(28f, 9.2f, 82f), new Color(1f, 0.93f, 0.8f), 18f, house);
    }

    static void OvalOffice(Transform house)
    {
        Slab("Oval Floor", -32f, 32f, 128f, 188f, 0f, 0.35f, Cream, house);
        Slab("Oval Ceiling", -32f, 32f, 128f, 188f, 8.4f, 0.3f, Wall, house);
        WallZ("Oval West", -32f, 128f, 188f, 0f, 8.1f, Wall, house);
        WallZ("Oval East", 32f, 128f, 188f, 0f, 8.1f, Wall, house);
        WallX("Oval North", -32f, 32f, 188f, 0f, 8.1f, Wall, house);
        WallX("Oval South Left", -32f, -8f, 128f, 0f, 8.1f, Wall, house);
        WallX("Oval South Right", 8f, 32f, 128f, 0f, 8.1f, Wall, house);
        Corner(-22f, 142f, house);
        Corner(22f, 142f, house);
        Corner(-22f, 176f, house);
        Corner(22f, 176f, house);
        Pillar(-12f, 150f, house);
        Pillar(12f, 150f, house);
        Pillar(-12f, 168f, house);
        Pillar(12f, 168f, house);
        DeskAt(new Vector3(0f, 0f, 180f), house);
        Sign("OVAL OFFICE", new Vector3(0f, 4.6f, 187.4f), 180f, house);
        Lamp(new Vector3(0f, 6.6f, 146f), new Color(1f, 0.9f, 0.7f), 36f, house);
        Lamp(new Vector3(0f, 6.6f, 170f), new Color(1f, 0.9f, 0.7f), 36f, house);

        GameObject gate = JANArt.Cube("Oval Door", At(0f, 2.8f, 127.7f), new Vector3(17.2f * Span, 5.6f, 0.5f), Wood, house);
    }

    static void Populate(Transform house)
    {
        JANPlayerController player = JANPlayerController.Create(At(0f, 0.2f, 8f));
        JANGameManager.Instance.BindPlayer(player);

        GameObject trigger = new GameObject("Oval Seal");
        trigger.transform.SetParent(house, false);
        JANOvalSeal seal = trigger.AddComponent<JANOvalSeal>();
        seal.gate = GameObject.Find("Oval Door");
        PJAController boss = PJAController.Create(At(0f, 0.2f, 158f));
        seal.boss = boss;
        boss.SetOfficePodium(PJAController.CreateOfficePodium(At(0f, 0.2f, 174f)));

        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(-18f, 0.2f, 26f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(6f, 0.2f, 38f));

        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(0f, 0.2f, 62f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Ranged, At(1f, 0.2f, 90f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(-2f, 0.2f, 112f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(2f, 0.2f, 122f));

        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(-22f, 0.2f, 58f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Ranged, At(-18f, 0.2f, 72f));
        DeskAt(new Vector3(-20f, 0f, 66f), house);
        CabinetAt(new Vector3(-26f, 0f, 74f), house);

        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(-24f, 0.2f, 94f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(-16f, 0.2f, 108f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Ranged, At(-22f, 0.2f, 116f));
        DeskAt(new Vector3(-18f, 0f, 100f), house);
        CabinetAt(new Vector3(-26f, 0f, 112f), house);

        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(26f, 0.2f, 64f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Ranged, At(24f, 0.2f, 90f));
        DeskAt(new Vector3(24f, 0f, 80f), house);
        CabinetAt(new Vector3(28f, 0f, 96f), house);

        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(24f, 6.45f, 58f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Ranged, At(22f, 6.45f, 76f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Ranged, At(28f, 6.45f, 94f));

        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(-24f, 0.2f, 42f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(-2f, 0.2f, 78f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(-28f, 0.2f, 70f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(20f, 0.2f, 96f));

        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(-44f, 0.2f, 32f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Ranged, At(-46f, 0.2f, 40f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(-42f, 0.2f, 68f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Ranged, At(-46f, 0.2f, 100f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(-40f, 0.2f, 124f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(-44f, 0.2f, 136f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Ranged, At(-46f, 0.2f, 154f));

        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(36f, 0.2f, 52f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Ranged, At(46f, 0.2f, 42f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(47f, 0.2f, 64f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(46f, 0.2f, 78f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Ranged, At(44f, 0.2f, 96f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(42f, 0.2f, 114f));

        JANEnemyAI.Create(JANEnemyAI.Kind.Melee, At(28f, 6.45f, 78f));
        JANEnemyAI.Create(JANEnemyAI.Kind.Ranged, At(29f, 6.45f, 86f));

        JANPickup.Create(JANPickup.Kind.Armor, At(-18f, 0.85f, 12f), 45f);
        JANPickup.Create(JANPickup.Kind.Health, At(-46f, 0.85f, 28f), 30f);
        JANPickup.Create(JANPickup.Kind.Ammo, At(-44f, 0.85f, 88f), 22f);
        JANPickup.Create(JANPickup.Kind.Armor, At(-38f, 0.85f, 156f), 45f);
        JANPickup.Create(JANPickup.Kind.Ammo, At(46f, 0.85f, 52f), 22f);
        JANPickup.Create(JANPickup.Kind.Health, At(46f, 0.85f, 92f), 30f);
        JANPickup.Create(JANPickup.Kind.Ammo, At(10f, 0.85f, 14f), 22f);
        JANPickup.Create(JANPickup.Kind.Health, At(-4f, 0.85f, 42f), 30f);
        JANPickup.Create(JANPickup.Kind.Ammo, At(4f, 0.85f, 58f), 22f);
        JANPickup.Create(JANPickup.Kind.Health, At(-26f, 0.85f, 114f), 30f);
        JANPickup.Create(JANPickup.Kind.Ammo, At(28f, 0.85f, 86f), 22f);
        JANPickup.Create(JANPickup.Kind.Ammo, At(26f, 7.05f, 66f), 22f);
        JANPickup.Create(JANPickup.Kind.Health, At(-4f, 0.85f, 118f), 30f);
        JANPickup.Create(JANPickup.Kind.Armor, At(16f, 0.85f, 150f), 50f);
        JANPickup.Create(JANPickup.Kind.Ammo, At(-14f, 0.85f, 178f), 24f);
    }

    public static float Across(float value)
    {
        return value * Span;
    }

    static Vector3 At(float x, float y, float z)
    {
        return new Vector3(x * Span, y, z * Span);
    }

    static void Corner(float x, float z, Transform house)
    {
        GameObject block = JANArt.Cube("Oval Corner", At(x, 4.05f, z), new Vector3(6.2f, 8.1f, 6.2f), Wall, house);
        block.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
    }

    static void Pillar(float x, float z, Transform house)
    {
        JANArt.Cube("Pillar", At(x, 4.05f, z), new Vector3(1.35f, 8.1f, 1.35f), Gold, house);
    }

    static void Column(float x, float z, float height, Transform house)
    {
        JANArt.Cube("Column", At(x, height * 0.5f, z), new Vector3(1.2f, height, 1.2f), Wall, house);
    }

    static void DeskAt(Vector3 feet, Transform house)
    {
        feet = At(feet.x, feet.y, feet.z);
        JANArt.Cube("Desk", feet + new Vector3(0f, 0.62f, 0f), new Vector3(2.6f, 0.9f, 1.25f), Desk, house);
        JANArt.Cube("Desktop", feet + new Vector3(0f, 1.12f, 0f), new Vector3(2.8f, 0.1f, 1.4f), Desk, house);
    }

    static void CabinetAt(Vector3 feet, Transform house)
    {
        feet = At(feet.x, feet.y, feet.z);
        JANArt.Cube("Filing Cabinet", feet + new Vector3(0f, 1.15f, 0f), new Vector3(1.05f, 2.3f, 0.8f), Cabinet, house);
    }

    static void Slab(string name, float x0, float x1, float z0, float z1, float top, float thick, Color color, Transform parent)
    {
        Vector3 position = new Vector3((x0 + x1) * 0.5f * Span, top - thick * 0.5f, (z0 + z1) * 0.5f * Span);
        Vector3 scale = new Vector3(Mathf.Abs(x1 - x0) * Span, thick, Mathf.Abs(z1 - z0) * Span);
        JANArt.Cube(name, position, scale, color, parent);
    }

    static void WallX(string name, float x0, float x1, float z, float y0, float y1, Color color, Transform parent)
    {
        if (Mathf.Abs(x1 - x0) < 0.05f || y1 - y0 < 0.05f)
            return;

        Vector3 position = new Vector3((x0 + x1) * 0.5f * Span, (y0 + y1) * 0.5f, z * Span);
        JANArt.Cube(name, position, new Vector3(Mathf.Abs(x1 - x0) * Span, y1 - y0, WallThick), color, parent);
    }

    static void WallZ(string name, float x, float z0, float z1, float y0, float y1, Color color, Transform parent)
    {
        if (Mathf.Abs(z1 - z0) < 0.05f || y1 - y0 < 0.05f)
            return;

        Vector3 position = new Vector3(x * Span, (y0 + y1) * 0.5f, (z0 + z1) * 0.5f * Span);
        JANArt.Cube(name, position, new Vector3(WallThick, y1 - y0, Mathf.Abs(z1 - z0) * Span), color, parent);
    }

    static void Ramp(string name, Vector3 topStart, Vector3 topEnd, float width, Color color, Transform parent)
    {
        topStart = new Vector3(topStart.x * Span, topStart.y, topStart.z * Span);
        topEnd = new Vector3(topEnd.x * Span, topEnd.y, topEnd.z * Span);
        width *= Span;
        Vector3 delta = topEnd - topStart;
        Vector3 flat = new Vector3(delta.x, 0f, delta.z);
        float flatLength = flat.magnitude;
        if (flatLength < 0.01f)
            return;

        float angle = Mathf.Atan2(delta.y, flatLength) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.LookRotation(flat.normalized, Vector3.up) * Quaternion.Euler(-angle, 0f, 0f);
        const float thick = 0.38f;
        Vector3 center = (topStart + topEnd) * 0.5f + rotation * (Vector3.down * thick * 0.5f);
        GameObject ramp = JANArt.Cube(name, center, new Vector3(width, thick, delta.magnitude), color, parent);
        ramp.transform.rotation = rotation;
    }

    static void Lamp(Vector3 position, Color color, float range, Transform parent)
    {
        GameObject lamp = new GameObject("Lamp");
        lamp.transform.SetParent(parent, false);
        lamp.transform.position = new Vector3(position.x * Span, position.y, position.z * Span);
        Light light = lamp.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.range = range;
        light.intensity = 2.4f;
        light.shadows = LightShadows.None;
    }

    static void Sign(string message, Vector3 position, float yaw, Transform parent)
    {
        GameObject sign = new GameObject("Sign");
        sign.transform.SetParent(parent, false);
        sign.transform.position = new Vector3(position.x * Span, position.y, position.z * Span);
        sign.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        TextMesh text = sign.AddComponent<TextMesh>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = message;
        text.characterSize = 0.28f;
        text.fontSize = 72;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.color = new Color(0.22f, 0.13f, 0.06f);
    }
}

public class JANOvalSeal : MonoBehaviour
{
    public PJAController boss;
    public GameObject gate;
    bool opened;
    bool sawAgents;

    void Start()
    {
        if (gate != null)
            gate.SetActive(true);
    }

    void Update()
    {
        JANGameManager game = JANGameManager.Instance;
        if (game == null)
            return;

        if (game.AgentsRemaining > 0)
            sawAgents = true;

        if (!opened && sawAgents && game.AgentsRemaining == 0)
        {
            opened = true;
            if (gate != null)
                gate.SetActive(false);
            game.ShowBanner("OVAL OFFICE DOOR IS OPEN");
        }

        if (!opened || boss == null || boss.IsAggro)
            return;

        JANPlayerController player = game.Player;
        if (player != null && player.transform.position.z >= JANWhiteHouse.Across(134f))
            boss.Aggro();
    }
}
