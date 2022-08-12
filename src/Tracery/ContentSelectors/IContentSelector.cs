using System.Collections.Generic;

namespace Tracery.ContentSelectors
{
    /// <summary>
    /// Defines a method for selecting rules.
    /// </summary>
    public interface IContentSelector
    {
        /// <summary>
        /// Selects a rule from a list of candidates.
        /// </summary>
        /// <param name="key">The rule that is currently being processed.</param>
        /// <param name="candidates">The list of candidate rules.</param>
        /// <returns>The chosen rule.</returns>
        string Select(string key, IReadOnlyList<string> candidates);
    }
}
