using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EPiServer.Core;

namespace EPiServer.Reference.Commerce.Domain.Contracts.Models
{
    public interface IMailPage : IContentData
    {
        string MailTitle { get; set; }
    }
}
