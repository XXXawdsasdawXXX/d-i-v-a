using Code.Enums;
using UnityEngine;

public class PositionService : MonoBehaviour
{
    private const float WIDTH_OFFSET = 64F;
    private const float HEIGHT_OFFSET = 164F;

    private static Camera _camera;

    
    private void Awake()
    {
        _camera = Camera.main;
        var cameraPosition =  ScreenToWorld(new Vector2(Screen.width / 2, Screen.height / 2));
        _camera.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, -10);
    }
    
    public static Vector3 GetPosition(PointAnchor pointAnchor)
    {
        return pointAnchor switch
        {
            PointAnchor.UpperLeft => GetUpperLeftPosition(),
            PointAnchor.UpperCenter => GetUpperCenterPosition(),
            PointAnchor.UpperRight => GetUpperRightPosition(),
            PointAnchor.MiddleLeft => GetMiddleLeftPosition(),
            PointAnchor.MiddleRight => GetMiddleRightPosition(),
            PointAnchor.LowerLeft => GetLowerLeftPosition(),
            PointAnchor.LowerCenter => GetLowerCenterPosition(),
            PointAnchor.LowerRight => GetLowerRightPosition(),
            _ => Vector3.zero
        };
    }
    
    //upper
    private static Vector3 GetUpperCenterPosition() => ScreenToWorld(new Vector2(Screen.width / 2, Screen.height - HEIGHT_OFFSET));
    private  static Vector3 GetUpperLeftPosition() => ScreenToWorld(new Vector2(WIDTH_OFFSET, Screen.height - HEIGHT_OFFSET));
    private static Vector3 GetUpperRightPosition() => ScreenToWorld(new Vector2(Screen.width - WIDTH_OFFSET, Screen.height- HEIGHT_OFFSET));
    
    //middle
    private static Vector3 GetMiddleRightPosition() => ScreenToWorld(new Vector2(Screen.width - WIDTH_OFFSET, Screen.height / 2));
    private static Vector3 GetMiddleLeftPosition() => ScreenToWorld(new Vector2(WIDTH_OFFSET, Screen.height / 2));
    
    //lower
    private static Vector3 GetLowerCenterPosition() => ScreenToWorld(new Vector2(Screen.width / 2, 0));
    private static Vector3 GetLowerLeftPosition() => ScreenToWorld(new Vector2(WIDTH_OFFSET, 0));
    private static Vector3 GetLowerRightPosition() => ScreenToWorld(new Vector2(Screen.width - WIDTH_OFFSET, 0));
    
    
    private static Vector3 ScreenToWorld(Vector2 screenPoint)
    {
        var worldPoint = _camera.ScreenToWorldPoint(screenPoint);
        return new Vector3(worldPoint.x, worldPoint.y, 0);
    }
}