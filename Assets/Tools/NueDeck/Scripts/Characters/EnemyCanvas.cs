using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NueGames.NueDeck.Scripts.Characters
{
    public class EnemyCanvas : CharacterCanvas
    {
        [Header("Enemy Canvas Settings")]
        [SerializeField] private Image intentImage;
        [SerializeField] private TextMeshProUGUI nextActionValueText;
        public Image IntentImage => intentImage;
        public TextMeshProUGUI NextActionValueText => nextActionValueText;

        private Coroutine _highlightCR = null;

        private Image _attackIcon = null;

        private void Awake()
        {
            _attackIcon = currentAttackText.transform.parent.GetComponent<Image>();
        }

        public void HighlightAttack(bool active)
        {
            if (active && _highlightCR == null)
                _highlightCR = StartCoroutine(GlowAttackCR());
            else
            {
                if (_highlightCR != null)
                    StopCoroutine(_highlightCR);
                _highlightCR = null;
                _attackIcon.color = Color.white;
            }
        }


        private IEnumerator GlowAttackCR()
        {
            float elapsedTime = 0f;
            float inTime = 0.5f;
            float outTime = 0.6f;

            Color initialColor = Color.white;
            Color targetColor = Color.red;

            while (true)
            {
                elapsedTime = 0;

                while (elapsedTime < inTime)
                {
                    elapsedTime += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsedTime / inTime);

                    _attackIcon.color = Color.Lerp(initialColor, targetColor, t);
                    //currentAttackText.color = Color.Lerp(initialColor, targetColor, t);

                    yield return null;
                }
                elapsedTime = 0f;
                while (elapsedTime < outTime)
                {
                    elapsedTime += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsedTime / outTime);

                    _attackIcon.color = Color.Lerp(targetColor, initialColor, t);
                    //currentAttackText.color = Color.Lerp(targetColor, initialColor, t);

                    yield return null;
                }

            }
        }
    }
}