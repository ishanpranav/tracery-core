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
        bool ICollection<KeyValuePair<string, IReadOnlyList<string>>>.IsReadOnly
        {
            get
            {
                return _rules.IsReadOnly;
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
            Modifiers = new Dictionary<string, Func<string, string>>(comparer);
            _rules = new Dictionary<string, IReadOnlyList<string>>(comparer);
        }

        /// <summary>
        /// Recursively resolves a rule using a content selector.
        /// </summary>
        /// <param name="rule">The starting rule.</param>
        /// <param name="contentSelector">The content selection strategy used to select a rule from a collection of candidates.</param>
        /// <returns>A string containing the flattened rule expansion.</returns>
        /// <remarks>
        /// <para>A rule is a string that can contain references to other rules. Each key refers to a collection of rules.</para>
        /// <para>For example, the rule <c>"Hello, world!"</c> is flattened as-is, while flattening <c>"Hello, #name#"</c> involves recursively expanding all sub-rules (in this case, by resolving the key <c>"#name#"</c>).</para>
        /// <para>Referenced rules can also be transformed. For example, a client can register a <c>plural</c> transformation by adding a function to the <see cref="Modifiers"/> property. Now, if <c>"#animal#"</c> resolves to <c>"eagle"</c>, <c>#animal.plural#</c> resolves to <c>"eagles"</c>.</para>
        /// <para>Finally, an expansion can be saved in a variable (a dynamic rule) for re-use. Flattening <c>"#[hero:#name#][heroPet:#animal#]story#"</c> resolves <c>#name#</c>, saves it in the variable <c>hero</c>, resolves <c>#animal#</c>, saves it in the variable <c>heroPet</c>, and returns the expansion of <c>#story#</c>. The <c>story</c> rule can contain many instances of <c>#hero#</c> and <c>#heroPet#</c>, each of which resolves to its constant value.</para>
        /// </remarks>
        public string Flatten(string rule, IContentSelector contentSelector)
        {
            MatchCollection expansionMatches = s_expansionRegex.Matches(rule);

            if (expansionMatches.Count == 0)
            {
                resolveVariables(rule);
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
                        expansion = expansion.Substring(startIndex: 0, index);
                    }

                    string selected;

                    if (_rules.TryGetValue(expansion, out IReadOnlyList<string> candidates))
                    {
                        selected = contentSelector.Select(expansion, candidates);
                    }
                    else
                    {
                        selected = expansion;
                    }

                    selected = Flatten(selected, contentSelector);

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

                    rule = rule.Replace(match.Value, Flatten(selected, contentSelector));
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

                        _rules[segments[0]] = new string[] { Flatten(segments[1], contentSelector) };
                    }
                    else
                    {
                        Flatten(ExpansionDelimiter + variable + ExpansionDelimiter, contentSelector);
                    }
                }
            }

            return rule;
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
        void ICollection<KeyValuePair<string, IReadOnlyList<string>>>.Add(KeyValuePair<string, IReadOnlyList<string>> item)
        {
            _rules.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _rules.Clear();
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<string, IReadOnlyList<string>>>.Contains(KeyValuePair<string, IReadOnlyList<string>> item)
        {
            return _rules.Contains(item);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<string, IReadOnlyList<string>>>.CopyTo(KeyValuePair<string, IReadOnlyList<string>>[] array, int arrayIndex)
        {
            _rules.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<string, IReadOnlyList<string>>>.Remove(KeyValuePair<string, IReadOnlyList<string>> item)
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
