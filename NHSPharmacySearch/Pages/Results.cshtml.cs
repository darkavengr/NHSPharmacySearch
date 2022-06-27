using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace NHSPharmacySearch.Pages
{

    public class ResultsModel : PageModel
    {
        [BindProperty]
        public List<Result> Results { get; set; }

        [BindProperty]
        public string ErrorMessage { get; set; }

        [BindProperty]
        public SearchParameters searchParameters { get; set; }

        //
        // POST handler
        //
        // In: NameOrPostCode       ODS code, postcode or name to search for
        //
        // Returns: 
        //
        public void OnPost(SearchParameters parameters)
        {
            SendRequest(searchParameters, Results);            // Get search results from parameters

            if (Results.Count == 0)
            {   // No results
                ViewData["ErrorMessage"] = "No results could be found for your search.";                
            }
        }


        //
        // Send request to API website and retrieve result
        //
        // In: searchParameters     search parameters to use
        //     
        // Returns: List of Result objects with results of search
        //
        async void SendRequest(SearchParameters searchParameters, List<Result> searchresults)
        {
            string apiparams = "https://api.nhs.uk/service-search";        // Base URL for API
            HttpResponseMessage response;
            string responsestring;
            Result result = new Result();
            int LineCount = 0;
            string token;
            int SubLine = 0;

            //
            // Create a connection to the API website
            //
            HttpClient client = new HttpClient();       // Create httpclient object

                                                // Subscription key
            client.DefaultRequestHeaders.Add("subscription-key", "7aae13998be5414497f5ad55c3f5e9ea");

            apiparams += "?search=" + searchParameters.search + "&searchMode=any&searchfields=OrganisationID,URL,Address1,Address2,Address3,City,County,Postcode";
            
            response = await client.GetAsync(apiparams);      // Get data from API website

            responsestring = response.ToString();

            Debug.WriteLine(responsestring);

            //
            // Process JSON data
            //
            string[] tokens = responsestring.Split(',');     // split response data into tokens

            Debug.WriteLine(tokens[0]);
   
            // Not found
            if (tokens[0] == "StatusCode: 404")
            {          
                return;
            }

                for (LineCount = 0; LineCount < tokens.Length; LineCount++)
                {
                token = tokens[LineCount];

                if (tokens[0] != "value")
                {         // if value
                    for (SubLine = LineCount + 1; SubLine < tokens.Length; SubLine++)
                    {                        
                        while (tokens[SubLine] != "]")
                        {

                            token = tokens[LineCount];

                            string[] subtokens = token.Split(':');        // sub-divide the token into name and value pairs

                            //
                            // Read JSON data into result object and add the object to list of results
                            //
                            switch (subtokens[0])
                            {          // name

                                case "organisationID":
                                    result.OrganisationID = subtokens[1];
                                    break;

                                case "URL":
                                    result.URL = subtokens[1];
                                    break;

                                case "Address1":
                                    result.Address1 = subtokens[1];
                                    break;

                                case "Address2":
                                    result.Address2 = subtokens[1];
                                    break;

                                case "Address3":
                                    result.Address3 = subtokens[1];
                                    break;

                                case "County":
                                    result.County = subtokens[1];
                                    break;

                                case "Postcode":
                                    result.Postcode = subtokens[1];
                                    break;

                            }
                        }

                        Results.Add(result);
                    }

                }
            }
        }
    }
                    
   
}