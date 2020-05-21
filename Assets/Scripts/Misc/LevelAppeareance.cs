using UnityEngine;

[System.Serializable]
public class LevelAppeareance
{
    public Sprite background;
    public Sprite foreground;

    public int bgrLayer;
    public int fgrLayer;

    [Range(0, 1f)] public float fgrOpacity;

    public Vector3 bgrOffset = new Vector3(0, 4.21f);
    public Vector3 fgrOffset;

    public GameObject filler;
    public GameObject postFiller;
}
