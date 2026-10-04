using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Core.Skills
{
    [CreateAssetMenu(menuName = "Game/Skills/Container")]
    public class SkillsInfoContainer : ScriptableObject
    {
        #region Fields

        [SerializeField] private List<SkillInfo> _skills;

        #endregion

        #region Properties

        public IReadOnlyList<SkillInfo> Skills => _skills;

        #endregion
    }
}
