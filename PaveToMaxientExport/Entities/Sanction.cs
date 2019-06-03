using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace PaveToMaxientExport.Entities
{
    [Serializable]
    public class Sanction
    {
        public virtual string IncidentNumber { get; set; }
        public virtual string RespondentId { get; set; }
        public virtual string SanctionName { get; set; }
        //public virtual string CompleteDate { get; set; }
        public virtual string FromDate{ get; set; }
        public virtual string DeadlineDate { get; set; }
        public virtual string Fine { get; set; }
        public virtual string ServiceHours { get; set; }
        public virtual string SanctionStatus { get; set; }
        public virtual string SanctionComment { get; set; }

        public Sanction()
        {

        }

        public Sanction(Microsoft.Office.Interop.Excel.Range range, int r)
        {
            this.IncidentNumber = range.Cells[r, 3].Value2.ToString();
            this.RespondentId = range.Cells[r, 18].Value2.ToString();
            this.SanctionName = range.Cells[r, 1].Value2.ToString();
            //this.CompleteDate = range.Cells[r, ?].Value2.ToString(); - Doesn't Exist
            this.FromDate = range.Cells[r, 9].Value2.ToString();
            this.DeadlineDate = range.Cells[r, 7].Value2.ToString();
            this.Fine = range.Cells[r, 5].Value2.ToString();
            this.ServiceHours = range.Cells[r, 4].Value2.ToString();
            this.SanctionStatus = range.Cells[r, 10].Value2.ToString();
            this.SanctionComment = range.Cells[r, 6].Value2.ToString();
        }
    }

    public class SanctionFacade
    {
        /// <summary>
        /// Loop through a number of Sanction report excel files and convert the results into
        /// a list of sanction objects
        /// </summary>
        /// <param name="path">File with all the Sanction Reports</param>
        /// <returns></returns>
        public List<Sanction> GetAllSanctions(string path)
        {
            List<Sanction> sanctions = new List<Sanction>();
            string[] filePaths = System.IO.Directory.GetFiles(path, "*.xlsx", SearchOption.TopDirectoryOnly);
            Application sanctApp = new Application
            {
                DisplayAlerts = false
            };

            foreach (string fp in filePaths)
            {
                try
                {
                    Missing missing = Missing.Value;

                    //Open excel file and repair it
                    Workbook workbook = sanctApp.Workbooks.Open(fp,
                        missing, missing, missing, missing, missing,
                        missing, missing, missing, missing, missing,
                        missing, missing, missing, XlCorruptLoad.xlRepairFile);

                    //Workbook sanctWorkBook = sanctApp.Workbooks.Open(fp);
                    _Worksheet sanctSheet = workbook.Sheets[1];
                    Range sanctRange = sanctSheet.UsedRange;

                    //iterate over the rows and columns and create the database
                    //excel is not zero based!!
                    for (int r = 2; r <= sanctRange.Rows.Count; r++)
                    {
                        //Create a new incident based on Excel row
                        Sanction sanct = new Sanction(sanctRange, r);
                        sanctions.Add(sanct);
                    }

                    workbook.Close(0);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(sanctSheet);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }

            sanctApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(sanctApp);

            return sanctions;
        }
    }
}
