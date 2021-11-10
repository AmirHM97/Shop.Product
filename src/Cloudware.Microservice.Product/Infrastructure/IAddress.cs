using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IAddress
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string LandlinePhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string PostalCode { get; set; }
        public string PostalAddress { get; set; }
    }
}
