using System.Collections.Generic;

namespace Tracery.ContentSelectors
{
    /// <summary>
    /// Defines a method for selecting symbols.
    /// </summary>
    public interface IContentSelector
    {
        /// <summary>
        /// Selects a symbol from a list of candidates.
        /// </summary>
        /// <param name="candidates">The list of candidate symbols.</param>
        /// <returns>The chosen symbol.</returns>
        string Select(IReadOnlyList<string> candidates);
    }
}
