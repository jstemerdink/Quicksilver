using System.ComponentModel.DataAnnotations;

using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Domain.Contracts.Models;

namespace EPiServer.Reference.Commerce.Domain.Models
{
    public class MailBasePage : PageData, IMailPage
    {
        [CultureSpecific]
        [Display(
            Name = "Mail Title",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string MailTitle { get; set; }
    }
}