using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelParserApp
{
    public class PatientSurveyDetail
    {
        public Guid PatientSurveyDetailId { get; set; }
        public string PatientName { get; set; }
        public string MRN { get; set; }
        public int PatientAge { get; set; }
        public string ProfessionalName { get; set; }
        public DateTime SurveyDate { get; set; }
        public string SurveyQuestionText { get; set; }
        public string SurveyQuestionAnswerText { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
