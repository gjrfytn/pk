using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedClasses.DB;

namespace PK.Forms
{
    partial class ApplicationDocsPrint : Form
    {
        private readonly DB_Connector _DB_Connection;
        private readonly uint _ID;

        public ApplicationDocsPrint(DB_Connector connection, uint id)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _ID = id;

            cbAdmAgreement.Enabled = _DB_Connection.Select(
                DB_Table.APPLICATIONS_ENTRANCES,
                new string[] { "is_agreed_date", "is_disagreed_date" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("application_id", Relation.EQUAL, _ID) }
                ).Any(s => s[0] as DateTime? != null && s[1] as DateTime? == null);
            cbAdmAgreement.Checked = cbAdmAgreement.Enabled;
        }

        private void bPrint_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            SharedClasses.Utility.Print(Classes.OutDocuments.Entrant.Documents(
                _DB_Connection,
                _ID,
                cbMoveJournal.Checked,
                cbInventory.Checked,
                cbPercRecordFace.Checked,
                cbReceipt.Checked,
                cbPercRecordBack.Checked,
                cbAdmAgreement.Checked
                ));
            Cursor.Current = Cursors.Default;

            DialogResult = DialogResult.OK;
        }

        private void bOpen_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            System.Diagnostics.Process.Start(Classes.OutDocuments.Entrant.Documents(
                _DB_Connection,
                _ID,
                cbMoveJournal.Checked,
                cbInventory.Checked,
                cbPercRecordFace.Checked,
                cbReceipt.Checked,
                cbPercRecordBack.Checked,
                cbAdmAgreement.Checked
                ));
            Cursor.Current = Cursors.Default;

            DialogResult = DialogResult.OK;
        }
    }
}
