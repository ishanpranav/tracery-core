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
                .AddSentenceCase()
                .AddTitleCase();
        }

        /// <summary>
        /// Adds the sentence case modifier (<c>*.capitalize</c> or <c>*.sentence</c>) to the specified <see cref="Grammar"/>.
        /// </summary>
        /// <param name="source">The <see cref="Grammar"/> to which modifiers are added.</param>
        /// <returns>The <see cref="Grammar"/> so that additional calls can be chained.</returns>
        public static Grammar AddSentenceCase(this Grammar source)
        {
            source.Modifiers["capitalize"] = modify;
            source.Modifiers["sentence"] = modify;

            string modify(string value) => value.Transform(To.SentenceCase);
        
            return source;
        }

        /// <summary>
        /// Adds the title case modifier (<c>*.capitalizeAll</c> or <c>*.title</c>) to the specified <see cref="Grammar"/>.
        /// </summary>
        /// <param name="source">The <see cref="Grammar"/> to which modifiers are added.</param>
        /// <returns>The <see cref="Grammar"/> so that additional calls can be chained.</returns>
        public static Grammar AddTitleCase(this Grammar source)
        {
            source.Modifiers["capitalizeAll"] = modify;
            source.Modifiers["title"] = modify;

            string modify(string value) => value.Transform(To.TitleCase);

            return source;
        }
    }
}
