namespace D_Dev.Conditions
{
    /// <summary>
    /// Defines how the conditions of a single transition are combined.
    /// The mode covers the normal and the fixed conditions as one flat group.
    /// </summary>
    public enum ConditionMatchMode
    {
        /// <summary>Every condition of the transition must be met.</summary>
        All = 0,

        /// <summary>At least one condition of the transition must be met.</summary>
        Any = 1
    }
}
