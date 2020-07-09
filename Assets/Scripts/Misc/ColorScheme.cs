using UnityEngine;

[CreateAssetMenu(fileName = "New Color Scheme", menuName = "Create Color Scheme")]
public class ColorScheme : ScriptableObject
{
    public int price;
    public bool purchased;
    public AnimationClip previewClip;
    public Sprite[] previewSprites;
    public RuntimeAnimatorController animatorController;
}
