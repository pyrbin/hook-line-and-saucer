using UnityEngine;
using UnityEngine.UI;

public class SpellBar : MonoBehaviour
{
    public TMPro.TMP_Text Text;
    public Image Image;

    public void SetFishIcon(Sprite sprite)
    {
        Image.sprite = sprite;
    }

    public void SetFishSpellName(string name)
    {
        Text.text = name;
    }
}
