using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace medicalsite
{

    public partial class Form1 : Form
    {
        private MedicalDBEntities db = new MedicalDBEntities();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(textBox1.Text)&&  Uri.TryCreate(textBox1.Text,UriKind.RelativeOrAbsolute,out Uri Url))
            {
                if (Url.IsAbsoluteUri)
                {
                    webBrowser1.Url = Url;
                    // new Uri(textBox1.Text);
                    webBrowser1.Navigate(textBox1.Text);
                }
                else
                {
                    MessageBox.Show("Pleas Insert Valid URL!");
                }
            }
            else
            {
                MessageBox.Show("Pleas enter the URL Lik first!");
            }

        }
        private void NavigateToPage()
        {
            button1.Enabled = false;
            textBox1.Enabled = false;
            webBrowser1.Navigate(textBox1.Text);

        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)ConsoleKey.Enter)
            {
                button1_Click(null, null);

            }
        }

        private void webBrowser1_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            if (e.CurrentProgress > 0 && e.MaximumProgress > 0)
            {
                toolStripProgressBar1.ProgressBar.Value = (int)(e.CurrentProgress * 100 / e.MaximumProgress);

            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            button1.Enabled = true;
            textBox1.Enabled = true;
        }
        private List<Chabter1_Tbl> GetData()
        {

            string content = webBrowser1.Document.Body.OuterHtml;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);


            List<Chabter1_Tbl> dt = new List<Chabter1_Tbl>();
            //dt.Columns.Add("CodeRange_from");
            //dt.Columns.Add("CodeRange_To");
            //dt.Columns.Add("SectionDescription");
            //dt.Columns.Add("link_data");

            var n = doc.DocumentNode.SelectNodes(xpath: "//div[@id='content']/table/tbody");



            foreach (HtmlNode table in n)

            {
                foreach (HtmlNode row in table.SelectNodes("tr"))
                {
                    //DataRow r = dt.NewRow();
                    Chabter1_Tbl chabter = new Chabter1_Tbl();
                    int i = 0;
                    ////
                    HtmlNodeCollection cells = row.SelectNodes("th|td");

                    var anchorval = cells[2].SelectSingleNode("a");
                    if (cells == null)
                        continue;
                    //r[0] = cells[0].InnerText;
                    //r[1] = cells[1].InnerText;
                    chabter.SectionDescription = cells[2].InnerText;
                    chabter.link_data = anchorval == null ? "link" : anchorval.Attributes["href"].Value;
                    var codeRange = cells[1].InnerText;
                    string[] title = codeRange.Split('-');
                    if (title!=null && title.Length>1)
                    {
                        chabter.CodeRange_from = title[0];
                        chabter.CodeRange_To = title[1];
                    }
                    //dt.Rows.Add(r);
                    dt.Add(chabter);

                    //DataRow r = dt.NewRow();
                    //int i = 0;

                    //HtmlNodeCollection cells = row.SelectNodes("th|td");
                    //var anchorval = cells[2].SelectSingleNode("a");
                    //if (cells == null)
                    //    continue;
                    //r[0] = cells[1].InnerText;
                    //r[1] = cells[2].InnerText;
                    //r[2] = anchorval == null ? "link" : anchorval.Attributes["href"].Value;

                    //dt.Rows.Add(r);
                    
                }
               
                //diseases.Section_Fk = Section_Fk;
                //List<Notes_Tbl> notes = GetData5(Html_string.FirstOrDefault().InnerHtml.ToString());
                //diseases.Diseases_destails_tbl = destails;
                //dt.Add(diseases);
            }
            dt.RemoveAt(0);
            return dt;
        }

        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventHandler e)
        {
        }

        private void BtnSave_Click(object sender, EventArgs e)
        { 
            try
            {

                if (webBrowser1.Url!=null)
                {
                    using (MedicalDBEntities db = new MedicalDBEntities())
                    {

                        List<Chabter1_Tbl> List = GetData();
                        bool result = new Home().insertChabter(List);
                        if (result)
                        {
                            foreach (var item in List)
                            {

                                List<Section_Tbl> obj = GetData(item.link_data, item.ID);
                                bool result2 = new Home().insertSection(obj);
                                foreach (var item2 in obj)
                                {
                                    List<Notes_Tbl> notes1 = GetData6(item2.link_data, item2.ID, 2);
                                        new Home().insertnotse1(notes1);
                                    List<Diseases_Tbl> dises = GetData4(item2.link_data, item2.ID);
                                    bool result3 = new Home().insertDiseases(dises);
                                    if (result3)
                                    {
                                        foreach (var item3 in dises)
                                        {
                                            foreach (var item4 in item3.Diseases_destails_tbl)
                                            {
                                                dises_Description_tbl _dises_Description = GetData3(item4.Link_data, item4.ID);
                                                new Home().insertdises(_dises_Description);
                                            }
                                        }
                                    }

                                }

                                List<Notes_Tbl> note = GetData6(item.link_data, item.ID, 1);

                                    new Home().insertnotse(note);

                            }

                        }

                        //foreach (var item4 in item6)
                        //{
                        //    Notes_Tbl _dises_Description = GetData6();
                        //    new Home().insertnotse();
                        //}
                        //List<Section_Tbl> obj = GetData();
                        //bool list1 = new Home().insertSection(obj);

                        //for (int i = 0; i < List.Count; i++)
                        //{
                        //    Chabter1_Tbl obj = new Chabter1_Tbl()
                        //    {
                        //        CodeRange_from = List.Rows[i][0].ToString(),
                        //        CodeRange_To = List.Rows[i][1].ToString(),
                        //        SectionDescription = List.Rows[i][2].ToString(),
                        //        link_data = List.Rows[i][3].ToString()
                        //    };
                        //    int result = new Home().insertChabter(obj);
                        //    if (result > 0)
                        //    {
                        //        DataTable notestbl1 = GetData6(obj.link_data);
                        //        foreach (DataRow detailsitem in notestbl1.Rows)
                        //        {
                        //            // insert chapter notes
                        //            Notes_Tbl notes5 = new Notes_Tbl()
                        //            {
                        //                Notes = detailsitem[0].ToString(),
                        //                Table_FK = result,
                        //                Fk_type = 1
                        //            };
                        //            int notes1 = new Home().insertnotse(notes5);
                        //        }
                        //        DataTable secoundtable = GetData(obj.link_data);
                        //        foreach (DataRow item in secoundtable.Rows)
                        //        {
                        //            Section_Tbl secoundobj = new Section_Tbl()
                        //            {
                        //                Code_from = item[0].ToString(),
                        //                Code_to = item[1].ToString(),
                        //                Name = item[2].ToString(),
                        //                link_data = item[3].ToString(),
                        //                Chabter1_fk = result
                        //            };
                        //            int secondresult = new Home().insertSection(secoundobj);

                        //            DataTable notestbl12 = GetData6(secoundobj.link_data);
                        //            foreach (DataRow detailsitem in notestbl1.Rows)
                        //            {
                        //                // insert chapter notes
                        //                Notes_Tbl notes5 = new Notes_Tbl()
                        //                {
                        //                    Notes = detailsitem[1].ToString(),
                        //                    Table_FK = secondresult,
                        //                    Fk_type = 2
                        //                };
                        //                int notes1 = new Home().insertnotse(notes5);
                        //            }

                        //            DataTable desis = GetData4(item[3].ToString());
                        //            foreach (DataRow desisitem in desis.Rows)
                        //            {
                        //                Diseases_Tbl thirdTable = new Diseases_Tbl()
                        //                {
                        //                    Code = desisitem[0].ToString(),
                        //                    Name = desisitem[1].ToString(),
                        //                    Section_Fk = secondresult
                        //                };
                        //                int thirdResult = new Home().insertDiseases(thirdTable);
                        //                DataTable details = GetData5(desisitem[2].ToString());
                        //                foreach (DataRow detailsitem in details.Rows)
                        //                {
                        //                    Diseases_destails_tbl fourthtable = new Diseases_destails_tbl()
                        //                    {
                        //                        Name = detailsitem[0].ToString(),
                        //                        Code = detailsitem[1].ToString(),
                        //                        Link_data = detailsitem[2].ToString(),
                        //                        Diseases_fk = thirdResult
                        //                    };
                        //                    int fourthResult = new Home().insertDiseases_destails(fourthtable);
                        //                    if (fourthResult > 0)
                        //                    {
                        //                        dises_Description_tbl thirdobj = GetData3(fourthtable.Link_data);
                        //                        thirdobj.details_fk = fourthResult;
                        //                        int descrnote = new Home().insertdises(thirdobj);
                        //                        DataTable notestbl7 = GetData7(fourthtable.Link_data);
                        //                        foreach (DataRow decrip in notestbl1.Rows)
                        //                        {
                        //                            // insert chapter notes
                        //                            Notes_Tbl notes5 = new Notes_Tbl()
                        //                            {
                        //                                Notes = detailsitem[1].ToString(),
                        //                                Table_FK = secondresult,
                        //                                Fk_type = 3
                        //                            };
                        //                            int notes1 = new Home().insertnotse(notes5);
                        //                        }
                        //                    }
                        //                }
                        //            }

                        //        };
                        //    }
                        MessageBox.Show("saved", "info", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        //}
                    }
                }
                else
                {
                    MessageBox.Show("Please load the web Page first!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                throw;
            }
        }
        private List<Section_Tbl> GetData(string Link, int Chabter1_fk)
        {

            Link = "https://icd.codes" + Link;

            //webBrowser1.Url = new Uri(Link);
            //webBrowser1.Navigate(Link);

            WebClient client = new WebClient();

            string content = client.DownloadString(Link);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);

            List<Section_Tbl> dt = new List<Section_Tbl>();
            //dt.Columns.Add("CodeRange_from");
            //dt.Columns.Add("CodeRange_To");
            //dt.Columns.Add("SectionDescription");
            //dt.Columns.Add("link_data");

            var n = doc.DocumentNode.SelectNodes(xpath: "//div[@id='content']/table");

            foreach (HtmlNode table in n)
            {
                foreach (HtmlNode row in table.SelectNodes("tr"))
                {
                    //DataRow r = dt.NewRow();
                    Section_Tbl section = new Section_Tbl();
                    int i = 0;
                    ////
                    HtmlNodeCollection cells = row.SelectNodes("th|td");

                    var anchorval = cells[1].SelectSingleNode("a");
                    if (cells == null)
                        continue;
                    //r[0] = cells[0].InnerText;
                    //r[1] = cells[1].InnerText;
                    section.Name = cells[1].InnerText;
                    section.link_data = anchorval == null ? "link" : anchorval.Attributes["href"].Value;
                    var codeRange = cells[0].InnerText;
                    string[] title = codeRange.Split('-');
                    if (title != null && title.Length > 1)
                    {
                        section.Code_from = title[0];
                        section.Code_to = title[1];
                    }
                    else if (title != null && title.Length == 1)
                    {
                        section.Code_from = title[0];
                        section.Code_to = title[0];
                    }
                    section.Chabter1_fk = Chabter1_fk;
                    //dt.Rows.Add(r);
                    dt.Add(section);
                    //DataRow r = dt.NewRow();
                    //int i = 0;

                    //HtmlNodeCollection cells = row.SelectNodes("th|td");
                    //var anchorval = cells[2].SelectSingleNode("a");
                    //if (cells == null)
                    //    continue;
                    //r[0] = cells[1].InnerText;
                    //r[1] = cells[2].InnerText;
                    //r[2] = anchorval == null ? "link" : anchorval.Attributes["href"].Value;

                    //dt.Rows.Add(r);

                }
            }
            dt.RemoveAt(0);
            return dt;
        
    }
        List<Diseases_destails_tbl> GetData2(string Link)
        {
            Link = "https://icd.codes" + Link;
            webBrowser1.Url = new Uri(Link);

            WebClient client = new WebClient();

            string content = client.DownloadString(Link);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);

            List<Diseases_destails_tbl> dt = new List<Diseases_destails_tbl>();
            //DataTable dt = new DataTable();
            //dt.Columns.Add("Code");
            //dt.Columns.Add("Name");
            //dt.Columns.Add("link_data");

            var n2 = doc.DocumentNode.SelectNodes(xpath: "//div[@id='content']/div[@class='panel-group']");

            foreach (HtmlNode table in n2)

            {
                foreach (HtmlNode row in table.SelectNodes("div[@class='panel panel-default']/div[@class='panel-heading collapse-next-tree']"))
                {
                    //DataRow r2 = dt.NewRow();
                    Diseases_destails_tbl destails = new Diseases_destails_tbl();
                    int i = 0;

                    HtmlNode cells = row.SelectSingleNode("a[@class='collapse-next collapsed']/h4/span");
                    string span = cells.InnerText;

                    HtmlNode codenode = row.SelectSingleNode("a[@class='collapse-next collapsed']/h4/b");
                    string code = codenode.InnerText;

                    HtmlNode coden = row.SelectSingleNode("ul[@class='list-group  collapse]/li/a");
                    string codes = coden.InnerText;
                    //r2[0] = span;
                    //r2[1] = code;
                    //r2[2] = codes;
                    //dt.Rows.Add(r2);
                    destails.Name = span[0].ToString();
                    destails.Code = code[1].ToString();
                    destails.Link_data = code[2].ToString();

                    //destails.Diseases_fk = Diseases_fk;
                }
            }
            dt.RemoveAt(0);
            return dt;
        }
        private dises_Description_tbl GetData3(string Link,int FK)
        {
            try
            {
                Link = "https://icd.codes" + Link;
                webBrowser1.Url = new Uri(Link);

                WebClient client = new WebClient();

                string content = client.DownloadString(Link);

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(content);

                var n2 = doc.DocumentNode.SelectNodes(xpath: "//div[@id='content']/div[@class='well']/div[@class='row']/div[@class='col-md-8']/p");
                dises_Description_tbl thirdobj = new dises_Description_tbl();
                if (n2 != null)
                {
                

                    //DataTable dt = new DataTable();
                    //dt.Columns.Add("Descraption");
                    //dt.Columns.Add("code_dec");
                    //dt.Columns.Add("Specialty");
                    //dt.Columns.Add("MeSH_Codes");
                    //dt.Columns.Add("ICD9Codes");
                    //dt.Columns.Add("img");

                    //HtmlNode Descraption = n2.FirstOrDefault().SelectSingleNode("//p");
                    HtmlNode Descraption = doc.DocumentNode.SelectNodes(xpath: "//div[@id='content']/div[@class='well']/div[@class='row']/div[@class='col-md-8']/p")[0];
                    HtmlNode code_dec = doc.DocumentNode.SelectNodes(xpath: "//div[@id='content']/div[@class='well']/h3")[0];
                    HtmlNode img = doc.DocumentNode.
                        SelectNodes(xpath: "//div[@id='content']/div[@class='well']/div[@class='row']/div[@class='col-md-4 text-right']/img")[0];

                    HtmlNodeCollection table = doc.DocumentNode.
                        SelectNodes(xpath: "//div[@id='content']/div[@class='well']/div[@class='row']/div[@class='col-md-8']/table/tr");
                    string Specialty = "", MeSH_Codes = "", ICD9Codes = "";

                    if (table != null)
                    {
                        HtmlNodeCollection tr1 = table[0].SelectNodes("//td");
                       Specialty = tr1 != null && tr1.Count >= 5 ? tr1[1].InnerText : "";
                        MeSH_Codes = tr1 != null && tr1.Count >= 5 ? tr1[3].InnerText : "";
                        ICD9Codes = tr1 != null && tr1.Count >= 5 ? tr1[5].InnerText : "";
                    }

                    thirdobj = new dises_Description_tbl()
                    {
                        Descraption = Descraption == null ? "" : Descraption.InnerText,
                        code_dec = code_dec == null ? "" : code_dec.InnerText,
                        Specialty = Specialty,
                        ICD9Codes = ICD9Codes,
                        MeSH_Codes = MeSH_Codes,
                        img = img.Attributes["src"] == null ? "" : img.Attributes["src"].Value.ToString(),
                        details_fk= FK
                    };
                }
                return thirdobj;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        private List<Diseases_Tbl> GetData4(string Link , int Section_Fk)
        {
            Link = "https://icd.codes" + Link;
            webBrowser1.Url = new Uri(Link);

            WebClient client = new WebClient();

            string content = client.DownloadString(Link);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);

            List<Diseases_Tbl> dt = new List<Diseases_Tbl>();
            //DataTable dt = new DataTable();
            //dt.Columns.Add("Code");
            //dt.Columns.Add("Name");
            //dt.Columns.Add("Html");
            var n = doc.DocumentNode.SelectNodes(xpath: "//div[@id='content']/div[@class='panel-group']/div[@class='panel panel-default']");
            //var n = doc.DocumentNode.SelectNodes(xpath: "//div[@id='content']/div[@class='panel-heading collapse-next-tree']/div[@class='collapse-next']/h4]");

            foreach (HtmlNode table in n)

            {
                Diseases_Tbl diseases = new Diseases_Tbl();
                //DataRow r = dt.NewRow();
                HtmlAgilityPack.HtmlDocument docu = new HtmlAgilityPack.HtmlDocument();
                docu.LoadHtml(table.InnerHtml);
                HtmlNodeCollection nodes = docu.DocumentNode.SelectNodes("//b[@class='text-warning']");

                HtmlNodeCollection muted = docu.DocumentNode.SelectNodes("//span[@class='text-muted']");
                HtmlNodeCollection success = docu.DocumentNode.SelectNodes("//b[@class='text-success']");
                HtmlNodeCollection Html_string = docu.DocumentNode.SelectNodes("//li[@class='list-group-item section-group']");
                var Name = muted == null ? "" : muted.FirstOrDefault().InnerText;
                var Code = nodes == null ? success == null ? "" : success.FirstOrDefault().InnerText : nodes.FirstOrDefault().InnerText;

                diseases.Code=Code;
                diseases.Name = Name;
                
                //r[0] = Code;
                //r[1] = Name;
                //r[2] = Html_string.FirstOrDefault().InnerHtml.ToString();
                //dt.Rows.Add(r);
                diseases.Section_Fk = Section_Fk;
                    List<Diseases_destails_tbl> destails = GetData5(Html_string.FirstOrDefault().InnerHtml.ToString());
                diseases.Diseases_destails_tbl = destails;
                dt.Add(diseases);
            }

            return dt;
        }
        private List<Diseases_destails_tbl> GetData5(string content)
        {

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);

            List<Diseases_destails_tbl> dt = new List<Diseases_destails_tbl>();
            //DataTable dt = new DataTable();
            //dt.Columns.Add("Name");
            //dt.Columns.Add("Code");
            //dt.Columns.Add("link_data");


            var n = doc.DocumentNode.SelectNodes(xpath: "//ul/li[@class='list-group-item']");
            if (n != null)
            {

                foreach (HtmlNode table in n)
                {
                    Diseases_destails_tbl destail = new Diseases_destails_tbl();
                    //DataRow r = dt.NewRow();
                    HtmlAgilityPack.HtmlDocument docu = new HtmlAgilityPack.HtmlDocument();
                    docu.LoadHtml(table.InnerHtml);

                    HtmlNodeCollection CodeName = docu.DocumentNode.SelectNodes("//a");
                    HtmlNodeCollection LabelName = docu.DocumentNode.SelectNodes("//span");
                    var code = CodeName == null ? "" : CodeName.FirstOrDefault().InnerText;
                  var Name = LabelName == null ? "" : LabelName.FirstOrDefault().InnerText;
                   var Link_data = CodeName == null ? "" : CodeName == null ? "" : CodeName.FirstOrDefault().Attributes["href"].Value;
                    //destail.Diseases_fk = Diseases_fk;
                    destail.Code = code;
                    destail.Name = Name;
                    destail.Link_data = Link_data;

                    dt.Add(destail);

                }
            }
            return dt;
        }

        private List<Notes_Tbl> GetData6(string Link , int Table_FK, int Fk_type)
        {

            Link = "https://icd.codes" + Link;

            //webBrowser1.Url = new Uri(Link);
            //webBrowser1.Navigate(Link);

            WebClient client = new WebClient();

            string content = client.DownloadString(Link);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);

            List<Notes_Tbl> dt = new List<Notes_Tbl>();
            //DataTable dt = new DataTable();
            //dt.Columns.Add("Notes");
            //dt.Columns.Add("Tablefk");

            var n = doc.DocumentNode.SelectNodes(xpath: "//*[@id='content']/dl");


            for (int i = 0; i < n.Count; i++)
            {
                Notes_Tbl notes = new Notes_Tbl();
                //DataRow r = dt.NewRow();
                HtmlNodeCollection dd = n[i].SelectNodes("dd");
                HtmlNode dtt = n[i].SelectSingleNode("dt");

                //notes.Notes = dd.InnerText;
                notes.Notes = dtt.InnerText;
                foreach (var item in dd)
                {
                    notes.Notes = notes.Notes + item.InnerText;
                }
                //r[0] = dd.InnerText;
                //r[1] = dtt.InnerText;
               notes.Table_FK = Table_FK;
                notes.Fk_type = Fk_type;
                 dt.Add(notes);
            }

            //foreach (HtmlNode item in doc.DocumentNode.SelectNodes("//*[@id='content']/dl"))
            //{
            //    DataRow r = dt.NewRow();

            //    HtmlNode dd = item.SelectSingleNode("//dd");
            //    HtmlNode dtt = item.SelectSingleNode("//dt");

            //    r[0] = dd.InnerText;
            //    r[1] = dtt.InnerText;


            //    dt.Rows.Add(r);
            //}
            return dt;
        }
        private List<Notes_Tbl> GetData7(string Link, int Table_FK, int Fk_type)
        {

            Link = "https://icd.codes" + Link;

            //webBrowser1.Url = new Uri(Link);
            //webBrowser1.Navigate(Link);

            WebClient client = new WebClient();

            string content = client.DownloadString(Link);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);

            List<Notes_Tbl> dt = new List<Notes_Tbl>();
            //DataTable dt = new DataTable();
            //dt.Columns.Add("Notes");
            //dt.Columns.Add("Tablefk");

            var n = doc.DocumentNode.SelectNodes(xpath: "//*[@id='content']/div[@class='outline-box']");


            for (int i = 0; i < n.Count; i++)
            {
                HtmlNodeCollection dlList = n[i].SelectNodes("dl");
                if (dlList!=null)
                {
                    for (int z = 0; z < dlList.Count; z++)
                    {
                        Notes_Tbl notes1 = new Notes_Tbl();
                        ///DataRow r = dt.NewRow();
                        HtmlNode dd = n[i].SelectNodes("dl")[z].SelectSingleNode("dd");
                        HtmlNode dtt = n[i].SelectNodes("dl")[z].SelectSingleNode("dt");
                        notes1.Notes = dd.InnerText;
                        notes1.Notes = dtt.InnerText;
                        //r[0] = dd.InnerText;
                        //r[1] = dtt.InnerText;
                        notes1.Table_FK = Table_FK;
                        notes1.Fk_type = Fk_type;
                        dt.Add(notes1);
                    }
                }
                else
                {
                    HtmlNodeCollection dlList1 = n[i].SelectNodes("ul");
                    if (dlList1 != null)
                    {
                        for (int z = 0; z < dlList1.Count; z++)
                        {
                            Notes_Tbl notes1 = new Notes_Tbl();
                            // DataRow r = dt.NewRow();
                            HtmlNode dd = n[i].SelectNodes("ul")[z].SelectSingleNode("li");
                          
                           notes1.Notes = dd.InnerText;
                      
                            dt.Add(notes1);
                        }
                    }
                }
            }

            //foreach (HtmlNode item in doc.DocumentNode.SelectNodes("//*[@id='content']/dl"))
            //{
            //    DataRow r = dt.NewRow();

            //    HtmlNode dd = item.SelectSingleNode("//dd");
            //    HtmlNode dtt = item.SelectSingleNode("//dt");

            //    r[0] = dd.InnerText;
            //    r[1] = dtt.InnerText;


            //    dt.Rows.Add(r);
            //}
            return dt;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "https://icd.codes/icd10cm";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            webBrowser1.Refresh();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            webBrowser1.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        //private void button1_Click_1(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(@"Data Source =DESKTOP-A73VQTE\MOHAMEDSQL; Initial Catalog =TestDB; Integrated Security=True"))
        //        {
        //            con.Open();
        //            var List = GetData2();
        //            for (int i = 0; i < List.Rows.Count; i++)
        //            {
        //                SqlCommand cmd = new SqlCommand("insert into Section_Tbl values (@Name ,@Code ,@link_data )", con);
        //                SqlParameter unitsParam = cmd.Parameters.AddWithValue("@Name", List.Rows[i][0]);
        //                SqlParameter unitsParam1 = cmd.Parameters.AddWithValue("@Code", List.Rows[i][1]);
        //                SqlParameter unitsParam2 = cmd.Parameters.AddWithValue("@link_data", List.Rows[i][2]);

        //                if (con.State == ConnectionState.Closed)

        //                {

        //                    con.Open();

        //                }
        //                if (List.Rows[i][0] == null || List.Rows[i][1] == null)
        //                {
        //                    unitsParam.Value = DBNull.Value;
        //                    unitsParam1.Value = DBNull.Value;
        //                    unitsParam1.Value = DBNull.Value;
        //                }
        //                cmd.ExecuteNonQuery();

        //            }
        //            MessageBox.Show("saved", "info", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);


        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}
    }

}
// ------------   Chapters    ------------
//private void Button1_Click(object sender, EventArgs e)
//{

//    if (webBrowser1.Document != null)
//    {
//        HtmlElementCollection elems = webBrowser1.Document.GetElementsByTagName("tbody");
//        foreach (HtmlElement elem in elems)
//        {
//            HtmlElementCollection nameStr = elem.GetElementsByTagName("tr");
//            if (nameStr != null && nameStr.Count != 0)
//            {
//                string code = "", desc = "";
//                foreach (HtmlElement item in nameStr)
//                {
//                    HtmlElementCollection dd = item.GetElementsByTagName("td");
//                    HtmlElement urlElement = dd.Cast<HtmlElement>().FirstOrDefault(q => q.FirstChild != null);
//                    var url = "";
//                    if (urlElement != null)
//                    {
//                        url = urlElement.GetElementsByTagName("a").Cast<HtmlElement>().FirstOrDefault().GetAttribute("href");
//                    }
//                    item.GetAttribute("attribute name");
//                    code = dd == null || dd.Count == 0 ? "" : dd[0].InnerText;

//                    var codeRange = code;
//                    string[] title = codeRange.Split('-');
//                    string from = "",
//                           to = "";
//                    if (title.Length > 1)
//                    {
//                        from = title[0];
//                        to = title[1];
//                    }
//                    desc = dd == null || dd.Count == 0 ? "" : dd[1].InnerText;