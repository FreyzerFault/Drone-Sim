using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AudioIcon : MonoBehaviour
{
    private Image image;
    
    [SerializeField] private Sprite muteIcon;
    [SerializeField] private Sprite audioIcon;

    private void Awake() => image = GetComponent<Image>();

    public void UpdateIcon(float value) => image.sprite = value == 0 ? muteIcon : audioIcon;
}
