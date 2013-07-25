using System;
using System.Linq;
using System.CodeDom.Compiler;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using Microsoft.VisualStudio.TextTemplating;
using T4Business;
using System.Text;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Smo;

namespace T4CustomHostWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker _backgroundWorker;

        public MainWindow()
        {
            InitializeComponent();
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
        }

        void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTemplates();
            LoadTables();
        }

        private void LoadTemplates()
        {
            foreach (var file in Directory.GetFiles("..\\..\\Templates", "*.tt"))
            {
                TemplateList.Items.Add(file);
            }
        }

        private void LoadTables()
        {

            Server dbServer = new Server();
            var db = dbServer.Databases["AdventureWorks2012"];
            var tables = db.Tables;

            foreach(Table table in tables)
            {
                TableList.Items.Add(table);
                //TableList.Items.Add(  table.Schema + "." + table.Name,);
            }

            TableList.Items.SortDescriptions.Add(new SortDescription("Schema", ListSortDirection.Ascending));
            TableList.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));



            //var connectionString =
            //                @"Data Source=.;Initial Catalog=AdventureWorks2012;Integrated Security=True";

            //var commandText = "select table_name as TableName from  INFORMATION_SCHEMA.Tables";

            //using (var connection = new SqlConnection(connectionString))
            //{
            //    connection.Open();
            //    using (var command = new SqlCommand(commandText, connection))
            //    using (var reader = command.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            TableList.Items.Add(reader["TableName"] as string);
            //        }
            //    }
            //}
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var template = TemplateList.SelectedValue as string;
                var tables = TableList.SelectedItems;

                if (string.IsNullOrWhiteSpace(template)) throw new ApplicationException("Please select a template.");
                if (tables.Count == 0) throw new ApplicationException("Please select at least one table.");

                GenerateOutput(template, tables);
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        private void GenerateOutput(string templateFileName, System.Collections.IList tables)
        {
            var host = new T4Host();
            var session = new TextTemplatingSession();
            var sessionHost = (ITextTemplatingSessionHost)host;
            sessionHost.Session = session;

            foreach (Table table in tables)
            {
                LogUpdate("Processing table {0}", table.Name);
                sessionHost.Session["table"] = new SqlTable { Name = table.Name, Schema = table.Schema };
                sessionHost.Session["tableName"] = table.Name;
                sessionHost.Session["namespace"] = NamespaceParameter.Text;
                //// Pass another value in CallContext:
                System.Runtime.Remoting.Messaging.CallContext.LogicalSetData("p2", "test");

                Engine engine = new Engine();
                host.TemplateFile = templateFileName;
                //Read the text template.
                string input = File.ReadAllText(templateFileName);
                //Transform the text template.
                string output = engine.ProcessTemplate(input, host);
                string outputFileName = table.Name;
                outputFileName = Path.Combine(Path.GetDirectoryName(templateFileName) ?? "", outputFileName);
                outputFileName = outputFileName + "1" + host.FileExtension;
                File.WriteAllText(outputFileName, output, host.FileEncoding);
                LogUpdate("Saved to file {0}", outputFileName);
                StringBuilder sb = new StringBuilder(); 
                foreach (CompilerError error in host.Errors)
                {
                    sb.AppendLine(error.ToString());
                }
                if (sb.Length > 0) LogUpdate( "Errors: " + sb.ToString());
            }

            LogUpdate("Done.");
        }

        //private void LogUpdate(string results)
        //{
        //    Results.Text += results;
        //}
        private void LogUpdate(string formatString, params object[] parameters)
        {
            var output = string.Format(formatString, parameters);
            Results.Text += output + Environment.NewLine;
        }

 
    }
}
