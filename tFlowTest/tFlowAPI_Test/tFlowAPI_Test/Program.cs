using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Collections;
using tFlow;

namespace tFlowAPI_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("!!!!!!!!!! --- RAPS Version: 14 --- !!!!!!!!!!");
            Console.WriteLine();


            tFlow.FileMakerData fmd = new tFlow.FileMakerData();
            string Path = @"\\MCRAESERVER-PC\McRae Files\tFlow\XML\" + args[0];

            tFlowAPICalls.ProcessFileMakerXML(Path);

            //*********************************************TESTING********************************************************
            //************************************************************************************************************
            //string Path = @"\\MCRAESERVER-PC\McRae Files\tFlow\XML\77777.xml"; //48077 PO TEST
            //string Path = @"\\MCRAESERVER-PC\McRae Files\tFlow\XML\47671.xml"; //47671 MCRAE TEST
            //tFlowAPICalls.ProcessFileMakerXML(Path);

            /*Console.WriteLine("Hit Enter Start Test");
            Console.ReadLine();*/
            //ShowRestResponseFormatted();
            //ListAllUsers();
            //Console.WriteLine(tFlowAPICalls.GetCustomerID("McRae Imaging"));*/
            //************************************************************************************************************
            //*******************************************END TESTING******************************************************

            Console.ReadLine();            
        }

        #region Testing
        private static void ShowRestResponseFormatted()
        {
            //List<dynamic> Props = tFlowAPICalls.GetPropsFields("Job");
            List<dynamic> Props = tFlowAPICalls.GetClientList();
            foreach (dynamic Prop in Props)
            {
                Console.WriteLine(Prop.name + " - " + Prop.id);
            }
        }
        private static void ListAllUsers()
        {
            List<dynamic> Props = tFlowAPICalls.GetUserList();
            foreach (dynamic Prop in Props)
            {
                Console.WriteLine("Name:" + Prop.name);
                Console.WriteLine("Id:" + Prop.id);
                Console.WriteLine("E-Mail:" + Prop.email);
                Console.WriteLine();
            }
        }
        #endregion

        #region DO NOT USE
        private void DO_NOT_USE_ORDERINFO()
        {
            tFlow.FileMakerData fmd = new tFlow.FileMakerData();
            string Path = @"\\MCRAESERVER-PC\McRae Files\tFlow\XML\43828.xml";

            fmd.Load(Path);

            Console.WriteLine("ORDER:");
            Console.WriteLine("\tClient Name: " + fmd.ClientName);
            Console.WriteLine("\tOrder Name: " + fmd.OrderName);
            Console.WriteLine("\tDescription: " + fmd.OrderDescription);
            Console.WriteLine("\tShip Date: " + fmd.ShipDate.ToString("yyyy-MM-ddT00:00:00-hh:mm"));
            Console.WriteLine("\tStart Date: " + fmd.StartDate.ToString("yyyy-MM-ddT00:00:00-hh:mm"));

            Console.WriteLine("Jobs:");
            foreach (tFlow.FileMakerJobInfo fmJob in fmd.Jobs)
            {
                Console.WriteLine("\tClient Name: " + fmJob.ClientName);
                Console.WriteLine("\tHeight: " + fmJob.Height);
                Console.WriteLine("\tDescription: " + fmJob.JobDescription);
                Console.WriteLine("\tJob Name: " + fmJob.JobName);
                Console.WriteLine("\tProduct Name: " + fmJob.ProductName);
                Console.WriteLine("\tQty: " + fmJob.Qty);
                Console.WriteLine("\tUser E-Mail: " + fmJob.UserEmail);
                Console.WriteLine("\tUsername: " + fmJob.UserName);
                Console.WriteLine("\tWidth: " + fmJob.Width);
                Console.WriteLine("\tShip Date: " + fmJob.ShipDate.ToString("yyyy-MM-ddT00:00:00-hh:mm"));
                Console.WriteLine();
            }

            Console.ReadLine();
        }
        private void DONT_USE_OldRequestTest()
        {
            string URL = @"http://209.222.20.203:9004/oauth/access_token";
            string DATA = @"?grant_type:'Client_credentials'&client_id:sDaOPg9QOigt1JQL&client_secret:4U2Zn7Z9TjzI6JgZTOImlRSUHjk0QLDS";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = DATA.Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            requestWriter.Write(DATA);
            requestWriter.Close();

            try
            {
                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                Console.Out.WriteLine(response);
                responseReader.Close();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
        private void DO_NOT_USE_GetInformation()
        {
            List<dynamic> jobsList = tFlow.tFlowAPICalls.GetJobList();
            List<dynamic> clientList = tFlowAPICalls.GetClientList();

            //Console.WriteLine(jobsList[0].name + "\nCustomer ID: " + iID);
            Console.WriteLine();
            foreach (dynamic curClient in clientList)
            {
                Console.WriteLine("Clinet: " + curClient.name);
            }
        }
#endregion
    }
}

   
