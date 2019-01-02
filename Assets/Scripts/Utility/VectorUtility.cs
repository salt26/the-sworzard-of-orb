using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorUtility {

    /// <summary>
    /// 두 좌표가 같은 위치이면 true, 다른 위치이면 false를 반환합니다.
    /// z값은 비교하지 않고, x, y값을 반올림한 정수 값이 각각 같은지 비교합니다.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool IsSamePosition(Vector3 a, Vector3 b)
    {
        return (Mathf.RoundToInt(a.x) == Mathf.RoundToInt(b.x)
            && Mathf.RoundToInt(a.y) == Mathf.RoundToInt(b.y));
    }

    /// <summary>
    /// 인자로 주어진 좌표의 x, y값을 정수로 반올림하여 반환합니다.
    /// z값은 그대로 반환됩니다.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Vector3 PositionToInt(Vector3 position)
    {
        return new Vector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), position.z);
    }

    /// <summary>
    /// 인자로 주어진 좌표의 x, y값을 정수로 반올림하여 반환합니다.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Vector2 PositionToInt(Vector2 position)
    {
        return new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    /// <summary>
    /// 두 위치 사이의 택시 거리를 반환합니다.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int ManhattanDistance(Vector3 a, Vector3 b)
    {
        return Mathf.RoundToInt(Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y));
    }

    /// <summary>
    /// 두 위치 사이의 체스보드 거리를 반환합니다.
    /// 체스에서 킹이 이동할 때 걸리는 최소 이동 횟수와 같습니다.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int ChebyshevDistance(Vector3 a, Vector3 b)
    {
        return Mathf.RoundToInt(Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y)));
    }
}
