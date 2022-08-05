using Humanizer;

namespace Tracery
{
    /// <summary>
    /// Extension methods for setting up Humanizer-based modifiers in a <see cref="Grammar"/>.
    /// </summary>
    public static class GrammarExtensions
    {
        /// <summary>
        /// Adds the built-in Tracery modifiers to the specified <see cref="Grammar"/>.
        /// </summary>
        /// <param name="source">The <see cref="Grammar"/> to which modifiers are added.</param>
        /// <returns>The <see cref="Grammar"/> so that additional calls can be chained.</returns>
        public static Grammar AddTracery(this Grammar source)
        {
            return source
                .AddPlural()
                .AddSentenceCase()
                .AddTitleCase();
        }

        /// <summary>
        /// Adds the built-in Tracery modifiers to the specified <see cref="Grammar"/>.
        /// </summary>
        /// <param name="source">The <see cref="Grammar"/> to which modifiers are added.</param>
        /// <returns>The <see cref="Grammar"/> so that additional calls can be chained.</returns>
        public static Grammar AddHumanizer(this Grammar source)
        {
            return source
                .AddPlural()
                .AddSentenceCase()
                .AddTitleCase();
        }

        /// <summary>
        /// Adds the plural modifier (<c>*.s</c> or <c>*.plural</c>) to the specified <see cref="Grammar"/>.
        /// </summary>
        /// <param name="source">The <see cref="Grammar"/> to which modifiers are added.</param>
        /// <returns>The <see cref="Grammar"/> so that additional calls can be chained.</returns>
        public static Grammar AddPlural(this Grammar source)
        {
            source.Modifiers["s"] = ToPlural;
            source.Modifiers["plural"] = ToPlural;

            return source;
        }

        private static string ToPlural(string value)
        {
            return value.Pluralize();
        }

        /// <summary>
        /// Adds the sentence case modifier (<c>*.capitalize</c> or <c>*.sentence</c>) to the specified <see cref="Grammar"/>.
        /// </summary>
        /// <param name="source">The <see cref="Grammar"/> to which modifiers are added.</param>
        /// <returns>The <see cref="Grammar"/> so that additional calls can be chained.</returns>
        public static Grammar AddSentenceCase(this Grammar source)
        {
            source.Modifiers["capitalize"] = ToSentenceCase;
            source.Modifiers["sentence"] = ToSentenceCase;

            return source;
        }

        private static string ToSentenceCase(string value)
        {
            return value.Transform(To.SentenceCase);
        }

        /// <summary>
        /// Adds the title case modifier (<c>*.capitalizeAll</c> or <c>*.title</c>) to the specified <see cref="Grammar"/>.
        /// </summary>
        /// <param name="source">The <see cref="Grammar"/> to which modifiers are added.</param>
        /// <returns>The <see cref="Grammar"/> so that additional calls can be chained.</returns>
        public static Grammar AddTitleCase(this Grammar source)
        {
            source.Modifiers["capitalizeAll"] = ToTitleCase;
            source.Modifiers["title"] = ToTitleCase;

            return source;
        }

        private static string ToTitleCase(string value)
        {
            return value.Transform(To.TitleCase);
        }
    }
}
