using TMPro;
using UnityEngine;

[ExecuteAlways]
public class ButtonLabeled : MonoBehaviour
{
    private void Awake() => GetComponentInChildren<TMP_Text>().text = name;
}
