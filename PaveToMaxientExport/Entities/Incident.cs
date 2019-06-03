using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaveToMaxientExport.Entities
{
    [Serializable]
    public class Incident
    {
        public virtual string IncidentNumber { get; set; }                                  //Pave and Maxient
        public virtual string IncidentStatus { get; set; }                                  //Maxient Only (populate with Care Track Status, Conduct Track Status, and Info Track Status)
        public virtual string RespondentId { get; set; }                                    //Pave and Maxient
        public virtual string RespondentFullName { get; set; }                              //Pave and Maxient
        public virtual string DateOfBirth { get; set; }                                     //Pave and Maxient
        public virtual string ScholarshipStatus { get; set; }                               //Pave and Maxient - EMPTY
        public virtual string ImmigrationStatus { get; set; }                               //Pave and Maxient
        public virtual string Sport { get; set; }                                           //Pave and Maxient
        public virtual string GPA { get; set; }                                             //Pave and Maxient
        public virtual string ClassYear { get; set; }                                       //Pave and Maxient
        public virtual string Ethnicity { get; set; }                                       //Pave and Maxient
        public virtual string Gender { get; set; }                                          //Pave and Maxient
        public virtual string Classification { get; set; }                                  //Pave and Maxient
        public virtual string EmailAddressPrimary { get; set; }                             //Pave and Maxient
        //public virtual string RespondentLocalAddressAtTimeOfIncident { get; set; }          //Pave and Maxient - EMPTY
        //public virtual string RespondentPermanentAddressAtTimeOfIncident { get; set; }      //Pave and Maxient - EMPTY
        public virtual string IncidentDate { get; set; }                                    //Pave and Maxient
        public virtual string IncidentTime { get; set; }                                    //Pave and Maxient
        public virtual string ReferralDate { get; set; }                                    //Pave and Maxient
        //public virtual string IncidentSubject { get; set; }                                 //Maxient Only - Leave Blank
        //public virtual string HearingOfficers { get; set; }                                 //Maxient Only - ?
        public virtual string IncidentLocation { get; set; }                                //Pave and Maxient
        public virtual string IncidentRoom { get; set; }                                    //Pave and Maxient
        public virtual string IncidentSpecificLocation { get; set; }                        //Pave and Maxient
        public virtual string IncidentReferrals { get; set; }                               //Pave and Maxient
        public virtual string IncidentVictims { get; set; }                                 //Pave and Maxient
        public virtual string IncidentWitnesses { get; set; }                               //Pave and Maxient
        public virtual string RespondentViolations { get; set; }                            //Pave and Maxient
        public virtual string ResponsesToViolations { get; set; }                           //Pave and Maxient
        public virtual string HearingFindings { get; set; }                                 //Pave and Maxient
        public virtual string RespondentHearingSanction { get; set; }                       //Pave and Maxient
        public virtual string HearingType { get; set; }                                     //Pave and Maxient                           
        public virtual string HearingLocation { get; set; }                                 //Pave and Maxient
        public virtual string JudicialHearingNotificationDate { get; set; }                 //Pave and Maxient
        public virtual string HearingActualDate { get; set; }                               //Pave and Maxient
        //public virtual string HearingDecisionMadeOn { get; set; }                           //Maxient Only - Leave Blank
        //public virtual string HearingDecisionLetterMailedOn { get; set; }                   //Maxient Only - Leave Blank
        public virtual string AppealRequestDate { get; set; }                               //Pave and Maxient
        public virtual string AppealRequestStatus { get; set; }                             //Pave and Maxient
        public virtual string AppealDate { get; set; }                                      //Pave and Maxient
        //public virtual string AppealTime { get; set; }                                      //Pave and Maxient
        public virtual string AppealOn { get; set; }                                        //Pave and Maxient
        public virtual string AppealOfficer { get; set; }                                   //Pave and Maxient
        public virtual string AppealFindings { get; set; }                                  //Pave and Maxient
        public virtual string RespondentAppealSanction { get; set; }                        //Pave and Maxient
        public virtual string AppealDecisionMadeOn { get; set; }                            //Pave and Maxient
        public virtual string AppealDecisionLetterMailedOn { get; set; }                    //Pave and Maxient
        public virtual string ADRType { get; set; }                                         //Pave and Maxient
        public virtual string ResolutionMeetingLocation { get; set; }                       //Pave and Maxient
        //public virtual string ADRNotificationsDate { get; set; }                            //Maxient Only - Leave Blank
        public virtual string ResolutionMeetingActualDate { get; set; }                     //Pave and Maxient
        public virtual string RespondentAssignments { get; set; }                           //Pave and Maxient - EMPTY
        //public virtual string RespondentLocalAddress { get; set; }                          //Maxient Only - Leave Blank
        public virtual string LivingOnCampus { get; set; }                                  //Pave and Maxient
        public virtual string IncidentType { get; set; }                                    //Maxient Only - Leave Blank
        public virtual string MajorMinor { get; set; }                                      //Pave and Maxient
        public virtual string MajorMinorAtTimeOfIncident { get; set; }                      //Pave and Maxient - EMPTY
        public virtual string CustomField1 { get; set; }                                    //Pave and Maxient
        //public virtual string CustomField2 { get; set; }                                    //Pave and Maxient - EMPTY
        //public virtual string CustomField3 { get; set; }                                    //Pave and Maxient - EMPTY
        //public virtual string CustomField4 { get; set; }                                    //Pave and Maxient - EMPTY
        //public virtual string CustomField5 { get; set; }                                    //Pave and Maxient - EMPTY
        //public virtual string CustomField1AtTimeOfIncident { get; set; }                    //Pave and Maxient - EMPTY
        //public virtual string CustomField2AtTimeOfIncident { get; set; }                    //Pave and Maxient - EMPTY
        //public virtual string CustomField3AtTimeOfIncident { get; set; }                    //Pave and Maxient - EMPTY
        //public virtual string CustomField4AtTimeOfIncident { get; set; }                    //Pave and Maxient - EMPTY
        //public virtual string CustomField5AtTimeOfIncident { get; set; }                    //Pave and Maxient - EMPTY

        //Custom Fields
        public virtual string IncidentNarrative { get; set; }                               //Pave Only -> Will go to the narrative text file (no text over 32,000 characters, max was ~23,000)
        public virtual string IncidentConcerns { get; set; }                                //Pave Only -> No corresponding Maxient Field
        public virtual string IncidentManager { get; set; }                                 //Pave Only -> No corresponding Maxient Field
        public virtual string IncidentZone { get; set; }                                    //Pave Only -> No corresponding Maxient Field
        public virtual string RespondentTracks { get; set; }                                //Pave Only -> Will go to Incident Subject and will be Conduct, Care, or Information (a few times it is both, Armina is trying to fix that)
        public virtual string ConductTrackStatus { get; set; }                              //Pave Only -> If track is Conduct, then populate Incident Status
        public virtual string ConductOfficers { get; set; }                                 //Pave Only -> If track is Conduct, then populate Hearing Officers
        public virtual string RespondentCareTypes { get; set; }                             //Pave Only -> If track is care, then populate Incident Type
        public virtual string RespondentFollowUps { get; set; }                             //Pave Only -> No corresponding Maxient Field
        public virtual string CareMeetingLocation { get; set; }                             //Pave Only -> No corresponding Maxient Field
        public virtual string CareTrackStatus { get; set; }                                 //Pave Only -> If track is Care, then populate Incident Status
        public virtual string CareMeetingType { get; set; }                                 //Pave Only
        public virtual string CareMeetingScheduledForDate { get; set; }                     //Pave only -> No corresponding Maxient Field
        public virtual string CareMeetingActualDate { get; set; }                           //Pave Only -> No corresponding Maxient Field
        public virtual string CareOfficers { get; set; }                                    //Pave Only -> If track is Care, then populate Hearing Officers
        public virtual string RespondentInfoTypes { get; set; }                             //Pave Only -> If track is Info, then populate Incident Type
        public virtual string InfoMeetingLocation { get; set; }                             //Pave Only - EMPTY
        public virtual string InfoMeetingType { get; set; }                                 //Pave Only - EMPTY
        public virtual string InfoMeetingScheduledForDate { get; set; }                     //Pave only - EMPTY
        public virtual string InfoMeetingActualDate { get; set; }                           //Pave Only - EMPTY
        public virtual string InfoTrackStatus { get; set; }                                 //Pave Only -> If track is Information, then populate Incident Status
        public virtual string PoliceInformation { get; set; }                               //Pave Only -> No corresponding Maxient field

        //Extra Info
        //
        //Info Track Status -----------------v 
        //Conduct Track Status --------------|-> IncidentStatus
        //Care Track Status -----------------^

        public Incident()
        {

        }

        /// <summary>
        /// Constructor creates an incident object using a datatable row
        /// </summary>
        /// <param name="dr"></param>
        public Incident(DataRow dr)
        {
            string track = dr[41].ToString();
            string status = String.Empty;
            string type = String.Empty;

            this.IncidentNumber = dr[0].ToString();
            this.IncidentNarrative = dr[1].ToString();
            this.RespondentId = dr[2].ToString();
            this.RespondentFullName = dr[3].ToString();
            this.DateOfBirth = dr[4].ToString();
            this.ScholarshipStatus = dr[5].ToString();
            this.ImmigrationStatus = dr[6].ToString();
            this.Sport = dr[7].ToString();
            this.GPA = dr[8].ToString();
            this.ClassYear = dr[9].ToString();
            this.Ethnicity = dr[10].ToString();
            this.Gender = dr[11].ToString();
            this.Classification = dr[12].ToString();
            this.EmailAddressPrimary = dr[13].ToString();
            this.LivingOnCampus = dr[14].ToString();
            //this.RespondentLocalAddressAtTimeOfIncident = dr[16].ToString(); - Empty
            //this.RespondentPermanentAddressAtTimeOfIncident = dr[17].ToString(); - Empty
            this.MajorMinor = dr[17].ToString();
            this.MajorMinorAtTimeOfIncident = dr[18].ToString();
            this.CustomField1 = dr[19].ToString();
            //this.CustomField2 = dr[20].ToString(); - Empty
            //this.CustomField3 = dr[21].ToString(); - Empty
            //this.CustomField4 = dr[22].ToString(); - Empty
            //this.CustomField5 = dr[23].ToString(); - Emptyt
            //this.CustomField1AtTimeOfIncident = dr[24].ToString(); - Empty
            //this.CustomField2AtTimeOfIncident = dr[25].ToString(); - Empty
            //this.CustomField3AtTimeOfIncident = dr[26].ToString(); - Empty
            //this.CustomField4AtTimeOfIncident = dr[27].ToString(); - Empty
            //this.CustomField5AtTimeOfIncident = dr[28].ToString(); - Empty
            this.IncidentDate = dr[29].ToString();
            this.IncidentTime = dr[30].ToString();
            this.ReferralDate = dr[31].ToString();
            this.IncidentConcerns = dr[32].ToString();
            this.IncidentManager = dr[33].ToString();
            this.IncidentZone = dr[34].ToString();
            this.IncidentLocation = dr[35].ToString();
            this.IncidentRoom = dr[36].ToString();
            this.IncidentSpecificLocation = dr[37].ToString();
            this.IncidentReferrals = dr[38].ToString();
            this.IncidentVictims = dr[39].ToString();
            this.IncidentWitnesses = dr[40].ToString();
            this.RespondentTracks = track;
            this.ConductOfficers = dr[42].ToString();
            this.RespondentViolations = dr[43].ToString();
            this.ResponsesToViolations = dr[44].ToString();
            this.HearingFindings = dr[45].ToString();
            this.RespondentHearingSanction = dr[46].ToString();
            this.HearingType = dr[47].ToString();
            this.HearingLocation = dr[48].ToString();
            this.JudicialHearingNotificationDate = dr[49].ToString();
            this.HearingActualDate = dr[50].ToString();
            this.AppealRequestDate = dr[51].ToString();
            this.AppealRequestStatus = dr[52].ToString();
            this.AppealDate = dr[53].ToString();
            this.AppealOn = dr[54].ToString();
            this.AppealOfficer = dr[55].ToString();
            this.AppealFindings = dr[56].ToString();
            this.RespondentAppealSanction = dr[57].ToString();
            this.AppealDecisionMadeOn = dr[58].ToString();
            this.AppealDecisionLetterMailedOn = dr[59].ToString();
            this.ADRType = dr[60].ToString();
            this.ResolutionMeetingLocation = dr[61].ToString();
            this.ResolutionMeetingActualDate = dr[63].ToString();
            this.RespondentAssignments = dr[64].ToString();
            this.ConductTrackStatus = dr[65].ToString();
            this.RespondentCareTypes = dr[66].ToString();
            this.CareOfficers = dr[67].ToString();
            this.CareMeetingLocation = dr[68].ToString();
            this.CareMeetingType = dr[69].ToString();
            this.CareMeetingScheduledForDate = dr[70].ToString();
            this.CareMeetingActualDate = dr[71].ToString();
            this.RespondentFollowUps = dr[72].ToString();
            this.CareTrackStatus = dr[73].ToString();
            this.RespondentInfoTypes = dr[74].ToString();
            this.InfoMeetingLocation = dr[75].ToString();
            this.InfoMeetingType = dr[76].ToString();
            this.InfoMeetingScheduledForDate = dr[77].ToString();
            this.InfoMeetingActualDate = dr[78].ToString();
            this.InfoTrackStatus = dr[79].ToString();
            this.PoliceInformation = dr[80].ToString();

            //this.IncidentSubject = dr[?].ToString(); - Doesn't Exist
            //this.HearingOfficers = dr[?].ToString(); - Doesn't Exist
            //this.HearingDecisionMadeOn = dr[?].ToString(); - Doesn't Exist
            //this.HearingDecisionLetterMailedOn = dr[?].ToString(); - Doesn't Exist
            //this.AppealTime = dr[?].ToString(); - Doesn't Exist
            //this.ADRNotificationsDate = dr[?].ToString(); Doesn't Exist
            //this.RespondentLocalAddress = dr[?].ToString(); - Doesn't Exist
            //this.IncidentType = dr[?].ToString(); - Doesn't Exist

            if (!String.IsNullOrWhiteSpace(track))
            {
                switch (track)
                {
                    case "Conduct":
                        status = dr[65].ToString();
                        type = dr[43].ToString();
                        break;

                    case "Care":
                        status = dr[73].ToString();
                        type = dr[66].ToString();
                        break;

                    case "Informational":
                        status = dr[79].ToString();
                        type = dr[74].ToString();
                        break;
                }
            }

            this.IncidentStatus = status;
            this.IncidentType = type;
        }
    }
}
