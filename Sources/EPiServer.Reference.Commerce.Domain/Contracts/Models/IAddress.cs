using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediachase.Commerce.Orders.Dto;

namespace EPiServer.Reference.Commerce.Domain.Contracts.Models
{
    public interface IAddress
    {
        bool SaveAddress { get; set; }

        string HtmlFieldPrefix { get; set; }

        Guid? AddressId { get; set; }

        DateTime? Modified { get; set; }

        string Name { get; set; }

        string FirstName { get; set; }

        string LastName { get; set; }

        string CountryName { get; set; }

        string CountryCode { get; set; }

        IEnumerable<CountryDto.CountryRow> CountryOptions { get; set; }

        string City { get; set; }

        string PostalCode { get; set; }

        string Line1 { get; set; }

        string Line2 { get; set; }

        IEnumerable<CountryDto.StateProvinceRow> RegionOptions { get; set; }

        string Region { get; set; }

        string Email { get; set; }

        bool ShippingDefault { get; set; }

        bool BillingDefault { get; set; }

        string DaytimePhoneNumber { get; set; }

        string Organization { get; set; }

        string ErrorMessage { get; set; }
    }
}
