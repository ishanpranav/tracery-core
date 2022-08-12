using System;
using System.Collections.Generic;

namespace Tracery.ContentSelectors
{
    /// <summary>
    /// An <see cref="IContentSelector"/> that uses the <see cref="Random"/> class to generate select pseudo-random symbols.
    /// </summary>
    public class RandomContentSelector : IContentSelector
    {
        private readonly Random _random;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomContentSelector"/> class.
        /// </summary>
        /// <param name="random">The pseudo-random number generator.</param>
        public RandomContentSelector(Random random)
        {
            _random = random;
        }

        /// <inheritdoc/>
        public string Select(string key, IReadOnlyList<string> candidates)
        {
            if (candidates.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                return candidates[_random.Next(candidates.Count)];
            }
        }
    }
}
