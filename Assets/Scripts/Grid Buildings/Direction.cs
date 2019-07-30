public enum Direction {
    Up, Right, Down, Left
}

public static class DirectionExtensions {
    public static Direction Opposite(this Direction direction) {
        return (int)direction < 2 ? direction + 2 : direction - 2; 
    }

    public static Direction Next(this Direction direction) {
        return (int)direction + 1 > (int)Direction.Left ? Direction.Up : direction + 1;
    }

    public static Direction Previous(this Direction direction) {
        return (int)direction - 1 < (int)Direction.Up ? Direction.Left : direction - 1;
    }
}