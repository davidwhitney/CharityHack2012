using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CharityHack2012.Code.Adapters
{
    public class CharityComissionAdapter
    {
        public string CharityComissionBaseUri { get { return "http://www.charity-commission.gov.uk"; } }
        public string UrlPart { get { return "/Showcharity/RegisterOfCharities/CharityWithPartB.aspx?SubsidiaryNumber=0&RegisteredCharityNumber=";; } }

        public string CharityComissionUriForRegistrationNumber(string registrationNumber)
        {
            return CharityComissionBaseUri + UrlPart + registrationNumber;
        }
    }
}