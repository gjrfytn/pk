using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using SharedClasses.DB;

namespace PK.Forms
{
    public partial class OnlineApplSearch : Form
    {
        class Entrant
        {
            public List<ApplicationEntrance> applications {get; set;}
            //public List<Doc>
        }

        class ApplicationEntrance
        {
            public uint application_id { get; set; }
            public string faculty_short_name { get; set; }
            public uint direction_id { get; set; }
            public uint edu_form_dict_id { get; set; }
            public uint edu_form_id { get; set; }
            public uint edu_source_dict_id { get; set; }
            public uint edu_source_id { get; set; }
            public string profile_short_name { get; set; }
            public uint target_organization_id { get; set; }
        }

        class Document
        {
            public uint id;
            public string type;
            public string series;
            public string number;
            public string date;
            public string organization;
        }

        class IdentityDocument:Document
        {
            public string last_name;
            public string first_name;
            public string middle_name;
            public string gender;
            public string subdivision_code;
            public string nationality;
            public string birth_date;
            public string birth_place;
            public string reg_region;
            public string reg_district;
            public string reg_town;
            public string reg_street;
            public string reg_house;
            public string reg_flat;
            public string reg_index;
        }

        private readonly DB_Connector _DB_Connection;
        private readonly DB_Connector _madi_priem_DB_Connection;
        private readonly string _UserLogin;
        private readonly string _UserRole;
        private uint selectedApplicationId = 0;

        public OnlineApplSearch(string userRole, string userLogin)
        {
            _UserRole = userRole;
            _UserLogin = userLogin;
            _DB_Connection = new DB_Connector(Properties.Settings.Default.pk_db_CS, _UserRole,
                    new DB_Connector(Properties.Settings.Default.pk_db_CS, "initial", "1234").Select(
                    DB_Table.ROLES_PASSWORDS,
                    new string[] { "password" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("role", Relation.EQUAL, _UserRole) }
                    )[0][0].ToString());
            _madi_priem_DB_Connection = new DB_Connector(Properties.Settings.Default.online_application_CS, "root", "s0hnut");
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }


        private void tbOnlineApplicationID_TextChanged(object sender, EventArgs e)
        {
            searchOnlineApplications();
        }

        private static DataTable GetData(string sqlCommand)
        {
            MySqlConnection _Connection = new MySqlConnection(Properties.Settings.Default.online_application_CS + " user = root; password = s0hnut;");
            _Connection.Open();

            MySqlCommand cmd = new MySqlCommand(sqlCommand, _Connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            adapter.SelectCommand = cmd;

            DataTable table = new DataTable();
            adapter.Fill(table);

            return table;
        }

        private void dgvOnlineApplications_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            MySqlCommand cmd;            
            List<ApplicationEntrance> dir_prof = new List<ApplicationEntrance>();
            List<Document> documnets = new List<Document>();
            TreeNode[] tns;

            void addEduFormToTreeView(List<ApplicationEntrance> appl, string key, string text, int edu_form, int edu_source, MySqlConnection Conn, TreeNode[] dir_prof_node)
            {
                string direction_name = "", direction_short_name = "", profile_name = "", target_org_name = "";
                foreach (ApplicationEntrance element in appl)
                {
                    cmd = new MySqlCommand("SELECT * FROM dictionary_10_items WHERE id=" + element.direction_id.ToString(), Conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            direction_name = reader.GetString(1);
                    };

                    cmd = new MySqlCommand("SELECT * FROM directions WHERE direction_id=" + element.direction_id.ToString() + " AND faculty_short_name='" + element.faculty_short_name + "'", Conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            direction_short_name = reader.GetString(2);
                    }

                    if (element.profile_short_name != "")
                    {
                        cmd = new MySqlCommand("SELECT * FROM profiles WHERE short_name='" + element.profile_short_name + "'", Conn);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                profile_name = reader.GetString(3);
                        }
                    }

                    if (element.target_organization_id != 0)
                    {
                        cmd = new MySqlCommand("SELECT * FROM target_organizations WHERE id=" + element.target_organization_id, Conn);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                target_org_name = reader.GetString(1);
                        }
                    }

                    if (element.edu_source_id == 14 || element.edu_source_id == 16 || element.edu_source_id == 20)
                    {
                        if (element.edu_source_id == edu_source && element.edu_form_id == edu_form)
                        {
                            if (!dir_prof_node[0].Nodes.ContainsKey(key))
                                dir_prof_node[0].Nodes.Add(key, text);
                            TreeNode[] tns_tmp = tvOnlineSelectedApplication.Nodes.Find(key, true);
                            tns_tmp[0].Nodes.Add(direction_short_name + " " + direction_name + " (" + element.faculty_short_name + ")");
                        }
                        /*
                        if (element.edu_source_id == 16) {
                            if (!tvOnlineSelectedApplication.Nodes.ContainsKey("target_org"))
                                tvOnlineSelectedApplication.Nodes.Add("target_org", "Целевые организации");
                            TreeNode[] tns_tmp = tvOnlineSelectedApplication.Nodes.Find("target_org", true);
                            if (element.edu_form_id == 11 && !tns_tmp[0].Nodes.ContainsKey("ob_target_org"))
                            {
                                tns_tmp[0].Nodes.Add("ob_target_org", "Очная форма обучения: " + target_org_name);
                            }
                            else if (element.edu_form_id == 12 && !tns_tmp[0].Nodes.ContainsKey("ozb_target_org"))
                            {
                                tns_tmp[0].Nodes.Add("ozb_target_org", "Очно-заочная форма обучения: " + target_org_name);
                            }
                        }
                        */
                    }
                    else if (element.edu_source_id == 15)
                        if (element.edu_source_id == edu_source && element.edu_form_id == edu_form)
                        {
                            if (!dir_prof_node[0].Nodes.ContainsKey(key))
                                dir_prof_node[0].Nodes.Add(key, text);
                            TreeNode[] tns_tmp = tvOnlineSelectedApplication.Nodes.Find(key, true);
                            tns_tmp[0].Nodes.Add(element.profile_short_name + " " + profile_name + " (" + element.faculty_short_name + ")");
                        }
                }
            } 

            if (dgvOnlineApplications.SelectedRows.Count > 0)
                selectedApplicationId = (uint)dgvOnlineApplications.SelectedRows[0].Cells[dgvOnlineApplications_application_id.Index].Value;

            if (selectedApplicationId > 0)
            {
                tvOnlineSelectedApplication.BeginUpdate();
                tvOnlineSelectedApplication.Nodes.Clear();
                MySqlConnection _Connection = new MySqlConnection(Properties.Settings.Default.online_application_CS + " user = root; password = s0hnut;");
                _Connection.Open();

                cmd = new MySqlCommand("SELECT * FROM applications_entrances WHERE application_id=" + selectedApplicationId.ToString(), _Connection);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ApplicationEntrance new_ApplicationEntrance = new ApplicationEntrance
                        {
                            application_id = (uint)reader.GetValue(0),
                            faculty_short_name = reader.GetString(1),
                            direction_id = (uint)reader.GetValue(2),
                            edu_form_dict_id = (uint)reader.GetValue(3),
                            edu_form_id = (uint)reader.GetValue(4),
                            edu_source_dict_id = (uint)reader.GetValue(5),
                            edu_source_id = (uint)reader.GetValue(6),
                            profile_short_name = !reader.IsDBNull(7) ? reader.GetString(7) : "",
                            target_organization_id = !reader.IsDBNull(8) ? (uint)reader.GetValue(8) : 0,
                        };
                        dir_prof.Add(new_ApplicationEntrance);
                    }
                }                
                if (!tvOnlineSelectedApplication.Nodes.ContainsKey("dir_prof"))
                    tvOnlineSelectedApplication.Nodes.Add("dir_prof", "Выбранные направления/профили");
                tns = tvOnlineSelectedApplication.Nodes.Find("dir_prof", true);
                addEduFormToTreeView(dir_prof, "ob", "Очная бюджетная форма", 11, 14, _Connection, tns);
                addEduFormToTreeView(dir_prof, "op", "Очная платная форма", 11, 15, _Connection, tns);
                addEduFormToTreeView(dir_prof, "ozb", "Очно-заочная бюджетная форма", 12, 14, _Connection, tns);
                addEduFormToTreeView(dir_prof, "ozp", "Очно-заочная платная форма", 12, 15, _Connection, tns);
                addEduFormToTreeView(dir_prof, "zp", "Заочная платная форма", 10, 15, _Connection, tns);
                addEduFormToTreeView(dir_prof, "ob_quota", "Очная бюджетная форма. Квота", 11, 20, _Connection, tns);
                addEduFormToTreeView(dir_prof, "ozb_quota", "Очно-заочная бюджетная форма. Квота", 12, 20, _Connection, tns);
                addEduFormToTreeView(dir_prof, "ob_target", "Очная бюджетная форма. Целевой приём", 11, 16, _Connection, tns);
                addEduFormToTreeView(dir_prof, "ozb_target", "Очно-заочная бюджетная форма. Целевой приём", 12, 16, _Connection, tns);

                cmd = new MySqlCommand("SELECT documents.* FROM _applications_has_documents AS ahd JOIN documents ON documents.id = ahd.documents_id WHERE ahd.applications_id = " + selectedApplicationId.ToString(), _Connection);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        documnets.Add(new Document {
                            id = (uint)reader.GetInt32("id"),
                            type = reader.GetString("type"),
                            series = !reader.IsDBNull(2) ? reader.GetString("series") : "",
                            number = !reader.IsDBNull(3) ? reader.GetString("number") : "",
                            date = !reader.IsDBNull(4) ? reader.GetString("date") : "",
                            organization = !reader.IsDBNull(5) ? reader.GetString("organization") : ""
                        });
                    }
                }
                foreach (Document document in documnets)
                {
                    if (document.type == "identity")
                    {

                        TreeNode identity_doc_node = tvOnlineSelectedApplication.Nodes.Add("identity_doc", "Документ, удостоверяющий личность");
                        cmd = new MySqlCommand(@"SELECT 
idad.`*`, 
di_doc_type.`name` AS doc_type,
di_gender.`name` AS gender, 
di_nationality.`name` AS nationality
FROM identity_docs_additional_data AS idad
JOIN dictionaries_items AS di_doc_type ON di_doc_type.dictionary_id=idad.type_dict_id AND di_doc_type.item_id=idad.type_id
JOIN dictionaries_items AS di_gender ON di_gender.dictionary_id=idad.gender_dict_id AND di_gender.item_id=idad.gender_id
JOIN dictionaries_items AS di_nationality ON di_nationality.dictionary_id=idad.nationality_dict_id AND di_nationality.item_id=idad.nationality_id 
WHERE idad.document_id=" + document.id.ToString(), _Connection);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string last_name = !reader.IsDBNull(1) ? reader.GetString("last_name") : "";
                                string first_name = !reader.IsDBNull(2) ? reader.GetString("first_name") : "";
                                string middle_name = !reader.IsDBNull(3) ? reader.GetString("middle_name") : "";
                                tvOnlineSelectedApplication.Nodes.Insert(0, last_name + " " + first_name + " " + middle_name);
                                identity_doc_node.Nodes.Add("Пол: " + (!reader.IsDBNull(21) ? reader.GetString("gender") : ""));
                                identity_doc_node.Nodes.Add("Дата рождения: " + (!reader.IsDBNull(11) ? reader.GetString("birth_date").Split(' ')[0] : ""));
                                identity_doc_node.Nodes.Add("Место рождения: " + (!reader.IsDBNull(12) ? reader.GetString("birth_place") : ""));
                                identity_doc_node.Nodes.Add("Гражданство: " + (!reader.IsDBNull(22) ? reader.GetString("nationality") : ""));
                                identity_doc_node.Nodes.Add("Тип документа: " + (!reader.IsDBNull(20) ? reader.GetString("doc_type") : ""));
                                identity_doc_node.Nodes.Add("Серия и номер: " + document.series + " " + document.number);
                                identity_doc_node.Nodes.Add("Кем выдан: " + document.organization);
                                identity_doc_node.Nodes.Add("Дата выдачи: " + document.date.Split(' ')[0]);
                                identity_doc_node.Nodes.Add("Код подразделения: " + (!reader.IsDBNull(6) ? reader.GetString("subdivision_code") : ""));
                                string reg_index = (reader.GetString("reg_index") !="" ? reader.GetString("reg_index") + ", " : "");
                                string reg_region = (reader.GetString("reg_region")!="" ? reader.GetString("reg_region") + ", " : "");
                                string reg_district = (reader.GetString("reg_district")!="" ? reader.GetString("reg_district") + ", " : "");
                                string reg_town = (reader.GetString("reg_town") !="" ? reader.GetString("reg_town") + ", " : "");
                                string reg_street = (reader.GetString("reg_street") !="" ? reader.GetString("reg_street") + ", " : "");
                                string reg_house = (reader.GetString("reg_house") !="" ? reader.GetString("reg_house") + ", " : "");
                                string reg_flat = (reader.GetString("reg_flat") !="" ? reader.GetString("reg_flat") + ", " : "");
                                identity_doc_node.Nodes.Add("Адрес регистрации: " + reg_index + reg_region + reg_district + reg_town + reg_street + reg_house + reg_flat);
                            }
                        }

                        
                    }
                }

                cmd = new MySqlCommand("SELECT * FROM applications WHERE id=" + selectedApplicationId.ToString(), _Connection);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tvOnlineSelectedApplication.Nodes.Add("others_features", "Дополнительные признаки");
                        tns = tvOnlineSelectedApplication.Nodes.Find("others_features", true);
                        tns[0].Nodes.Add("Нуждается в общежитии: " + (!reader.IsDBNull(3) ? (reader.GetBoolean(3) ? "да" : "нет") : "нет"));
                        tns[0].Nodes.Add("Иностранный язык: " + (!reader.IsDBNull(4) ? reader.GetString(4) : ""));
                        tns[0].Nodes.Add("Высшее образование получаю: " + (reader.GetBoolean(5) ? "впервые" : "повторно"));
                        tns[0].Nodes.Add("МЦАДО: " + (!reader.IsDBNull(6) ? (reader.GetBoolean(6) ? "да" : "нет") : "нет"));
                        tns[0].Nodes.Add("Чернобыльская зона: " + (!reader.IsDBNull(7) ? (reader.GetBoolean(7) ? "да" : "нет") : "нет"));
                        tns[0].Nodes.Add("Преимущественное право: " + (!reader.IsDBNull(8) ? (reader.GetBoolean(8) ? "да" : "нет") : "нет"));
                        tns[0].Nodes.Add("Специальные условия на вступ. исп.: " + (!reader.IsDBNull(9) ? (reader.GetBoolean(9) ? "да" : "нет") : "нет"));
                        tns[0].Nodes.Add("Соотечественник: " + (!reader.IsDBNull(11) ? (reader.GetBoolean(11) ? "да" : "нет") : "нет"));
                        tns[0].Nodes.Add("Посещал подготовительные курсы МАДИ: " + (!reader.IsDBNull(12) ? (reader.GetBoolean(12) ? "да" : "нет") : "нет"));

                    }
                }

                tvOnlineSelectedApplication.EndUpdate();
            }
        }
        
        private void searchOnlineApplications()
        {
            Cursor.Current = Cursors.WaitCursor;
            dgvOnlineApplications.AutoGenerateColumns = false;
            BindingSource bindingSource = new BindingSource();
            string where_conditions = "";
            if (tbOnlineApplicationID.Text.Trim() != "")
                where_conditions = "application_id=" + tbOnlineApplicationID.Text.Trim();
            if (tbOnlineSeries.Text.Trim() != "")
                where_conditions += where_conditions == "" ? ("series=" + tbOnlineSeries.Text.Trim()) : (" AND series=" + tbOnlineSeries.Text.Trim());
            if (tbOnlineNumber.Text.Trim() != "")
                where_conditions += where_conditions == "" ? ("number=" + tbOnlineNumber.Text.Trim()) : (" AND number=" + tbOnlineNumber.Text.Trim());

            if (where_conditions != "")
            {
                bindingSource.DataSource = GetData("SELECT * FROM application_id_entrants_view WHERE " + where_conditions);
                dgvOnlineApplications.DataSource = bindingSource;
                if (bindingSource.Count > 0)
                {
                    dgvOnlineApplications_CellClick(dgvOnlineApplications, new DataGridViewCellEventArgs(0, 0));
                }
                
            }
            Cursor.Current = Cursors.Default;
        }

        private void tbOnlineSeries_TextChanged(object sender, EventArgs e)
        {
            searchOnlineApplications();
        }

        private void tbOnlineNumber_TextChanged(object sender, EventArgs e)
        {
            searchOnlineApplications();
        }

        private void btnCopyOnlineApplication_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            ApplicationEdit form = new ApplicationEdit(_DB_Connection, Classes.Settings.CurrentCampaignID, _UserLogin, null);
            form._Loading = true;
            form.LoadApplication(true, _madi_priem_DB_Connection, selectedApplicationId);
            form._Loading = false;
            Cursor.Current = Cursors.Default;

            form.ShowDialog();
        }

    }
}
