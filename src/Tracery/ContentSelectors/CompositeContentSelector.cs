using System.Collections.Generic;

namespace Tracery.ContentSelectors
{
    /// <summary>
    /// An <see cref="IContentSelector"/> that uses content-selection strategies defined within the grammar rules.
    /// </summary>
    /// <remarks>
    /// Keys containing a <c>*</c> character followed by the name of a content selector registered in the <see cref="ContentSelectors"/> property are resolved using that content selector.
    /// </remarks>
    public class CompositeContentSelector : IContentSelector
    {
        /// <summary>
        /// Gets the named content selectors.
        /// </summary>
        /// <value>The named content selectors.</value>
        public IReadOnlyDictionary<string, IContentSelector> ContentSelectors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeContentSelector"/> class.
        /// </summary>
        /// <param name="contentSelectors">The named content selectors.</param>
        public CompositeContentSelector(IReadOnlyDictionary<string, IContentSelector> contentSelectors)
        {
            ContentSelectors = contentSelectors;
        }

        /// <inheritdoc/>
        public string Select(string key, IReadOnlyList<string> candidates)
        {
            int startIndex = key.LastIndexOf(value: '*') + 1;

            if (ContentSelectors.TryGetValue(key.Substring(startIndex, key.Length - startIndex), out IContentSelector contentSelector))
            {
                return contentSelector.Select(key, candidates);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
