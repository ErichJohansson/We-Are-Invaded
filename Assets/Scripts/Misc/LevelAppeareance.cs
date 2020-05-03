using UnityEngine;

[System.Serializable]
public class LevelAppeareance
{
    public Sprite background;
    public Sprite foreground;

    public int bgrLayer;
    public int fgrLayer;

    public Vector3 bgrOffset;
    public Vector3 fgrOffset;
}
