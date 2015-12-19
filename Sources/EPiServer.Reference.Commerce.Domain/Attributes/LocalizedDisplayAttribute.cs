using System;
using System.ComponentModel;

using EPiServer.Framework.Localization;

namespace EPiServer.Reference.Commerce.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)] // | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate
    public sealed class LocalizedDisplayAttribute : DisplayNameAttribute
    {
        public LocalizedDisplayAttribute(string displayNameKey)
            : base(displayNameKey)
        {
        }

        public override string DisplayName
        {
            get
            {
                string s = LocalizationService.Current.GetString(base.DisplayName);
                return string.IsNullOrWhiteSpace(s) ? base.DisplayName : s;
            }
        }
    }
}