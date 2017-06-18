using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Schema;
using PK;
using System.Net;

namespace SitePost
{
    public partial class MainForm : Form
    {
        private readonly PK.Classes.DB_Connector _DB_Connection;
        private readonly PK.Classes.DB_Helper _DB_Helper;
        private static readonly string SchemaPath = "Root.xsd";
        private static readonly XmlSchemaSet _SchemaSet = new XmlSchemaSet();

        private uint _CampaignID;
        private string _AuthData;

        public MainForm()
        {
            InitializeComponent();
            _DB_Connection = new PK.Classes.DB_Connector(Properties.Settings.Default.pk_db_CS, "administrator", "adm1234");
            _DB_Helper = new PK.Classes.DB_Helper(_DB_Connection);
            
            Dictionary<uint, string> campaigns = new Dictionary<uint, string>();
            foreach (object[] campaign in _DB_Connection.Select(DB_Table.CAMPAIGNS, new string[] { "id", "name" }))
                if (!_DB_Helper.IsMasterCampaign((uint)campaign[0]))
                    campaigns.Add((uint)campaign[0],campaign[1].ToString());

            cbCampaigns.DataSource = campaigns.ToList();
            cbCampaigns.ValueMember = "Key";
            cbCampaigns.DisplayMember = "Value";
            cbAdress.SelectedIndex = 0;
            cbUnits.SelectedIndex = 1;
            for (int i = 1; i <= 60; i++)
                cbInterval.Items.Add(i);
            cbInterval.SelectedItem = 15;
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            MessageBox.Show("Таймер сработал. Частота : " + timer.Interval + " мс");
            PostApplications();
            timer.Start();
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            if (timer.Enabled)
            {
                timer.Stop();
                cbCampaigns.Enabled = true;
                cbAdress.Enabled = true;
                cbInterval.Enabled = true;
                cbUnits.Enabled = true;
                btStart.Text = "Старт";
            }
            else if (cbCampaigns.SelectedIndex == -1)
                    MessageBox.Show("Выберите кампанию");
                else
                {
                    cbCampaigns.Enabled = false;
                    cbAdress.Enabled = false;
                    cbInterval.Enabled = false;
                    cbUnits.Enabled = false;
                    btStart.Text = "Стоп";

                    _CampaignID = (uint)cbCampaigns.SelectedValue;

                    timer.Interval = (int)cbInterval.SelectedItem;
                    if (cbUnits.SelectedItem.ToString() == "сек.")
                        timer.Interval = timer.Interval * 1000;
                    else if (cbUnits.SelectedItem.ToString() == "мин.")
                        timer.Interval = timer.Interval * 60000;
                    else if (cbUnits.SelectedItem.ToString() == "ч")
                        timer.Interval = timer.Interval * 3600000;

                    PostApplications();
                    timer.Start();
                }
        }


        private void PostApplications()
        {
            List<object[]> appsData = _DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "id", "mcado", "chernobyl", "needs_hostel", "passing_examinations",
                "priority_right", "entrant_id" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CampaignID),
                new Tuple<string, Relation, object>("master_appl", Relation.EQUAL, false)
            });
            if (appsData.Count > 0)
            {
                XDocument PackageData = new XDocument(new XElement("Root"));
                PackageData.Root.Add(new XElement("AuthData", _AuthData));
                PackageData.Root.Add(new XElement("PackageData"));

                foreach (object[] application in appsData)
                {
                    string password = _DB_Connection.Select(DB_Table.ENTRANTS, new string[] { "personal_password" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)application[6])
                    })[0][0].ToString();

                    XElement abitur = new XElement("Abitur",
                        new XElement("Uin", application[0]),
                        new XElement("Surname", ""),
                        new XElement("Name", ""),
                        new XElement("Name2", ""),
                        new XElement("Password", password),
                        new XElement("MathBall", 0),
                        new XElement("CheckedByFISMath", 0),
                        new XElement("PhisBall", 0),
                        new XElement("CheckedByFISPhis", 0),
                        new XElement("RusBall", 0),
                        new XElement("CheckedByFISRus", 0),
                        new XElement("ObshBall", 0),
                        new XElement("CheckedByFISObsh", 0),
                        new XElement("ForenBall", 0),
                        new XElement("CheckedByFISForen", 0),
                        new XElement("IABall", 0),
                        new XElement("ODO", 0),
                        new XElement("Olymp", 0),
                        new XElement("Exam", 0),
                        new XElement("Hostel", 0),
                        new XElement("PP", 0),
                        new XElement("MCADO", 0),
                        new XElement("Chern", 0),
                        new XElement("Documents",
                            new XElement("ApplicationOfAdmission", 1),
                            new XElement("PassportCopy", 0),
                            new XElement("CertificateDiplomCopy", 0),
                            new XElement("HRRefCopy", 0),
                            new XElement("ReferralPK", 0),
                            new XElement("MedRef", 0),
                            new XElement("Photo", 0),
                            new XElement("OrphanDocument", 0),
                            new XElement("InvalidDocument", 0),
                            new XElement("PMPDocument", 0),
                            new XElement("AbsenceOfContraindicationsForTraining", 0)),
                        new XElement("Applications"));

                    if ((bool)application[4])
                        abitur.SetElementValue("Exam", 1);
                    if ((bool)application[3])
                        abitur.SetElementValue("Hostel", 1);
                    if ((bool)application[5])
                        abitur.SetElementValue("PP", 1);
                    if ((bool)application[1])
                        abitur.SetElementValue("MCADO", 1);
                    if ((bool)application[2])
                        abitur.SetElementValue("Chern", 1);

                    SetMarks((uint)application[0], abitur);
                    SetDocuments((uint)application[0], abitur);
                    foreach (XElement appl in GetEntrances((uint)application[0]))
                        abitur.Element("Applications").Add(appl);

                    PackageData.Root.Element("PackageData").Add(abitur);
                }
                //PackageData.Save("doc.xml");

                _SchemaSet.Add(null, SchemaPath);
                PackageData.Validate(_SchemaSet, (send, ee) => { throw ee.Exception; });

                byte[] bytesData = System.Text.Encoding.UTF8.GetBytes("XMLData=" + PackageData.ToString());
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cbAdress.Text);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytesData.Length;
                request.Method = "POST";
                System.IO.Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytesData, 0, bytesData.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    System.IO.Stream responseStream = response.GetResponseStream();
                    string responseStr = new System.IO.StreamReader(responseStream).ReadToEnd();
                    tbResponse.Text = responseStr;
                }
            }
        }

        private void SetMarks(uint appID, XElement abitur)
        {
            List<Tuple<uint, string>> subjectsCodes = new List<Tuple<uint, string>> {
                                new Tuple<uint, string>(_DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, PK.Classes.DB_Helper.SubjectMath), "Math"),
                                new Tuple<uint, string>(_DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, PK.Classes.DB_Helper.SubjectPhis), "Phis"),
                                new Tuple<uint, string>(_DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, PK.Classes.DB_Helper.SubjectRus), "Rus"),
                                new Tuple<uint, string>(_DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, PK.Classes.DB_Helper.SubjectObsh), "Obsh"),
                                new Tuple<uint, string>(_DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, PK.Classes.DB_Helper.SubjectForen), "Foren") };
            List<PK.Classes.DB_Queries.Mark> marks = PK.Classes.DB_Queries.GetMarks(_DB_Connection, new uint[] { appID }, _CampaignID).ToList();
            foreach (Tuple<uint, string> subject in subjectsCodes)
            {
                List<PK.Classes.DB_Queries.Mark> results = marks.FindAll(x => x.SubjID == subject.Item1);
                byte value = 0;
                byte check = 0;
                foreach (PK.Classes.DB_Queries.Mark mark in results)
                    if (mark.Value > value)
                    {
                        value = mark.Value;
                        DateTime? date = mark.FromExamDate as DateTime?;
                        if (date != null)
                            check = 2;
                        else if (mark.Checked)
                            check = 1;
                        else check = 0;
                    }
                abitur.SetElementValue(subject.Item2 + "Ball", value);
                abitur.SetElementValue("CheckedByFIS" + subject.Item2, check);
            }
        }

        private void SetDocuments(uint appID, XElement abitur)
        {
            var docs = _DB_Connection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new string[] { "documents_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, appID)
                    }).Join(_DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "id", "type" }),
                    appDocs => (uint)appDocs[0],
                    ds => (uint)ds[0],
                    (s1, s2) => new Tuple<uint, string>((uint)s2[0], s2[1].ToString())).ToList();

            foreach (Tuple<uint, string> doc in docs)
            {
                if (doc.Item2 == "identity")
                {
                    object[] passportData = _DB_Connection.Select(DB_Table.IDENTITY_DOCS_ADDITIONAL_DATA, new string[] { "last_name", "first_name", "middle_name" },
                        new List<Tuple<string, Relation, object>> {
                                    new Tuple<string, Relation, object>("document_id", Relation.EQUAL, doc.Item1)
                        })[0];
                    abitur.SetElementValue("Surname", passportData[0].ToString());
                    abitur.SetElementValue("Name", passportData[1].ToString());
                    abitur.SetElementValue("Name2", passportData[2].ToString());
                    abitur.Element("Documents").SetElementValue("PassportCopy", 1);
                }
                else if (doc.Item2 == "medical")
                {
                    List<object[]> medData = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "name" }, new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("document_id", Relation.EQUAL, doc.Item1)
                            });
                    if (medData.Count > 0 && medData[0][0].ToString() == PK.Classes.DB_Helper.MedCertificate)
                        abitur.Element("Documents").SetElementValue("MedRef", 1);
                }
                else if (doc.Item2 == "olympic" || doc.Item2 == "olympic_total" || doc.Item2 == "ukraine_olympic" || doc.Item2 == "international_olympic")
                    abitur.SetElementValue("Olymp", 1);
                else if (doc.Item2 == "photo")
                    abitur.Element("Documents").SetElementValue("Photo", 1);
                else if (doc.Item2 == "orphan")
                    abitur.Element("Documents").SetElementValue("OrphanDocument", 1);
                else if (doc.Item2 == "disability")
                    abitur.Element("Documents").SetElementValue("InvalidDocument", 1);
                else if (doc.Item2 == "high_edu_diploma" || doc.Item2 == "school_certificate")
                {
                    abitur.Element("Documents").SetElementValue("CertificateDiplomCopy", 1);
                    object origDate = _DB_Connection.Select(DB_Table.DOCUMENTS, new string[] { "original_recieved_date" }, new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("id", Relation.EQUAL, doc.Item1)
                            })[0][0];
                    DateTime? date = origDate as DateTime?;
                    if (date != null)
                        abitur.SetElementValue("ODO", 1);
                }
                else if (doc.Item2 == "academic_diploma")
                    abitur.Element("Documents").SetElementValue("HRRefCopy", 1);
                else if (doc.Item2 == "allow_education")
                    abitur.Element("Documents").SetElementValue("AbsenceOfContraindicationsForTraining", 1);
            }
        }

        private List<XElement> GetEntrances(uint appID)
        {
            List<object[]> entrances = _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "edu_form_id", "direction_id", "profile_short_name",
                        "is_agreed_date", "edu_source_id" }, new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("application_id", Relation.EQUAL, appID)
                        });
            int agreedCount = 0;
            uint lastAgreedDir = 0;
            DateTime lastAgreedDate = DateTime.MinValue;

            foreach (object[] entrance in entrances)
            {
                DateTime? date = entrance[3] as DateTime?;
                if (date != null)
                {
                    agreedCount++;
                    if ((DateTime)entrance[3] > lastAgreedDate)
                    {
                        lastAgreedDate = (DateTime)entrance[3];
                        lastAgreedDir = (uint)entrance[1];
                    }
                }
            }
            List<XElement> appls = new List<XElement>();
            foreach (object[] entrance in entrances)
            {
                XElement appl = new XElement("Application");

                if ((uint)entrance[0] == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, PK.Classes.DB_Helper.EduFormO))
                    appl.Add(new XElement("FormOfEducation", 1));
                else if ((uint)entrance[0] == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, PK.Classes.DB_Helper.EduFormOZ))
                    appl.Add(new XElement("FormOfEducation", 2));
                else if ((uint)entrance[0] == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, PK.Classes.DB_Helper.EduFormZ))
                    appl.Add(new XElement("FormOfEducation", 3));

                if ((uint)entrance[4] == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, PK.Classes.DB_Helper.EduSourceB))
                {
                    appl.Add(new XElement("Direction", (uint)entrance[1]));
                    appl.Add(new XElement("Profile", ""));
                    appl.Add(new XElement("Condition", 1));
                }
                else if ((uint)entrance[4] == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, PK.Classes.DB_Helper.EduSourceT))
                {
                    appl.Add(new XElement("Direction", (uint)entrance[1]));
                    appl.Add(new XElement("Profile", ""));
                    appl.Add(new XElement("Condition", 2));
                }
                else if ((uint)entrance[4] == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, PK.Classes.DB_Helper.EduSourceQ))
                {
                    appl.Add(new XElement("Direction", (uint)entrance[1]));
                    appl.Add(new XElement("Profile", ""));
                    appl.Add(new XElement("Condition", 3));
                }
                else if ((uint)entrance[4] == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, PK.Classes.DB_Helper.EduSourceP))
                {
                    appl.Add(new XElement("Direction", 0));
                    appl.Add(new XElement("Profile", entrance[2].ToString()));
                    appl.Add(new XElement("Condition", 4));
                }

                appl.Add(new XElement("ApplicationOfConsent", 0));

                DateTime? date = entrance[3] as DateTime?;
                if (date != null)
                    if (agreedCount == 1)
                        appl.SetElementValue("ApplicationOfConsent", 1);
                    else if (lastAgreedDir == (uint)entrance[1])
                        appl.SetElementValue("ApplicationOfConsent", 2);
                    else appl.SetElementValue("ApplicationOfConsent", 1);

                appls.Add(appl);
            }
            return appls;
        }        
    }
}
