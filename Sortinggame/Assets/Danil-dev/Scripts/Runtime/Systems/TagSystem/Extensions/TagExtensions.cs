using UnityEngine;

namespace D_Dev.TagSystem.Extensions
{
    public static class TagExtensions
    {
        public static TagComponent GetTagComponent(this GameObject go)
        {
            go.TryGetComponent(out TagComponent tagComponent);
            return tagComponent;
        }

        public static bool HasTag(this GameObject go, Tag tag)
        {
            var comp = go.GetTagComponent();
            return comp != null && comp.HasAnyTag(tag);
        }

        public static bool HasTags(this GameObject go, Tag[] tags)
        {
            var comp = go.GetTagComponent();
            return comp != null && comp.HasAnyTags(tags);
        }
    }
}