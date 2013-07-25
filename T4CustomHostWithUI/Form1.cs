using Microsoft.VisualStudio.TextTemplating;
using System;
using System.CodeDom.Compiler;
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
using T4Sandbox;

namespace T4CustomHostWithUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void GoButton_Click(object sender, EventArgs e)
        {

            CustomCmdLineHost host = new CustomCmdLineHost();
            var session = new TextTemplatingSession();

            string templateFileName = TemplateList.SelectedItem.ToString();

            var sessionHost = (ITextTemplatingSessionHost)host;
            sessionHost.Session = session;

            foreach (string tableName in TableList.CheckedItems)
            {
                sessionHost.Session["tableName"] = tableName;
                //// Pass another value in CallContext:
                System.Runtime.Remoting.Messaging.CallContext.LogicalSetData("p2", "test");

                Engine engine = new Engine();
                host.TemplateFileValue = templateFileName;
                //Read the text template.
                string input = File.ReadAllText(templateFileName);
                //Transform the text template.
                string output = engine.ProcessTemplate(input, host);
                string outputFileName = tableName;
                outputFileName = Path.Combine(Path.GetDirectoryName(templateFileName), outputFileName);
                outputFileName = outputFileName + "1" + host.FileExtension;
                File.WriteAllText(outputFileName, output, host.FileEncoding);

                foreach (CompilerError error in host.Errors)
                {
                    Console.WriteLine(error.ToString());
                }
            }

            Console.ReadLine();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadTemplates();
            LoadTables();
        }

        private void LoadTemplates()
        {
            foreach (var file in Directory.GetFiles("..\\..\\Templates"))
            {
                TemplateList.Items.Add(file);
            }
        }

        private void LoadTables()
        {

            var connectionString =
                            @"Data Source=.;Initial Catalog=AdventureWorks2012;Integrated Security=True";

            var commandText = "select table_name as TableName from  INFORMATION_SCHEMA.Tables";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(commandText, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TableList.Items.Add(reader["TableName"] as string);
                    }
                }
            }
        }
    }
}
   
