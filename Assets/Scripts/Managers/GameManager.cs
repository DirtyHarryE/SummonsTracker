using SummonsTracker.Characters;
using SummonsTracker.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SummonsTracker.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Screens")]
        public MainPanel MainScene;
        public StatPanel StatScene;
        public SpellsPanel SelectSpellPanel;
        public CastSpellPanel CastSpellPanel;
        public SelectAttackPanel AttackPanel;
        public AttackRollPanel AttackRollPanel;
        public CharacterPanel CharacterPanel;

        public NumberInput ButtonInput;

        private void OnEnable()
        {
            Instance = this;
        }
        private void OnDisable()
        {
            Instance = null;
        }

        private void Awake()
        {
        }
        private void Start()
        {
            var panels = Object.FindObjectsOfType<Panel>(true);
            if (panels.Any())
            {
                panels.OrderBy(p => p.Layer).ThenByDescending(p => p.Priority).First().Open();
            }
        }
    }
}