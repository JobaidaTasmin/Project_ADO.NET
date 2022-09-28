using ComProductsProject.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComProductsProject
{
    public partial class FormCompanyRpt : Form
    {
        public FormCompanyRpt()
        {
            InitializeComponent();
        }

        private void FormCompanyRpt_Load(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM company", con))
                {
                    da.Fill(ds, "companyi");
                    ds.Tables["companyi"].Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for (var i = 0; i < ds.Tables["companyi"].Rows.Count; i++)
                    {
                        ds.Tables["companyi"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), ds.Tables["companyi"].Rows[i]["picture"].ToString()));
                    }
                    CompanyRpt rpt = new CompanyRpt();
                    rpt.SetDataSource(ds);
                    crystalReportViewer1.ReportSource = rpt;
                    rpt.Refresh();
                    crystalReportViewer1.Refresh();
                }
            }
        }
    }
}
