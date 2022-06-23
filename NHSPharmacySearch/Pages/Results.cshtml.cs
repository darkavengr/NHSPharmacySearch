using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NHSPharmacySearch.Pages
{  

    public class ResultsModel : PageModel
    {
        public List<Result> Results { get; set; }
        public void OnGet()
        {
        }

        //
        // POST handler
        //
        // In: NameOrPostCode       ODS code, postcode or name to search for
        //
        // Returns: nothing
        //
        public void OnPost(SearchParameters searchParameters)
        {
            Results = SendRequest(searchParameters);            // Get search results from parameters

        }


        //
        // Send request to API website and retrieve result
        //
        // In: searchParameters     search parameters to use
        //     
        // Returns: List of Result objects with results of search
        //
        async Task<List<Result>> SendRequest(SearchParameters searchParameters)
        {
            List<Result> searchresults = new List<Result>();
            string apiparams = "https://api.nhs.uk/etp";        // Base URL for API
            HttpResponseMessage response;
            string responsestring;
            Result result=new Result();

                        //
                        // Create a connection to the API website
                        //
            HttpClient client = new HttpClient();       // Create httpclient object

            // Search using name only
            if ((searchParameters.OrganisationName != "") && (searchParameters.location == "") && (searchParameters.Postcode == ""))
            {
                apiparams += "/SearchByName";
            }
            else if ((searchParameters.OrganisationName != "") && (searchParameters.location != "") && (searchParameters.Postcode != ""))
            {
                apiparams += "/SearchByLocation";
            }
            else if (searchParameters.eps != "")
            {
                apiparams += "/SearchByOdsCode";
            }
            else if ((searchParameters.OrganisationName == "") && (searchParameters.location == "") && (searchParameters.Postcode != ""))
            {
                apiparams += "/SearchByPostCode";
            }

            response=await client.GetAsync(apiparams);      // Get data from API website

            responsestring = response.ToString();
            
            string[] tokens= responsestring.Split(',');     // split response data into tokens

            foreach (var token in tokens)  {                  // Loop through the tokens
                                                       
                string[] subtokens = token.Split(':');        // sub-divide the token into name and value pairs

                //
                // Read JSON data into result object and add the object to list of results
                //
                switch (subtokens[0])
                {          // name

                    case "organisationCode":
                        result.organisationCode = subtokens[1];
                        break;

                    case "name":
                        result.name = subtokens[1];
                        break;

                    case "phone":
                        result.phone = subtokens[1];
                        break;

                    case "fax":
                        result.fax = subtokens[1];
                        break;

                    case "street":
                        result.street = subtokens[1];
                        break;

                    case "locality":
                        result.locality = subtokens[1];
                        break;

                    case "administrative":
                        result.postcode = subtokens[1];
                        break;

                    case "postcode":
                        result.organisationCode = subtokens[1];
                        break;

                    // for services a 
                    case "services":
                        break;

                    case "}":           // end of object
                        searchresults.Add(result);          // add to list
                        break;
                }
            }

            return searchresults;
        }
    }
}