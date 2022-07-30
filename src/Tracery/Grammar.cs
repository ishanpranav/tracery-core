using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tracery.ContentSelectors;

namespace Tracery
{
    /// <summary>
    /// Represents a Tracery grammar used to generate randomized strings.
    /// </summary>
    public class Grammar : IDictionary<string, IReadOnlyList<string>>
    {
        private const char ModifierDelimiterChar = '.';
        private const char VariableDelimiterChar = ':';
        private const string ExpansionDelimiter = "#";

        private static readonly Regex s_expansionRegex = new Regex(pattern: @"(?<!\[|:)(?!\])#.+?(?<!\[|:)#(?!\])", RegexOptions.Compiled);
        private static readonly Regex s_variableRegex = new Regex(pattern: @"\[.+?\]", RegexOptions.Compiled);

        private readonly IDictionary<string, IReadOnlyList<string>> _rules;
        private readonly Dictionary<string, string> _variables;

        /// <summary>
        /// Gets the modifiers included in the grammar.
        /// </summary>
        /// <value>The modifiers included in the grammar.</value>
        public IDictionary<string, Func<string, string>> Modifiers { get; }

        /// <inheritdoc/>
        public ICollection<string> Keys
        {
            get
            {
                return _rules.Keys;
            }
        }

        /// <inheritdoc/>
        public ICollection<IReadOnlyList<string>> Values
        {
            get
            {
                return _rules.Values;
            }
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                return _rules.Count;
            }
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> this[string key]
        {
            get
            {
                return _rules[key];
            }
            set
            {
                _rules[key] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Grammar"/> class using a case-insensitive string comparer.
        /// </summary>
        public Grammar() : this(StringComparer.OrdinalIgnoreCase) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Grammar"/> class using the specified string comparer.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys.</param>
        public Grammar(IEqualityComparer<string> comparer)
        {
            _rules = new Dictionary<string, IReadOnlyList<string>>(comparer);
            Modifiers = new Dictionary<string, Func<string, string>>(comparer);
            _variables = new Dictionary<string, string>(comparer);
        }

        /// <summary>
        /// Recursively resolves a rule using a content selector.
        /// </summary>
        /// <param name="key">The starting rule.</param>
        /// <param name="selector">The content selector.</param>
        /// <returns>The flattened string.</returns>
        public string Flatten(string key, IContentSelector selector)
        {
            MatchCollection expansionMatches = s_expansionRegex.Matches(key);

            if (expansionMatches.Count == 0)
            {
                resolveVariables(key);
            }
            else
            {
                foreach (Match match in expansionMatches)
                {
                    resolveVariables(match.Value);

                    string expansion = s_variableRegex.Replace(match.Value.Replace(ExpansionDelimiter, string.Empty), string.Empty);
                    int index = expansion.IndexOf(ModifierDelimiterChar);

                    if (index != -1)
                    {
                        expansion = expansion.Substring(0, index);
                    }

                    string selected;

                    if (_rules.TryGetValue(expansion, out IReadOnlyList<string> candidates))
                    {
                        selected = selector.Select(candidates);
                    }
                    else
                    {
                        if (_variables.TryGetValue(expansion, out string value))
                        {
                            selected = value;
                        }
                        else
                        {
                            selected = expansion;
                        }
                    }

                    selected = Flatten(selected, selector);

                    string[] modifierSegments = match.Value
                        .Replace(ExpansionDelimiter, string.Empty)
                        .Split(ModifierDelimiterChar);

                    for (int i = 1; i < modifierSegments.Length; i++)
                    {
                        if (Modifiers.TryGetValue(modifierSegments[i], out Func<string, string> modifier))
                        {
                            selected = modifier(selected);
                        }
                    }

                    key = key.Replace(match.Value, Flatten(selected, selector));
                }
            }

            void resolveVariables(string variableRule)
            {
                foreach (Match variableMatch in s_variableRegex.Matches(variableRule))
                {
                    string variable = variableMatch.Value
                        .Replace(oldValue: "[", string.Empty)
                        .Replace(oldValue: "]", string.Empty);
#if NETSTANDARD1_3
                    if (variable.IndexOf(VariableDelimiterChar) != -1)
#else
                    if (variable.Contains(VariableDelimiterChar))
#endif
                    {
                        string[] segments = variable.Split(VariableDelimiterChar);

                        _variables[segments[0]] = Flatten(segments[1], selector);
                    }
                    else
                    {
                        Flatten(ExpansionDelimiter + variable + ExpansionDelimiter, selector);
                    }
                }
            }

            return key;
        }

        /// <inheritdoc/>
        public bool ContainsKey(string key)
        {
            return _rules.ContainsKey(key);
        }

        /// <inheritdoc/>
        public bool TryGetValue(string key, out IReadOnlyList<string> value)
        {
            return _rules.TryGetValue(key, out value);
        }

        /// <inheritdoc/>
        public void Add(string key, IReadOnlyList<string> value)
        {
            _rules.Add(key, value);
        }

        /// <inheritdoc/>
        public bool Remove(string key)
        {
            return _rules.Remove(key);
        }

        /// <inheritdoc/>
        public void Add(KeyValuePair<string, IReadOnlyList<string>> item)
        {
            _rules.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _rules.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<string, IReadOnlyList<string>> item)
        {
            return _rules.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<string, IReadOnlyList<string>>[] array, int arrayIndex)
        {
            _rules.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<string, IReadOnlyList<string>> item)
        {
            return _rules.Remove(item);
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, IReadOnlyList<string>>> GetEnumerator()
        {
            return _rules.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _rules.GetEnumerator();
        }
    }
}
