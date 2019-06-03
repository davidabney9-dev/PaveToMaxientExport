using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaveToMaxientExport.Entities
{
    [Serializable]
    public class Narrative
    {
        public virtual string IncidentNumber { get; set; }
        public virtual string _Narrative { get; set; }

        public Narrative()
        {

        }

        public Narrative(string incidentNumber, string _narrative)
        {
            this.IncidentNumber = incidentNumber;
            this._Narrative = _narrative;
        }
    }
}
