namespace JLL.Components
{
    public enum ColliderType
    {
        Unknown = -1,
        Player = 0,
        Enemy = 1,
        Vehicle = 2,
        Object = 3
    }
    public enum PlayerBone
    {
        Base = 6,
        Neck = 0,
        Spine = 5,
        RightArmUpper = 9,
        LeftArmUpper = 10,
        RightArmLower = 1,
        LeftArmLower = 2,
        RightThigh = 7,
        LeftThigh = 8,
        RightShin = 3,
        LeftShin = 4,
    }
    public enum RotationType
    {
        ObjectRotation,
        RandomRotation,
        NoRotation,
    }
    public enum PowerCap
    {
        None,
        Indoor,
        Daytime,
        Nighttime
    }
    public enum SpawnPoolSource
    {
        CustomList,
        AllItems,
        LevelItems,
        StoreItems,
    }
    public enum ActiveCondition
    {
        None,
        ActiveOutdoors,
        ActiveIndoors,
        ActiveOutdoorsOutsideShip,
    }
    public enum SpawnRotation
    {
        Random,
        FacingWall,
        FacingAwayFromWall,
        BackToWall
    }
    public enum SpawnNodes
    {
        Children,
        OutsideAINodes,
        InsideAINodes,
        AINodes,
    }
    public enum NavMeshToRebake
    {
        None = 0,
        Exterior,
        Custom = -1,
    }
    public enum RandomTeleportRegion
    {
        Indoor,
        Outdoor,
        Moon,
        Nearby,
        RandomPlayer,
    }
    public enum Region
    {
        None,
        Outdoor,
        Indoor,
    }
    public enum TeleportEffect
    {
        None,
        ShipTeleport,
        InverseTeleport,
    }

    public enum PlayerTarget
    {
        Local,
        Closest,
        Farthest,
    }
}
