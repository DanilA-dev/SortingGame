using System.Collections.Generic;
using UnityEngine;

namespace D_Dev.TagSystem
{
    public class TagComponent : MonoBehaviour
    {
        #region Fields

        [SerializeField] private List<Tag> _tags;

        private HashSet<Tag> _tagsSet;

        #endregion

        #region Properties

        public IReadOnlyCollection<Tag> Tags => _tagsSet;

        #endregion

        #region Monobehaviour

        private void Awake() => _tagsSet = new HashSet<Tag>(_tags);

        #endregion
        
        #region Public

        public bool HasAnyTag(Tag checkTag)
        {
            return checkTag != null && _tagsSet.Contains(checkTag);
        }

        public bool HasAnyTags(Tag[] checkTags)
        {
            if (checkTags == null || checkTags.Length == 0)
                return false;

            for (int i = 0; i < checkTags.Length; i++)
            {
                if (_tagsSet.Contains(checkTags[i]))
                    return true;
            }
            return false;
        }

        public void AddTag(Tag tag)
        {
            if (tag == null)
                return;

            if (_tagsSet.Add(tag))
                _tags.Add(tag);
        }

        public void RemoveTag(Tag tag)
        {
            if (tag == null)
                return;

            if (_tagsSet.Remove(tag))
                _tags.Remove(tag);
        }

        #endregion
    }
}