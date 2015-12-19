using System;
using System.ComponentModel.DataAnnotations;

using EPiServer.Framework.Localization;

namespace EPiServer.Reference.Commerce.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)] // | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate
    public sealed class LocalizedCompareAttribute : CompareAttribute
    {
        private readonly string _translationPath;

        public string TranslationPath
        {
            get
            {
                return _translationPath;
            }
        }

        public LocalizedCompareAttribute(string otherProperty, string translationPath)
            : base(otherProperty)
        {
            this._translationPath = translationPath;
        }

        public override string FormatErrorMessage(string name)
        {
            this.ErrorMessage = LocalizationService.Current.GetString(this._translationPath);
            return base.FormatErrorMessage(name);
        }
    }
}