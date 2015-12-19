using System;
using System.ComponentModel.DataAnnotations;

using EPiServer.Framework.Localization;

namespace EPiServer.Reference.Commerce.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)] // | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate
    public sealed class LocalizedStringLengthAttribute : StringLengthAttribute
    {
        private readonly string _translationPath;

        public string TranslationPath
        {
            get
            {
                return _translationPath;
            }
        }

        public LocalizedStringLengthAttribute(string translationPath, int maximumLength)
            : base(maximumLength)
        {
            this._translationPath = translationPath;
        }

        public LocalizedStringLengthAttribute(string translationPath, int minimumLength, int maximumLength)
            : base(maximumLength)
        {
            this._translationPath = translationPath;
            this.MinimumLength = minimumLength;
        }

        public override string FormatErrorMessage(string name)
        {
            this.ErrorMessage = LocalizationService.Current.GetString(this._translationPath);
            return base.FormatErrorMessage(name);
        }
    }
}