using UnityEngine;
using UnityEngine.UI;

public class AudioIcon : MonoBehaviour
{
    private Image image;
    
    public Sprite muteIcon;
    public Sprite audioIcon;

    private void Awake() => image = GetComponent<Image>();

    public void UpdateIcon(float value) => image.sprite = value == 0 ? muteIcon : audioIcon;
}
