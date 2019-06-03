using Newtonsoft.Json.Linq;
using PaveToMaxientExport.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Office.Interop.Excel;
using System.Data.OleDb;
using System.Data;
using System.Configuration;

namespace PaveToMaxientExport
{
    internal class Utilities
    {
        //List of the types of sanctions statuses (or types)
        private static readonly Dictionary<int, string> _sanctStatus =
            new Dictionary<int, string>{
                {1, "Complete"},
                {2, "Incomplete"},
                {3, "Past Due"}
            };
        //Connection string to connect an excel file like a database
        private static string _excelConnection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=Yes;TypeGuessRows=0;\"";

        /// <summary>
        /// The Pave search returns a block of HTML that needs to be parsed for incident view ids, respondent view ids,
        /// and student id numbers
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        internal static List<string> ParseSearchResults(string identifier, string ending, string results)
        {
            //data-schoolid="[id number]" data-streetaddress

            List<string> values = new List<string>();

            while (results.Length > 0)
            {
                int index = 0;
                index = results.IndexOf(identifier);

                if (index > -1)
                {
                    index = index + identifier.Length;
                    int length = results.Substring(index).IndexOf(ending) - 1;

                    if (length > -1)
                        values.Add(results.Substring(index, length));
                    else
                        break;

                    results = results.Substring(index + length);
                }
                else
                    break;
            }

            return values;
        }

        /// <summary>
        /// Take the JSON response and convert the row id, document name, and rid into a list of document objects
        /// </summary>
        /// <param name="results">File Cabinet JSON</param>
        /// <returns>Row Ids</returns>
        internal static List<Document> ParseFileCabinetJSON(string results, string rid = "", string respondentId = "", string track = "")
        {
            List<Document> documents = new List<Document>();
            dynamic docs = JObject.Parse(results);

            if (docs.rows != null)
            {
                foreach (dynamic row in docs.rows)
                {
                    //We create a document object based on the parsing of the JSON 
                    //the document name must be split from a set of cells with other information
                    Document d = 
                        new Document(
                            row.id.ToString(), 
                            RemoveSpecialCharacters(row.cell.ToString().Split(new string[] { "," + System.Environment.NewLine }, StringSplitOptions.None)[1]),
                            rid,
                            respondentId,
                            track
                        );
                    documents.Add(d);
                }
            }

            return documents;
        }

        /// <summary>
        /// Takes a generic list of objects and then creates a spreadsheet, the columns are the object properties
        /// and the rows are the object property values
        /// </summary>
        /// <param name="incidents"></param>
        /// <param name="v"></param>
        internal static void CreateSpreadSheet<T>(List<T> list, string filePath)
        {
            //Give us a list of the object property names
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

            //Start an excel application, but don't give us any popups
            Application excelApp = new Application
            {
                Visible = false
            };
            //Open a new workbook and sheet
            Workbook workbook = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            _Worksheet sheet = workbook.Sheets[1];

            try
            {
                //Create spreadsheet header using the property names
                for (int p = 0; p < properties.Count; p++)
                    sheet.Cells[1, p+1].Value = properties[p].Name;

                //Create the spreadsheet rows with the values of the object properties
                for (int r = 0; r < list.Count; r++)
                {
                    Console.WriteLine(r);
                    for (int c = 0; c < properties.Count; c++)
                    {
                        sheet.Cells[r + 2, c + 1].Value = properties[c].GetValue(list[r]);
                    }
                }

                //Save the spreadsheet
                workbook.SaveAs(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //Close the workbook and excel app
                workbook.Close(0);
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            }
        }

        /// <summary>
        /// Take the JSON response and return the sanction type names as a list of strings
        /// </summary>
        /// <param name="results">File Cabinet JSON</param>
        /// <returns>Row Ids</returns>
        internal static List<Tuple<string, string>> ParseSanctionTypeJSON(string results)
        {
            List<Tuple<string, string>> sanctionTypes = new List<Tuple<string, string>>();
            dynamic docs = JObject.Parse(results);

            if (docs.rows != null)
            {
                //Create a sanction object from the JSON returned by Pave
                foreach (dynamic row in docs.rows)
                {
                    //Needs to split the cells
                    string[] cells = row.cell.ToString().Split(new string[] { "," + System.Environment.NewLine }, StringSplitOptions.None);

                    for (int i = 1; i < cells.Length; i++)
                    {
                        if (Convert.ToInt16(RemoveSpecialCharacters(cells[i])) > 0)
                        {
                            sanctionTypes.Add(
                                new Tuple<string, string>(HttpUtility.UrlEncode(RemoveFileNameCharacters(cells[0])).Replace("%0d%0a", "%0a"), _sanctStatus[i])                       
                            );
                        }
                    }
                }
            }

            return sanctionTypes;
        }

        /// <summary>
        /// Make a connection to the excel file using OleDB and read from it like a SQL database
        /// and then create a dataTable to be converted into a list of objects
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        internal static System.Data.DataTable ReadExcelFile(string filePath)
        {
            string sheetName = null;
            string connStr = String.Format(_excelConnection, filePath);
            System.Data.DataTable tablesList = new System.Data.DataTable();
            System.Data.DataTable excelData = new System.Data.DataTable();
            OleDbCommand oleExcelCommand = default(OleDbCommand);
            OleDbDataReader oleExcelReader = default(OleDbDataReader);
            OleDbConnection oleExcelConnection = default(OleDbConnection);

            //Connect to Excel File
            oleExcelConnection = new OleDbConnection(connStr);
            oleExcelConnection.Open();

            try
            {

                tablesList = oleExcelConnection.GetSchema("Tables");

                //Grab all the first sheet, which work as a table
                if (tablesList.Rows.Count > 0)
                {
                    sheetName = tablesList.Rows[0]["TABLE_NAME"].ToString();
                }

                tablesList.Clear();
                tablesList.Dispose();

                //If a sheet exists, then select all the content from it and load it into a data file
                if (!string.IsNullOrEmpty(sheetName))
                {
                    oleExcelCommand = oleExcelConnection.CreateCommand();
                    oleExcelCommand.CommandText = "Select * From [" + sheetName + "]";
                    oleExcelCommand.CommandType = CommandType.Text;
                    oleExcelReader = oleExcelCommand.ExecuteReader();

                    excelData.Load(oleExcelReader);
                    oleExcelReader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                tablesList.Clear();
                tablesList.Dispose();
                oleExcelConnection.Close();
            }

            return excelData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string RemoveSpecialCharacters(string oldValue)
        {
            string newValue = String.Empty;
            newValue = oldValue.Replace("[", "").Replace(System.Environment.NewLine, "").Replace("\"", "").Replace(@"\", "").Replace("]", "").Trim();
            return newValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        internal static string RemoveFileNameCharacters(string oldValue)
        {
            string newValue = String.Empty;
            newValue = oldValue.Replace("[", "").Replace("\\n", System.Environment.NewLine).Replace("\"", "").Replace("]", "").Trim();
            return newValue;
        }
    }
}
