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
    public partial class Form1 : Form,ICrossFormdataqSync
    {
        DataSet ds;
        BindingSource csCompany = new BindingSource();
        BindingSource csProducts = new BindingSource();
        public Form1()
        {
            InitializeComponent();
        }
        public Form1 OpenerForm { get; set; }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AddCompany { OpenerForm = this }.ShowDialog();
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new AddProducts { OpenerForm = this }.ShowDialog();
        }

        private void editDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new EditProduct { OpenerForm = this }.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            BindData();
            showNavigation();
        }
        public void LoadData()
        {
            ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM company", con))
                {
                    da.Fill(ds, "company");
                    ds.Tables["company"].Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for (var i = 0; i < ds.Tables["company"].Rows.Count; i++)
                    {
                        ds.Tables["company"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), ds.Tables["company"].Rows[i]["picture"].ToString()));
                    }
                    da.SelectCommand.CommandText = "SELECT * FROM products";
                    da.Fill(ds, "products");
                    DataRelation rel = new DataRelation("FK_BOOK_TOC",
                        ds.Tables["company"].Columns["companyid"],
                        ds.Tables["products"].Columns["companyid"]);
                    ds.Relations.Add(rel);
                    ds.AcceptChanges();
                }
            }
        }
        private void showNavigation()
        {
            this.lblOf.Text = (csCompany.Position + 1).ToString();
            this.lblTotal.Text = csCompany.Count.ToString();
        }
        private void BindData()
        {
            csCompany.DataSource = ds;
            csCompany.DataMember = "company";
            csProducts.DataSource = csCompany;
            csProducts.DataMember = "FK_BOOK_TOC";
            this.dataGridView1.DataSource = csProducts;
            Id.DataBindings.Add(new Binding("Text", csCompany, "companyid"));
            name.DataBindings.Add(new Binding("Text", csCompany, "name"));
            date.DataBindings.Add(new Binding("Text", csCompany, "startingdate"));
            email.DataBindings.Add(new Binding("Text", csCompany, "email"));
            address.DataBindings.Add(new Binding("Text", csCompany, "address"));     
            pictureBox1.DataBindings.Add(new Binding("Image", csCompany, "image", true));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (csCompany.Position < csCompany.Count - 1)
            {
                csCompany.MoveNext();
                showNavigation();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            csCompany.MoveLast();
            showNavigation();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            csCompany.MoveFirst();
            showNavigation();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (csCompany.Position > 0)
            {
                csCompany.MovePrevious();
                showNavigation();
            }
        }

        private void companyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new FormCompanyRpt().ShowDialog();
        }

        private void companyProductsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormCompanyProductRpt().ShowDialog();
        }

        private void editDeleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int id = (int)(this.csCompany.Current as DataRowView).Row[0];
            new CompanyEditDelete { CompanyToEditDelete = id, FormToReload = this }.ShowDialog();
        }

        public void ReloadData(List<Company> company)
        {
            foreach (var c in company)
            {
                DataRow dr = ds.Tables["company"].NewRow();
                dr[0] = c.companyid;
                dr["name"] = c.name;
                dr["startingdate"] = c.startingdate;
                dr["email"] = c.email;
                dr["address"] = c.address;
                dr["picture"] = c.picture;
                dr["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), c.picture));
                ds.Tables["employees"].Rows.Add(dr);

            }
            ds.AcceptChanges();
            csCompany.MoveLast();
        }

        public void UpdateCompany(Company company)
        {
            for (var i = 0; i < ds.Tables["company"].Rows.Count; i++)
            {
                if ((int)ds.Tables["company"].Rows[i]["companyid"] == company.companyid)
                {
                    ds.Tables["company"].Rows[i]["name"] = company.name;
                    ds.Tables["company"].Rows[i]["startingdate"] = company.startingdate;
                    ds.Tables["company"].Rows[i]["email"] = company.email;
                    ds.Tables["company"].Rows[i]["address"] = company.address;
                    ds.Tables["company"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), company.picture));
                    break;
                }
            }
            ds.AcceptChanges();
        }

        public void RemoveCompany(int id)
        {
            for (var i = 0; i < ds.Tables["company"].Rows.Count; i++)
            {
                if ((int)ds.Tables["company"].Rows[i]["companyid"] == id)
                {
                    ds.Tables["company"].Rows.RemoveAt(i);
                    break;
                }
            }
            ds.AcceptChanges();
        }
    }
}
