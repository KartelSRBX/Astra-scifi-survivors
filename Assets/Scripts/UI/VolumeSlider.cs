using UnityEngine;
using UnityEngine.UI;

namespace GamingApp.UI
{
    /// <summary>Binds a slider and label to the saved master volume.</summary>
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] Slider slider;
        [SerializeField] Text label;

        void Awake()
        {
            slider.SetValueWithoutNotify(GameSettings.Volume);
            Refresh(slider.value);
            slider.onValueChanged.AddListener(v => { GameSettings.Volume = v; Refresh(v); });
        }

        void Refresh(float v) => label.text = $"Volume {Mathf.RoundToInt(v * 100f)}%"; // TODO: localize
    }
}
