using PaveToMaxientExport.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PaveToMaxientExport
{
    internal class Connections
    {
        private static readonly string _paveUsername = ConfigurationManager.AppSettings["PaveUsername"];
        private static readonly string _pavePwd = ConfigurationManager.AppSettings["PavePwd"];
        private static string _PaveURL = "https://www.pavesuite.com/SeattleU";
        private static string _logonFormURL = _PaveURL + "/Account/LogOn";
        private static string _searchURL = _PaveURL + "/HomePage/IncidentAdvancedSearch";
        private static string _careDownloadURL = _PaveURL + "/Discussion/RespondentTrackDiscussionExportToExcel?respondentId={0}&track=care";
        private static string _IncidentDocsURL = _PaveURL + "/Incident/GetIncidentDocuments?incidentId={0}&_search=false&nd=0&rows=10000&page=1&sidx=name&sord=asc";
        private static string _RespondentDocsURL = _PaveURL + "/Incident/GetRespondentDocuments?incidentId={0}&respondentId={1}&track={2}&_search=false&nd=0&rows=10000&page=1&sidx=name&sord=asc";
        private static string _CaseNotesURL = _PaveURL + "/Discussion/RespondentTrackDiscussionExportToExcel?respondentId={0}&track={1}";
        private static string _incidentViewURL = _PaveURL + "Incident/View/{0}";
        private static string _docDownloadURL = _PaveURL + "/Incident/DownloadDocument?documentId={0}";
        private static string _sanctReportTypesURL = _PaveURL + "/SanctionReports/LoadSanctionsReport_Count?_search=false&nd=0&rows=1000&page=1&sidx=StudentCountIdx&sord=desc";
        private static string _sanctDownloadURL = _PaveURL + "/SanctionReports/SanctionsCountDrillDownExportToExcelSheet?sanctionName={0}&sanctionDueType=-1&type={1}";
        private static string _searchObject = "{{\"searchCriteria\":" +
                                                  "{{\"IncidentNumber\":\"{0}\"," +
                                                  "\"IncidentDateFrom\":\"\"," +
                                                  "\"IncidentDateTo\":\"\"," +
                                                  "\"StudentId\":\"\"," +
                                                  "\"IncidentManager\":\"\"," +
                                                  "\"StudentsIds\":null," +
                                                  "\"ConcernTypes\":null," +
                                                  "\"RespondentTypes\":null," +
                                                  "\"VictimTypes\":null," +
                                                  "\"ReferralTypes\":null," +
                                                  "\"WitnesseTypes\":null," +
                                                  "\"Zone\":\"-1\"," +
                                                  "\"Location\":\"-1\"," +
                                                  "\"Room\":\"-1\"," +
                                                  "\"PoliceDepartments\":null," +
                                                  "\"DashBoardComponent\":\"AllIncidents\"," +
                                                  "\"NoOfSkippedItems\":0," +
                                                  "\"PoliceNumber\":\"\"}}," +
                                                "\"sortby\":\"Date\"}}";

        /// <summary>
        /// Sets up the sanction file download URL and file path before sending it off to the function that will download the file
        /// </summary>
        internal static void DownloadSanctionReports(string sanctName, string sanctStatus, string path, CookieContainer cookieJar)
        {
            DownloadFile(string.Format(_sanctDownloadURL, sanctName, sanctStatus), path + sanctName + "_" + sanctStatus + ".xlsx", cookieJar);
        }

        /// <summary>
        /// Calls a Pave URL that returns the list of sanction reports in a JSON Object
        /// </summary>
        /// <returns>Sanction report JSON object</returns>
        internal static string GetSanctionTypes(CookieContainer cookieJar)
        {
            string results = String.Empty;

            //Create Pave Search Request POST
            HttpWebRequest searchRequest = (HttpWebRequest)WebRequest.Create(_sanctReportTypesURL);
            searchRequest.ContentType = "application/json";
            searchRequest.Method = "GET";
            searchRequest.CookieContainer = cookieJar;

            //Retrieve the search results
            HttpWebResponse searchResp = (HttpWebResponse)searchRequest.GetResponse();
            using (StreamReader sr = new StreamReader(searchResp.GetResponseStream()))
            {
                results = sr.ReadToEnd();
            }

            return results;
        }

        /// <summary>
        /// Sets up a connection to the Pave website and authenticates into the system with a POST parammeter, 
        /// then saves that cookie to authenticate in later connections without logging in to the website
        /// </summary>
        /// <param name="incidentData"></param>
        internal static CookieContainer AuthenticateToPave()
        {
            //Setup POST parammeters
            string formParams = string.Format("LoginUserName={0}&Password={1}", _paveUsername, _pavePwd);
            string result;
            CookieContainer cookieJar = new CookieContainer();

            //Create Pave POST Request
            HttpWebRequest loginRequest = (HttpWebRequest)WebRequest.Create(_logonFormURL);
            loginRequest.CookieContainer = cookieJar;
            loginRequest.Method = WebRequestMethods.Http.Post;
            loginRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
            loginRequest.AllowWriteStreamBuffering = true;
            loginRequest.ProtocolVersion = HttpVersion.Version11;
            loginRequest.AllowAutoRedirect = true;
            loginRequest.ContentType = "application/x-www-form-urlencoded";

            //Add POST parameters and submit request
            byte[] byteArray = Encoding.ASCII.GetBytes(formParams);
            loginRequest.ContentLength = byteArray.Length;
            Stream newStream = loginRequest.GetRequestStream(); //open connection
            newStream.Write(byteArray, 0, byteArray.Length); // Send the data.
            newStream.Close();

            //Retrieve the response and read the response
            HttpWebResponse resp = (HttpWebResponse)loginRequest.GetResponse();
            using (var reader = new StreamReader(resp.GetResponseStream(), Encoding.GetEncoding(1255)))
            {
                result = reader.ReadToEnd();
            }

            //Add cookies to CookieJar (Cookie Container)
            foreach (Cookie cookie in resp.Cookies)
            {
                cookieJar.Add(new Cookie(cookie.Name.Trim(), cookie.Value.Trim(), cookie.Path, cookie.Domain));
            }

            return cookieJar;
        }

        /// <summary>
        /// Based on if the document is a general incident document, respondent document, or a case note
        /// we have to setup the connection URL properly
        /// </summary>
        /// <param name="incidentViewId"></param>
        /// <param name="cookieJar"></param>
        /// <returns></returns>
        internal static string CallFileCabinet(string incidentViewId, CookieContainer cookieJar, bool incidentDocs, string rid = "", string track = "")
        {
            string results = String.Empty;
            //
            string docURL = String.Format(_IncidentDocsURL, incidentViewId);

            if (!incidentDocs && !String.IsNullOrWhiteSpace(rid))
                docURL = String.Format(_RespondentDocsURL, incidentViewId, rid, track);

            //Create Pave Search Request POST
            HttpWebRequest fileCabRequest = (HttpWebRequest)WebRequest.Create(docURL);
            fileCabRequest.ContentType = "application/json";
            fileCabRequest.Method = "POST";
            fileCabRequest.CookieContainer = cookieJar;
            fileCabRequest.ContentLength = 0;

            //Retrieve the search results
            HttpWebResponse searchResp = (HttpWebResponse)fileCabRequest.GetResponse();
            using (StreamReader sr = new StreamReader(searchResp.GetResponseStream()))
            {
                results = sr.ReadToEnd();
            }

            return results;
        }

        /// <summary>
        /// Submits the incident number to the Pave incident search, so we can get the
        /// Pave incident view id and the respondent view ids in order to download the
        /// extra documents and care notes
        /// </summary>
        /// <param name="incidentNo"></param>
        /// <param name="cookieJar"></param>
        /// <returns>Search results from Pave web service</returns>
        internal static string CallSearchService(string incidentNo, CookieContainer cookieJar)
        {
            string results = String.Empty;

            //Create Pave Search Request POST
            HttpWebRequest searchRequest = (HttpWebRequest)WebRequest.Create(_searchURL);
            searchRequest.ContentType = "application/json";
            searchRequest.Method = "POST";
            searchRequest.CookieContainer = cookieJar;

            //Create the search criteria JSON parameter
            using (var streamWriter = new StreamWriter(searchRequest.GetRequestStream()))
            {
                string jsonObject = String.Format(_searchObject, incidentNo);

                streamWriter.Write(jsonObject);
                streamWriter.Flush();
                streamWriter.Close();
            }

            //Retrieve the search results
            HttpWebResponse searchResp = (HttpWebResponse)searchRequest.GetResponse();
            using (StreamReader sr = new StreamReader(searchResp.GetResponseStream()))
            {
                results = sr.ReadToEnd();
            }

            return results;
        }

        /// <summary>
        /// Properly formats the URL to download the extra files based on what type of document it is
        /// </summary>
        internal static void DownloadDocuments(string incidentNumber, List<Document> documents, string path, CookieContainer cookieJar)
        {
            //Create path to hold files for each incident
            path += incidentNumber + "\\";
            System.IO.Directory.CreateDirectory(path);

            //Loop through the documents, download them, and then save them to the incident folder
            foreach (Document d in documents)
            {
                //Case Notes
                if(d.CaseNotes && !String.IsNullOrWhiteSpace(d.RespondentViewId))
                {
                    string tempPath = path + d.RespondentId + "\\";
                    System.IO.Directory.CreateDirectory(tempPath);

                    //Download and create file
                    DownloadFile(String.Format(_CaseNotesURL, d.RespondentViewId, d.Track), tempPath + d.DocName, cookieJar);
                }
                // Respondent Documents
                else if (!d.CaseNotes && !String.IsNullOrWhiteSpace(d.RespondentId) && d.RespondentId != "N/A")
                {
                    string tempPath = path + d.RespondentId + "\\";
                    System.IO.Directory.CreateDirectory(tempPath);

                    //Download and create file
                    DownloadFile(String.Format(_docDownloadURL, d.DocId, d.Track), tempPath+d.DocName, cookieJar);
                }
                //General Incident Documents
                else if (!d.CaseNotes)
                {
                    //Download and create file
                    DownloadFile(String.Format(_docDownloadURL, d.DocId), path+d.DocName, cookieJar);
                }

            }

        }

        /// <summary>
        /// Makes a connection to the remote file location and downloads it to the local file name provided
        /// </summary>
        /// <param name="remoteFilename"></param>
        /// <param name="localFilename"></param>
        /// <param name="cookieJar"></param>
        /// <returns></returns>
        internal static int DownloadFile(String remoteFilename, String localFilename, CookieContainer cookieJar)
        {
            // Function will return the number of bytes processed
            // to the caller. Initialize to 0 here.
            int bytesProcessed = 0;

            // Assign values to these objects here so that they can
            // be referenced in the finally block
            Stream remoteStream = null;
            Stream localStream = null;
            WebResponse response = null;

            // Use a try/catch/finally block as both the WebRequest and Stream
            // classes throw exceptions upon error
            try
            {
                // Create a request for the specified remote file name
                HttpWebRequest fileRequest = (HttpWebRequest)WebRequest.Create(remoteFilename);
                fileRequest.ContentType = "application/json";
                fileRequest.Method = "GET";
                fileRequest.Timeout = 1200000;  //20 min timeout
                fileRequest.CookieContainer = cookieJar;

                if (fileRequest != null)
                {
                    // Send the request to the server and retrieve the
                    // WebResponse object 
                    response = fileRequest.GetResponse();
                    if (response != null)
                    {
                        // Once the WebResponse object has been retrieved,
                        // get the stream object associated with the response's data
                        remoteStream = response.GetResponseStream();

                        // Create the local file
                        localStream = File.Create(localFilename);

                        // Allocate a 1k buffer
                        byte[] buffer = new byte[1024];
                        int bytesRead;

                        // Simple do/while loop to read from stream until
                        // no bytes are returned
                        do
                        {
                            // Read data (up to 10k) from the stream
                            bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

                            // Write the data to the local file
                            localStream.Write(buffer, 0, bytesRead);

                            // Increment total bytes processed
                            bytesProcessed += bytesRead;
                        } while (bytesRead > 0);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " File Name: " + localFilename + " " + remoteFilename);
            }
            finally
            {
                // Close the response and streams objects here 
                // to make sure they're closed even if an exception
                // is thrown at some point
                if (response != null) response.Close();
                if (remoteStream != null) remoteStream.Close();
                if (localStream != null) localStream.Close();
            }

            // Return total bytes processed to caller.
            return bytesProcessed;
        }
    }
}
