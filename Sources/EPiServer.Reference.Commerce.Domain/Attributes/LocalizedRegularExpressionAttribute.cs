using System;
using System.ComponentModel.DataAnnotations;

using EPiServer.Framework.Localization;

namespace EPiServer.Reference.Commerce.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)] // | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate
    public class LocalizedRegularExpressionAttribute : RegularExpressionAttribute
    {
        private readonly string _name;

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public LocalizedRegularExpressionAttribute(string pattern, string name)
            : base(pattern)
        {
            this._name = name;
        }

        public override string FormatErrorMessage(string name)
        {
            this.ErrorMessage = LocalizationService.Current.GetString(this._name);
            return base.FormatErrorMessage(name);
        }
    }
}