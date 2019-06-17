using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SharedClasses.FIS
{
    public static class FIS_ExportClasses
    {
        #region Types

        public class TUID
        {
            public readonly string Value; //200

            public TUID(string value)
            {
                Value = value;
            }

            public TUID(uint value)
            {
                Value = value.ToString();
            }
        }

        public class TEntranceTestSubject : IXElemementsConvertable
        {
            public readonly uint? SubjectID = null; //ИД дисциплины (справочник №1)
            public readonly string SubjectName = null; //Наименование дисциплины

            public TEntranceTestSubject(uint? subjectID)
            {
                SubjectID = subjectID;
            }

            public TEntranceTestSubject(string subjectName)
            {
                SubjectName = subjectName;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    ToElementOrNull("SubjectID", SubjectID),
                    SubjectName != null ? new XElement("SubjectName", SubjectName) : null
                };
            }
        }

        public class TDateTime
        {
            public readonly string Value;
			private DateTime? returnDate;

			public TDateTime(DateTime? returnDate)
			{
				this.returnDate = returnDate;
			}

			public TDateTime(System.DateTime datetime)
            {
                Value = datetime.ToString("yyyy-MM-ddTHH:mm:ss");
            }
        }

        public class TOlympicDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа
            public readonly TDate DocumentDate; //Дата выдачи документа
            public readonly uint DiplomaTypeID; //ИД типа диплома (справочник №18)
            public readonly uint OlympicID; //ИД олимпиады (справочник №19)
            public readonly uint ProfileID; //ИД профиля олимпиады (справочник №39)
            public readonly uint ClassNumber; //Класс обучения (7,8,9,10 или 11)
            public readonly uint? OlympicSubjectID; //ИД предмета олимпиады  (должен соответствовать профилю олимпиады) (справочник № 1)
            public readonly uint? EgeSubjectID; //ИД предмета, по которому будет осуществляться проверка ЕГЭ (справочник № 1)

            public TOlympicDocument(TUID uid, uint diplomaTypeID, uint olympicID, uint profileID, uint classNumber, TDate originalReceivedDate = null, TDocumentSeries documentSeries = null, TDocumentNumber documentNumber = null, TDate documentDate = null, uint? olympicSubjectID = null, uint? egeSubjectID = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DiplomaTypeID = diplomaTypeID;
                OlympicID = olympicID;
                ProfileID = profileID;
                ClassNumber = classNumber;
                OlympicSubjectID = olympicSubjectID;
                EgeSubjectID = egeSubjectID;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value),
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    DocumentSeries !=null? new XElement("DocumentSeries", DocumentSeries.Value):null,
                    DocumentNumber!=null?new XElement("DocumentNumber", DocumentNumber.Value):null,
                    DocumentDate!=null?new XElement("DocumentDate", DocumentDate.Value):null,
                    new XElement("DiplomaTypeID", DiplomaTypeID),
                    new XElement("OlympicID", OlympicID),
                    new XElement("ProfileID", ProfileID),
                    new XElement("ClassNumber", ClassNumber),
                    ToElementOrNull("OlympicSubjectID", OlympicSubjectID),
                    ToElementOrNull("EgeSubjectID", EgeSubjectID)
                };
            }
        }

        public class TDate
        {
            public readonly string Value;

            public TDate(System.DateTime date)
            {
                Value = date.ToString("yyyy-MM-dd");
            }
        }

        public class TDocumentSeries
        {
            public readonly string Value; //10

            public TDocumentSeries(string value)
            {
                Value = value;
            }
        }

        public class TDocumentNumber
        {
            public readonly string Value; //100

            public TDocumentNumber(string value)
            {
                Value = value;
            }
        }

        public class TOlympicTotalDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly uint DiplomaTypeID; //Тип диплома (справочник №18)
            public readonly uint ClassNumber; //Класс обучения (9,10,11)
            public readonly uint OlympicID; //ИД олимпиады (справочник №19)
            public readonly List<SubjectID> Subjects; //Профильные предметы олимпиады //s

            public TOlympicTotalDocument(TUID uid, TDocumentNumber documentNumber, uint diplomaTypeID, uint classNumber, uint olympicID, List<SubjectID> subjects, TDate originalReceivedDate = null, TDocumentSeries documentSeries = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DiplomaTypeID = diplomaTypeID;
                ClassNumber = classNumber;
                OlympicID = olympicID;
                Subjects = subjects;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    new XElement("DocumentNumber", DocumentNumber.Value),
                    new XElement("DiplomaTypeID", DiplomaTypeID),
                    new XElement("ClassNumber", ClassNumber),
                    new XElement("OlympicID", OlympicID),
                    new XElement("Subjects", Subjects.Select(i => i.ConvertToXElement()))
                };
            }
        }

        public class SubjectID : IXElemementConvertable
        {
            public readonly uint Value; //ИД дисциплины  (справочник №1)

            public SubjectID(uint value)
            {
                Value = value;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("SubjectID", Value);
            }
        }

        public class TUkraineOlympic : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly uint DiplomaTypeID; //Тип диплома (справочник №18)
            public readonly string OlympicName; //Наименование олимпиады //255
            public readonly string OlympicProfile; //Профиль олимпиады //255
            public readonly TDate OlympicDate; //Дата проведения олимпиады
            public readonly string OlympicPlace; //Место проведения олимпиады //us

            public TUkraineOlympic(TUID uid, TDocumentNumber documentNumber, uint diplomaTypeID, string olympicName, string olympicProfile, TDate originalReceivedDate = null, TDocumentSeries documentSeries = null, TDate olympicDate = null, string olympicPlace = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DiplomaTypeID = diplomaTypeID;
                OlympicName = olympicName;
                OlympicProfile = olympicProfile;
                OlympicDate = olympicDate;
                OlympicPlace = olympicPlace;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    new XElement("DocumentNumber", DocumentNumber.Value),
                    new XElement("DiplomaTypeID", DiplomaTypeID),
                    new XElement("OlympicName", OlympicName),
                    new XElement("OlympicProfile",OlympicProfile),
                    OlympicDate!=null?new XElement("OlympicDate", OlympicDate.Value):null,
                    OlympicPlace!=null?new XElement("OlympicPlace", OlympicPlace):null
                };
            }
        }

        public class TInternationalOlympic : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly uint CountryID; //Член сборной команды (справочник № 7)
            public readonly string OlympicName; //Наименование олимпиады //255
            public readonly string OlympicProfile; //Профиль олимпиады //255
            public readonly TDate OlympicDate; //Дата проведения олимпиады
            public readonly string OlympicPlace; //Место проведения олимпиады //255 //us

            public TInternationalOlympic(TUID uid, TDocumentNumber documentNumber, uint countryID, string olympicName, string olympicProfile, TDate originalReceivedDate = null, TDocumentSeries documentSeries = null, TDate olympicDate = null, string olympicPlace = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                CountryID = countryID;
                OlympicName = olympicName;
                OlympicProfile = olympicProfile;
                OlympicDate = olympicDate;
                OlympicPlace = olympicPlace;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    new XElement("DocumentNumber", DocumentNumber.Value),
                    new XElement("CountryID", CountryID),
                    new XElement("OlympicName", OlympicName),
                    new XElement("OlympicProfile",OlympicProfile),
                    OlympicDate!=null?new XElement("OlympicDate", OlympicDate.Value):null,
                    OlympicPlace!=null?new XElement("OlympicPlace", OlympicPlace):null
                };
            }
        }

        public class TDisabilityDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly TDocumentSeries DocumentSeries; //Серия документа //s
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly TDate DocumentDate; //Дата выдачи документа
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500 //us
            public readonly uint DisabilityTypeID; //Группа инвалидности (справочник №23)

            public TDisabilityDocument(TUID uid, TDocumentSeries documentSeries, TDocumentNumber documentNumber, uint disabilityTypeID, TDate originalReceivedDate = null, TDate documentDate = null, string documentOrganization = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
                DisabilityTypeID = disabilityTypeID;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    new XElement("DocumentSeries", DocumentSeries.Value),
                    new XElement("DocumentNumber", DocumentNumber.Value),
                    DocumentDate!=null?new XElement("DocumentDate", DocumentDate.Value):null,
                    DocumentOrganization!=null? new XElement("DocumentOrganization", DocumentOrganization):null,
                    new XElement("DisabilityTypeID",DisabilityTypeID)
                };
            }
        }

        public class TMedicalDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly TDate DocumentDate; //Дата выдачи документа
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500 //us

            public TMedicalDocument(TUID uid, TDocumentNumber documentNumber, TDate originalReceivedDate = null, TDate documentDate = null, string documentOrganization = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    new XElement("DocumentNumber", DocumentNumber.Value),
                    DocumentDate!=null?new XElement("DocumentDate", DocumentDate.Value):null,
                    DocumentOrganization!=null? new XElement("DocumentOrganization", DocumentOrganization):null
                };
            }
        }

        public class TAllowEducationDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly TDate DocumentDate; //Дата выдачи документа //s
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500 //us

            public TAllowEducationDocument(TUID uid, TDocumentNumber documentNumber, TDate documentDate, TDate originalReceivedDate = null, string documentOrganization = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    new XElement("DocumentNumber", DocumentNumber.Value),
                    new XElement("DocumentDate", DocumentDate.Value),
                    DocumentOrganization!=null? new XElement("DocumentOrganization", DocumentOrganization):null
                };
            }
        }

        public class TOrphanDocument : IXElemementsConvertable //Плохая спецификация
        {
            public readonly TUID UID; //Идентификатор //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly uint OrphanCategoryID; //Тип документа, подтверждающего сиротство (справочник № 42)
            public readonly TDocumentName DocumentName; //Наименование документа //s
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа
            public readonly TDate DocumentDate; //Дата выдачи документа //s
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500

            public TOrphanDocument(TUID uid, uint orphanCategoryID, TDocumentName documentName, TDate documentDate, string documentOrganization, TDate originalReceivedDate = null, TDocumentSeries documentSeries = null, TDocumentNumber documentNumber = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                OrphanCategoryID = orphanCategoryID;
                DocumentName = documentName;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    new XElement("OrphanCategoryID", OrphanCategoryID),
                    new XElement("DocumentName", DocumentName.Value),
                    DocumentSeries !=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    DocumentNumber !=null?new XElement("DocumentNumber", DocumentNumber.Value):null,
                    new XElement("DocumentDate", DocumentDate.Value),
                    new XElement("DocumentOrganization", DocumentOrganization)
                };
            }
        }

        public class TDocumentName
        {
            public readonly string Value; //1000

            public TDocumentName(string value)
            {
                Value = value;
            }
        }

        public class TSportDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly uint SportCategoryID; //Тип диплома в области спорта (справочник № 43)
            public readonly string DocumentName; //Наименование документа
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа
            public readonly TDate DocumentDate; //Дата выдачи документа //s
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500
            public readonly string AdditionalInfo; //Дополнительные сведения //4000 //us

            public TSportDocument(TUID uid, uint sportCategoryID, string documentName, TDate documentDate, string documentOrganization, TDate originalReceivedDate = null, TDocumentSeries documentSeries = null, TDocumentNumber documentNumber = null, string additionalInfo = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                SportCategoryID = sportCategoryID;
                DocumentName = documentName;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
                AdditionalInfo = additionalInfo;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    new XElement("SportCategoryID", SportCategoryID),
                    new XElement("DocumentName", DocumentName),
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    DocumentNumber!=null?new XElement("DocumentNumber", DocumentNumber.Value):null,
                    new XElement("DocumentDate", DocumentDate.Value),
                    new XElement("DocumentOrganization", DocumentOrganization),
                    AdditionalInfo!=null?new XElement("AdditionalInfo",AdditionalInfo):null
                };
            }
        }

        public class TCustomDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly string DocumentName; //Наименование документа //s
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа
            public readonly TDate DocumentDate; //Дата выдачи документа //s
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500
            public readonly string AdditionalInfo; //Дополнительные сведения //4000 //us

            public TCustomDocument(TUID uid, string documentName, TDate documentDate, string documentOrganization, TDate originalReceivedDate = null, TDocumentSeries documentSeries = null, TDocumentNumber documentNumber = null, string additionalInfo = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentName = documentName;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
                AdditionalInfo = additionalInfo;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    new XElement("DocumentName", DocumentName),
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    DocumentNumber!=null?new XElement("DocumentNumber", DocumentNumber.Value):null,
                    new XElement("DocumentDate", DocumentDate.Value),
                    new XElement("DocumentOrganization", DocumentOrganization),
                    AdditionalInfo!=null?new XElement("AdditionalInfo",AdditionalInfo):null
                };
            }
        }

        public class TSchoolCertificateDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов/заверенных копий
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly TDate DocumentDate; //Дата выдачи документа
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500 //us
            public readonly uint? EndYear; //Год окончания
            public readonly float? GPA; //Средний балл
            public readonly List<SubjectData> Subjects; //Баллы по предметам
            public readonly bool? StateServicePreparation; //Осуществляет подготовку к военной или иной государственной службе (преимущественное право)

            public TSchoolCertificateDocument(TUID uid, TDocumentNumber documentNumber, TDate originalReceivedDate = null, TDocumentSeries documentSeries = null, TDate documentDate = null, string documentOrganization = null, uint? endYear = null, float? gpa = null, List<SubjectData> subjects = null, bool? stateServicePreparation = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
                EndYear = endYear;
                GPA = gpa;
                Subjects = subjects;
                StateServicePreparation = stateServicePreparation;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    new XElement("DocumentNumber", DocumentNumber.Value),
                    DocumentDate!=null?new XElement("DocumentDate", DocumentDate.Value):null,
                    DocumentOrganization!=null?new XElement("DocumentOrganization", DocumentOrganization):null,
                    ToElementOrNull("EndYear",EndYear),
                    ToElementOrNull("GPA",GPA),
                    Subjects!=null?new XElement("Subjects",Subjects.Select(d => d.ConvertToXElement())):null,
                    ToElementOrNull("StateServicePreparation",StateServicePreparation)
                };
            }
        }

        public class THighEduDiplomaDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов/заверенных копий
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly TDate DocumentDate; //Дата выдачи документа
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500 //us
            public readonly TDocumentNumber RegistrationNumber; //регистрационный номер
            public readonly uint? QualificationTypeID; //Поле зарезервировано (значение не обрабатывается)
            public readonly uint? SpecialityID; //Код направления подготовки (справочник № 10)
            public readonly ushort? SpecializationID; //Поле зарезервировано (значение не обрабатывается)
            public readonly uint? EndYear; //Год окончания
            public readonly float? GPA; //Средний балл

            public THighEduDiplomaDocument(TUID uid, TDocumentNumber documentNumber, TDocumentSeries documentSeries = null, TDate originalReceivedDate = null, TDate documentDate = null, string documentOrganization = null, TDocumentNumber registrationNumber = null, uint? qualificationTypeID = null, uint? specialityID = null, ushort? specializationID = null, uint? endYear = null, float? gpa = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
                RegistrationNumber = registrationNumber;
                QualificationTypeID = qualificationTypeID;
                SpecialityID = specialityID;
                SpecializationID = specializationID;
                EndYear = endYear;
                GPA = gpa;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    new XElement("DocumentSeries", DocumentSeries.Value),
                    new XElement("DocumentNumber", DocumentNumber.Value),
                    DocumentDate!=null?new XElement("DocumentDate", DocumentDate.Value):null,
                    DocumentOrganization!=null?new XElement("DocumentOrganization", DocumentOrganization):null,
                    RegistrationNumber!=null?new XElement("RegistrationNumber",RegistrationNumber.Value):null,
                    ToElementOrNull("QualificationTypeID",QualificationTypeID),
                    ToElementOrNull("SpecialityID",SpecialityID),
                    ToElementOrNull("SpecializationID",SpecializationID),
                    ToElementOrNull("EndYear",EndYear),
                    ToElementOrNull("GPA",GPA)
                };
            }
        }

        public class TPostGraduateDiplomaDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов/заверенных копий
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly TDate DocumentDate; //Дата выдачи документа
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500 //us
            public readonly TDocumentNumber RegistrationNumber; //регистрационный номер
            public readonly uint? QualificationTypeID; //Поле зарезервировано (значение не обрабатывается)
            public readonly uint? SpecialityID; //Код направления подготовки (справочник № 10)
            public readonly ushort? SpecializationID; //Поле зарезервировано (значение не обрабатывается)
            public readonly uint? EndYear; //Год окончания
            public readonly float? GPA; //Средний балл

            public TPostGraduateDiplomaDocument(TUID uid, TDocumentNumber documentNumber, TDocumentSeries documentSeries = null, TDate originalReceivedDate = null, TDate documentDate = null, string documentOrganization = null, TDocumentNumber registrationNumber = null, uint? qualificationTypeID = null, uint? specialityID = null, ushort? specializationID = null, uint? endYear = null, float? gpa = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
                RegistrationNumber = registrationNumber;
                QualificationTypeID = qualificationTypeID;
                SpecialityID = specialityID;
                SpecializationID = specializationID;
                EndYear = endYear;
                GPA = gpa;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    new XElement("DocumentNumber", DocumentNumber.Value),
                    DocumentDate!=null?new XElement("DocumentDate", DocumentDate.Value):null,
                    DocumentOrganization!=null?new XElement("DocumentOrganization", DocumentOrganization):null,
                    RegistrationNumber!=null?new XElement("RegistrationNumber",RegistrationNumber.Value):null,
                    ToElementOrNull("QualificationTypeID",QualificationTypeID),
                    ToElementOrNull("SpecialityID",SpecialityID),
                    ToElementOrNull("SpecializationID",SpecializationID),
                    ToElementOrNull("EndYear",EndYear),
                    ToElementOrNull("GPA",GPA)
                };
            }
        }

        public class TPhDDiplomaDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов/заверенных копий
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly TDate DocumentDate; //Дата выдачи документа
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500 //us
            public readonly TDocumentNumber RegistrationNumber; //регистрационный номер
            public readonly uint? QualificationTypeID; //Поле зарезервировано (значение не обрабатывается)
            public readonly uint? SpecialityID; //Код направления подготовки (справочник № 10)
            public readonly ushort? SpecializationID; //Поле зарезервировано (значение не обрабатывается)
            public readonly uint? EndYear; //Год окончания
            public readonly float? GPA; //Средний балл

            public TPhDDiplomaDocument(TUID uid, TDocumentNumber documentNumber, TDocumentSeries documentSeries = null, TDate originalReceivedDate = null, TDate documentDate = null, string documentOrganization = null, TDocumentNumber registrationNumber = null, uint? qualificationTypeID = null, uint? specialityID = null, ushort? specializationID = null, uint? endYear = null, float? gpa = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
                RegistrationNumber = registrationNumber;
                QualificationTypeID = qualificationTypeID;
                SpecialityID = specialityID;
                SpecializationID = specializationID;
                EndYear = endYear;
                GPA = gpa;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    new XElement("DocumentNumber", DocumentNumber.Value),
                    DocumentDate!=null?new XElement("DocumentDate", DocumentDate.Value):null,
                    DocumentOrganization!=null?new XElement("DocumentOrganization", DocumentOrganization):null,
                    RegistrationNumber!=null?new XElement("RegistrationNumber",RegistrationNumber.Value):null,
                    ToElementOrNull("QualificationTypeID",QualificationTypeID),
                    ToElementOrNull("SpecialityID",SpecialityID),
                    ToElementOrNull("SpecializationID",SpecializationID),
                    ToElementOrNull("EndYear",EndYear),
                    ToElementOrNull("GPA",GPA)
                };
            }
        }

        public class TMiddleEduDiplomaDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов/заверенных копий
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly TDate DocumentDate; //Дата выдачи документа
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500 //us
            public readonly TDocumentNumber RegistrationNumber; //регистрационный номер
            public readonly uint? QualificationTypeID; //Поле зарезервировано (значение не обрабатывается)
            public readonly uint? SpecialityID; //Код направления подготовки (справочник № 10)
            public readonly ushort? SpecializationID; //Поле зарезервировано (значение не обрабатывается)
            public readonly uint? EndYear; //Год окончания
            public readonly float? GPA; //Средний балл
            public readonly bool? StateServicePreparation; //Осуществляет подготовку к военной или иной государственной службе (преимущественное право)

            public TMiddleEduDiplomaDocument(TUID uid, TDocumentNumber documentNumber, TDocumentSeries documentSeries = null, TDate originalReceivedDate = null, TDate documentDate = null, string documentOrganization = null, TDocumentNumber registrationNumber = null, uint? qualificationTypeID = null, uint? specialityID = null, ushort? specializationID = null, uint? endYear = null, float? gpa = null, bool? stateServicePreparation = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
                RegistrationNumber = registrationNumber;
                QualificationTypeID = qualificationTypeID;
                SpecialityID = specialityID;
                SpecializationID = specializationID;
                EndYear = endYear;
                GPA = gpa;
                StateServicePreparation = stateServicePreparation;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    new XElement("DocumentNumber", DocumentNumber.Value),
                    DocumentDate!=null?new XElement("DocumentDate", DocumentDate.Value):null,
                    DocumentOrganization!=null?new XElement("DocumentOrganization", DocumentOrganization):null,
                    RegistrationNumber!=null?new XElement("RegistrationNumber",RegistrationNumber.Value):null,
                    QualificationTypeID!=null?new XElement("QualificationTypeID",QualificationTypeID):null,
                    ToElementOrNull("SpecialityID",SpecialityID),
                    ToElementOrNull("SpecializationID",SpecializationID),
                    ToElementOrNull("EndYear",EndYear),
                    ToElementOrNull("GPA",GPA),
                    ToElementOrNull("StateServicePreparation",StateServicePreparation)
                };
            }
        }

        public class TBasicDiplomaDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов/заверенных копий
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly TDate DocumentDate; //Дата выдачи документа
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500 //us
            public readonly TDocumentNumber RegistrationNumber; //регистрационный номер
            public readonly uint? QualificationTypeID; //Поле зарезервировано (значение не обрабатывается)
            public readonly uint? ProfessionID; //Поле зарезервировано (значение не обрабатывается)
            public readonly uint? SpecialityID; //Код направления подготовки (справочник № 10)
            public readonly uint? EndYear; //Год окончания
            public readonly float? GPA; //Средний балл
            public readonly bool? StateServicePreparation; //Осуществляет подготовку к военной или иной государственной службе (преимущественное право)

            public TBasicDiplomaDocument(TUID uid, TDocumentNumber documentNumber, TDocumentSeries documentSeries = null, TDate originalReceivedDate = null, TDate documentDate = null, string documentOrganization = null, TDocumentNumber registrationNumber = null, uint? qualificationTypeID = null, uint? specialityID = null, ushort? professionID = null, uint? endYear = null, float? gpa = null, bool? stateServicePreparation = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
                RegistrationNumber = registrationNumber;
                QualificationTypeID = qualificationTypeID;
                SpecialityID = specialityID;
                ProfessionID = professionID;
                EndYear = endYear;
                GPA = gpa;
                StateServicePreparation = stateServicePreparation;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    DocumentSeries!=null? new XElement("DocumentSeries", DocumentSeries.Value):null,
                    new XElement("DocumentNumber", DocumentNumber.Value),
                    DocumentDate!=null?new XElement("DocumentDate", DocumentDate.Value):null,
                    DocumentOrganization!=null?new XElement("DocumentOrganization", DocumentOrganization):null,
                    RegistrationNumber!=null?new XElement("RegistrationNumber",RegistrationNumber.Value):null,
                    ToElementOrNull("QualificationTypeID",QualificationTypeID),
                    ToElementOrNull("SpecialityID",SpecialityID),
                    ToElementOrNull("ProfessionID",ProfessionID),
                    ToElementOrNull("EndYear",EndYear),
                    ToElementOrNull("GPA",GPA),
                    ToElementOrNull("StateServicePreparation",StateServicePreparation)
                };
            }
        }

        public class TIncomplHighEduDiplomaDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов/заверенных копий
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly TDate DocumentDate; //Дата выдачи документа
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500 //us
            public readonly TDocumentNumber RegistrationNumber; //регистрационный номер
            public readonly uint? QualificationTypeID; //Поле зарезервировано (значение не обрабатывается)
            public readonly uint? SpecialityID; //Код направления подготовки (справочник № 10)
            public readonly ushort? SpecializationID; //Поле зарезервировано (значение не обрабатывается)

            public TIncomplHighEduDiplomaDocument(TUID uid, TDocumentNumber documentNumber, TDocumentSeries documentSeries = null, TDate originalReceivedDate = null, TDate documentDate = null, string documentOrganization = null, TDocumentNumber registrationNumber = null, uint? qualificationTypeID = null, uint? specialityID = null, ushort? specializationID = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
                RegistrationNumber = registrationNumber;
                QualificationTypeID = qualificationTypeID;
                SpecialityID = specialityID;
                SpecializationID = specializationID;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    new XElement("DocumentNumber", DocumentNumber.Value),
                    DocumentDate!=null?new XElement("DocumentDate", DocumentDate.Value):null,
                    DocumentOrganization!=null?new XElement("DocumentOrganization", DocumentOrganization):null,
                    RegistrationNumber!=null?new XElement("RegistrationNumber",RegistrationNumber.Value):null,
                    ToElementOrNull("QualificationTypeID",QualificationTypeID),
                    ToElementOrNull("SpecialityID",SpecialityID),
                    ToElementOrNull("SpecializationID",SpecializationID)
                };
            }
        }

        public class TAcademicDiplomaDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов/заверенных копий
            public readonly TDocumentNumber RegistrationNumber; //регистрационный номер
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly TDate DocumentDate; //Дата выдачи документа
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500 //us
            public readonly uint? QualificationTypeID; //Поле зарезервировано (значение не обрабатывается)
            public readonly uint? SpecialityID; //Код направления подготовки (справочник № 10)
            public readonly ushort? SpecializationID; //Поле зарезервировано (значение не обрабатывается)

            public TAcademicDiplomaDocument(TUID uid, TDocumentNumber documentNumber, TDocumentSeries documentSeries = null, TDate originalReceivedDate = null, TDocumentNumber registrationNumber = null, TDate documentDate = null, string documentOrganization = null, uint? qualificationTypeID = null, uint? specialityID = null, ushort? specializationID = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                RegistrationNumber = registrationNumber;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
                QualificationTypeID = qualificationTypeID;
                SpecialityID = specialityID;
                SpecializationID = specializationID;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    new XElement("DocumentNumber", DocumentNumber.Value),
                    DocumentDate!=null?new XElement("DocumentDate", DocumentDate.Value):null,
                    DocumentOrganization!=null?new XElement("DocumentOrganization", DocumentOrganization):null,
                    RegistrationNumber!=null?new XElement("RegistrationNumber",RegistrationNumber.Value):null,
                    ToElementOrNull("QualificationTypeID",QualificationTypeID),
                    ToElementOrNull("SpecialityID",SpecialityID),
                    ToElementOrNull("SpecializationID",SpecializationID)
                };
            }
        }

        public class TEduCustomDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов/заверенных копий
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа
            public readonly TDate DocumentDate; //Дата выдачи документа //s
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500 //us
            public readonly string DocumentTypeNameText; //Наименование документа //500

            public TEduCustomDocument(TUID uid, TDate documentDate, string documentTypeNameText, TDate originalReceivedDate = null, TDocumentSeries documentSeries = null, TDocumentNumber documentNumber = null, string documentOrganization = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
                DocumentTypeNameText = documentTypeNameText;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    DocumentNumber!=null?new XElement("DocumentNumber", DocumentNumber.Value):null,
                    new XElement("DocumentDate", DocumentDate.Value),
                    DocumentOrganization!=null?new XElement("DocumentOrganization", DocumentOrganization):null,
                    DocumentTypeNameText!=null?new XElement("DocumentTypeNameText",DocumentTypeNameText):null
                };
            }
        }

        public class TCompatriotDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly uint CompariotCategoryID; //Тип документа, подтверждающего принадлежность к соотечественникам (справочник № 44)
            public readonly TDocumentName DocumentName; //Наименование документа
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа
            public readonly TDate DocumentDate; //Дата выдачи документа //s
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500

            public TCompatriotDocument(TUID uid, uint compariotCategoryID, TDate documentDate, string documentOrganization, TDate originalReceivedDate = null, TDocumentName documentName = null, TDocumentSeries documentSeries = null, TDocumentNumber documentNumber = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                CompariotCategoryID = compariotCategoryID;
                DocumentName = documentName;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    new XElement("CompariotCategoryID", CompariotCategoryID),
                    DocumentName !=null?new XElement("DocumentName", DocumentName.Value):null,
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    DocumentNumber!=null?new XElement("DocumentNumber", DocumentNumber.Value):null,
                    new XElement("DocumentDate", DocumentDate.Value),
                    new XElement("DocumentOrganization", DocumentOrganization)
                };
            }
        }

        public class TInstitutionDocument : IXElemementsConvertable
        {
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly TDate DocumentDate; //Дата выдачи документа
            public readonly uint? DocumentTypeID; //Тип документа (справочник №33)

            public TInstitutionDocument(TDocumentNumber documentNumber, TDate documentDate = null, uint? documentTypeID = null)
            {
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentTypeID = documentTypeID;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("DocumentNumber", DocumentNumber.Value) ,
                    DocumentDate !=null? new XElement("DocumentDate", DocumentDate.Value):null,
                    ToElementOrNull("DocumentTypeID", DocumentTypeID)
                };
            }
        }

        public class TVeteranDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly uint VeteranCategoryID; //Тип документа, подтверждающего принадлежность к участникам и ветеранам боевых действий (справочник № 45)
            public readonly TDocumentName DocumentName; //Наименование документа
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа
            public readonly TDate DocumentDate; //Дата выдачи документа //s
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500

            public TVeteranDocument(TUID uid, uint veteranCategoryID, TDate documentDate, string documentOrganization, TDocumentName documentName = null, TDate originalReceivedDate = null, TDocumentSeries documentSeries = null, TDocumentNumber documentNumber = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                VeteranCategoryID = veteranCategoryID;
                DocumentName = documentName;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    new XElement("VeteranCategoryID", VeteranCategoryID),
                    DocumentName !=null?new XElement("DocumentName", DocumentName.Value):null,
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    DocumentNumber!=null?new XElement("DocumentNumber", DocumentNumber.Value):null,
                    new XElement("DocumentDate", DocumentDate.Value),
                    new XElement("DocumentOrganization", DocumentOrganization)
                };
            }
        }

        public class TPauperDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly TDocumentName DocumentName; //Наименование документа
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа
            public readonly TDate DocumentDate; //Дата выдачи документа //s
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500

            public TPauperDocument(TUID uid, TDate documentDate, string documentOrganization, TDocumentName documentName = null, TDate originalReceivedDate = null, TDocumentSeries documentSeries = null, TDocumentNumber documentNumber = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentName = documentName;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    DocumentName !=null?new XElement("DocumentName", DocumentName.Value):null,
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    DocumentNumber!=null?new XElement("DocumentNumber", DocumentNumber.Value):null,
                    new XElement("DocumentDate", DocumentDate.Value),
                    new XElement("DocumentOrganization", DocumentOrganization)
                };
            }
        }

        public class TParentsLostDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly uint ParentsLostCategoryID; //Тип документа, подтверждающего принадлежность родителей и опекунов к погибшим в связи с исполнением служебных обязанностей (справочник № 46)
            public readonly TDocumentName DocumentName; //Наименование документа
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа
            public readonly TDate DocumentDate; //Дата выдачи документа //s
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500

            public TParentsLostDocument(TUID uid, uint parentsLostCategoryID, TDate documentDate, string documentOrganization, TDocumentName documentName = null, TDate originalReceivedDate = null, TDocumentSeries documentSeries = null, TDocumentNumber documentNumber = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                ParentsLostCategoryID = parentsLostCategoryID;
                DocumentName = documentName;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    new XElement("ParentsLostCategoryID", ParentsLostCategoryID),
                    DocumentName !=null?new XElement("DocumentName", DocumentName.Value):null,
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    DocumentNumber!=null?new XElement("DocumentNumber", DocumentNumber.Value):null,
                    new XElement("DocumentDate", DocumentDate.Value),
                    new XElement("DocumentOrganization", DocumentOrganization)
                };
            }
        }

        public class TStateEmployeeDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly uint StateEmployeeCategoryID; //Тип документа, подтверждающего принадлежность к сотрудникам государственных органов Российской Федерации (справочник № 47)
            public readonly TDocumentName DocumentName; //Наименование документа
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа
            public readonly TDate DocumentDate; //Дата выдачи документа //s
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500

            public TStateEmployeeDocument(TUID uid, uint stateEmployeeCategoryID, TDate documentDate, string documentOrganization, TDocumentName documentName = null, TDate originalReceivedDate = null, TDocumentSeries documentSeries = null, TDocumentNumber documentNumber = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                StateEmployeeCategoryID = stateEmployeeCategoryID;
                DocumentName = documentName;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    new XElement("StateEmployeeCategoryID", StateEmployeeCategoryID),
                    DocumentName !=null?new XElement("DocumentName", DocumentName.Value):null,
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    DocumentNumber!=null?new XElement("DocumentNumber", DocumentNumber.Value):null,
                    new XElement("DocumentDate", DocumentDate.Value),
                    new XElement("DocumentOrganization", DocumentOrganization)
                };
            }
        }

        public class TRadiationWorkDocument : IXElemementsConvertable
        {
            public readonly TUID UID; //Идентификатор //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly uint RadiationWorkCategoryID; //Тип документа, подтверждающего участие в работах на радиационных объектах или воздействие радиации (справочник № 48)
            public readonly TDocumentName DocumentName; //Наименование документа
            public readonly TDocumentSeries DocumentSeries; //Серия документа
            public readonly TDocumentNumber DocumentNumber; //Номер документа
            public readonly TDate DocumentDate; //Дата выдачи документа //s
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500

            public TRadiationWorkDocument(TUID uid, uint radiationWorkCategoryID, TDate documentDate, string documentOrganization, TDocumentName documentName = null, TDate originalReceivedDate = null, TDocumentSeries documentSeries = null, TDocumentNumber documentNumber = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                RadiationWorkCategoryID = radiationWorkCategoryID;
                DocumentName = documentName;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
            }

            public object[] ConvertToXElements()
            {
                return new object[]
                {
                    new XElement("UID", UID.Value) ,
                    OriginalReceivedDate !=null? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value):null,
                    new XElement("RadiationWorkCategoryID", RadiationWorkCategoryID),
                    DocumentName !=null?new XElement("DocumentName", DocumentName.Value):null,
                    DocumentSeries!=null?new XElement("DocumentSeries", DocumentSeries.Value):null,
                    DocumentNumber!=null?new XElement("DocumentNumber", DocumentNumber.Value):null,
                    new XElement("DocumentDate", DocumentDate.Value),
                    new XElement("DocumentOrganization", DocumentOrganization)
                };
            }
        }

        #endregion

        public class PackageData : IXElemementConvertable
        {
            public readonly CampaignInfo CampaignInfo; //Информация о приемных кампаниях
            public readonly AdmissionInfo AdmissionInfo; //Сведения об объеме и структуре приема
            public readonly List<InstitutionAchievement> InstitutionAchievements; //Индивидуальные достижения, учитываемые образовательной организацией
            public readonly List<TargetOrganizationImp> TargetOrganizations; //Целевые организации
            public readonly List<Application> Applications; //Заявления абитуриентов
            public readonly Orders Orders; //Приказы

            public PackageData(CampaignInfo campaignInfo = null, AdmissionInfo admissionInfo = null, List<InstitutionAchievement> institutionAchievements = null, List<TargetOrganizationImp> targetOrganizations = null, List<Application> applications = null, Orders orders = null)
            {
                CampaignInfo = campaignInfo;
                AdmissionInfo = admissionInfo;
                InstitutionAchievements = institutionAchievements;
                TargetOrganizations = targetOrganizations;
                Applications = applications;
                Orders = orders;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("PackageData",
                    CampaignInfo != null ? CampaignInfo.ConvertToXElement() : null,
                    AdmissionInfo != null ? AdmissionInfo.ConvertToXElement() : null,
                    InstitutionAchievements != null ? new XElement("InstitutionAchievements", InstitutionAchievements.Select(a => a.ConvertToXElement())) : null,
                    TargetOrganizations != null ? new XElement("TargetOrganizations", TargetOrganizations.Select(o => o.ConvertToXElement())) : null,
                    Applications != null ? new XElement("Applications", Applications.Select(a => a.ConvertToXElement())) : null,
                    Orders != null ? Orders.ConvertToXElement() : null
                    );
            }
        }

        #region Campaigns

        public class CampaignInfo : IXElemementConvertable
        {
            public readonly List<Campaign> Campaigns; //Элементы приемной кампании //s

            public CampaignInfo(List<Campaign> campaigns)
            {
                Campaigns = campaigns;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("CampaignInfo",
                    new XElement("Campaigns", Campaigns.Select(c => c.ConvertToXElement()))
                    );
            }
        }

        public class Campaign : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly string Name; //Название //100
            public readonly uint YearStart; //Год начала
            public readonly uint YearEnd; //Год окончания
            public readonly List<EducationFormID> EducationForms; //Формы обучения //s
            public readonly uint StatusID; //Статус (справочник №34)
            public readonly List<EducationLevelID> EducationLevels; //Уровни образования //s
            public readonly uint CampaignTypeID; //Тип приемной кампании (справочник №38)

            public Campaign(TUID uid, string name, uint yearStart, uint yearEnd, List<EducationFormID> educationForms, uint statusID, List<EducationLevelID> educationLevels, uint campaignTypeID)
            {
                UID = uid;
                Name = name;
                YearStart = yearStart;
                YearEnd = yearEnd;
                EducationForms = educationForms;
                StatusID = statusID;
                EducationLevels = educationLevels;
                CampaignTypeID = campaignTypeID;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("Campaign",
                    new XElement("UID", UID.Value),
                    new XElement("Name", Name),
                    new XElement("YearStart", YearStart),
                    new XElement("YearEnd", YearEnd),
                    new XElement("EducationForms", EducationForms.Select(i => i.ConvertToXElement())),
                    new XElement("StatusID", StatusID),
                    new XElement("EducationLevels", EducationLevels.Select(i => i.ConvertToXElement())),
                    new XElement("CampaignTypeID", CampaignTypeID)
                    );
            }
        }

        public class EducationFormID : IXElemementConvertable
        {
            public readonly uint Value; //ИД формы обучения (справочник №14)

            public EducationFormID(uint value)
            {
                Value = value;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("EducationFormID", Value);
            }
        }

        public class EducationLevelID : IXElemementConvertable
        {
            public readonly uint Value; //ИД Уровня образования (справочник №2)

            public EducationLevelID(uint value)
            {
                Value = value;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("EducationLevelID", Value);
            }
        }

        #endregion

        #region Admission

        public class AdmissionInfo : IXElemementConvertable
        {
            public readonly List<AVItem> AdmissionVolume; //Объем приема (контрольные цифры)
            public readonly List<DAVItem> DistributedAdmissionVolume; //Объем приема, распределенный по уровню бюджета
            public readonly List<CompetitiveGroup> CompetitiveGroups; //Конкурсы

            public AdmissionInfo(List<AVItem> admissionVolume = null, List<DAVItem> distributedAdmissionVolume = null, List<CompetitiveGroup> competitiveGroups = null)
            {
                AdmissionVolume = admissionVolume;
                DistributedAdmissionVolume = distributedAdmissionVolume;
                CompetitiveGroups = competitiveGroups;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("AdmissionInfo",
                    AdmissionVolume != null ? new XElement("AdmissionVolume", AdmissionVolume.Select(i => i.ConvertToXElement())) : null,
                    DistributedAdmissionVolume != null ? new XElement("DistributedAdmissionVolume", DistributedAdmissionVolume.Select(i => i.ConvertToXElement())) : null,
                    CompetitiveGroups != null ? new XElement("CompetitiveGroups", CompetitiveGroups.Select(g => g.ConvertToXElement())) : null
                    );
            }
        }

        public class AVItem : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TUID CampaignUID; //Идентификатор приемной кампании (UID) //s
            public readonly uint EducationLevelID; //ИД уровня образования (справочник №2)
            public readonly uint DirectionID; //ИД направления подготовки (справочник №10)
            public readonly uint? NumberBudgetO; //Бюджетные места очной формы
            public readonly uint? NumberBudgetOZ; //Бюджетные места очно-заочной формы
            public readonly uint? NumberBudgetZ; //Бюджетные места заочной формы
            public readonly uint? NumberPaidO; //Места с оплатой обучения очной формы
            public readonly uint? NumberPaidOZ; //Места с оплатой обучения очно-заочной формы
            public readonly uint? NumberPaidZ; //Места с оплатой обучения заочной формы
            public readonly uint? NumberTargetO; //Места целевого приема очной формы
            public readonly uint? NumberTargetOZ; //Места целевого приема очно-заочной формы
            public readonly uint? NumberTargetZ; //Места целевого приема заочной формы
            public readonly uint? NumberQuotaO; //Места приёма по квоте лиц, имеющих особые права, очное обучение
            public readonly uint? NumberQuotaOZ; //Места приёма по квоте лиц, имеющих особые права, очно-заочное (вечернее) обучение
            public readonly uint? NumberQuotaZ; //Места приёма по квоте лиц, имеющих особые права, заочное обучение
			public readonly bool IsPlan = true;

            public AVItem(TUID uid, TUID campaignUID, uint educationLevelID, uint directionID, uint? numberBudgetO = null, 
				uint? numberBudgetOZ = null, uint? numberBudgetZ = null, uint? numberPaidO = null, uint? numberPaidOZ = null, 
				uint? numberPaidZ = null, uint? numberTargetO = null, uint? numberTargetOZ = null, uint? numberTargetZ = null, 
				uint? numberQuotaO = null, uint? numberQuotaOZ = null, uint? numberQuotaZ = null, bool IsPlan = true)
            {
                UID = uid;
                CampaignUID = campaignUID;
                EducationLevelID = educationLevelID;
                DirectionID = directionID;
                NumberBudgetO = numberBudgetO;
                NumberBudgetOZ = numberBudgetOZ;
                NumberBudgetZ = numberBudgetZ;
                NumberPaidO = numberPaidO;
                NumberPaidOZ = numberPaidOZ;
                NumberPaidZ = numberPaidZ;
                NumberTargetO = numberTargetO;
                NumberTargetOZ = numberTargetOZ;
                NumberTargetZ = numberTargetZ;
                NumberQuotaO = numberQuotaO;
                NumberQuotaOZ = numberQuotaOZ;
                NumberQuotaZ = numberQuotaZ;
				IsPlan = true;

			}

            public XElement ConvertToXElement()
            {
				return new XElement("Item",
					new XElement("UID", UID.Value),
					new XElement("CampaignUID", CampaignUID.Value),
					new XElement("EducationLevelID", EducationLevelID),
					new XElement("DirectionID", DirectionID),
					ToElementOrNull("NumberBudgetO", NumberBudgetO),
					ToElementOrNull("NumberBudgetOZ", NumberBudgetOZ),
					ToElementOrNull("NumberBudgetZ", NumberBudgetZ),
					ToElementOrNull("NumberPaidO", NumberPaidO),
					ToElementOrNull("NumberPaidOZ", NumberPaidOZ),
					ToElementOrNull("NumberPaidZ", NumberPaidZ),
					ToElementOrNull("NumberTargetO", NumberTargetO),
					ToElementOrNull("NumberTargetOZ", NumberTargetOZ),
					ToElementOrNull("NumberTargetZ", NumberTargetZ),
					ToElementOrNull("NumberQuotaO", NumberQuotaO),
					ToElementOrNull("NumberQuotaOZ", NumberQuotaOZ),
					ToElementOrNull("NumberQuotaZ", NumberQuotaZ),
					new XElement("IsPlan", IsPlan)
					);
            }
        }

        public class DAVItem : IXElemementConvertable
        {
            public readonly TUID AdmissionVolumeUID; //Идентификатор объема приема по направлению подготовки //s
            public readonly uint LevelBudget; //ИД уровня бюджета (справочник №35)
            public readonly uint? NumberBudgetO; //Бюджетные места очной формы
            public readonly uint? NumberBudgetOZ; //Бюджетные места очно-заочной формы
            public readonly uint? NumberBudgetZ; //Бюджетные места заочной формы
            public readonly uint? NumberTargetO; //Места целевого приема очной формы
            public readonly uint? NumberTargetOZ; //Места целевого приема очно-заочной формы
            public readonly uint? NumberTargetZ; //Места целевого приема заочной формы
            public readonly uint? NumberQuotaO; //Места приёма по квоте лиц, имеющих особые права, очное обучение
            public readonly uint? NumberQuotaOZ; //Места приёма по квоте лиц, имеющих особые права, очно-заочное (вечернее) обучение
            public readonly uint? NumberQuotaZ; //Места приёма по квоте лиц, имеющих особые права, заочное обучение
			public readonly bool IsPlan = true;

            public DAVItem(TUID admissionVolumeUID, uint levelBudget, uint? numberBudgetO = null, 
				uint? numberBudgetOZ = null, uint? numberBudgetZ = null, uint? numberTargetO = null,
				uint? numberTargetOZ = null, uint? numberTargetZ = null, uint? numberQuotaO = null, 
				uint? numberQuotaOZ = null, uint? numberQuotaZ = null, bool IsPlan = true)
            {
                AdmissionVolumeUID = admissionVolumeUID;
                LevelBudget = levelBudget;
                NumberBudgetO = numberBudgetO;
                NumberBudgetOZ = numberBudgetOZ;
                NumberBudgetZ = numberBudgetZ;
                NumberTargetO = numberTargetO;
                NumberTargetOZ = numberTargetOZ;
                NumberTargetZ = numberTargetZ;
                NumberQuotaO = numberQuotaO;
                NumberQuotaOZ = numberQuotaOZ;
                NumberQuotaZ = numberQuotaZ;
				IsPlan = true;

			}

            public XElement ConvertToXElement()
            {
                return new XElement("Item",
                    new XElement("AdmissionVolumeUID", AdmissionVolumeUID.Value),
                    new XElement("LevelBudget", LevelBudget),
                    ToElementOrNull("NumberBudgetO", NumberBudgetO),
                    ToElementOrNull("NumberBudgetOZ", NumberBudgetOZ),
                    ToElementOrNull("NumberBudgetZ", NumberBudgetZ),
                    ToElementOrNull("NumberTargetO", NumberTargetO),
                    ToElementOrNull("NumberTargetOZ", NumberTargetOZ),
                    ToElementOrNull("NumberTargetZ", NumberTargetZ),
                    ToElementOrNull("NumberQuotaO", NumberQuotaO),
                    ToElementOrNull("NumberQuotaOZ", NumberQuotaOZ),
                    ToElementOrNull("NumberQuotaZ", NumberQuotaZ),
					new XElement("IsPlan", IsPlan)

					);
            }
        }

        public class CompetitiveGroup : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TUID CampaignUID; //Идентификатор приемной кампании (UID) //s
            public readonly string Name; //Наименование конкурса //250
            public readonly uint EducationLevelID; //ИД уровня образования (справочник №2)
            public readonly uint EducationSourceID; //ИД источника финансирования (справочник №15)
            public readonly uint EducationFormID; //ИД формы обучения (справочник №14)
            public readonly uint DirectionID; //ИД направления подготовки (справочник №10)
            public readonly List<EduProgram> EduPrograms; //Образовательные программы
            public readonly bool? IsAdditional; //Конкурс для дополнительного набора
            public readonly CompetitiveGroupItem CompetitiveGroupItem; //Места в конкурсе
            public readonly List<TargetOrganization> TargetOrganizations; //Сведения о целевом наборе
            public readonly List<CommonBenefitItem> CommonBenefit; //Сведения об общей льготе (без в.и.) (олимпиада школьников)
            public readonly List<EntranceTestItem> EntranceTestItems; //Вступительные испытания конкурса
			public readonly uint LevelBudget = 1;

            public CompetitiveGroup(TUID uid, TUID campaignUID, string name, uint educationLevelID, uint educationSourceID, uint educationFormID,
				uint directionID, List<EduProgram> eduPrograms = null, bool? isAdditional = null, CompetitiveGroupItem competitiveGroupItem = null,
				List<TargetOrganization> targetOrganizations = null, List<CommonBenefitItem> commonBenefit = null, List<EntranceTestItem> entranceTestItems = null,
				uint LevelBudget = 1)
            {
                UID = uid;
                CampaignUID = campaignUID;
                Name = name;
                EducationLevelID = educationLevelID;
                EducationSourceID = educationSourceID;
                EducationFormID = educationFormID;
                DirectionID = directionID;
                EduPrograms = eduPrograms;
                IsAdditional = isAdditional;
                CompetitiveGroupItem = competitiveGroupItem;
                TargetOrganizations = targetOrganizations;
                CommonBenefit = commonBenefit;
                EntranceTestItems = entranceTestItems;
				LevelBudget = 1;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("CompetitiveGroup",
                    new XElement("UID", UID.Value),
                    new XElement("CampaignUID", CampaignUID.Value),
                    new XElement("Name", Name),
                    new XElement("EducationLevelID", EducationLevelID),
                    new XElement("EducationSourceID", EducationSourceID),
                    new XElement("EducationFormID", EducationFormID),
                    new XElement("DirectionID", DirectionID),
                    EduPrograms != null ? new XElement("EduPrograms", EduPrograms.Select(p => p.ConvertToXElement())) : null,
                    ToElementOrNull("IsAdditional", IsAdditional),
                    CompetitiveGroupItem != null ? CompetitiveGroupItem.ConvertToXElement() : null,
                    TargetOrganizations != null ? new XElement("TargetOrganizations", TargetOrganizations.Select(o => o.ConvertToXElement())) : null,
                    CommonBenefit != null ? new XElement("CommonBenefit", CommonBenefit.Select(i => i.ConvertToXElement())) : null,
                    EntranceTestItems != null ? new XElement("EntranceTestItems", EntranceTestItems.Select(i => i.ConvertToXElement())) : null,
					new XElement("LevelBudget",LevelBudget)
                    );
            }
        }

        public class EduProgram : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly string Name; //Наименование ОП //200
            public readonly string Code; //Код ОП //us //10

            public EduProgram(TUID uid, string name, string code = null)
            {
                UID = uid;
                Name = name;
                Code = code;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("EduProgram",
                    new XElement("UID", UID.Value),
                    new XElement("Name", Name),
                    Code != null ? new XElement("Code", Code) : null
                    );
            }
        }

        public class CompetitiveGroupItem : IXElemementConvertable
        {
            public enum Variants : byte
            {
                NumberBudgetO, //Бюджетные места очной формы
                NumberBudgetOZ, //Бюджетные места очно-заочной формы
                NumberBudgetZ, //Бюджетные места заочной формы
                NumberPaidO, //Места с оплатой обучения очной формы
                NumberPaidOZ, //Места с оплатой обучения очно-заочной формы
                NumberPaidZ, //Места с оплатой обучения заочной формы
                NumberQuotaO, //Места по квоте для лиц, имеющих особое право, очная форма
                NumberQuotaOZ, //Места по квоте для лиц, имеющих особое право, очно-заочная (вечерняя) форма
                NumberQuotaZ, //Места по квоте для лиц, имеющих особое право, заочная форма
                NumberTargetO, //Места целевого приема очной формы (без разделения по целевым организациям)
                NumberTargetOZ, //Места целевого приема очно-заочной формы (без разделения по целевым организациям)
                NumberTargetZ //Места целевого приема заочной формы (без разделения по целевым организациям)
            }
            public readonly Variants Choice;
            public readonly uint Number; //Места

            public CompetitiveGroupItem(Variants choice, uint number)
            {
                Choice = choice;
                Number = number;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("CompetitiveGroupItem",
                    new XElement(System.Enum.GetName(typeof(Variants), Choice), Number)
                    );
            }
        }

        public class TargetOrganization : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly CompetitiveGroupTargetItem CompetitiveGroupTargetItem; //Места для целевого приема //s

            public TargetOrganization(TUID uid, CompetitiveGroupTargetItem competitiveGroupTargetItem)
            {
                UID = uid;
                CompetitiveGroupTargetItem = competitiveGroupTargetItem;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("TargetOrganization",
                    new XElement("UID", UID.Value),
                    CompetitiveGroupTargetItem.ConvertToXElement()
                    );

            }
        }

        public class CompetitiveGroupTargetItem : IXElemementConvertable
        {
            public enum Variants : byte
            {
                NumberTargetO, //Места целевого приема очной формы
                NumberTargetOZ, //Места целевого приема очно-заочной формы
                NumberTargetZ //Места целевого приема заочной формы
            }
            public readonly Variants Choice;
            public readonly uint Number; //Места

            public CompetitiveGroupTargetItem(Variants choice, uint number)
            {
                Choice = choice;
                Number = number;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("CompetitiveGroupTargetItem",
                    new XElement(System.Enum.GetName(typeof(Variants), Choice), Number)
                    );
            }
        }

        public class CommonBenefitItem : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly List<OlympicDiplomTypeID> OlympicDiplomTypes; //Типы дипломов //s
            public readonly uint BenefitKindID; //Вид льготы (справочник №30)
            public readonly bool IsForAllOlympics; //Флаг действия льготы для всех олимпиад
            public readonly bool? IsCreative; //Творческие олимпиады 
            public readonly bool? IsAthletic; //Олимпиады в области спорта 
            public readonly uint? LevelForAllOlympics; //Уровни для всех олимпиад (справочник №3)
            public readonly List<ProfileID> ProfileForAllOlympics; //Профили для всех олимпиад
            public readonly uint? ClassForAllOlympics; //Классы для всех олимпиад (справочник №40)
            public readonly List<OlympicID> Olympics; //Перечень олимпиад, для которых действует льгота
            public readonly List<Olympic> OlympicsLevels; //Уровни, профили и классы олимпиад
            public readonly MinEgeMarks MinEgeMarks; //Минимальная оценка по предмету

            public CommonBenefitItem(TUID uid, List<OlympicDiplomTypeID> olympicDiplomTypes, uint benefitKindID, bool isForAllOlympics, bool? isCreative = null, bool? isAthletic = null, uint? levelForAllOlympics = null, List<ProfileID> profileForAllOlympics = null, uint? classForAllOlympics = null, List<OlympicID> olympics = null, List<Olympic> olympicsLevels = null, MinEgeMarks minEgeMarks = null)
            {
                UID = uid;
                OlympicDiplomTypes = olympicDiplomTypes;
                BenefitKindID = benefitKindID;
                IsForAllOlympics = isForAllOlympics;
                IsCreative = isCreative;
                IsAthletic = isAthletic;
                LevelForAllOlympics = levelForAllOlympics;
                ProfileForAllOlympics = profileForAllOlympics;
                ClassForAllOlympics = classForAllOlympics;
                Olympics = olympics;
                OlympicsLevels = olympicsLevels;
                MinEgeMarks = minEgeMarks;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("CommonBenefitItem",
                    new XElement("UID", UID.Value),
                    new XElement("OlympicDiplomTypes", OlympicDiplomTypes.Select(t => t.ConvertToXElement())),
                    new XElement("BenefitKindID", BenefitKindID),
                    new XElement("IsForAllOlympics", IsForAllOlympics),
                    ToElementOrNull("IsCreative", IsCreative),
                    ToElementOrNull("IsAthletic", IsAthletic),
                    ToElementOrNull("LevelForAllOlympics", LevelForAllOlympics),
                    ProfileForAllOlympics != null ? new XElement("ProfileForAllOlympics", ProfileForAllOlympics.Select(i => i.ConvertToXElement())) : null,
                    ToElementOrNull("ClassForAllOlympics", ClassForAllOlympics),
                    Olympics != null ? new XElement("Olympics", Olympics.Select(i => i.ConvertToXElement())) : null,
                    OlympicsLevels != null ? new XElement("OlympicsLevels", OlympicsLevels.Select(o => o.ConvertToXElement())) : null,
                    MinEgeMarks != null ? MinEgeMarks.ConvertToXElement() : null
                    );
            }
        }

        public class OlympicDiplomTypeID : IXElemementConvertable
        {
            public readonly uint Value; //ИД типа диплома (справочник №18)

            public OlympicDiplomTypeID(uint value)
            {
                Value = value;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("OlympicDiplomTypeID", Value);
            }
        }

        public class ProfileID : IXElemementConvertable
        {
            public readonly uint Value; //ИД профиля олимпиады (справочник №39)

            public ProfileID(uint value)
            {
                Value = value;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("ProfileID", Value);
            }
        }

        public class OlympicID : IXElemementConvertable
        {
            public readonly uint Value; //ИД олимпиады (справочник №19)

            public OlympicID(uint value)
            {
                Value = value;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("OlympicID", Value);
            }
        }

        public class Olympic : IXElemementConvertable
        {
            public readonly uint OlympicID; //ИД олимпиады
            public readonly uint? LevelID; //Уровни олимпиады (справочник №3)
            public readonly List<ProfileID> Profiles; //Профили олимпиады //s
            public readonly uint ClassID; //Классы олимпиады (справочник №40)

            public Olympic(uint olympicID, uint classID, uint? levelID = null, List<ProfileID> profiles = null)
            {
                OlympicID = olympicID;
                LevelID = levelID;
                Profiles = profiles;
                ClassID = classID;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("Olympic",
                    new XElement("OlympicID", OlympicID),
                    ToElementOrNull("LevelID", LevelID),
                    Profiles != null ? new XElement("Profiles", Profiles.Select(p => p.ConvertToXElement())) : null,
                    new XElement("ClassID", ClassID)
                    );
            }
        }

        public class MinEgeMarks : IXElemementConvertable
        {
            public readonly MinMarks MinMarks; //Минимальная оценка по предмету

            public MinEgeMarks(MinMarks minMarks)
            {
                MinMarks = minMarks;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("MinEgeMarks", MinMarks.ConvertToXElement());
            }
        }

        public class MinMarks : IXElemementConvertable
        {
            public readonly uint SubjectID; //ИД дисциплины (справочник №1)
            public readonly uint MinMark; //Минимальная оценка по предмету

            public MinMarks(uint subjectID, uint minMark)
            {
                SubjectID = subjectID;
                MinMark = minMark;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("MinMarks",
                    new XElement("SubjectID", SubjectID),
                    new XElement("MinMark", MinMark)
                    );
            }
        }

        public class EntranceTestItem : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly uint EntranceTestTypeID; //Тип вступительного испытания (справочник №11)
            public readonly decimal? MinScore; //Минимальное количество баллов
            public readonly uint EntranceTestPriority; //Приоритет вступительного испытания
            public readonly TEntranceTestSubject EntranceTestSubject; //Дисциплина вступительного испытания //s
            public readonly List<EntranceTestBenefitItem> EntranceTestBenefits; //Условия предоставления льгот
            public readonly IsForSPOandVO IsForSPOandVO; //Испытание для поступающих на основании профильного СПО/ВО

            public EntranceTestItem(TUID uid, uint entranceTestTypeID, uint entranceTestPriority, TEntranceTestSubject entranceTestSubject, decimal? minScore = null, List<EntranceTestBenefitItem> entranceTestBenefits = null, IsForSPOandVO isForSPOandVO = null)
            {
                UID = uid;
                EntranceTestTypeID = entranceTestTypeID;
                MinScore = minScore;
                EntranceTestPriority = entranceTestPriority;
                EntranceTestSubject = entranceTestSubject;
                EntranceTestBenefits = entranceTestBenefits;
                IsForSPOandVO = isForSPOandVO;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("EntranceTestItem",
                    new XElement("UID", UID.Value),
                    new XElement("EntranceTestTypeID", EntranceTestTypeID),
                    ToElementOrNull("MinScore", MinScore),
                    new XElement("EntranceTestPriority", EntranceTestPriority),
                    new XElement("EntranceTestSubject", EntranceTestSubject.ConvertToXElements()),
                    EntranceTestBenefits != null ? new XElement("EntranceTestBenefits", EntranceTestBenefits.Select(i => i.ConvertToXElement())) : null,
                    IsForSPOandVO != null ? IsForSPOandVO.ConvertToXElement() : null
                    );
            }
        }

        public class IsForSPOandVO : IXElemementConvertable
        {
            public readonly TUID ReplacedEntranceTestItemUID; //UID заменяемого испытания //s

            public IsForSPOandVO(TUID replacedEntranceTestItemUID)
            {
                ReplacedEntranceTestItemUID = replacedEntranceTestItemUID;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("IsForSPOandVO",
                    new XElement("ReplacedEntranceTestItemUID", ReplacedEntranceTestItemUID.Value)
                    );
            }
        }

        public class EntranceTestBenefitItem : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly List<OlympicDiplomTypeID> OlympicDiplomTypes; //Типы дипломов //s
            public readonly uint BenefitKindID; //Вид льготы (справочник №30)
            public readonly bool IsForAllOlympics; //Флаг действия льготы для всех олимпиад
            public readonly uint? LevelForAllOlympics; //Уровни для всех олимпиад (справочник №3)
            public readonly List<ProfileID> ProfileForAllOlympics; //Профили для всех олимпиад
            public readonly uint? ClassForAllOlympics; //Классы для всех олимпиад (справочник №40)
            public readonly int? MinEgeMark; //Минимальный балл ЕГЭ, при котором можно использовать льготу
            public readonly List<OlympicID> Olympics; //Перечень олимпиад, для которых действует льгота
            public readonly List<Olympic> OlympicsLevels; //Уровни, профили и классы олимпиад

            public EntranceTestBenefitItem(TUID uid, List<OlympicDiplomTypeID> olympicDiplomTypes, uint benefitKindID, bool isForAllOlympics, uint? levelForAllOlympics = null, List<ProfileID> profileForAllOlympics = null, uint? classForAllOlympics = null, int? minEgeMark = null, List<OlympicID> olympics = null, List<Olympic> olympicsLevels = null)
            {
                UID = uid;
                OlympicDiplomTypes = olympicDiplomTypes;
                BenefitKindID = benefitKindID;
                IsForAllOlympics = isForAllOlympics;
                LevelForAllOlympics = levelForAllOlympics;
                ProfileForAllOlympics = profileForAllOlympics;
                ClassForAllOlympics = classForAllOlympics;
                MinEgeMark = minEgeMark;
                Olympics = olympics;
                OlympicsLevels = olympicsLevels;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("EntranceTestBenefitItem",
                    new XElement("UID", UID.Value),
                    new XElement("OlympicDiplomTypes", OlympicDiplomTypes.Select(t => t.ConvertToXElement())),
                    new XElement("BenefitKindID", BenefitKindID),
                    new XElement("IsForAllOlympics", IsForAllOlympics),
                    ToElementOrNull("LevelForAllOlympics", LevelForAllOlympics),
                    ProfileForAllOlympics != null ? new XElement("ProfileForAllOlympics", ProfileForAllOlympics.Select(i => i.ConvertToXElement())) : null,
                    ToElementOrNull("ClassForAllOlympics", ClassForAllOlympics),
                    ToElementOrNull("MinEgeMark", MinEgeMark),
                    Olympics != null ? new XElement("Olympics", Olympics.Select(i => i.ConvertToXElement())) : null,
                    OlympicsLevels != null ? new XElement("OlympicsLevels", OlympicsLevels.Select(o => o.ConvertToXElement())) : null
                    );
            }
        }

        #endregion

        public class InstitutionAchievement : IXElemementConvertable
        {
            public readonly TUID InstitutionAchievementUID; //Идентификатор в ИС ОО //s
            public readonly string Name; //Наименование индивидуального достижения //500
            public readonly uint IdCategory; //ИД индивидуального достижения (справочник №36)
            public readonly decimal MaxValue; //Максимальный балл, начисляемый за индивидуальное достижение
            public readonly TUID CampaignUID; //Идентификатор приемной кампании //s

            public InstitutionAchievement(TUID institutionAchievementUID, string name, uint idCategory, decimal maxValue, TUID campaignUID)
            {
                InstitutionAchievementUID = institutionAchievementUID;
                Name = name;
                IdCategory = idCategory;
                MaxValue = maxValue;
                CampaignUID = campaignUID;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("InstitutionAchievement",
                    new XElement("InstitutionAchievementUID", InstitutionAchievementUID.Value),
                    new XElement("Name", Name),
                    new XElement("IdCategory", IdCategory),
                    new XElement("MaxValue", MaxValue),
                    new XElement("CampaignUID", CampaignUID.Value));
            }
        }

        public class TargetOrganizationImp : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly string Name; //Наименование целевой организации //250

            public TargetOrganizationImp(TUID uid, string name)
            {
                UID = uid;
                Name = name;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("TargetOrganization",
                    new XElement("UID", UID.Value),
                    new XElement("Name", Name));
            }
        }

        #region Applications

        public class Application : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly string ApplicationNumber; //Номер заявления ОО //50
            public readonly Entrant Entrant; //Абитуриент //s
            public readonly TDateTime RegistrationDate; //Дата регистрации заявления //s
            public readonly bool NeedHostel; //Признак необходимости общежития
            public readonly uint StatusID; //Статус заявления (справочник №4)
            public readonly string StatusComment; //Комментарий к статусу заявления //us //4000
			public readonly uint ReturnDocumentsTypeID; //Способ возврата заявления (справочник 49)
			public readonly DateTime? ReturnDate; //Дата возврата документов
			public readonly List<FinSourceEduForm> FinSourceEduForms; //Условия приема //s
            public readonly List<ApplicationCommonBenefit> ApplicationCommonBenefits; //Льготы, предоставленные абитуриенту
            public readonly ApplicationDocuments ApplicationDocuments; //Документы, приложенные к заявлению //s
            public readonly List<EntranceTestResult> EntranceTestResults; //Результаты вступительных испытаний
            public readonly List<IndividualAchievement> IndividualAchievements; //Индивидуальные достижения
			public readonly string Status;


			public Application(TUID uid, string applicationNumber, Entrant entrant, TDateTime registrationDate, bool needHostel, uint statusID,
				List<FinSourceEduForm> finSourceEduForms, ApplicationDocuments applicationDocuments, string statusComment = null, uint returnType = 0, DateTime? returnDate = null,
				List<ApplicationCommonBenefit> applicationCommonBenefits = null, List<EntranceTestResult> entranceTestResults = null, 
				List<IndividualAchievement> individualAchievements = null )
            {
                UID = uid;
                ApplicationNumber = applicationNumber;
                Entrant = entrant;
                RegistrationDate = registrationDate;
                NeedHostel = needHostel;
                StatusID = statusID;
                StatusComment = statusComment;
				ReturnDocumentsTypeID = returnType;
				ReturnDate = returnDate;
				FinSourceEduForms = finSourceEduForms;
                ApplicationCommonBenefits = applicationCommonBenefits;
                ApplicationDocuments = applicationDocuments;
                EntranceTestResults = entranceTestResults;
                IndividualAchievements = individualAchievements;
				
				
            }

            public XElement ConvertToXElement()
            {
                return new XElement("Application",
                    new XElement("UID", UID.Value),
                    new XElement("ApplicationNumber", ApplicationNumber),
                    Entrant.ConvertToXElement(),
                    new XElement("RegistrationDate", RegistrationDate.Value),
                    new XElement("NeedHostel", NeedHostel),
                    new XElement("StatusID", StatusID),
                    StatusComment != null ? new XElement("StatusComment", StatusComment) : null,
					ReturnDocumentsTypeID != 0 ? new XElement("ReturnDocumentsTypeId", ReturnDocumentsTypeID) : null,
					ReturnDocumentsTypeID != 0 ? new XElement("ReturnDocumentsDate", ReturnDate) : null,
					new XElement("FinSourceAndEduForms", FinSourceEduForms.Select(f => f.ConvertToXElement())),
                    ApplicationCommonBenefits != null ? new XElement("ApplicationCommonBenefits", ApplicationCommonBenefits.Select(b => b.ConvertToXElement())) : null,
                    ApplicationDocuments.ConvertToXElement(),
                    EntranceTestResults != null ? new XElement("EntranceTestResults", EntranceTestResults.Select(r => r.ConvertToXElement())) : null,
                    IndividualAchievements != null ? new XElement("IndividualAchievements", IndividualAchievements.Select(a => a.ConvertToXElement())) : null
					

                    );
            }
        }

        public class Entrant : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly string LastName; //Фамилия //200
            public readonly string FirstName; //Имя //250
            public readonly string MiddleName; //Отчество //250 //us
            public readonly uint GenderID; //Пол (справочник №5)
            public readonly string CustomInformation; //Дополнительные сведения, предоставленные абитуриентом //4000 //us
            public readonly EmailOrMailAddress EmailOrMailAddress; //Электронный или почтовый адрес //s
            public readonly IsFromKrym IsFromKrym; //Гражданин Крыма

            public Entrant(TUID uid, string lastName, string firstName, uint genderID, EmailOrMailAddress emailOrMailAddress, string middleName = null, string customInformation = null, IsFromKrym isFromKrym = null)
            {
                UID = uid;
                LastName = lastName;
                FirstName = firstName;
                MiddleName = middleName;
                GenderID = genderID;
                CustomInformation = customInformation;
                EmailOrMailAddress = emailOrMailAddress;
                IsFromKrym = isFromKrym;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("Entrant",
                    new XElement("UID", UID.Value),
                    new XElement("LastName", LastName),
                    new XElement("FirstName", FirstName),
                    MiddleName != null ? new XElement("MiddleName", MiddleName) : null,
                    new XElement("GenderID", GenderID),
                    CustomInformation != null ? new XElement("CustomInformation", CustomInformation) : null,
                    EmailOrMailAddress.ConvertToXElement(),
                    IsFromKrym != null ? IsFromKrym.ConvertToXElement() : null
                    );
            }
        }

        public class EmailOrMailAddress : IXElemementConvertable
        {
            public readonly string Email = null; //Электронный адрес //150 //us
            public readonly MailAddress MailAddress = null; //Почтовый адрес

            public EmailOrMailAddress(string email)
            {
                Email = email;
            }

            public EmailOrMailAddress(MailAddress mailAddress)
            {
                MailAddress = mailAddress;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("EmailOrMailAddress",
                    Email != null ? new XElement("Email", Email) : null,
                    MailAddress != null ? MailAddress.ConvertToXElement() : null
                    );
            }
        }

        public class MailAddress : IXElemementConvertable
        {
            public readonly uint RegionID; //Регион (справочник № 8)
            public readonly uint TownTypeID; //Тип населенного пункта (справочник № 41)
            public readonly string Address; //Адрес //500

            public MailAddress(uint regionID, uint townTypeID, string address)
            {
                RegionID = regionID;
                TownTypeID = townTypeID;
                Address = address;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("MailAddress",
                    new XElement("RegionID", RegionID),
                    new XElement("TownTypeID", TownTypeID),
                    new XElement("Address", Address)
                    );
            }
        }

        public class IsFromKrym : IXElemementConvertable
        {
            public readonly TUID DocumentUID; //UID подтверждающего документа //s

            public IsFromKrym(TUID documentUID)
            {
                DocumentUID = documentUID;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("IsFromKrym", new XElement("DocumentUID", DocumentUID.Value));
            }
        }
		
					

        public class FinSourceEduForm : IXElemementConvertable
        {
            public readonly TUID CompetitiveGroupUID; //UID кокнкурсной группы //s
            public readonly TUID TargetOrganizationUID; //UID организации целевого приема
            public readonly TDateTime IsAgreedDate; //Дата согласия на зачисление (необходимо передать при наличии согласия на зачисление)
            public readonly TDateTime IsDisagreedDate; //Дата отказа от зачисления (необходимо передать при включении заявления в приказ об исключении)
            public readonly bool? IsForSPOandVO; //Абитуриент поступает с профильным СПО/ВО

            public FinSourceEduForm(TUID competitiveGroupUID, TUID targetOrganizationUID = null, TDateTime isAgreedDate = null, TDateTime isDisagreedDate = null, bool? isForSPOandVO = null)
            {
                CompetitiveGroupUID = competitiveGroupUID;
                TargetOrganizationUID = targetOrganizationUID;
                IsAgreedDate = isAgreedDate;
                IsDisagreedDate = isDisagreedDate;
                IsForSPOandVO = isForSPOandVO;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("FinSourceEduForm",
                    new XElement("CompetitiveGroupUID", CompetitiveGroupUID.Value),
                    TargetOrganizationUID != null ? new XElement("TargetOrganizationUID", TargetOrganizationUID.Value) : null,
                    IsAgreedDate != null ? new XElement("IsAgreedDate", IsAgreedDate.Value) : null,
                    IsDisagreedDate != null ? new XElement("IsDisagreedDate", IsDisagreedDate.Value) : null,
                    ToElementOrNull("IsForSPOandVO", IsForSPOandVO)
                    );
            }
        }

        public class ApplicationCommonBenefit : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TUID CompetitiveGroupUID; //UID конкурса для льготы //s
            public readonly uint DocumentTypeID; //Ид типа документа-основания (справочник №31)
            public readonly DocumentReason DocumentReason; //Сведения о документе-основании //s
            public readonly uint BenefitKindID; //ИД вида льготы (справочник №30)

            public ApplicationCommonBenefit(TUID uid, TUID competitiveGroupUID, uint documentTypeID, DocumentReason documentReason, uint benefitKindID)
            {
                UID = uid;
                CompetitiveGroupUID = competitiveGroupUID;
                DocumentTypeID = documentTypeID;
                DocumentReason = documentReason;
                BenefitKindID = benefitKindID;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("ApplicationCommonBenefit",
                    new XElement("UID", UID.Value),
                    new XElement("CompetitiveGroupUID", CompetitiveGroupUID.Value),
                    new XElement("DocumentTypeID", DocumentTypeID),
                    DocumentReason.ConvertToXElement(),
                    new XElement("BenefitKindID", BenefitKindID)
                    );
            }
        }

        public class DocumentReason : IXElemementConvertable
        {
            public readonly TOlympicDocument OlympicDocument = null; //Диплом победителя/призера олимпиады школьников
            public readonly TOlympicTotalDocument OlympicTotalDocument = null; //Диплом победителя/призера всероссийской олимпиады школьников
            public readonly TUkraineOlympic UkraineOlympicDocument = null; //Диплом 4 этапа всеукраинской ученической олимпиады
            public readonly TInternationalOlympic InternationalOlympiсDocument = null; //Диплом участия в международной олимпиаде
            public readonly MedicalDocuments MedicalDocuments = null; //Основание для льготы по медицинским показаниям
            public readonly TOrphanDocument OrphanDocument = null; //Документ, подтверждающий сиротство
            public readonly TVeteranDocument VeteranDocument = null; //Документ, подтверждающий принадлежность к участникам и ветеранам боевых действий
            public readonly TSportDocument SportDocument = null; //Диплом победителя/призера в области спорта
            public readonly TPauperDocument PauperDocument = null; //Документ, подтверждающий наличие только одного родителя - инвалида I группы и принадлежность к числу малоимущих семей
            public readonly TParentsLostDocument ParentsLostDocument = null; //Документ, подтверждающий принадлежность родителей и опекунов к погибшим в связи с исполнением служебных обязанностей
            public readonly TStateEmployeeDocument StateEmployeeDocument = null; //Документ, подтверждающий принадлежность к сотрудникам государственных органов Российской Федерации
            public readonly TRadiationWorkDocument RadiationWorkDocument = null; //Документ, подтверждающий участие в работах на радиационных объектах или воздействие радиации

            public DocumentReason(TOlympicDocument olympicDocument)
            {
                OlympicDocument = olympicDocument;
            }

            public DocumentReason(TOlympicTotalDocument olympicTotalDocument)
            {
                OlympicTotalDocument = olympicTotalDocument;
            }

            public DocumentReason(TUkraineOlympic ukraineOlympicDocument)
            {
                UkraineOlympicDocument = ukraineOlympicDocument;
            }

            public DocumentReason(TInternationalOlympic internationalOlympiсDocument)
            {
                InternationalOlympiсDocument = internationalOlympiсDocument;
            }

            public DocumentReason(MedicalDocuments medicalDocuments)
            {
                MedicalDocuments = medicalDocuments;
            }

            public DocumentReason(TOrphanDocument orphanDocument)
            {
                OrphanDocument = orphanDocument;
            }

            public DocumentReason(TVeteranDocument veteranDocument)
            {
                VeteranDocument = veteranDocument;
            }

            public DocumentReason(TSportDocument sportDocument)
            {
                SportDocument = sportDocument;
            }

            public DocumentReason(TPauperDocument pauperDocument)
            {
                PauperDocument = pauperDocument;
            }

            public DocumentReason(TParentsLostDocument parentsLostDocument)
            {
                ParentsLostDocument = parentsLostDocument;
            }

            public DocumentReason(TStateEmployeeDocument stateEmployeeDocument)
            {
                StateEmployeeDocument = stateEmployeeDocument;
            }

            public DocumentReason(TRadiationWorkDocument radiationWorkDocument)
            {
                RadiationWorkDocument = radiationWorkDocument;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("DocumentReason",
                    OlympicDocument != null ? new XElement("OlympicDocument", OlympicDocument.ConvertToXElements()) : null,
                    OlympicTotalDocument != null ? new XElement("OlympicTotalDocument", OlympicTotalDocument.ConvertToXElements()) : null,
                    UkraineOlympicDocument != null ? new XElement("UkraineOlympicDocument", UkraineOlympicDocument.ConvertToXElements()) : null,
                    InternationalOlympiсDocument != null ? new XElement("InternationalOlympiсDocument", InternationalOlympiсDocument.ConvertToXElements()) : null,
                    MedicalDocuments != null ? MedicalDocuments.ConvertToXElement() : null,
                    OrphanDocument != null ? new XElement("OrphanDocument", OrphanDocument.ConvertToXElements()) : null,
                    SportDocument != null ? new XElement("SportDocument", SportDocument.ConvertToXElements()) : null,
                    PauperDocument != null ? new XElement("PauperDocument", PauperDocument.ConvertToXElements()) : null,
                    ParentsLostDocument != null ? new XElement("ParentsLostDocument", ParentsLostDocument.ConvertToXElements()) : null,
                    StateEmployeeDocument != null ? new XElement("StateEmployeeDocument", StateEmployeeDocument.ConvertToXElements()) : null,
                    RadiationWorkDocument != null ? new XElement("RadiationWorkDocument", RadiationWorkDocument.ConvertToXElements()) : null
                    );
            }
        }

        public class MedicalDocuments : IXElemementConvertable
        {
            public readonly BenefitDocument BenefitDocument; //Основание для льготы //s
            public readonly TAllowEducationDocument AllowEducationDocument; //Заключение об отсутствии противопоказаний для обучения

            public MedicalDocuments(BenefitDocument benefitDocument, TAllowEducationDocument allowEducationDocument = null)
            {
                BenefitDocument = benefitDocument;
                AllowEducationDocument = allowEducationDocument;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("MedicalDocuments",
                    BenefitDocument.ConvertToXElement(),
                    AllowEducationDocument != null ? new XElement("AllowEducationDocument", AllowEducationDocument.ConvertToXElements()) : null
                    );
            }
        }

        public class BenefitDocument : IXElemementConvertable
        {
            public readonly TDisabilityDocument DisabilityDocument = null; //Справка об установлении инвалидности
            public readonly TMedicalDocument MedicalDocument = null; //Заключение психолого-медико-педагогической комиссии

            public BenefitDocument(TDisabilityDocument disabilityDocument)
            {
                DisabilityDocument = disabilityDocument;
            }

            public BenefitDocument(TMedicalDocument medicalDocument)
            {
                MedicalDocument = medicalDocument;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("BenefitDocument",
                    DisabilityDocument != null ? new XElement("DisabilityDocument", DisabilityDocument.ConvertToXElements()) : null,
                    MedicalDocument != null ? new XElement("MedicalDocument", MedicalDocument.ConvertToXElements()) : null
                    );
            }
        }

        public class ApplicationDocuments : IXElemementConvertable
        {
            public readonly List<EgeDocument> EgeDocuments; //Свидетельства о результатах ЕГЭ
            public readonly List<GiaDocument> GiaDocuments; //Справки ГИА
            public readonly IdentityDocument IdentityDocument; //Документ, удостоверяющий личность //s
            public readonly List<IdentityDocument> OtherIdentityDocuments; //дополнительный перечень документов, удостоверяющих личность
            public readonly List<EduDocument> EduDocuments; //Документы об образовании
            public readonly MilitaryCardDocument MilitaryCardDocument; //Военный билет
            public readonly StudentDocument StudentDocument; //Справка об обучении в другом ВУЗе
            public readonly List<OrphanDocument> OrphanDocuments; //Документы, подтверждающие сиротство
            public readonly List<VeteranDocument> VeteranDocuments; //Документы, подтверждающие принадлежность к участникам и ветеранам боевых действий
            public readonly List<SportDocument> SportDocuments; //Дипломы победителя/призера в области спорта
            public readonly List<CompatriotDocument> CompatriotDocuments; //Документы, подтверждающие принадлежность к соотечественникам
            public readonly List<PauperDocument> PauperDocuments; //Документы, подтверждающие наличие только одного родителя - инвалида I группы и принадлежность к числу малоимущих семей
            public readonly List<ParentsLostDocument> ParentsLostDocuments; //Документы, подтверждающие принадлежность родителей и опекунов к погибшим в связи с исполнением служебных обязанностей
            public readonly List<StateEmployeeDocument> StateEmployeeDocuments; //Документы, подтверждающие принадлежность к сотрудникам государственных органов Российской Федерации
            public readonly List<RadiationWorkDocument> RadiationWorkDocuments; //Документы, подтверждающие участие в работах на радиационных объектах или воздействие радиации
            public readonly List<CustomDocument> CustomDocuments; //Иные документы

            public ApplicationDocuments(IdentityDocument identityDocument, List<EgeDocument> egeDocuments = null, List<GiaDocument> giaDocuments = null, List<IdentityDocument> otherIdentityDocuments = null, List<EduDocument> eduDocuments = null, MilitaryCardDocument militaryCardDocument = null, StudentDocument studentDocument = null, List<OrphanDocument> orphanDocuments = null, List<VeteranDocument> veteranDocuments = null, List<SportDocument> sportDocuments = null, List<CompatriotDocument> compatriotDocuments = null, List<PauperDocument> pauperDocuments = null, List<ParentsLostDocument> parentsLostDocuments = null, List<StateEmployeeDocument> stateEmployeeDocuments = null, List<RadiationWorkDocument> radiationWorkDocuments = null, List<CustomDocument> customDocuments = null)
            {
                EgeDocuments = egeDocuments;
                GiaDocuments = giaDocuments;
                IdentityDocument = identityDocument;
                OtherIdentityDocuments = otherIdentityDocuments;
                EduDocuments = eduDocuments;
                MilitaryCardDocument = militaryCardDocument;
                StudentDocument = studentDocument;
                OrphanDocuments = orphanDocuments;
                VeteranDocuments = veteranDocuments;
                SportDocuments = sportDocuments;
                CompatriotDocuments = compatriotDocuments;
                PauperDocuments = pauperDocuments;
                ParentsLostDocuments = parentsLostDocuments;
                StateEmployeeDocuments = stateEmployeeDocuments;
                RadiationWorkDocuments = radiationWorkDocuments;
                CustomDocuments = customDocuments;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("ApplicationDocuments",
                    EgeDocuments != null ? new XElement("EgeDocuments", EgeDocuments.Select(d => d.ConvertToXElement())) : null,
                    GiaDocuments != null ? new XElement("GiaDocuments", GiaDocuments.Select(d => d.ConvertToXElement())) : null,
                    IdentityDocument.ConvertToXElement(),
                    OtherIdentityDocuments != null ? new XElement("OtherIdentityDocuments", OtherIdentityDocuments.Select(d => d.ConvertToXElement())) : null,
                    EduDocuments != null ? new XElement("EduDocuments", EduDocuments.Select(d => d.ConvertToXElement())) : null,
                    MilitaryCardDocument != null ? MilitaryCardDocument.ConvertToXElement() : null,
                    StudentDocument != null ? StudentDocument.ConvertToXElement() : null,
                    OrphanDocuments != null ? new XElement("OrphanDocuments", OrphanDocuments.Select(d => d.ConvertToXElement())) : null,
                    VeteranDocuments != null ? new XElement("VeteranDocuments", VeteranDocuments.Select(d => d.ConvertToXElement())) : null,
                    SportDocuments != null ? new XElement("SportDocuments", SportDocuments.Select(d => d.ConvertToXElement())) : null,
                    CompatriotDocuments != null ? new XElement("CompatriotDocuments", CompatriotDocuments.Select(d => d.ConvertToXElement())) : null,
                    PauperDocuments != null ? new XElement("PauperDocuments", PauperDocuments.Select(d => d.ConvertToXElement())) : null,
                    ParentsLostDocuments != null ? new XElement("ParentsLostDocuments", ParentsLostDocuments.Select(d => d.ConvertToXElement())) : null,
                    StateEmployeeDocuments != null ? new XElement("StateEmployeeDocuments", StateEmployeeDocuments.Select(d => d.ConvertToXElement())) : null,
                    RadiationWorkDocuments != null ? new XElement("RadiationWorkDocuments", RadiationWorkDocuments.Select(d => d.ConvertToXElement())) : null,
                    CustomDocuments != null ? new XElement("CustomDocuments", CustomDocuments.Select(d => d.ConvertToXElement())) : null
                    );
            }
        }

        public class EgeDocument : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly TDocumentNumber DocumentNumber; //Номер свидетельства о результатах ЕГЭ
            public readonly TDate DocumentDate; //Дата выдачи свидетельства
            public readonly uint DocumentYear; //Год выдачи свидетельства
            public readonly List<SubjectData> Subjects; //Дисциплины //s

            public EgeDocument(TUID uid, uint documentYear, List<SubjectData> subjects, TDate originalReceivedDate = null, TDocumentNumber documentNumber = null, TDate documentDate = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentYear = documentYear;
                Subjects = subjects;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("EgeDocument",
                    new XElement("UID", UID.Value),
                    OriginalReceivedDate != null ? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value) : null,
                    DocumentNumber != null ? new XElement("DocumentNumber", DocumentNumber.Value) : null,
                    DocumentDate != null ? new XElement("DocumentDate", DocumentDate.Value) : null,
                    new XElement("DocumentYear", DocumentYear),
                    new XElement("Subjects", Subjects.Select(d => d.ConvertToXElement()))
                    );
            }
        }

        public class SubjectData : IXElemementConvertable
        {
            public readonly uint SubjectID; //ИД дисциплины (справочник №1)
            public readonly uint Value; //Балл

            public SubjectData(uint subjectID, uint value)
            {
                SubjectID = subjectID;
                Value = value;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("SubjectData",
                    new XElement("SubjectID", SubjectID),
                    new XElement("Value", Value)
                    );
            }
        }

        public class GiaDocument : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly TDocumentNumber DocumentNumber; //Номер справки ГИА //s
            public readonly TDate DocumentDate; //Дата выдачи свидетельства
            public readonly string DocumentOrganization; //Год выдачи свидетельства //500 //us
            public readonly List<SubjectData> Subjects; //Дисциплины //s

            public GiaDocument(TUID uid, TDocumentNumber documentNumber, List<SubjectData> subjects, TDate originalReceivedDate = null, TDate documentDate = null, string documentOrganization = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
                Subjects = subjects;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("GiaDocument",
                    new XElement("UID", UID.Value),
                    OriginalReceivedDate != null ? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value) : null,
                    new XElement("DocumentNumber", DocumentNumber.Value),
                    DocumentDate != null ? new XElement("DocumentDate", DocumentDate.Value) : null,
                    DocumentOrganization != null ? new XElement("DocumentOrganization", DocumentOrganization) : null,
                    new XElement("Subjects", Subjects.Select(d => d.ConvertToXElement()))
                    );
            }
        }

        public class IdentityDocument : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly string LastName; //Фамилия //250
            public readonly string FirstName; //Имя //250
            public readonly string MiddleName; //Отчество //250 //us
            public readonly uint? GenderID; //Пол (справочник №5)
            public readonly string DocumentSeries; //Серия документа //20 //us
            public readonly string DocumentNumber; //Номер документа //50
            public readonly string SubdivisionCode; //Код подразделения //7 //pattern //us
            public readonly TDate DocumentDate; //Дата выдачи документа //s
            public readonly string DocumentOrganization; //Кем выдан //500 //us
            public readonly uint IdentityDocumentTypeID; //Ид типа документа, удостоверяющего личность (справочник №22)
            public readonly uint NationalityTypeID; //ИД гражданства (справочник №7)
            public readonly TDate BirthDate; //Дата рождения //s
            public readonly string BirthPlace; //Место рождения //250 //us

            public IdentityDocument(TUID uid, string lastName, string firstName, string documentNumber, TDate documentDate, uint identityDocumentTypeID, uint nationalityTypeID, TDate birthDate, TDate originalReceivedDate = null, string middleName = null, uint? genderID = null, string documentSeries = null, string subdivisionCode = null, string documentOrganization = null, string birthPlace = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                LastName = lastName;
                FirstName = firstName;
                MiddleName = middleName;
                GenderID = genderID;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                SubdivisionCode = subdivisionCode;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
                IdentityDocumentTypeID = identityDocumentTypeID;
                NationalityTypeID = nationalityTypeID;
                BirthDate = birthDate;
                BirthPlace = birthPlace;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("IdentityDocument",
                    new XElement("UID", UID.Value),
                    OriginalReceivedDate != null ? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value) : null,
                    LastName != null ? new XElement("LastName", LastName) : null,
                    FirstName != null ? new XElement("FirstName", FirstName) : null,
                    MiddleName != null ? new XElement("MiddleName", MiddleName) : null,
                    ToElementOrNull("GenderID", GenderID),
                    DocumentSeries != null ? new XElement("DocumentSeries", DocumentSeries) : null,
                    new XElement("DocumentNumber", DocumentNumber),
                    SubdivisionCode != null ? new XElement("SubdivisionCode", SubdivisionCode) : null,
                    new XElement("DocumentDate", DocumentDate.Value),
                    DocumentOrganization != null ? new XElement("DocumentOrganization", DocumentOrganization) : null,
                    new XElement("IdentityDocumentTypeID", IdentityDocumentTypeID),
                    new XElement("NationalityTypeID", NationalityTypeID),
                    new XElement("BirthDate", BirthDate.Value),
                    BirthPlace != null ? new XElement("BirthPlace", BirthPlace) : null
                    );
            }
        }

        public class EduDocument : IXElemementConvertable
        {
            public readonly TSchoolCertificateDocument SchoolCertificateDocument = null; //Аттестат о среднем (полном) общем образовании
            public readonly TSchoolCertificateDocument SchoolCertificateBasicDocument = null; //Аттестат об основном общем образовании
            public readonly THighEduDiplomaDocument HighEduDiplomaDocument = null; //Диплом о высшем профессиональном образовании
            public readonly TPostGraduateDiplomaDocument PostGraduateDiplomaDocument = null; //Диплом об окончании аспирантуры (адъюнкантуры)
            public readonly TPhDDiplomaDocument PhDDiplomaDocument = null; //Диплом кандидата наук
            public readonly TMiddleEduDiplomaDocument MiddleEduDiplomaDocument = null; //Диплом о среднем профессиональном образовании
            public readonly TBasicDiplomaDocument BasicDiplomaDocument = null; //Диплом о начальном профессиональном образовании
            public readonly TIncomplHighEduDiplomaDocument IncomplHighEduDiplomaDocument = null; //Диплом о неполном высшем профессиональном образовании
            public readonly TAcademicDiplomaDocument AcademicDiplomaDocument = null; //Академическая справка
            public readonly TEduCustomDocument EduCustomDocument = null; //Иной документ об образовании

            public EduDocument(TSchoolCertificateDocument schoolCertificateDocument)
            {
                SchoolCertificateDocument = schoolCertificateDocument;
            }

            public EduDocument(TSchoolCertificateDocument schoolCertificateBasicDocument, bool unusedBasic)
            {
                SchoolCertificateBasicDocument = schoolCertificateBasicDocument;
            }

            public EduDocument(THighEduDiplomaDocument highEduDiplomaDocument)
            {
                HighEduDiplomaDocument = highEduDiplomaDocument;
            }

            public EduDocument(TPostGraduateDiplomaDocument postGraduateDiplomaDocument)
            {
                PostGraduateDiplomaDocument = postGraduateDiplomaDocument;
            }

            public EduDocument(TPhDDiplomaDocument phdDiplomaDocument)
            {
                PhDDiplomaDocument = phdDiplomaDocument;
            }

            public EduDocument(TMiddleEduDiplomaDocument middleEduDiplomaDocument)
            {
                MiddleEduDiplomaDocument = middleEduDiplomaDocument;
            }

            public EduDocument(TBasicDiplomaDocument basicDiplomaDocument)
            {
                BasicDiplomaDocument = basicDiplomaDocument;
            }

            public EduDocument(TIncomplHighEduDiplomaDocument incomplHighEduDiplomaDocument)
            {
                IncomplHighEduDiplomaDocument = incomplHighEduDiplomaDocument;
            }

            public EduDocument(TAcademicDiplomaDocument academicDiplomaDocument)
            {
                AcademicDiplomaDocument = academicDiplomaDocument;
            }

            public EduDocument(TEduCustomDocument eduCustomDocument)
            {
                EduCustomDocument = eduCustomDocument;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("EduDocument",
                    SchoolCertificateDocument != null ? new XElement("SchoolCertificateDocument", SchoolCertificateDocument.ConvertToXElements()) : null,
                    SchoolCertificateBasicDocument != null ? new XElement("SchoolCertificateBasicDocument", SchoolCertificateBasicDocument.ConvertToXElements()) : null,
                    HighEduDiplomaDocument != null ? new XElement("HighEduDiplomaDocument", HighEduDiplomaDocument.ConvertToXElements()) : null,
                    PostGraduateDiplomaDocument != null ? new XElement("PostGraduateDiplomaDocument", PostGraduateDiplomaDocument.ConvertToXElements()) : null,
                    PhDDiplomaDocument != null ? new XElement("PhDDiplomaDocument", PhDDiplomaDocument.ConvertToXElements()) : null,
                    MiddleEduDiplomaDocument != null ? new XElement("MiddleEduDiplomaDocument", MiddleEduDiplomaDocument.ConvertToXElements()) : null,
                    BasicDiplomaDocument != null ? new XElement("BasicDiplomaDocument", BasicDiplomaDocument.ConvertToXElements()) : null,
                    IncomplHighEduDiplomaDocument != null ? new XElement("IncomplHighEduDiplomaDocument", IncomplHighEduDiplomaDocument.ConvertToXElements()) : null,
                    AcademicDiplomaDocument != null ? new XElement("AcademicDiplomaDocument", AcademicDiplomaDocument.ConvertToXElements()) : null,
                    EduCustomDocument != null ? new XElement("EduCustomDocument", EduCustomDocument.ConvertToXElements()) : null
                    );
            }
        }

        public class MilitaryCardDocument : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDate OriginalReceivedDate; //Дата предоставления оригиналов документов
            public readonly TDocumentSeries DocumentSeries; //Серия документа //s
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly TDate DocumentDate; //Дата выдачи документа //s
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500 //us

            public MilitaryCardDocument(TUID uid, TDocumentSeries documentSeries, TDocumentNumber documentNumber, TDate documentDate, TDate originalReceivedDate = null, string documentOrganization = null)
            {
                UID = uid;
                OriginalReceivedDate = originalReceivedDate;
                DocumentSeries = documentSeries;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("MilitaryCardDocument",
                    new XElement("UID", UID.Value),
                    OriginalReceivedDate != null ? new XElement("OriginalReceivedDate", OriginalReceivedDate.Value) : null,
                    new XElement("DocumentSeries", DocumentDate.Value),
                    new XElement("DocumentNumber", DocumentNumber),
                    new XElement("DocumentDate", DocumentDate.Value),
                    DocumentOrganization != null ? new XElement("DocumentOrganization", DocumentOrganization) : null
                    );
            }
        }

        public class StudentDocument : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly TDocumentNumber DocumentNumber; //Номер документа //s
            public readonly TDate DocumentDate; //Дата выдачи документа //s
            public readonly string DocumentOrganization; //Организация, выдавшая документ //500

            public StudentDocument(TUID uid, TDocumentNumber documentNumber, TDate documentDate, string documentOrganization)
            {
                UID = uid;
                DocumentNumber = documentNumber;
                DocumentDate = documentDate;
                DocumentOrganization = documentOrganization;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("StudentDocument",
                    new XElement("UID", UID.Value),
                    new XElement("DocumentNumber", DocumentNumber.Value),
                    new XElement("DocumentDate", DocumentDate.Value),
                    new XElement("DocumentOrganization", DocumentOrganization)
                    );
            }
        }

        public class OrphanDocument : IXElemementConvertable
        {
            public readonly TOrphanDocument Value; //Документ, подтверждающий сиротство //s

            public OrphanDocument(TOrphanDocument value)
            {
                Value = value;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("OrphanDocument", Value.ConvertToXElements());
            }
        }

        public class VeteranDocument : IXElemementConvertable
        {
            public readonly TVeteranDocument Value; //Документ, подтверждающий принадлежность к участникам и ветеранам боевых действий //s

            public VeteranDocument(TVeteranDocument value)
            {
                Value = value;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("VeteranDocument", Value.ConvertToXElements());
            }
        }

        public class SportDocument : IXElemementConvertable
        {
            public readonly TSportDocument Value; //Диплом победителя/призера в области спорта //s

            public SportDocument(TSportDocument value)
            {
                Value = value;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("SportDocument", Value.ConvertToXElements());
            }
        }

        public class CompatriotDocument : IXElemementConvertable
        {
            public readonly TCompatriotDocument Value; //Документ, подтверждающий принадлежность к соотечественникам //s

            public CompatriotDocument(TCompatriotDocument value)
            {
                Value = value;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("CompatriotDocument", Value.ConvertToXElements());
            }
        }

        public class PauperDocument : IXElemementConvertable
        {
            public readonly TPauperDocument Value; //Документ, подтверждающий наличие только одного родителя - инвалида I группы и принадлежность к числу малоимущих семей //s

            public PauperDocument(TPauperDocument value)
            {
                Value = value;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("PauperDocument", Value.ConvertToXElements());
            }
        }

        public class ParentsLostDocument : IXElemementConvertable
        {
            public readonly TParentsLostDocument Value; //Документ, подтверждающий принадлежность родителей и опекунов к погибшим в связи с исполнением служебных обязанностей //s

            public ParentsLostDocument(TParentsLostDocument value)
            {
                Value = value;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("ParentsLostDocument", Value.ConvertToXElements());
            }
        }

        public class StateEmployeeDocument : IXElemementConvertable
        {
            public readonly TStateEmployeeDocument Value; //Документ, подтверждающий принадлежность к сотрудникам государственных органов Российской Федерации //s

            public StateEmployeeDocument(TStateEmployeeDocument value)
            {
                Value = value;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("StateEmployeeDocument", Value.ConvertToXElements());
            }
        }

        public class RadiationWorkDocument : IXElemementConvertable
        {
            public readonly TRadiationWorkDocument Value; //Документ, подтверждающий участие в работах на радиационных объектах или воздействие радиации //s

            public RadiationWorkDocument(TRadiationWorkDocument value)
            {
                Value = value;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("RadiationWorkDocument", Value.ConvertToXElements());
            }
        }

        public class CustomDocument : IXElemementConvertable
        {
            public readonly TCustomDocument Value; //Иной документ //s

            public CustomDocument(TCustomDocument value)
            {
                Value = value;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("CustomDocument", Value.ConvertToXElements());
            }
        }

        public class EntranceTestResult : IXElemementConvertable
        {
            public readonly TUID UID; //Идентификатор в ИС ОО //s
            public readonly decimal ResultValue; //Балл
            public readonly uint ResultSourceTypeID; //ИД основания для оценки (справочник №6)
            public readonly TEntranceTestSubject EntranceTestSubject; //Дисциплина вступительного испытания //s
            public readonly uint EntranceTestTypeID; //ИД типа вступительного испытания (справочник №11)
            public readonly TUID CompetitiveGroupUID; //UID конкурсной группы //s
            public readonly ResultDocument ResultDocument; //Сведения об основании для оценки
            public readonly IsDistant IsDistant; //ВИ с использованием дистанционных технологий
            public readonly IsDisabled IsDisabled; //ВИ с созданием специальных условий

            public EntranceTestResult(TUID uid, decimal resultValue, uint resultSourceTypeID, TEntranceTestSubject entranceTestSubject, uint entranceTestTypeID, TUID competitiveGroupUID, ResultDocument resultDocument = null, IsDistant isDistant = null, IsDisabled isDisabled = null)
            {
                UID = uid;
                ResultValue = resultValue;
                ResultSourceTypeID = resultSourceTypeID;
                EntranceTestSubject = entranceTestSubject;
                EntranceTestTypeID = entranceTestTypeID;
                CompetitiveGroupUID = competitiveGroupUID;
                ResultDocument = resultDocument;
                IsDistant = isDistant;
                IsDisabled = isDisabled;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("EntranceTestResult",
                    new XElement("UID", UID.Value),
                    new XElement("ResultValue", ResultValue),
                    new XElement("ResultSourceTypeID", ResultSourceTypeID),
                    new XElement("EntranceTestSubject", EntranceTestSubject.ConvertToXElements()),
                    new XElement("EntranceTestTypeID", EntranceTestTypeID),
                    new XElement("CompetitiveGroupUID", CompetitiveGroupUID.Value),
                    ResultDocument != null ? ResultDocument.ConvertToXElement() : null,
                    IsDistant != null ? IsDistant.ConvertToXElement() : null,
                    IsDisabled != null ? IsDisabled.ConvertToXElement() : null
                    );
            }
        }

        public class ResultDocument : IXElemementConvertable
        {
            public readonly TOlympicDocument OlympicDocument = null; //Диплом победителя/призера олимпиады школьников //s
            public readonly TOlympicTotalDocument OlympicTotalDocument = null; //Диплом победителя/призера всероссийской олимпиады школьников //s
            public readonly TSportDocument SportDocument = null; //Диплом победителя/призера в области спорта
            public readonly TUkraineOlympic UkraineOlympicDocument = null; //Диплом 4 этапа всеукраинской ученической олимпиады 
            public readonly TInternationalOlympic InternationalOlympiсDocument = null; //Диплом участия в международной олимпиаде 
            public readonly TInstitutionDocument InstitutionDocument = null; //Испытание ОО //s
            public readonly TUID EgeDocumentID = null; //UID свидетельства о результатах ЕГЭ, приложенного к заявлению //s

            public ResultDocument(TOlympicDocument olympicDocument)
            {
                OlympicDocument = olympicDocument;
            }

            public ResultDocument(TOlympicTotalDocument olympicTotalDocument)
            {
                OlympicTotalDocument = olympicTotalDocument;
            }

            public ResultDocument(TSportDocument sportDocument)
            {
                SportDocument = sportDocument;
            }

            public ResultDocument(TUkraineOlympic ukraineOlympicDocument)
            {
                UkraineOlympicDocument = ukraineOlympicDocument;
            }

            public ResultDocument(TInternationalOlympic internationalOlympiсDocument)
            {
                InternationalOlympiсDocument = internationalOlympiсDocument;
            }

            public ResultDocument(TInstitutionDocument institutionDocument)
            {
                InstitutionDocument = institutionDocument;
            }

            public ResultDocument(TUID egeDocumentID)
            {
                EgeDocumentID = egeDocumentID;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("ResultDocument",
                    OlympicDocument != null ? new XElement("OlympicDocument", OlympicDocument.ConvertToXElements()) : null,
                    OlympicTotalDocument != null ? new XElement("OlympicTotalDocument", OlympicTotalDocument.ConvertToXElements()) : null,
                    SportDocument != null ? new XElement("SportDocument", SportDocument.ConvertToXElements()) : null,
                    UkraineOlympicDocument != null ? new XElement("UkraineOlympicDocument", UkraineOlympicDocument.ConvertToXElements()) : null,
                    InternationalOlympiсDocument != null ? new XElement("InternationalOlympiсDocument", InternationalOlympiсDocument.ConvertToXElements()) : null,
                    InstitutionDocument != null ? new XElement("InstitutionDocument", InstitutionDocument.ConvertToXElements()) : null,
                    EgeDocumentID != null ? new XElement("EgeDocumentID", EgeDocumentID.Value) : null
                    );
            }
        }

        public class IsDistant : IXElemementConvertable
        {
            public readonly string DistantPlace; //место сдачи ВИ //200

            public IsDistant(string distantPlace)
            {
                DistantPlace = distantPlace;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("IsDistant",
                    new XElement("DistantPlace", DistantPlace)
                    );
            }
        }

        public class IsDisabled : IXElemementConvertable
        {
            public readonly TUID DisabledDocumentUID; //место сдачи ВИ //s

            public IsDisabled(TUID disabledDocumentUID)
            {
                DisabledDocumentUID = disabledDocumentUID;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("IsDisabled",
                    new XElement("DisabledDocumentUID", DisabledDocumentUID.Value)
                    );
            }
        }

        public class IndividualAchievement : IXElemementConvertable
        {
            public readonly TUID IAUID; //Идентификатор индивидуального достижения, учитываемого в заявлении //s
            public readonly TUID InstitutionAchievementUID; //Идентификатор достижения, указанный в приемной кампании //s
            public readonly uint? IAMark; //Балл за достижение
            public readonly TUID IADocumentUID; //Идентификатор документа, подтверждающего индивидуальное достижение, в ИС ОО //s
            public readonly bool? isAdvantageRight_; //Подтверждает преимущественное право

            public IndividualAchievement(TUID iaUID, TUID institutionAchievementUID, TUID iaDocumentUID, uint? iaMark = null, bool? isAdvantageRight = null)
            {
                IAUID = iaUID;
                InstitutionAchievementUID = institutionAchievementUID;
                IAMark = iaMark;
                IADocumentUID = iaDocumentUID;
                isAdvantageRight_ = isAdvantageRight;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("IndividualAchievement",
                    new XElement("IAUID", IAUID.Value),
                    new XElement("InstitutionAchievementUID", InstitutionAchievementUID.Value),
                    ToElementOrNull("IAMark", IAMark),
                    new XElement("IADocumentUID", IADocumentUID.Value),
                    ToElementOrNull("isAdvantageRight", isAdvantageRight_)
                    );
            }
        }

        #endregion

        #region Orders

        public class Orders : IXElemementConvertable
        {
            public readonly List<OrderOfAdmission> OrdersOfAdmission; //Приказы о зачислении
            public readonly List<OrderOfException> OrdersOfException; //Приказы об исключении из приказа о зачислении
            public readonly List<ApplicationOrd> Applications; //Заявления, включенные в приказы

            public Orders(List<OrderOfAdmission> ordersOfAdmission = null, List<OrderOfException> ordersOfException = null, List<ApplicationOrd> applications = null)
            {
                OrdersOfAdmission = ordersOfAdmission;
                OrdersOfException = ordersOfException;
                Applications = applications;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("Orders",
                    OrdersOfAdmission != null ? new XElement("OrdersOfAdmission", OrdersOfAdmission.Select(o => o.ConvertToXElement())) : null,
                    OrdersOfException != null ? new XElement("OrdersOfException", OrdersOfException.Select(o => o.ConvertToXElement())) : null,
                    Applications != null ? new XElement("Applications", Applications.Select(a => a.ConvertToXElement())) : null
                    );
            }
        }

        public class OrderOfAdmission : IXElemementConvertable
        {
            public readonly TUID OrderOfAdmissionUID; //Идентификатор в ИС ОО //s
            public readonly TUID CampaignUID; //UID приемной кампании //s
            public readonly string OrderName; //Наименование (текстовое описание) приказа //200
            public readonly string OrderNumber; //Номер приказа //50 //us
            public readonly TDate OrderDate; //Дата регистрации приказа
            public readonly TDate OrderDatePublished; //Дата фактической публикации приказа
            public readonly uint? EducationFormID; //ИД Формы обучения(Справочник 14 "Форма обучения")
            public readonly uint? FinanceSourceID; //ИД источника финансирования (Справочник 15 "Источник финансирования")
            public readonly uint? EducationLevelID; //ИД Уровня образования(Справочник 2 "Уровень образования")
            public readonly uint? Stage; //Этап приема (В случае зачисления на места в рамках контрольных цифр (бюджет) по программам бакалавриата и программам специалитета по очной и очно-заочной формам обучения, принимает значения 1 или 2). Иначе принимает значение 0.

            public OrderOfAdmission(TUID orderOfAdmissionUID, TUID campaignUID, string orderName, string orderNumber = null, TDate orderDate = null, TDate orderDatePublished = null, uint? educationFormID = null, uint? financeSourceID = null, uint? educationLevelID = null, uint? stage = null)
            {
                OrderOfAdmissionUID = orderOfAdmissionUID;
                CampaignUID = campaignUID;
                OrderName = orderName;
                OrderNumber = orderNumber;
                OrderDate = orderDate;
                OrderDatePublished = orderDatePublished;
                EducationFormID = educationFormID;
                FinanceSourceID = financeSourceID;
                EducationLevelID = educationLevelID;
                Stage = stage;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("OrderOfAdmission",
                    new XElement("OrderOfAdmissionUID", OrderOfAdmissionUID.Value),
                    new XElement("CampaignUID", CampaignUID.Value),
                    new XElement("OrderName", OrderName),
                    OrderNumber != null ? new XElement("OrderNumber", OrderNumber) : null,
                    OrderDate != null ? new XElement("OrderDate", OrderDate.Value) : null,
                    OrderDatePublished != null ? new XElement("OrderDatePublished", OrderDatePublished.Value) : null,
                    ToElementOrNull("EducationFormID", EducationFormID),
                    ToElementOrNull("FinanceSourceID", FinanceSourceID),
                    ToElementOrNull("EducationLevelID", EducationLevelID),
                    ToElementOrNull("Stage", Stage)
                    );
            }
        }

        public class OrderOfException : IXElemementConvertable
        {
            public readonly TUID OrderOfExceptionUID; //Идентификатор в ИС ОО //s
            public readonly TUID CampaignUID; //UID приемной кампании //s
            public readonly string OrderName; //Наименование (текстовое описание) приказа //200
            public readonly string OrderNumber; //Номер приказа //50 //us
            public readonly TDate OrderDate; //Дата регистрации приказа
            public readonly TDate OrderDatePublished; //Дата фактической публикации приказа
            public readonly uint? EducationFormID; //ИД Формы обучения(Справочник 14 "Форма обучения")
            public readonly uint? FinanceSourceID; //ИД источника финансирования (Справочник 15 "Источник финансирования")
            public readonly uint? EducationLevelID; //ИД Уровня образования(Справочник 2 "Уровень образования")
            public readonly uint? Stage; //Этап приема (В случае зачисления на места в рамках контрольных цифр (бюджет) по программам бакалавриата и программам специалитета по очной и очно-заочной формам обучения, принимает значения 1 или 2). Иначе принимает значение 0.

            public OrderOfException(TUID orderOfExceptionUID, TUID campaignUID, string orderName, string orderNumber = null, TDate orderDate = null, TDate orderDatePublished = null, uint? educationFormID = null, uint? financeSourceID = null, uint? educationLevelID = null, uint? stage = null)
            {
                OrderOfExceptionUID = orderOfExceptionUID;
                CampaignUID = campaignUID;
                OrderName = orderName;
                OrderNumber = orderNumber;
                OrderDate = orderDate;
                OrderDatePublished = orderDatePublished;
                EducationFormID = educationFormID;
                FinanceSourceID = financeSourceID;
                EducationLevelID = educationLevelID;
                Stage = stage;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("OrderOfException",
                    new XElement("OrderOfExceptionUID", OrderOfExceptionUID.Value),
                    new XElement("CampaignUID", CampaignUID.Value),
                    new XElement("OrderName", OrderName),
                    OrderNumber != null ? new XElement("OrderNumber", OrderNumber) : null,
                    OrderDate != null ? new XElement("OrderDate", OrderDate.Value) : null,
                    OrderDatePublished != null ? new XElement("OrderDatePublished", OrderDatePublished.Value) : null,
                    ToElementOrNull("EducationFormID", EducationFormID),
                    ToElementOrNull("FinanceSourceID", FinanceSourceID),
                    ToElementOrNull("EducationLevelID", EducationLevelID),
                    ToElementOrNull("Stage", Stage)
                    );
            }
        }

        public class ApplicationOrd : IXElemementConvertable
        {
            public readonly TUID ApplicationUID; //Идентификатор заявления, включаемого в приказ //s
            public readonly TUID OrderUID; //Идентификатор приказа //s
            public readonly uint OrderTypeID; //Тип приказа (1 - приказ о зачислении; 2 - приказ об исключении из приказа о зачислении)
            public readonly TUID CompetitiveGroupUID; //Конкурс, по которому заявление включается в приказ/исключается из приказа //s
            public readonly uint? OrderIdLevelBudget; //ИД уровня бюджета (справочник № 35)
            public readonly uint? BenefitKindID; //ИД вида льготы (справочник № 30)
            public readonly TDateTime IsDisagreedDate; //Дата отказа от зачисления (необходимо передать при включении заявления в приказ об исключении)

            public ApplicationOrd(TUID applicationUID, TUID orderUID, uint orderTypeID, TUID competitiveGroupUID, uint? orderIdLevelBudget = null, uint? benefitKindID = null, TDateTime isDisagreedDate = null)
            {
                ApplicationUID = applicationUID;
                OrderUID = orderUID;
                OrderTypeID = orderTypeID;
                CompetitiveGroupUID = competitiveGroupUID;
                OrderIdLevelBudget = orderIdLevelBudget;
                BenefitKindID = benefitKindID;
                IsDisagreedDate = isDisagreedDate;
            }

            public XElement ConvertToXElement()
            {
                return new XElement("Application",
                    new XElement("ApplicationUID", ApplicationUID.Value),
                    new XElement("OrderUID", OrderUID.Value),
                    new XElement("OrderTypeID", OrderTypeID),
                    new XElement("CompetitiveGroupUID", CompetitiveGroupUID.Value),
                    ToElementOrNull("OrderIdLevelBudget", OrderIdLevelBudget),
                    ToElementOrNull("BenefitKindID", BenefitKindID),
                    IsDisagreedDate != null ? new XElement("IsDisagreedDate", IsDisagreedDate.Value) : null
                    );
            }
        }

        #endregion

        private interface IXElemementConvertable
        {
            XElement ConvertToXElement();
        }

        private interface IXElemementsConvertable
        {
            object[] ConvertToXElements();
        }

        private static XElement ToElementOrNull<T>(string name, T? value) where T : struct
        {
            return value.HasValue ? new XElement(name, value.Value) : null;
        }

        //private static XElement ToElementOrNull<T>(string name, T value) where T : class
        //{
        //    return value != null ? new XElement(name, value) : null;
        //}
    }
}
