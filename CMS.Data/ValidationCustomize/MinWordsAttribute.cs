using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CMS.Data.ValidationCustomize
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    internal sealed class MinWordsAttribute : ValidationAttribute
    {
        // Internal field to hold the MinWords value.
        private readonly int _MinWords;

        public int MinWords
        {
            get { return _MinWords; }
        }

        public MinWordsAttribute(int MinWords)
        {
            _MinWords = MinWords;
        }

        public override bool IsValid(object value)
        {
            var inputText = (String)value;
            bool result = true;
#pragma warning disable CS0472 // The result of the expression is always 'true' since a value of type 'int' is never equal to 'null' of type 'int?'
            if (this.MinWords != null)
#pragma warning restore CS0472 // The result of the expression is always 'true' since a value of type 'int' is never equal to 'null' of type 'int?'
            {
                result = MatchesMinCountWord(this.MinWords, inputText);
            }
            return result;
        }

        internal bool MatchesMinCountWord(int MinWords, string inputText)
        {
            var wordCount = CMS.Common.Utils.CountWords(inputText);
            if (wordCount < MinWords)
            {
                return false;
            }
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              ErrorMessageString, name, this.MinWords);
        }
    }
}