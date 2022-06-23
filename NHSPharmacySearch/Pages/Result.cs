namespace NHSPharmacySearch.Pages
{
    public class Result
    {
        public string organisationCode;
        public string name;
        public string phone;
        public string fax;
        public string street;
        public string locality;
        public string town;
        public string administrative;
        public string postcode;
        public Boolean epstring;
        public string serviceType;     
        public List<NHSService> services;
        public List<OpeningTimes> openings;
    }

}
