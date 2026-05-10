using UnityEngine;
[CreateAssetMenu(menuName = "Horror Game/Guidebook Page")]
public class GuidebookPage : ScriptableObject
{
    public string pageTitle;
    [TextArea(5, 15)]
    public string pageText;
    public Sprite pageImage;
}
