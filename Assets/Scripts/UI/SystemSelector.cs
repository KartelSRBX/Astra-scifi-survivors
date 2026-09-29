using UnityEngine;
using UnityEngine.UI;
using GamingApp.Meta;
using GamingApp.Weapons;

namespace GamingApp.UI
{
    /// <summary>Menu showcase carousel: star system art, threat clearance, lock state; START follows the choice.</summary>
    public class SystemSelector : MonoBehaviour
    {
        [SerializeField] Button prevButton;
        [SerializeField] Button nextButton;
        [SerializeField] Button startButton;
        [SerializeField] Image systemImage;
        [SerializeField] Image glow;
        [SerializeField] Image nebulaA;
        [SerializeField] Image nebulaB;
        [SerializeField] Text numberText;
        [SerializeField] Text nameText;
        [SerializeField] Text bestText;
        [SerializeField] Text threatText;
        [SerializeField] Text clearanceText;
        [SerializeField] RectTransform clearanceFill;
        [SerializeField] float clearanceWidth = 740f;
        [SerializeField] GameObject lockGroup;
        [SerializeField] Text lockText;

        GameDatabase db;
        int index;

        void Awake()
        {
            db = GameDatabase.Get();
            index = Mathf.Clamp(RunSetup.SystemIndex, 0, db.systems.Count - 1);
            prevButton.onClick.AddListener(() => Show(index - 1));
            nextButton.onClick.AddListener(() => Show(index + 1));
            Profile.Changed += Refresh;
            Show(index);
        }

        void OnDestroy() => Profile.Changed -= Refresh;

        void Show(int i)
        {
            index = Mathf.Clamp(i, 0, db.systems.Count - 1);
            if (Profile.IsSystemUnlocked(index)) RunSetup.SystemIndex = index;
            Refresh();
        }

        void Refresh()
        {
            var s = db.systems[index];
            bool unlocked = Profile.IsSystemUnlocked(index);
            float clear = Profile.SystemClearance(index);
            float best = Profile.SystemBest(index);

            systemImage.sprite = s.systemSprite;
            systemImage.color = unlocked ? Color.white : new Color(0.35f, 0.35f, 0.45f, 1f);
            var g = s.starColor; g.a = 0.5f;
            glow.color = g;
            nebulaA.sprite = s.nebulaA;
            nebulaB.sprite = s.nebulaB;

            numberText.text = $"SYSTEM {s.Number}";
            numberText.color = s.starColor;
            nameText.text = s.displayName.ToUpper();
            bestText.text = best > 0f ? $"{(int)best / 60:00}:{(int)best % 60:00}" : "--:--";
            threatText.text = $"+{(s.Difficulty - 1f) * 100f:0}%";
            clearanceText.text = clear >= 1f ? "THREAT CLEARED" : $"THREAT CLEARED  {clear * 100f:0}%";
            clearanceText.color = clear >= 1f ? UiTheme.Teal : UiTheme.SoftText;
            clearanceFill.sizeDelta = new Vector2(Mathf.Max(24f, clearanceWidth * clear), clearanceFill.sizeDelta.y);
            clearanceFill.gameObject.SetActive(clear > 0f);

            lockGroup.SetActive(!unlocked);
            if (!unlocked) lockText.text = $"Clear {db.systems[index - 1].displayName} to unlock";
            startButton.interactable = unlocked;
            prevButton.interactable = index > 0;
            nextButton.interactable = index < db.systems.Count - 1;
        }
    }
}
