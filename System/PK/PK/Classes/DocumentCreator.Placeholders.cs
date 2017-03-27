﻿using System.Collections.Generic;

namespace PK.Classes
{
    static partial class DocumentCreator
    {
        static readonly Dictionary<string, string> _PH_Single = new Dictionary<string, string>
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
          {"examinationDate",           "examinations.date" }
        };

        static readonly Dictionary<string, string> _PH_Table = new Dictionary<string, string>
        {
            {"campaignEduForms",    "get_campaign_edu_forms"},
            {"entrantsMarks",       "get_entrants_marks"}
        };
    }
}