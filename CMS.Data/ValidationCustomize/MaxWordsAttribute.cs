using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CMS.Data.ValidationCustomize
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed class MaxWordsAttribute : ValidationAttribute
    {
        // Internal field to hold the maxWords value.
        readonly int _maxWords;

        public int maxWords
        {
            get { return _maxWords; }
        }

        public MaxWordsAttribute(int maxWords)
        {
            _maxWords = maxWords;
        }
        public override bool IsValid(object value)
        {
            var inputText = (String)value;
            bool result = true;
            if (this.maxWords != null)
            {
                result = MatchesCountWord(this.maxWords, inputText);
            }
            return result;
        }
        internal bool MatchesCountWord(int maxWords, string inputText)
        {
            var wordCount = CMS.Common.Utils.CountWords(inputText);
            if (wordCount > maxWords)
            {
                return false;
            }
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              ErrorMessageString, name, this.maxWords);
        }
    }
}
