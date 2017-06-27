using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PK.Forms
{
    partial class Statistics : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;

        public Statistics(Classes.DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;
        }

        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Name == tpGeneral.Name)
            {
                var dateGroups = _DB_Connection.Select(
                DB_Table.APPLICATIONS,
                new string[] { "id", "entrant_id", "registration_time" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Classes.Settings.CurrentCampaignID) }
                ).Join(
                _DB_Connection.Select(
                DB_Table.ENTRANTS,
                "id", "home_phone", "mobile_phone"
                ),
                k1 => k1[1],
                k2 => k2[0],
                (s1, s2) => new
                {
                    ApplID = (uint)s1[0],
                    EntrID = (uint)s1[1],
                    HomePhone = s2[1].ToString(),
                    MobilePhone = s2[2].ToString(),
                    RegTime = (DateTime)s1[2]
                }).Join(
                _DB_Connection.Select(
                DB_Table.ENTRANTS_VIEW,
                "id", "last_name", "first_name", "middle_name"
                ),
                k1 => k1.EntrID,
                k2 => k2[0],
                (s1, s2) => new
                {
                    s1.ApplID,
                    LastName = s2[1].ToString(),
                    FirstName = s2[2].ToString(),
                    MiddleName = s2[3].ToString(),
                    s1.HomePhone,
                    s1.MobilePhone,
                    s1.RegTime
                }).GroupBy(
                k => k.RegTime.Date,
                (k, g) => new
                {
                    Date = k,
                    Applications = g
                });

                foreach (var dateGroup in dateGroups)
                    chartGeneral.Series[0].Points.AddXY(dateGroup.Date.ToShortDateString(), dateGroup.Applications.Count());
            }
            if (e.TabPage.Name == tpRegistrators.Name)
            {
                var users = _DB_Connection.Select(                DB_Table.USERS, "login"                 ).GroupJoin(
                _DB_Connection.Select(                DB_Table.APPLICATIONS,                "registrator_login"                ),
                k1 => k1[0],
                k2 => k2[0],
                (k, g) => new
                {
                    Login = k[0].ToString(),
                    ApplicationsCount = g.Count()
                });
                
                foreach (var user in users.Where(s=>s.ApplicationsCount!=0).OrderBy(s=>s.ApplicationsCount))
                    chartRegistrators.Series[0].Points.AddXY(user.Login,user.ApplicationsCount );
            }
        }
    }
}
