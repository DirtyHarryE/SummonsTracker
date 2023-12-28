using SummonsTracker.Characters;
using SummonsTracker.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SummonsTracker.UI
{
    [ExecuteInEditMode]
    public class StatBox : MonoBehaviour
    {
        [System.Serializable]
        public class UnityFloatEvent : UnityEvent<float> { }

        public enum StatBoxType
        {
            ScoreOnly,
            ScoreAndMod,
            ModOnly
        }

        public int Score
        {
            get => _score;
            set
            {
                var newScore = _positiveOnly ? Mathf.Max(0, value) : value;
                if (_score != newScore)
                {
                    _score = newScore;
                    UpdateUI();
                    OnScoreChanged?.Invoke(newScore);
                }
            }
        }

        public void UpdateUI()
        {
            if (_downArrow != null)
            {
                _downArrow.interactable = !_positiveOnly || _score > 0;
            }
            if (_scoreText != null)
            {
                _scoreText.text = _statBoxType == StatBoxType.ScoreOnly || _statBoxType == StatBoxType.ScoreAndMod
                    ? _score.ToString()
                    : TextUtils.AddPlus(_score);
            }
            if (_modText != null)
            {
                _modText.text = _statBoxType == StatBoxType.ScoreAndMod
                    ? TextUtils.AddPlus(CharacterHelper.GetMod(_score))
                    : string.Empty;
            }
        }

        public void Increase(int increase)
        {
            Score = _score + increase;
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                Score = _score;
            }
        }

        [SerializeField]
        private StatBoxType _statBoxType;
        [SerializeField]
        private bool _positiveOnly;
        [SerializeField]
        private int _score = 10;
        [Space]
        [SerializeField]
        private TextMeshProUGUI _scoreText;
        [SerializeField]
        private TextMeshProUGUI _modText;
        [SerializeField]
        private Button _upArrow;
        [SerializeField]
        private Button _downArrow;

        public UnityFloatEvent OnScoreChanged;


        public static implicit operator int (StatBox statBox) => statBox.Score;
    }
}