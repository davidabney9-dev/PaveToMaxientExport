using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaveToMaxientExport.Entities
{
    public class Document
    {
        public virtual string DocId { get; set; }
        public virtual string DocName { get; set; }
        public virtual string RespondentId { get; set; }
        public virtual string RespondentViewId { get; set; }
        public virtual string Track { get; set; }
        public virtual bool CaseNotes { get; set; }

        public Document()
        {

        }

        public Document(string id, string name, string respondentId, string respondentViewId, string track, bool caseNotes = false)
        {
            this.DocId = id;
            this.DocName = name;
            this.RespondentId = respondentId;
            this.RespondentViewId = respondentViewId;
            this.Track = track;
            this.CaseNotes = caseNotes;
        }
    }
}
