using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CMS.Data.ValidationCustomize
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed class MinWordsAttribute : ValidationAttribute
    {
        // Internal field to hold the MinWords value.
        readonly int _MinWords;

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
            if (this.MinWords != null)
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
