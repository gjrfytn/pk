﻿
namespace SharedClasses.DB
{
    public enum DB_Table : byte
    {
        DICTIONARIES,
        DICTIONARIES_ITEMS,
        USERS,
        TARGET_ORGANIZATIONS,
        DICTIONARY_10_ITEMS,
        FACULTIES,
        PROFILES,
        DIRECTIONS,
        CAMPAIGNS,
        CAMPAIGNS_DIRECTIONS_DATA,
        CAMPAIGNS_FACULTIES_DATA,
        CAMPAIGNS_PROFILES_DATA,
        _CAMPAIGNS_HAS_DICTIONARIES_ITEMS,
        TARGET_ORGANISATIONS_ADM_VOLUMES,
        ENTRANCE_TESTS,
        EXAMINATIONS,
        EXAMINATIONS_AUDIENCES,
        ENTRANTS_EXAMINATIONS_MARKS,
        ENTRANTS,
        APPLICATIONS,
        DOCUMENTS,
        IDENTITY_DOCS_ADDITIONAL_DATA,
        _APPLICATIONS_HAS_DOCUMENTS,
        DIPLOMA_DOCS_ADDITIONAL_DATA,
        DOCUMENTS_SUBJECTS_DATA,
        OTHER_DOCS_ADDITIONAL_DATA,
        APPLICATION_COMMON_BENEFITS,
        INDIVIDUAL_ACHIEVEMENTS,
        INSTITUTION_ACHIEVEMENTS,
        DICTIONARY_19_ITEMS,
        DICTIONARY_OLYMPIC_PROFILES,
        _DICTIONARY_OLYMPIC_PROFILES_HAS_DICTIONARIES_ITEMS,
        APPLICATIONS_ENTRANCES,
        OLYMPIC_DOCS_ADDITIONAL_DATA,
        CAMPAIGNS_DIRECTIONS_TARGET_ORGANIZATIONS_DATA,
        CONSTANTS,
        ENTRANTS_VIEW,
        ROLES_PASSWORDS,
        ORDERS,
        ORDERS_HAS_APPLICATIONS,
        APPLICATION_EGE_RESULTS,
        MASTERS_EXAMS_MARKS,
        APPLICATIONS_DOCUMENTS_VIEW,
        APPLICATION_ID_ENTRANTS_VIEW
    }
}
