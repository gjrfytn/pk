using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Net;
using SharedClasses.DB;

namespace SitePost
{
    public partial class MainForm : Form
    {
        private readonly DB_Connector _DB_Connection;
        private readonly DB_Helper _DB_Helper;
        private static readonly string SchemaPath = "Root.xsd";
        private static readonly XmlSchemaSet _SchemaSet = new XmlSchemaSet();

        private uint _CampaignID;
        private string _AuthData;
        private Dictionary<uint, string> _SubjectsCodes;

        public MainForm()
        {
            InitializeComponent();

            //_DB_Connection = new PK.Classes.DB_Connector("server = localhost; port = 3306; database = pk_db;"/*Properties.Settings.Default.pk_db_CS*/, "administrator", "adm1234");
            _DB_Connection = new DB_Connector("server = serv-priem; port = 3306; database = pk_db;"/*Properties.Settings.Default.pk_db_CS*/, "administrator", "adm1234");

            _DB_Helper = new DB_Helper(_DB_Connection);
            Dictionary<uint, string> campaigns = new Dictionary<uint, string>();
            foreach (object[] campaign in _DB_Connection.Select(DB_Table.CAMPAIGNS, new string[] { "id", "name" }))
                if (!_DB_Helper.IsMasterCampaign((uint)campaign[0]))
                    campaigns.Add((uint)campaign[0],campaign[1].ToString());

            cbCampaigns.DataSource = campaigns.ToList();
            cbCampaigns.ValueMember = "Key";
            cbCampaigns.DisplayMember = "Value";
            cbAdress.SelectedIndex = 0;
            cbAdress.Enabled = cbPost.Checked;
            cbUnits.SelectedIndex = 1;
            for (int i = 1; i <= 60; i++)
                cbInterval.Items.Add(i);
            cbInterval.SelectedItem = 15;

            _SubjectsCodes = new Dictionary<uint, string>
            {
                { _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, DB_Helper.SubjectMath), "Math"},
                { _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, DB_Helper.SubjectPhis), "Phis"},
                { _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, DB_Helper.SubjectRus), "Rus"},
                { _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, DB_Helper.SubjectObsh), "Obsh"},
                { _DB_Helper.GetDictionaryItemID(FIS_Dictionary.SUBJECTS, DB_Helper.SubjectForen), "Foren"}
            };
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            PostApplications();
            timer.Start();
            Cursor.Current = Cursors.Default;
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

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                ShowInTaskbar = false;
                notifyIcon.Visible = true;
            }
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
                ShowInTaskbar = true;
                notifyIcon.Visible = false;
            }
        }

        private void cbPost_CheckedChanged(object sender, EventArgs e)
        {
            methodGroupBox.Enabled = ((CheckBox)sender).Checked;
            cbAdress.Enabled = ((CheckBox)sender).Checked;
        }


        private void PostApplications()
        {
            List<object[]> appsData = _DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "id", "mcado", "chernobyl", "needs_hostel", "passing_examinations",
                "priority_right", "entrant_id" }, new List<Tuple<string, Relation, object>>
            {
                new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, _CampaignID),
                new Tuple<string, Relation, object>("master_appl", Relation.EQUAL, false),
                new Tuple<string, Relation, object>("status", Relation.NOT_EQUAL, "withdrawn")
            });

            string connectionStr = "server=" + tbDirectToDBServer.Text + ";port=3306;database=" + tbDirectToDBBaseName.Text + ";user=" + tbDirectToDBUser.Text + ";password=" + tbDirectToDBPassword.Text + ";";
            MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(connectionStr);

            if (appsData.Count > 0)
            {
                if ((cbPost.Checked) && (rbDirectToDB.Checked))
                {
                    connection.Open();
                    string Query = "SET foreign_key_checks = 0;" +
                            "TRUNCATE TABLE `" + tbDirectToDBBaseName.Text + "`.`abitur_tmptable`;" +
                            "TRUNCATE TABLE `" + tbDirectToDBBaseName.Text + "`.`application_tmptable`;";
                    new MySql.Data.MySqlClient.MySqlCommand(Query, connection).ExecuteNonQuery();
                }

                XDocument PackageData = new XDocument(new XElement("Root"));
                PackageData.Root.Add(new XElement("AuthData", _AuthData));
                PackageData.Root.Add(new XElement("PackageData"));

                IEnumerable<DB_Queries.Mark> marks = DB_Queries.GetMarks(_DB_Connection, appsData.Select(s=>(uint)s[0]), _CampaignID);
                IEnumerable<DB_Queries.Document> documents = DB_Queries.GetDocuments(_DB_Connection, appsData.Select(s => (uint)s[0]), _CampaignID);
                var passwords=  _DB_Connection.Select(DB_Table.ENTRANTS, "id","personal_password").Select(s=>new { EntrID=(uint)s[0],Password=s[1].ToString() });
                var names = _DB_Connection.Select(DB_Table.ENTRANTS_VIEW, "id", "last_name", "first_name", "middle_name").Select(s => new
                {
                    EntrID = (uint)s[0],
                    LastName = s[1].ToString(),
                    FirstName = s[2].ToString(),
                    MiddleName = s[3] as string
                });
                
                foreach (object[] application in appsData)
                {
                    XElement abitur = new XElement("Abitur",
                        new XElement("Uin", application[0]),
                        new XElement("Surname", names.Single(s=>s.EntrID==(uint) application[6]).LastName),
                        new XElement("Name", names.Single(s => s.EntrID == (uint)application[6]).FirstName),
                        new XElement("Name2", names.Single(s => s.EntrID == (uint)application[6]).MiddleName),
                        new XElement("Password", passwords.Single(s => s.EntrID == (uint)application[6]).Password),
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
                            //new XElement("PMPDocument", 0),
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

                    SetMarks((uint)application[0], abitur, marks);
                    SetDocuments((uint)application[0], abitur,documents);
                    SetIA((uint)application[0], abitur);
                    foreach (XElement appl in GetEntrances((uint)application[0]))
                        abitur.Element("Applications").Add(appl);

                    if ((cbPost.Checked) && (rbDirectToDB.Checked))
                    {
                        string Query = "INSERT INTO `" + tbDirectToDBBaseName.Text + "`.`abitur_tmptable` " + 
                        "(`abitur_id`, `uin`, `surname`, `_name`, `name2`, " + 
                        "`math_ball`, `checked_by_FIS_math`, `phis_ball`, `checked_by_FIS_phis`, `rus_ball`, `checked_by_FIS_rus`, " +
                        "`obsh_ball`, `checked_by_FIS_obsh`, `foren_ball`, `checked_by_FIS_foren`, `IA_ball`, " + 
                        "`ODO`, `Olymp`, `Exam`, `hostel`, `PP`, `MCADO`, `Chern`, `appl_of_admission`, " + 
                        "`passport_copy`, `a_d_copy`, `hr_ref_copy`, `referral_pk`, `med_ref`, `photo`, `password`, " +
                        "`orphan_document`, `invalid_document`, `pmp_document`, `absence_of_contraindications`) VALUES " +
                        "(0, " + abitur.Element("Uin").Value.ToString() + ", '" + abitur.Element("Surname").Value.ToString() + "', '" + 
                        abitur.Element("Name").Value.ToString() + "' ,'" + abitur.Element("Name2").Value.ToString() + "' ," + 
                        abitur.Element("MathBall").Value.ToString() + " ," + abitur.Element("CheckedByFISMath").Value.ToString() + " ," +
                        abitur.Element("PhisBall").Value.ToString() + " ," + abitur.Element("CheckedByFISPhis").Value.ToString() + " ," +
                        abitur.Element("RusBall").Value.ToString() + " ," + abitur.Element("CheckedByFISRus").Value.ToString() + " ," +
                        abitur.Element("ObshBall").Value.ToString() + " ," + abitur.Element("CheckedByFISObsh").Value.ToString() + " ," +
                        abitur.Element("ForenBall").Value.ToString() + " ," + abitur.Element("CheckedByFISForen").Value.ToString() + " ," +
                        abitur.Element("IABall").Value.ToString() + " ," + abitur.Element("ODO").Value.ToString() + ", " +
                        abitur.Element("Olymp").Value.ToString() + " ," + abitur.Element("Exam").Value.ToString() + ", " +
                        abitur.Element("Hostel").Value.ToString() + " ," + abitur.Element("PP").Value.ToString() + ", " +
                        abitur.Element("MCADO").Value.ToString() + ", " + abitur.Element("Chern").Value.ToString() + " ," +
                        abitur.Element("Documents").Element("ApplicationOfAdmission").Value.ToString() + " ," +
                        abitur.Element("Documents").Element("PassportCopy").Value.ToString() + " ," +
                        abitur.Element("Documents").Element("CertificateDiplomCopy").Value.ToString() + " ," +
                        abitur.Element("Documents").Element("HRRefCopy").Value.ToString() + " ," +
                        abitur.Element("Documents").Element("ReferralPK").Value.ToString() + " ," +
                        abitur.Element("Documents").Element("MedRef").Value.ToString() + " ," +
                        abitur.Element("Documents").Element("Photo").Value.ToString() + ", '" + abitur.Element("Password").Value.ToString() + "', " +
                        abitur.Element("Documents").Element("OrphanDocument").Value.ToString() + ", " +
                        abitur.Element("Documents").Element("InvalidDocument").Value.ToString() + ", 0, " +
                        abitur.Element("Documents").Element("AbsenceOfContraindicationsForTraining").Value.ToString() + ");";

                        string direction, profile;
                        int direction_id = 1000, profile_id = 1000;
                        string query_tmp;

                        foreach (XElement appl in abitur.Element("Applications").Elements())
                        {
                            direction = appl.Element("Direction").Value;
                            profile = appl.Element("Profile").Value;
                            if (direction != "0")
                            {
                                query_tmp = "SELECT dt.direction_id FROM `" + tbDirectToDBBaseName.Text + "`.`direction_table` AS dt, " +
                                "`" + tbDirectToDBBaseName.Text + "`.`faculty_table` AS ft WHERE (dt.id_by_FIS=" + direction + ") AND " +
                                "(ft.short_caption='" + appl.Element("Faculty").Value + "') AND " + 
                                "(dt.faculty_id=ft.faculty_id)";
                                direction_id = Convert.ToInt16(new MySql.Data.MySqlClient.MySqlCommand(query_tmp, connection).ExecuteScalar().ToString());
                            }
                            if (profile != "") {
                                query_tmp = "SELECT profile_id FROM `" + tbDirectToDBBaseName.Text + "`.`profile_table` WHERE short_caption='" + profile + "'";
                                profile_id = Convert.ToInt16(new MySql.Data.MySqlClient.MySqlCommand(query_tmp, connection).ExecuteScalar().ToString());
                            }
                            Query = Query + "INSERT INTO `" + tbDirectToDBBaseName.Text + "`.`application_tmptable` " +
                            "(`application_id`, `uin`, `direction_id`, `profile_id`, `_condition`, `appl_of_consent`, `form_of_education`) VALUES " +
                            "(0, " + abitur.Element("Uin").Value + ", " + direction_id.ToString() + ", " + profile_id.ToString() + " ," +
                            appl.Element("Condition").Value + " ," + appl.Element("ApplicationOfConsent").Value + " ," + appl.Element("FormOfEducation").Value + ");";
                        }
                        new MySql.Data.MySqlClient.MySqlCommand(Query, connection).ExecuteNonQuery();
                    }

                    PackageData.Root.Element("PackageData").Add(abitur);
                }
                _SchemaSet.Add(null, SchemaPath);
                PackageData.Validate(_SchemaSet, (send, ee) => { throw ee.Exception; });
                if ((cbPost.Checked) && (rbDirectToDB.Checked))
                {
                    connection.Close();
                    HttpWebRequest HttpRequest = (HttpWebRequest)WebRequest.Create(tbDirectToDBAdressForScript.Text);
                    HttpWebResponse HttpResponse;
                    HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();
                    if (HttpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        System.IO.Stream responseStream = HttpResponse.GetResponseStream();
                        string responseStr = new System.IO.StreamReader(responseStream).ReadToEnd();
                        tbResponse.Text = tbResponse.Text + responseStr + "\r\n";
                        HttpResponse.Close();
                    }

                }
                if (cbSave.Checked)
                    PackageData.Save("doc.xml");

                if ((cbPost.Checked) && (rbPostMethod.Checked))
                {
                    byte[] bytesData = System.Text.Encoding.UTF8.GetBytes("XMLData=" + PackageData.ToString());
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cbAdress.Text);
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
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
                if ((cbPost.Checked) && (rbFTPMethod.Checked))
                {
                    // Get the object used to communicate with the server.
                    FtpWebRequest FtpRequest = (FtpWebRequest)WebRequest.Create(tbServer.Text+ "/doc.xml");
                    FtpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                    // This example assumes the FTP site uses anonymous logon.
                    FtpRequest.Credentials = new NetworkCredential(tbUser.Text, tbPassword.Text);

                    // Copy the contents of the file to the request stream.
                    System.IO.StreamReader sourceStream = new System.IO.StreamReader("doc.xml");
                    byte[] fileContents = System.Text.Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                    sourceStream.Close();
                    FtpRequest.ContentLength = fileContents.Length;

                    System.IO.Stream requestStream = FtpRequest.GetRequestStream();
                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();
                                        
                    FtpWebResponse FtpResponse = (FtpWebResponse)FtpRequest.GetResponse();
                    tbResponse.Text = tbResponse.Text + DateTime.Now.ToString() + ": " + FtpResponse.StatusDescription + "\n";
                    FtpResponse.Close();
                    //FtpResponse.StatusCode=FtpStatusCode.ClosingData
                    if (FtpResponse.StatusCode == FtpStatusCode.ClosingData)
                    {
                        HttpWebRequest HttpRequest = (HttpWebRequest)WebRequest.Create(tbAdress.Text);
                        HttpWebResponse HttpResponse;
                        HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();
                        if (HttpResponse.StatusCode == HttpStatusCode.OK)
                        {
                            System.IO.Stream responseStream = HttpResponse.GetResponseStream();
                            string responseStr = new System.IO.StreamReader(responseStream).ReadToEnd();
                            tbResponse.Text = tbResponse.Text + responseStr + "\r\n";
                            HttpResponse.Close();
                        }
                    }

                }

            }
        }

        private void SetMarks(uint appID, XElement abitur, IEnumerable<DB_Queries.Mark> marks)
        {
            IEnumerable< DB_Queries.Mark > applResults= marks.Where(x => x.ApplID == appID );
            foreach (var subject in _SubjectsCodes)
            {
                byte value = 0;
                byte check = 0;
                foreach (DB_Queries.Mark mark in applResults.Where(x => x.SubjID == subject.Key))
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
                abitur.SetElementValue(subject.Value + "Ball", value);
                abitur.SetElementValue("CheckedByFIS" + subject.Value, check);
            }
        }

        private void SetDocuments(uint appID, XElement abitur, IEnumerable<DB_Queries.Document> documents)
        {
            foreach (DB_Queries.Document doc in documents.Where(s=>s.ApplID==appID))
            {
                if (doc.Type == "identity")
                    abitur.Element("Documents").SetElementValue("PassportCopy", 1);
                else if (doc.Type == "medical")
                {
                    List<object[]> medData = _DB_Connection.Select(DB_Table.OTHER_DOCS_ADDITIONAL_DATA, new string[] { "name" }, new List<Tuple<string, Relation, object>>
                            {
                                new Tuple<string, Relation, object>("document_id", Relation.EQUAL, doc.ID)
                            });
                    if (medData.Count > 0 && medData[0][0].ToString() == DB_Helper.MedCertificate)
                        abitur.Element("Documents").SetElementValue("MedRef", 1);
                }
                else if (doc.Type == "olympic" || doc.Type == "olympic_total" || doc.Type == "ukraine_olympic" || doc.Type == "international_olympic")
                    abitur.SetElementValue("Olymp", 1);
                else if (doc.Type == "photos")
                    abitur.Element("Documents").SetElementValue("Photo", 1);
                else if (doc.Type == "orphan")
                    abitur.Element("Documents").SetElementValue("OrphanDocument", 1);
                else if (doc.Type == "disability")
                    abitur.Element("Documents").SetElementValue("InvalidDocument", 1);
                else if (doc.Type == "high_edu_diploma" || doc.Type == "school_certificate" || doc.Type == "middle_edu_diploma")
                {
                    abitur.Element("Documents").SetElementValue("CertificateDiplomCopy", 1);
                    if (doc.OrigDate.HasValue)
                        abitur.SetElementValue("ODO", 1);
                }
                else if (doc.Type == "academic_diploma")
                    abitur.Element("Documents").SetElementValue("HRRefCopy", 1);
                else if (doc.Type == "allow_education")
                    abitur.Element("Documents").SetElementValue("AbsenceOfContraindicationsForTraining", 1);
            }
        }

        private List<XElement> GetEntrances(uint appID)
        {
            List<object[]> entrances = _DB_Connection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "edu_form_id", "direction_id", "profile_short_name",
                        "is_agreed_date", "edu_source_id", "faculty_short_name" }, new List<Tuple<string, Relation, object>>
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

                if ((uint)entrance[0] == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, DB_Helper.EduFormO))
                    appl.Add(new XElement("FormOfEducation", 1));
                else if ((uint)entrance[0] == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, DB_Helper.EduFormOZ))
                    appl.Add(new XElement("FormOfEducation", 2));
                else if ((uint)entrance[0] == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_FORM, DB_Helper.EduFormZ))
                    appl.Add(new XElement("FormOfEducation", 3));

                if ((uint)entrance[4] == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceB))
                {
                    appl.Add(new XElement("Faculty", entrance[5].ToString()));
                    appl.Add(new XElement("Direction", (uint)entrance[1]));
                    appl.Add(new XElement("Profile", ""));
                    appl.Add(new XElement("Condition", 1));
                }
                else if ((uint)entrance[4] == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceT))
                {
                    appl.Add(new XElement("Faculty", entrance[5].ToString()));
                    appl.Add(new XElement("Direction", (uint)entrance[1]));
                    appl.Add(new XElement("Profile", ""));
                    appl.Add(new XElement("Condition", 2));
                }
                else if ((uint)entrance[4] == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceQ))
                {
                    appl.Add(new XElement("Faculty", entrance[5].ToString()));
                    appl.Add(new XElement("Direction", (uint)entrance[1]));
                    appl.Add(new XElement("Profile", ""));
                    appl.Add(new XElement("Condition", 3));
                }
                else if ((uint)entrance[4] == _DB_Helper.GetDictionaryItemID(FIS_Dictionary.EDU_SOURCE, DB_Helper.EduSourceP))
                {
                    appl.Add(new XElement("Faculty", entrance[5].ToString()));
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

        private void SetIA(uint appID, XElement abitur)
        {
            IEnumerable<ushort> IABalls = _DB_Connection.Select(DB_Table.INSTITUTION_ACHIEVEMENTS, "id", "value").Join(
                    _DB_Connection.Select(
                        DB_Table.INDIVIDUAL_ACHIEVEMENTS,
                        new string[] { "institution_achievement_id" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("application_id",Relation.EQUAL, appID)
                        }),
                    k1 => k1[0],
                    k2 => k2[0],
                    (s1, s2) => (ushort)s1[1]);

            if (IABalls.Any())
            {
                abitur.SetElementValue("IABall", IABalls.Max());
            }            
        }

        private void rbFTPMethod_Click(object sender, EventArgs e)
        {
            cbSave.Checked = true;
            cbSave.Enabled = !((RadioButton)sender).Checked;
            tcMethod.SelectedIndex = tcMethod.TabPages.IndexOf(tpFTPMethod);
        }

        private void rbPostMethod_CheckedChanged(object sender, EventArgs e)
        {       
            cbSave.Enabled = ((RadioButton)sender).Checked;
            lAdress.Enabled = ((RadioButton)sender).Checked;
            cbAdress.Enabled = ((RadioButton)sender).Checked;

            lServer.Enabled = !((RadioButton)sender).Checked;
            tbServer.Enabled = !((RadioButton)sender).Checked;
            lUser.Enabled = !((RadioButton)sender).Checked;
            tbUser.Enabled = !((RadioButton)sender).Checked;
            lPassword.Enabled = !((RadioButton)sender).Checked;
            tbPassword.Enabled = !((RadioButton)sender).Checked;
            tbAdress.Enabled = !((RadioButton)sender).Checked;
            lAdressForScript.Enabled = !((RadioButton)sender).Checked;

            lDirectToDBServer.Enabled = !((RadioButton)sender).Checked;
            tbDirectToDBServer.Enabled = !((RadioButton)sender).Checked;
            lDirectToDBUser.Enabled = !((RadioButton)sender).Checked;
            tbDirectToDBUser.Enabled = !((RadioButton)sender).Checked;
            lDirectToDBPassword.Enabled = !((RadioButton)sender).Checked;
            tbDirectToDBPassword.Enabled = !((RadioButton)sender).Checked;
            lDirectToDBBaseName.Enabled = !((RadioButton)sender).Checked;
            tbDirectToDBBaseName.Enabled = !((RadioButton)sender).Checked;
            tbDirectToDBAdressForScript.Enabled = !((RadioButton)sender).Checked;
            lDirectToDBAdressForScript.Enabled = !((RadioButton)sender).Checked;
        }

        private void rbPostMethod_Click(object sender, EventArgs e)
        {
            tcMethod.SelectedIndex = tcMethod.TabPages.IndexOf(tpPostMethod);
        }

        private void rbFTPMethod_CheckedChanged(object sender, EventArgs e)
        {
            cbSave.Enabled = !((RadioButton)sender).Checked;
            lAdress.Enabled = !((RadioButton)sender).Checked;
            cbAdress.Enabled = !((RadioButton)sender).Checked;

            lServer.Enabled = ((RadioButton)sender).Checked;
            tbServer.Enabled = ((RadioButton)sender).Checked;
            lUser.Enabled = ((RadioButton)sender).Checked;
            tbUser.Enabled = ((RadioButton)sender).Checked;
            lPassword.Enabled = ((RadioButton)sender).Checked;
            tbPassword.Enabled = ((RadioButton)sender).Checked;
            tbAdress.Enabled = ((RadioButton)sender).Checked;
            lAdressForScript.Enabled = ((RadioButton)sender).Checked;

            lDirectToDBServer.Enabled = !((RadioButton)sender).Checked;
            tbDirectToDBServer.Enabled = !((RadioButton)sender).Checked;
            lDirectToDBUser.Enabled = !((RadioButton)sender).Checked;
            tbDirectToDBUser.Enabled = !((RadioButton)sender).Checked;
            lDirectToDBPassword.Enabled = !((RadioButton)sender).Checked;
            tbDirectToDBPassword.Enabled = !((RadioButton)sender).Checked;
            lDirectToDBBaseName.Enabled = !((RadioButton)sender).Checked;
            tbDirectToDBBaseName.Enabled = !((RadioButton)sender).Checked;
            tbDirectToDBAdressForScript.Enabled = !((RadioButton)sender).Checked;
            lDirectToDBAdressForScript.Enabled = !((RadioButton)sender).Checked;
        }

        private void rbDirectToDB_CheckedChanged(object sender, EventArgs e)
        {
            cbSave.Enabled = ((RadioButton)sender).Checked;
            lAdress.Enabled = !((RadioButton)sender).Checked;
            cbAdress.Enabled = !((RadioButton)sender).Checked;

            lServer.Enabled = !((RadioButton)sender).Checked;
            tbServer.Enabled = !((RadioButton)sender).Checked;
            lUser.Enabled = !((RadioButton)sender).Checked;
            tbUser.Enabled = !((RadioButton)sender).Checked;
            lPassword.Enabled = !((RadioButton)sender).Checked;
            tbPassword.Enabled = !((RadioButton)sender).Checked;
            tbAdress.Enabled = !((RadioButton)sender).Checked;
            lAdressForScript.Enabled = !((RadioButton)sender).Checked;

            lDirectToDBServer.Enabled = ((RadioButton)sender).Checked;
            tbDirectToDBServer.Enabled = ((RadioButton)sender).Checked;
            lDirectToDBUser.Enabled = ((RadioButton)sender).Checked;
            tbDirectToDBUser.Enabled = ((RadioButton)sender).Checked;
            lDirectToDBPassword.Enabled = ((RadioButton)sender).Checked;
            tbDirectToDBPassword.Enabled = ((RadioButton)sender).Checked;
            lDirectToDBBaseName.Enabled = ((RadioButton)sender).Checked;
            tbDirectToDBBaseName.Enabled = ((RadioButton)sender).Checked;
            tbDirectToDBAdressForScript.Enabled = ((RadioButton)sender).Checked;
            lDirectToDBAdressForScript.Enabled = ((RadioButton)sender).Checked;
        }

        private void rbDirectToDB_Click(object sender, EventArgs e)
        {
            tcMethod.SelectedIndex = tcMethod.TabPages.IndexOf(tpDirectToDBMethod);
        }
    }
}
