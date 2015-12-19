using Mediachase.Commerce.Customers;

using Microsoft.AspNet.Identity;

namespace EPiServer.Reference.Commerce.Domain.Models.Identity
{
    /// <summary>
    /// An IdentityResult in which also a CustomerContact is involved.
    /// </summary>
    public class ContactIdentityResult
    {
        private readonly CustomerContact _contact;
        private readonly IdentityResult _result;

        /// <summary>
        /// Returns a new instance of a ContactIdentityResult.
        /// </summary>
        /// <param name="result">An IdentityResult created as part of an ASP.NET Identity action.</param>
        /// <param name="contact">A CustomerContact entity related to the IdentityResult.</param>
        public ContactIdentityResult(IdentityResult result, CustomerContact contact)
        {
            this._contact = contact;
            this._result = result;
        }

        /// <summary>
        /// Gets the CustomerContact involved in the Identity action.
        /// </summary>
        public CustomerContact Contact
        {
            get { return this._contact; }
        }

        /// <summary>
        /// Gets the outcome of the related identity action.
        /// </summary>
        public IdentityResult Result
        {
            get { return this._result; }
        }
    }
}