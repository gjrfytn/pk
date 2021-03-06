﻿using System.Collections.Generic;

namespace PK.Classes
{
    static partial class DocumentCreator
    {
        private static readonly Dictionary<string, System.Func<string>> _PH_SingleSpecial = new Dictionary<string, System.Func<string>>
        {
          {"PC_Name", ()=>System.Windows.Forms.SystemInformation.ComputerName}
        };

        private static readonly Dictionary<string, string> _PH_Single = new Dictionary<string, string>
        {
          {"campaignName",              "campaigns.name" },
          {"campaignStartYear",         "campaigns.start_year" },
          {"campaignStatusDictID",      "campaigns.status_dict_id" },
          {"campaignStatusID",          "campaigns.status_id" },
          {"campaignStatus",            "dictionaries_items.name:dictionary_id=@campaignStatusDictID,item_id=@campaignStatusID" },
          {"dictionaryName",            "dictionaries.name" },
          {"examinationSubjectDictID",  "examinations.subject_dict_id" },
          {"examinationSubjectID",      "examinations.subject_id" },
          {"examinationSubject",        "dictionaries_items.name:dictionary_id=@examinationSubjectDictID,item_id=@examinationSubjectID" },
          {"examinationDate",           "examinations.date" },
          {"applicationEntrantID",      "applications.entrant_id" },
          {"applicationEntrFirstName",  "entrants_view.first_name:id=@applicationEntrantID" },
          {"applicationEntrLastName",   "entrants_view.last_name:id=@applicationEntrantID" },
          {"applicationEntrMiddleName", "entrants_view.middle_name:id=@applicationEntrantID" },
          {"applicationID",             "applications.id" },
          {"applicationTime",           "applications.registration_time" },
          {"applicationRegistrLogin",   "applications.registrator_login" },
          {"applicationRegistrName",    "users.name:login=@applicationRegistrLogin" }
        };

        private static readonly Dictionary<string, System.Func<object, string>> _PH_Functions = new Dictionary<string, System.Func<object, string>>
        {
            {"UPPERCASE",               (o)=>o.ToString().ToUpper()},
            {"DATE_ONLY",               (o)=>((System.DateTime)o).ToShortDateString()},
            {"YEAR_ONLY",               (o)=>((System.DateTime)o).Year.ToString()},
            {"SPLIT_WHITESPACE_FIRST",  (o)=>o.ToString().Split(' ')[0]},
            {"FIRST_LETTER_DOT",        (o)=>o.ToString()[0]+"."}
        };

        private static readonly Dictionary<string, string> _PH_Table = new Dictionary<string, string>
        {
            {"campaignEduForms",    "get_campaign_edu_forms"}
        };
    }
}
