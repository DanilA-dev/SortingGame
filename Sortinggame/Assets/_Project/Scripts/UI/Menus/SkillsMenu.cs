using System.Collections.Generic;
using _Project.Scripts.Core.Skills;
using _Project.Scripts.UI.Views;
using D_Dev.MenuHandler;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class SkillsMenu : BaseMenu
    {
        #region Fields

        [Title("Data")]
        [SerializeField] private SkillsInfoContainer _skillsContainer;
        [SerializeField] private SkillItemView _skillItemViewPrefab;
        [Title("UI")]
        [SerializeField] private RectTransform _content;

        private List<SkillItemView> _createdSkills;

        #endregion

        #region Monobehaviour

        private void OnEnable() => InitSkills();

        #endregion

        #region Private

        private void InitSkills()
        {
            if (_createdSkills != null)
                return;

            _createdSkills = new();
            for (int i = 0; i < _skillsContainer.Skills.Count; i++)
            {
                var skillView = Instantiate(_skillItemViewPrefab, _content);
                skillView.Init(_skillsContainer.Skills[i], (i + 1).ToString());
                _createdSkills.Add(skillView);
            }
        }

        #endregion
    }
}
