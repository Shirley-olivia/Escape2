using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxMap : MonoBehaviour
{
    public GameObject Plane;
    public GameObject Player;
    public Texture MapTexture;
    public Texture PlayerTexture;

    private float MaxMapWidth;
    private float MaxMapHeight;
    private float MinMapWidth;
    private float MinMapHeight;
    private float MaxMapRealyWidth;
    private float MaxMapRealyHeight;
    private float PlayerInMapWidth;
    private float PlayerInMapHeight;

    private void Start()
    {
        MaxMapRealyWidth = Plane.GetComponent<MeshFilter>().mesh.bounds.size.x;
        MaxMapRealyHeight = Plane.GetComponent<MeshFilter>().mesh.bounds.size.z;
        //得到大地图高度缩放地理
        float scal_z = Plane.transform.localScale.z;
        MaxMapRealyHeight = MaxMapRealyHeight * scal_z;
        //得到大地图高度缩放地理
        float scal_x = Plane.transform.localScale.x;
        MaxMapRealyWidth = MaxMapRealyWidth * scal_x;
        Check();
    }

    private void FixedUpdate()
    {
        Check();
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(Screen.width - MapTexture.width / 4, 0, MapTexture.width / 4, MapTexture.height / 4), MapTexture);
        GUI.DrawTexture(new Rect(MinMapWidth, MinMapHeight, 20, 20), PlayerTexture);
    }

    void Check()
    {
        //根据比例计算小地图“主角”的坐标
        MinMapWidth = (MapTexture.width * Player.transform.position.x / MaxMapRealyWidth) + ((MapTexture.width / 4 / 2) - (20 / 2)) + (Screen.width - MapTexture.width / 4);
        MinMapHeight = MapTexture.height / 4 - ((MapTexture.height / 4 * Player.transform.position.z / MaxMapRealyHeight) + (MapTexture.height / 4 / 2 + 30));
    }
}
