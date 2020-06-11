using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using Newtonsoft.Json.Linq;
using PaveToMaxientExport.Entities;

namespace PaveToMaxientExport
{
    class ExportProcess
    {
        private static readonly string _path = ConfigurationManager.AppSettings["ExportPath"];
        private static readonly string _sanctionPath = _path + @"SanctionsReports\";
        private static readonly string _incidentFileName = ConfigurationManager.AppSettings["IncidentExportFileName"];
        private static readonly string _sanctionFileName = ConfigurationManager.AppSettings["SanctionExportFileName"];

        /// <summary>
        /// 1. Loop through the Excel file that contains all the incident information in Pave and create Incident objects
        /// 2. Connect to Pave and save authentication cookie and use authentication cookie to make new connections to Pave
        /// 3. Use the incident information in order to download the extra files attached to each incident
        ///    Create folders containing the extra files organized by incidents and then by respondent ids
        /// 4. Create the narrative txt file
        /// 5. Download all the sanction reports from Pave
        /// 6. Create the sanctions export file by reading all the sanction reports
        /// </summary>
        static void Main(string[] args)
        {
            List<Incident> incidents = new List<Incident>();
            List<string> uniqueIncidentIds = new List<string>();
            CookieContainer cookieJar = new CookieContainer();

            //SetupFolderToStoreFiles
            System.IO.Directory.CreateDirectory(_path);

            //1. Populate Incident Information
            Console.WriteLine("Loading Incidents...");
            incidents = LoadIncidents();

            //Create Maxient Incident Report
            Console.WriteLine("Creating spread sheet of Incidents...");
            Utilities.CreateSpreadSheet(incidents, _path + _incidentFileName);

            //Select list of distinct incident numbers
            uniqueIncidentIds = incidents.Select(x => x.IncidentNumber).Distinct().ToList();
           
            //2. Get Authentication Cookie
            Console.WriteLine("Connecting to Pave...");
            cookieJar = Connections.AuthenticateToPave();

            //3. Get document Ids and download the extra documentation
            Console.WriteLine("Getting attachment files...");
            GetAttachedDocuments(uniqueIncidentIds, cookieJar, incidents);

            //4. Populate Narrative text file
            Console.WriteLine("Creating narrative text file...");
            string[] narratives = incidents.Select(x => string.Join("|", x.IncidentNumber, x.IncidentNarrative.Replace("|", "--"))).Distinct().ToArray();
            //Create Narrative Text String
            string narrativeTxt = "IncidentNumber|Narrative" + System.Environment.NewLine;
            narrativeTxt += string.Join(System.Environment.NewLine, narratives);
            //Write Narrative File
            using (StreamWriter sw = File.CreateText(_path + ConfigurationManager.AppSettings["NarrativeExportFileName"]))
            {
                sw.Write(narrativeTxt);
            }

            //5. Populate Sanction report
            Console.WriteLine("Downloading sanction reports...");
            List<Tuple<string, string>> sanctNames = Utilities.ParseSanctionTypeJSON(Connections.GetSanctionTypes(cookieJar));
            System.IO.Directory.CreateDirectory(_sanctionPath);

            //Loop through the sanction names and download files using parallels (multithreading)
            ParallelOptions parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 10
            };
            Parallel.ForEach(sanctNames.Take(10), parallelOptions, sanctName =>
            {
                Connections.DownloadSanctionReports(sanctName.Item1, sanctName.Item2, _sanctionPath, cookieJar);
            });

            //6. Loop through the sanctions and create the report
            Console.WriteLine("Creating sanction reports...");
            List<Sanction> sanctions = new SanctionFacade().GetAllSanctions(_sanctionPath);
            Utilities.CreateSpreadSheet(sanctions, _path + _sanctionFileName);

            //Complete
            Console.WriteLine(System.Environment.NewLine + "Press any key to exit...");
            Console.Read();
        }

        /// <summary>
        /// Loops through the incidents and makes connections to Pave to downlaod the extra attached files
        /// There will be 3 types of files downloaded 1 - overall incident files, 2 - files for each respondent (student),
        /// and 3 - case notes for each respondent (student) - case notes may be empty
        /// </summary>
        /// <param name="uniqueIncidentIds"></param>
        /// <param name="cookieJar"></param>
        private static void GetAttachedDocuments(List<string> uniqueIncidentIds, CookieContainer cookieJar, List<Incident> incidents)
        {
            //Starting and ending strings to parse the Pave HTML for incident view ids, respondent view ids, 
            // and student ids
            List<Tuple<string, string>> identifiers = new List<Tuple<string, string>> {
                new Tuple<string, string>("/SeattleU/Incident/View/", "\"\\u003e"),
                new Tuple<string, string>("/SeattleU/Respondents/ViewRespondent/", "\"\\u003e"),
                new Tuple<string, string>("data-SchoolId=\\\"", "\" data-StreetAddress")
            };

            //Loop through the incidents using parallels (multithreading)
            ParallelOptions parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 20
            };
            Parallel.ForEach(uniqueIncidentIds, parallelOptions, id =>
            {
                string searchResults = String.Empty;
                string incidentFileCabinetResults = String.Empty;
                string respondentFileCabinetResults = String.Empty;
                string incidentViewId = String.Empty;
                List<string> respondentViewIds = new List<string>();
                List<string> respondentIds = new List<string>();
                List<Document> documents = new List<Document>();

                Console.WriteLine("Starting search for incident no: {0}...", id);

                //Submit the incident number to the Pave Advanced Search 
                searchResults = Connections.CallSearchService(id, cookieJar);

                //Parse the results to retrieve the incident view id and respondent view id
                incidentViewId = Utilities.ParseSearchResults(identifiers[0].Item1, identifiers[0].Item2, searchResults).FirstOrDefault();
                respondentViewIds = Utilities.ParseSearchResults(identifiers[1].Item1, identifiers[1].Item2, searchResults);
                respondentIds = Utilities.ParseSearchResults(identifiers[2].Item1, identifiers[2].Item2, searchResults);

                //Get and parse the Incident general document results and create document objects
                documents = Utilities.ParseFileCabinetJSON(
                    Connections.CallFileCabinet
                    (
                        incidentViewId,
                        cookieJar,
                        true
                    )
                );

                //Get and parse the Respondent document results based on the respondent view ids
                foreach (string rid in respondentViewIds)
                {
                    string stuId = String.Empty;
                    string track = String.Empty;

                    //If the respondent view id index is not outside the respondent id array
                    // then store the student id and track code
                    if (respondentIds.Count - 1 >= respondentViewIds.IndexOf(rid))
                    {
                        stuId = respondentIds[respondentViewIds.IndexOf(rid)];
                        track = incidents.Where(x => x.IncidentNumber == id && x.RespondentId == stuId).Select(s => s.RespondentTracks).FirstOrDefault();
                    }

                    //Add a document record to download the case notes
                    if (!String.IsNullOrEmpty(track))
                        documents.Add(new Document("0", "CaseNotes_" + track + ".xlsx", stuId, rid, track.ToLower(), true));

                    //Parse the JSON results that contains the individual respondent document information
                    // and them use it create document objects
                    documents.AddRange(
                        Utilities.ParseFileCabinetJSON(
                            Connections.CallFileCabinet
                            (
                                incidentViewId,
                                cookieJar,
                                false,
                                rid,
                                track
                            ),
                            stuId,
                            rid,
                            track
                        )
                    );
                }

                //Loop through the document objects and download the files
                Connections.DownloadDocuments(id, documents, _path, cookieJar);
                Console.WriteLine("Downloaded files for incident no: {0}...", id);
            });
        }

        /// <summary>
        /// Takes the Pave export file and converts into a list of Incident objects
        /// that will be used to create the other export files that are needed
        /// </summary>
        /// <returns>List of all Pave Incidents</returns>
        private static List<Incident> LoadIncidents()
        {
            List<Incident> incidents = new List<Incident>();
            
            //Grab the incident data from the Pave export file
            DataTable excelData = Utilities.ReadExcelFile(ConfigurationManager.AppSettings["IncidentDownloadFile"]);

            //Skip the first row because it is headers and loop through all the rows and make incident objects
            for (int r = 0; r < excelData.Rows.Count; r++)
            {
                //Create a new incident based on Excel row
                Incident pi = new Incident(excelData.Rows[r]);
                incidents.Add(pi);
            }

            return incidents;
        }

    }
}
