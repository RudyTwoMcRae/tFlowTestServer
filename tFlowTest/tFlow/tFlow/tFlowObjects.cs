using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Deserializers;
using System.Collections;
using System.Xml;

namespace tFlow
{
#region tFlow Objects
    public class Authorization
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
    public class DynamicJsonDeserializer : IDeserializer
    {
        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }

        public T Deserialize<T>(RestResponse response) where T : new()
        {
            return JsonConvert.DeserializeObject<dynamic>(response.Content);
        }

        public T Deserialize<T>(IRestResponse response)
        {
            throw new NotImplementedException();
        }
    }
    //Order Objects
    public class OrderObject
    {
        public int client_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string ship_date { get; set; }
        public int product_id { get; set; }
        public string external_id { get; set; }
        public string source { get; set; }
        public assignments assignments { get; set; }
        public OrderProps props { get; set; }
    }
    public class assignments
    {
        public List<int> instance_user_ids { get; set; }
        public List<int> non_instance_user_ids { get; set; }
    }
    public class OrderProps
    {
        public string rep_name { get; set; }
        public string pm_name { get; set; }
    }
    
    //Job Objects
    public class JobObject
    {
        public string name { get; set; }
        public string description { get; set; }
        public string notes { get; set; }
        public int order_id { get; set; }
        public string ship_date { get; set; }
        public string start_date { get; set; }
        public int product_id { get; set; }
        public bool attach_proofs { get; set; }
        public string external_id { get; set; }
        public string source { get; set; }
        public assignments assignments { get; set; }
        //public List<string> tflows { get; set; }
        //public int finishing_profile_id { get; set; }
        //public bool inherit_finishing_profile { get; set; }
        public int proof_profile_id { get; set; }
        //public int production_profile_id { get; set; }
        public JobProps props { get; set; }
        public string id { get; internal set; }
    }
    public class UpdateJobObject
    {
        public string name { get; set; }
        public string description { get; set; }
        public string notes { get; set; }
        public int order_id { get; set; }
        public string ship_date { get; set; }
        public string start_date { get; set; }
        public int product_id { get; set; }
        public bool attach_proofs { get; set; }
        public string external_id { get; set; }
        public string source { get; set; }
        public assignments assignments { get; set; }
        //public int finishing_profile_id { get; set; }
        //public bool inherit_finishing_profile { get; set; }
        public int proof_profile_id { get; set; }
        //public int production_profile_id { get; set; }
        public JobProps props { get; set; }
        public string id { get; internal set; }
    }
    public class JobDeleteObject
    {
        public int id { get; set; }
    }
    public class JobProps
    {
        public float print_width { get; set; }
        public float print_height { get; set; }
        public float art_width { get; set; }
        public float art_height { get; set; }
        public string art_size { get; set; }
        public int min_ppi { get; set; }
        public int sales_rep { get; set; }
        public int quantity { get; set; }
        public string shipping_address { get; set; }
        public string phone_contact { get; set; }
        public string email_contact { get; set; }
        public string mis_job_number { get; set; }
        public string mis_contact { get; set; }
        public string rep_name { get; set; }
        public string pm_name { get; set; } //Product Material Name (PolyFlex, Lux, PolyPro, etc.)
        public string scale_factor { get; set; }  //Number to scale eg 10 = 10%
    }
    public class UserInfo
    {
        public string email { get; set; }
        public int client_id { get; set; }
        public string name { get; set; }
        public bool enabled { get; set; }
        public string locale { get; set; }
        public string password { get; set; }
        public List<string> alertTypes { get; set; }
    }
    public static class Globals
    {
        public static string xmlPath;
        public static string orderExist;
        public static string checkJOBNUMBER;
        public static List<string> curjobNameXML = new List<string>();
        public static List<string> curjobNameTFLOW = new List<string>();
        public static List<string> curUserEmail = new List<string>();

        public static List<int> iarrInstanceUserIDs = new List<int>();
        public static List<int> iarrNonInstanceUserIds = new List<int>();

        public static bool doubleSided;
        public static bool SideAOnly = false;
        public static bool SideBOnly = false;
    }
    #endregion



    public class tFlowAPICalls
    {
        #region Get Functions
        public static int GetCustomerID(string sName)
        {
            //Variables
            int iID = -1;

            foreach (dynamic ocurCustomer in GetClientList())
            {
                if (ocurCustomer.name == sName)
                { iID = ocurCustomer.id; }
            }

            return iID;
        }
        private static int GetProductID(string sName)
        {
            //Variables
            int iID = -1;

            foreach (dynamic ocurProduct in GetProductList())
            {
                if (ocurProduct.name == sName)
                { iID = ocurProduct.id; }
            }

            return iID;
        }
        private static int GetUserID(string sEMail)
        {
            //Variables
            int iID = -1;

            foreach (dynamic ocurUser in GetUserList())
            {
                if (ocurUser.name == sEMail)
                { iID = ocurUser.id; }
            }

            return iID;
        }
        private static int GetOrderID(string sName) 
        {
            //Variables
            int iID = -1;

            foreach (dynamic ocurOrder in GetOrdersList())
            {
                if (ocurOrder.name == sName)
                { iID = ocurOrder.id; }
            }

            return iID;
        }
        private static int GetJobID(int sID, string sJob)
        {
            //Variables
            int iID = -2;

            foreach (dynamic ocurJob in GetJobList())
            {
                if (ocurJob.order_id == sID && ocurJob.name == sJob)
                { iID = ocurJob.id; }
            }

            return iID;
        }
        private static void GetTFLOWJobName(int sID)
        {
            //Variables
            string iID;

            foreach (dynamic ocurJob in GetJobList())
            {
                if (ocurJob.order_id == sID)
                {
                    iID = ocurJob.name;
                    Globals.curjobNameTFLOW.Add(iID);
                }
            }

        }
        private static int GetProfileID(string sName)
        {
            //Variables
            int iID = -1;
            
            foreach (dynamic ocurProfile in GetProfilesList())
            {
                if (ocurProfile.name == sName)
                { iID = ocurProfile.id; }
            }

            //Anothey way to grab proof_profile_id
            /*
            foreach (dynamic ocurProfile in GetProductList())
            {
                if (ocurProfile.name == sName)
                { iID = ocurProfile.proof_profile_id; }
            }
            */
            return iID;
        }
        private static string GetProofProfile(string sProduct)
        {
            //Variables
            string sProofName = null;
            XmlDocument doc = new XmlDocument();
            doc.Load(@"\\MCRAESERVER-PC\McRae Files\tFlow\CONFIG\Product-ProofScripts.xml");
            XmlNode xmlProof;

            //Return an error if the XML file is corrupt
            try
            {
                //Find the Product and get the matching Proof Script
                xmlProof = doc.SelectSingleNode("//Proof[@product='" + sProduct + "']");
                sProofName = xmlProof.InnerText;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

                return sProofName;
        }
        private static float[] GetPrintDimensions(string sProduct)
        {
            //Variables
            float[] farrDims = new float[2];
            XmlDocument doc = new XmlDocument();
            doc.Load(@"\\MCRAESERVER-PC\McRae Files\tFlow\CONFIG\Bleed.xml");
            XmlNode xmlLBleed;
            XmlNode xmlRBleed;
            XmlNode xmlTBleed;
            XmlNode xmlBBleed;
            float iWidth = -1;
            float iHeight = -1;

            //Return an error if the XML file is corrupt
            try
            {
                //Find the Product and get the Bleeds
                xmlLBleed = doc.SelectSingleNode("//Product[@name='" + sProduct + "']/LBleed");
                xmlRBleed = doc.SelectSingleNode("//Product[@name='" + sProduct + "']/RBleed");
                xmlTBleed = doc.SelectSingleNode("//Product[@name='" + sProduct + "']/TBleed");
                xmlBBleed = doc.SelectSingleNode("//Product[@name='" + sProduct + "']/BBleed");

                //Get the width and height
                iWidth = float.Parse(xmlLBleed.InnerText) + float.Parse(xmlRBleed.InnerText);
                iHeight = float.Parse(xmlTBleed.InnerText) + float.Parse(xmlBBleed.InnerText);

                //Set the width and height in the Dims Array
                farrDims[0] = iWidth;
                farrDims[1] = iHeight;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

            return farrDims;
        }
        public static string[] Authorize()
        {
            //FOR SSL
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

            string URL = @"https://raps.mcraeimaging.com/oauth/access_token";
            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest(Method.POST);
            string[] sarrResult = { "Bearer", "" };

            //Add Parameters to Request
            request.AddParameter("grant_type", "client_credentials");
            
            //RUDY
            //request.AddParameter("client_id", "sDaOPg9QOigt1JQL");
            //request.AddParameter("client_secret", "4U2Zn7Z9TjzI6JgZTOImlRSUHjk0QLDS");

            //LIN
            //request.AddParameter("client_id", "RyQOWqBwLnQgSV4x");
            //request.AddParameter("client_secret", "8Om0dS813GK0JQPCBKAqrhuOYwwUxwWm");

            //TFLOW UPLOAD
            //request.AddParameter("client_id", "ip7h2OB0X4mMVFEs");
            //request.AddParameter("client_secret", "VKckhmi7YhHa0BGBjdceW2ffMdUMXT1P");
            
            //RAPS
            request.AddParameter("client_id", "B0aB2Y8h2cS1qC7Q");
            request.AddParameter("client_secret", "0yOktmhlez5NcRJ0QYi4pvNnn9JS2non");                  

            //Execute Request and handle response
            IRestResponse<tFlow.Authorization> response = client.Execute<tFlow.Authorization>(request);
            sarrResult[0] = response.Data.token_type;
            sarrResult[1] = response.Data.access_token;
            

            return sarrResult;
        }
        public static List<dynamic> GetProfilesList()
        {
            string sURL = @"https://raps.mcraeimaging.com/api/v2/profile/list";
            RestClient rcClient = new RestClient(sURL);
            RestRequest rrRequest = new RestRequest(Method.GET);
            string[] sarrCredentials = Authorize();
            JArray jaProfile;
            List<dynamic> ldAllProfiles = new List<dynamic>();

            //Add Header and Parameters
            rcClient.AddHandler("application.json", new DynamicJsonDeserializer());
            rrRequest.AddHeader("Authorization", sarrCredentials[0] + " " + sarrCredentials[1]);

            //Execute Request
            try
            {
                IRestResponse<dynamic> rrResponse = rcClient.Execute<dynamic>(rrRequest);
                dynamic allProfiles = rrResponse.Data;

                //Populate a List with all the Profiles
                jaProfile = JArray.Parse(allProfiles.ToString());
                foreach (JObject thisJob in jaProfile)
                {
                    ldAllProfiles.Add(thisJob);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Source + "\n\n" + ex.Message);
                Console.ReadLine();
            }

            return ldAllProfiles;
        }
        public static List<dynamic> GetOrdersList()
        {
            string sURL = @"https://raps.mcraeimaging.com/api/v2/order/list";
            RestClient rcClient = new RestClient(sURL);
            RestRequest rrRequest = new RestRequest(Method.GET);
            string[] sarrCredentials = Authorize();
            JArray jaOrders;
            List<dynamic> ldAllOrders = new List<dynamic>();

            //Add Header and Parameters
            rcClient.AddHandler("application.json", new DynamicJsonDeserializer());
            rrRequest.AddHeader("Authorization", sarrCredentials[0] + " " + sarrCredentials[1]);

            //Execute Request
            try
            {
                IRestResponse<dynamic> rrResponse = rcClient.Execute<dynamic>(rrRequest);
                dynamic allJobs = rrResponse.Data;

                //Populate a List with all the Orders
                jaOrders = JArray.Parse(allJobs.ToString());
                foreach (JObject thisJob in jaOrders)
                {
                    ldAllOrders.Add(thisJob);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

            return ldAllOrders;
        }
        public static List<dynamic> GetJobList()
        {
            string sURL = @"https://raps.mcraeimaging.com/api/v2/job/list";
            RestClient rcClient = new RestClient(sURL);
            RestRequest rrRequest = new RestRequest(Method.GET);
            string[] sarrCredentials = Authorize();
            JArray jaJobs;
            List<dynamic> ldAllJobs = new List<dynamic>();

            //Add Header and Parameters
            rcClient.AddHandler("application.json", new DynamicJsonDeserializer());
            rrRequest.AddHeader("Authorization", sarrCredentials[0] + " " + sarrCredentials[1]);

            //Execute Request
            try
            {
                IRestResponse<dynamic> rrResponse = rcClient.Execute<dynamic>(rrRequest);
                dynamic allJobs = rrResponse.Data;
                
                //Populate a List with all the Jobs
                jaJobs = JArray.Parse(allJobs.ToString());
                foreach (JObject thisJob in jaJobs)
                {
                    ldAllJobs.Add(thisJob);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

            return ldAllJobs;
        }
        public static List<dynamic> GetUserList()
        {
            string sURL = @"https://raps.mcraeimaging.com/api/v2/user/list";
            RestClient rcClient = new RestClient(sURL);
            RestRequest rrRequest = new RestRequest(Method.GET);
            string[] sarrCredentials = Authorize();
            JArray jaUsers;
            List<dynamic> ldAllUsers = new List<dynamic>();

            //Add Header and Parameters
            rcClient.AddHandler("application.json", new DynamicJsonDeserializer());
            rrRequest.AddHeader("Authorization", sarrCredentials[0] + " " + sarrCredentials[1]);

            //Execute Request
            try
            {
                IRestResponse<dynamic> rrResponse = rcClient.Execute<dynamic>(rrRequest);
                dynamic allJobs = rrResponse.Data;

                //Populate a List with all the Users
                jaUsers = JArray.Parse(allJobs.ToString());
                foreach (JObject thisUser in jaUsers)
                {
                    ldAllUsers.Add(thisUser);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
            //Console.WriteLine(String.Join("\n", ldAllUsers));

            return ldAllUsers;
        }
        public static List<dynamic> GetClientList()
        {
            string sURL = @"https://raps.mcraeimaging.com/api/v2/client/list";
            RestClient rcClient = new RestClient(sURL);
            RestRequest rrRequest = new RestRequest(Method.GET);
            string[] sarrCredentials = Authorize();
            JArray jaUsers;
            List<dynamic> ldAllUsers = new List<dynamic>();

            //Add Header and Parameters
            rcClient.AddHandler("application.json", new DynamicJsonDeserializer());
            rrRequest.AddHeader("Authorization", sarrCredentials[0] + " " + sarrCredentials[1]);

            //Execute Request
            try
            {
                IRestResponse<dynamic> rrResponse = rcClient.Execute<dynamic>(rrRequest);
                dynamic allJobs = rrResponse.Data;

                //Populate a List with all the Clients
                jaUsers = JArray.Parse(allJobs.ToString());
                foreach (JObject thisUser in jaUsers)
                {
                    ldAllUsers.Add(thisUser);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
            //Console.WriteLine(String.Join("\n", ldAllUsers));

            return ldAllUsers;
        }
        public static List<dynamic> GetProductList()
        {
            string sURL = @"https://raps.mcraeimaging.com/api/v2/product/list";
            RestClient rcClient = new RestClient(sURL);
            RestRequest rrRequest = new RestRequest(Method.GET);
            string[] sarrCredentials = Authorize();
            JArray jaProducts;
            List<dynamic> ldAllProducts = new List<dynamic>();

            //Add Header and Parameters
            rcClient.AddHandler("application.json", new DynamicJsonDeserializer());
            rrRequest.AddHeader("Authorization", sarrCredentials[0] + " " + sarrCredentials[1]);

            //Execute Request
            try
            {
                IRestResponse<dynamic> rrResponse = rcClient.Execute<dynamic>(rrRequest);
                dynamic allJobs = rrResponse.Data;

                //Populate a List with all the Products
                jaProducts = JArray.Parse(allJobs.ToString());
                foreach (JObject thisUser in jaProducts)
                {
                    ldAllProducts.Add(thisUser);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

            return ldAllProducts;
        }


        /// <summary>
        /// Get a list of all the available Props for either a Order or Job
        /// </summary>
        /// <param name="EntityType">Can be either 'Job' or 'Order'</param>
        /// <returns></returns>
        public static List<dynamic> GetPropsFields(string EntityType)
        {
            string sURL = @"https://raps.mcraeimaging.com/api/v2/prop/" + EntityType;
            RestClient rcClient = new RestClient(sURL);
            RestRequest rrRequest = new RestRequest(Method.GET);
            string[] sarrCredentials = Authorize();
            JArray jaProps;
            List<dynamic> ldAllProps = new List<dynamic>();

            //Add Header and Parameters
            rcClient.AddHandler("application.json", new DynamicJsonDeserializer());
            rrRequest.AddHeader("Authorization", sarrCredentials[0] + " " + sarrCredentials[1]);

            //Execute Request
            try
            {
                IRestResponse<dynamic> rrResponse = rcClient.Execute<dynamic>(rrRequest);
                dynamic allJobs = rrResponse.Data;

                //Populate a List with all the Products
                jaProps = JArray.Parse(allJobs.ToString());
                foreach (JObject thisProp in jaProps)
                {
                    ldAllProps.Add(thisProp);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

            return ldAllProps;
        }
        public static List<dynamic> GetAlertList()
        {
            string sURL = @"https://raps.mcraeimaging.com/api/v2/alertType/list";
            RestClient rcClient = new RestClient(sURL);
            RestRequest rrRequest = new RestRequest(Method.GET);
            string[] sarrCredentials = Authorize();
            JArray jaProducts;
            List<dynamic> ldAllALerts = new List<dynamic>();

            //Add Header and Parameters
            rcClient.AddHandler("application.json", new DynamicJsonDeserializer());
            rrRequest.AddHeader("Authorization", sarrCredentials[0] + " " + sarrCredentials[1]);

            //Execute Request
            try
            {
                IRestResponse<dynamic> rrResponse = rcClient.Execute<dynamic>(rrRequest);
                dynamic allJobs = rrResponse.Data;

                //Populate a List with all the Products
                jaProducts = JArray.Parse(allJobs.ToString());
                foreach (JObject thisUser in jaProducts)
                {
                    ldAllALerts.Add(thisUser);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

            return ldAllALerts;
        }

        #endregion


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region API Calls

        public static int CreateOrder(string Path)
        {
            //Console.WriteLine("FROM CREATE ORDER");
            string sURL = @"https://raps.mcraeimaging.com/api/v2/order/create";
            RestClient rcClient = new RestClient(sURL);
            RestRequest rrRequest = new RestRequest(Method.POST);
            string[] sarrCredentials = Authorize();

            Globals.checkJOBNUMBER = "YES";
            int iOrderID = -1;
                       
            FileMakerData fmdOrder = new FileMakerData();
            OrderObject oOrderObj = new OrderObject();
            assignments oAssignments = new assignments();
            OrderProps orderProps = new OrderProps();
            fmdOrder.Load(Path);

            oAssignments.instance_user_ids = Globals.iarrInstanceUserIDs;
            oAssignments.non_instance_user_ids = Globals.iarrNonInstanceUserIds;
            
            orderProps.rep_name = fmdOrder.SalesRep;
            orderProps.pm_name = "";
            oOrderObj.client_id = GetCustomerID(fmdOrder.ClientName);
            oOrderObj.name = fmdOrder.OrderName;
            oOrderObj.description = fmdOrder.OrderDescription;
            oOrderObj.ship_date = fmdOrder.ShipDate.ToString("yyyy-MM-ddT00:00:00-hh:mm");
            oOrderObj.product_id = GetProductID("Graphics Product");
            oOrderObj.external_id = "";
            oOrderObj.source = "API";
            oOrderObj.assignments = oAssignments;
            oOrderObj.props = orderProps;
            
            /*
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!FROM ORDER!!!!!!!!!!!!!!!!!!!!!!!!!!");
            var json = new JavaScriptSerializer().Serialize(oOrderObj);
            Console.WriteLine(json);
            */

            try
            {
                //Add Header and Parameters
                rrRequest.AddHeader("Authorization", sarrCredentials[0] + " " + sarrCredentials[1]);
                rrRequest.AddJsonBody(oOrderObj);

                //Execute Request
                IRestResponse<dynamic> rrResponse = rcClient.Execute<dynamic>(rrRequest);
                              
                iOrderID = GetOrderID(fmdOrder.OrderName);
                
                /*
                Console.WriteLine("Response: ");
                Console.WriteLine(rrResponse.StatusCode);
                Console.WriteLine(rrResponse.Content.ToString());
                Console.WriteLine();                
                */

                if (rrResponse.StatusCode.ToString() == "OK") //new order.
                {
                    Globals.orderExist = "NO";
                }
                else if (rrResponse.StatusCode.ToString() == "BadRequest") //order exist.
                {
                    if (rrResponse.Content.ToString().Contains("Trying to assign not allowed users"))
                    {
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine("WARNING WARNING WARNING WARNING WARNING WARNING");
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine();
                        Console.WriteLine("CREATE ORDER ERROR: Users.");
                        //Console.WriteLine(fmdOrder.SalesRep + " not found in database.");
                        Console.WriteLine();
                        Globals.orderExist = "ERROR";
                    }
                    else if (rrResponse.Content.ToString().Contains("The selected client id is invalid."))
                    {
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine("WARNING WARNING WARNING WARNING WARNING WARNING");
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine();
                        Console.WriteLine("CREATE ORDER ERROR: Client Name.");
                        //Console.WriteLine(fmdOrder.ClientName + " not found in database.");
                        Console.WriteLine();
                        Globals.orderExist = "ERROR";
                    }
                    else if (rrResponse.Content.ToString().Contains("The name has already been taken."))
                    {
                        Globals.orderExist = "YES";
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

            return iOrderID;           
        }
        
        public static void UpdateOrder(string Path, int OrderID)
        {
            //Console.WriteLine("FROM CREATE ORDER");
            string sURL = @"https://raps.mcraeimaging.com/api/v2/order/" + OrderID + "/update";
            RestClient rcClient = new RestClient(sURL);
            RestRequest rrRequest = new RestRequest(Method.POST);
            string[] sarrCredentials = Authorize();

            Globals.checkJOBNUMBER = "YES";
            int iOrderID = -1;

            FileMakerData fmdOrder = new FileMakerData();
            OrderObject oOrderObj = new OrderObject();
            assignments oAssignments = new assignments();
            OrderProps orderProps = new OrderProps();
            fmdOrder.Load(Path);

            oAssignments.instance_user_ids = Globals.iarrInstanceUserIDs;
            oAssignments.non_instance_user_ids = Globals.iarrNonInstanceUserIds;

            orderProps.rep_name = fmdOrder.SalesRep;
            orderProps.pm_name = "";
            oOrderObj.client_id = GetCustomerID(fmdOrder.ClientName);
            oOrderObj.name = fmdOrder.OrderName;
            oOrderObj.description = fmdOrder.OrderDescription;
            oOrderObj.ship_date = fmdOrder.ShipDate.ToString("yyyy-MM-ddT00:00:00-hh:mm");
            oOrderObj.product_id = GetProductID("Graphics Product");
            oOrderObj.external_id = "";
            oOrderObj.source = "API";
            oOrderObj.assignments = oAssignments;
            oOrderObj.props = orderProps;
            
            /*
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!FROM ORDER!!!!!!!!!!!!!!!!!!!!!!!!!!");
            var json = new JavaScriptSerializer().Serialize(oOrderObj);
            Console.WriteLine(json);            
            */

            try
            {
                //Add Header and Parameters
                rrRequest.AddHeader("Authorization", sarrCredentials[0] + " " + sarrCredentials[1]);
                rrRequest.AddJsonBody(oOrderObj);

                //Execute Request
                IRestResponse<dynamic> rrResponse = rcClient.Execute<dynamic>(rrRequest);
                               
                iOrderID = GetOrderID(fmdOrder.OrderName);
   
                /*
                Console.WriteLine("Response: ");
                Console.WriteLine(rrResponse.StatusCode);
                Console.WriteLine(rrResponse.Content.ToString());
                Console.WriteLine();
                */

                if (rrResponse.StatusCode.ToString() == "OK") //new order.
                {
                    Globals.orderExist = "NO";
                }
                else if (rrResponse.StatusCode.ToString() == "BadRequest") //order exist.
                {
                    if (rrResponse.Content.ToString().Contains("Trying to assign not allowed users"))
                    {
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine("WARNING WARNING WARNING WARNING WARNING WARNING");
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine();
                        Console.WriteLine("CREATE ORDER ERROR: Users.");
                        //Console.WriteLine(fmdOrder.SalesRep + " not found in database.");
                        Console.WriteLine();
                        Globals.orderExist = "ERROR";
                    }
                    else if (rrResponse.Content.ToString().Contains("The selected client id is invalid."))
                    {
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine("WARNING WARNING WARNING WARNING WARNING WARNING");
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine();
                        Console.WriteLine("CREATE ORDER ERROR: Client Name.");
                        //Console.WriteLine(fmdOrder.ClientName + " not found in database.");
                        Console.WriteLine();
                        Globals.orderExist = "ERROR";
                    }
                    else if (rrResponse.Content.ToString().Contains("The name has already been taken."))
                    {
                        Globals.orderExist = "YES";
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
        public static void DeleteOrder(int toDelete)
        {
            string sURL = @"https://raps.mcraeimaging.com/api/v2/order/" + toDelete + "";
            RestClient rcClient = new RestClient(sURL);
            RestRequest rrRequest = new RestRequest(Method.DELETE);
            string[] sarrCredentials = Authorize();

            try
            {
                //Add Header and Parameters
                rrRequest.AddHeader("Authorization", sarrCredentials[0] + " " + sarrCredentials[1]);
                //rrRequest.AddJsonBody(oJobObj);

                //Execute Request
                IRestResponse<dynamic> rrResponse = rcClient.Execute<dynamic>(rrRequest);
                //Console.WriteLine(""+toDelete+" Job Deleted");

                Console.WriteLine("Response: ");
                Console.WriteLine(rrResponse.StatusCode);
                Console.WriteLine(rrResponse.Content.ToString());
                Console.WriteLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
        public static void CreateJob(FileMakerJobInfo JobInfo, int OrderID)
        {   
            if(Globals.SideAOnly == true)
            {
                string TempName = JobInfo.JobName;
                JobInfo.JobName = TempName + " | " + JobInfo.Width + "x" + JobInfo.Height + " | " + JobInfo.Fabric + " | " + JobInfo.JobDescription + " - SIDE A";
                createIT();
                JobInfo.JobName = TempName;
            }
            else if (Globals.SideBOnly == true)
            {
                string TempName = JobInfo.JobName;
                JobInfo.JobName = TempName + " | " + JobInfo.Width + "x" + JobInfo.Height + " | " + JobInfo.Fabric + " | " + JobInfo.JobDescription + " - SIDE B";
                createIT();
                JobInfo.JobName = TempName;
            }
            else if (JobInfo.DoubleSided == 1 && Globals.SideAOnly == false && Globals.SideAOnly == false)
            {
                string TempName = JobInfo.JobName;
                JobInfo.JobName = TempName + " | " + JobInfo.Width + "x" + JobInfo.Height + " | " + JobInfo.Fabric + " | " + JobInfo.JobDescription + " - SIDE A";
                createIT();
                JobInfo.JobName = TempName + " | " + JobInfo.Width + "x" + JobInfo.Height + " | " + JobInfo.Fabric + " | " + JobInfo.JobDescription + " - SIDE B";
                createIT();
                JobInfo.JobName = TempName;
            }
            else  if (JobInfo.DoubleSided == 0 && Globals.SideAOnly == false && Globals.SideAOnly == false)
            {
                string TempName = JobInfo.JobName;
                JobInfo.JobName = TempName + " | " + JobInfo.Width + "x" + JobInfo.Height + " | " + JobInfo.Fabric + " | " + JobInfo.JobDescription;
                createIT();
                JobInfo.JobName = TempName;
            }

            void createIT()
            {
                //Console.WriteLine("CREATE IT NAME: " + JobInfo.JobName);       
                //Console.WriteLine(String.Join("\n", GetOrdersList()));
                string sURL = @"https://raps.mcraeimaging.com/api/v2/job/create";
                RestClient rcClient = new RestClient(sURL);
                RestRequest rrRequest = new RestRequest(Method.POST);
                string[] sarrCredentials = Authorize();

                JobObject oJobObj = new JobObject();
                JobProps oProps = new JobProps();
                assignments oAssignments = new assignments();
                List<string> arrTFlows = new List<string>();
                float[] fPrintDimensions = GetPrintDimensions(JobInfo.ProductName);

                oAssignments.instance_user_ids = Globals.iarrInstanceUserIDs;
                oAssignments.non_instance_user_ids = Globals.iarrNonInstanceUserIds;

                //Set the values for the JobProps Object
                oProps.print_width = JobInfo.Width + fPrintDimensions[0];
                oProps.print_height = JobInfo.Height + fPrintDimensions[1];
                oProps.art_width = (JobInfo.Width + fPrintDimensions[0]) - 2;
                oProps.art_height = (JobInfo.Height + fPrintDimensions[1]) - 2;
                oProps.min_ppi = 100;
                oProps.sales_rep = GetUserID(JobInfo.SalesRep);
                oProps.quantity = JobInfo.Qty;
                oProps.shipping_address = "See on Order";
                oProps.phone_contact = JobInfo.UserName;
                oProps.email_contact = "";
                oProps.mis_job_number = "";
                oProps.mis_contact = "";
                oProps.rep_name = JobInfo.SalesRep;
                oProps.pm_name = JobInfo.Fabric;
                oProps.scale_factor = JobInfo.ScaleFactor;
                oProps.art_size = ""+oProps.art_width.ToString("0.000") + " x "+oProps.art_height.ToString("0.000") + " in";

                //Set the values for the Job Object
                oJobObj.name = JobInfo.JobName;
                oJobObj.description = JobInfo.JobDescription;
                oJobObj.notes = "";
                oJobObj.order_id = OrderID;
                oJobObj.ship_date = JobInfo.ShipDate.ToString("yyyy-MM-ddT00:00:00-hh:mm");
                oJobObj.start_date = JobInfo.StartDate.ToString("yyyy-MM-ddT00:00:00-hh:mm");
                oJobObj.product_id = GetProductID(JobInfo.ProductName);
                oJobObj.external_id = "External ID";
                oJobObj.source = "API";
                oJobObj.attach_proofs = false;
                //oJobObj.proof_profile_id = GetProfileID(JobInfo.ProductName);
                oJobObj.proof_profile_id = GetProfileID(GetProofProfile(JobInfo.ProductName));
                oJobObj.notes = JobInfo.Customer_PO;
                oJobObj.assignments = oAssignments;
                oJobObj.props = oProps;

                //oJobObj.finishing_profile_id = iFinishingProfileID;
                //oJobObj.inherit_finishing_profile = bInheritFinishingProfile;
                //oJobObj.production_profile_id = iProductionProfileID;

                /*
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!FROM JOB!!!!!!!!!!!!!!!!!!!!!!!!!!");
                var json = new JavaScriptSerializer().Serialize(oJobObj);
                Console.WriteLine(json);
                */
             
            try
            {
                //Add Header and Parameters
                rrRequest.AddHeader("Authorization", sarrCredentials[0] + " " + sarrCredentials[1]);
                rrRequest.AddJsonBody(oJobObj);

                //Execute Request
                IRestResponse<dynamic> rrResponse = rcClient.Execute<dynamic>(rrRequest);
                
                /*
                Console.WriteLine("Response: ");
                Console.WriteLine(rrResponse.StatusCode);
                Console.WriteLine(rrResponse.Content.ToString());
                Console.WriteLine();
                */
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

         }
     }
        public static void UpdateJob(FileMakerJobInfo JobInfo, int OrderID)
        {        
            if (JobInfo.DoubleSided == 1 )
             {
                 string TempName = JobInfo.JobName;
                 JobInfo.JobName = TempName + " | " + JobInfo.Width + "x" + JobInfo.Height + " | " + JobInfo.Fabric + " | " + JobInfo.JobDescription + " - SIDE A";
                 updateIT();
                 JobInfo.JobName = TempName + " | " + JobInfo.Width + "x" + JobInfo.Height + " | " + JobInfo.Fabric + " | " + JobInfo.JobDescription + " - SIDE B";
                 updateIT();
                 JobInfo.JobName = TempName;
             }
             else if (JobInfo.DoubleSided == 0)
             {
                string TempName = JobInfo.JobName;
                JobInfo.JobName = TempName + " | " + JobInfo.Width + "x" + JobInfo.Height + " | " + JobInfo.Fabric + " | " + JobInfo.JobDescription;
                updateIT();
                JobInfo.JobName = TempName;
            }

            //Console.WriteLine(String.Join("\n", GetOrdersList()));
            void updateIT()
            {
                int toUpdate = GetJobID(OrderID, JobInfo.JobName);
                //Console.WriteLine("UPDATE JOB NAME: " + JobInfo.JobName);
                string sURL = @"https://raps.mcraeimaging.com/api/v2/job/" + toUpdate + "/update";
                RestClient rcClient = new RestClient(sURL);
                RestRequest rrRequest = new RestRequest(Method.POST);
                string[] sarrCredentials = Authorize();

                JobObject oJobUpdateObj = new JobObject();
                JobProps oProps = new JobProps();
                assignments oAssignments = new assignments();
                List<string> arrTFlows = new List<string>();
                float[] fPrintDimensions = GetPrintDimensions(JobInfo.ProductName);

                oAssignments.instance_user_ids = Globals.iarrInstanceUserIDs;
                oAssignments.non_instance_user_ids = Globals.iarrNonInstanceUserIds;

                //Set the values for the JobProps Object
                oProps.print_width = JobInfo.Width + fPrintDimensions[0];
                oProps.print_height = JobInfo.Height + fPrintDimensions[1];
                oProps.art_width = (JobInfo.Width + fPrintDimensions[0]) - 2;
                oProps.art_height = (JobInfo.Height + fPrintDimensions[1]) - 2;
                oProps.min_ppi = 100;
                oProps.sales_rep = GetUserID(JobInfo.SalesRep);
                oProps.quantity = JobInfo.Qty;
                oProps.shipping_address = "See on Order";
                oProps.phone_contact = JobInfo.UserName;
                //oProps.email_contact = JobInfo.UserEmail;
                oProps.mis_job_number = "";
                oProps.mis_contact = "";
                oProps.rep_name = JobInfo.SalesRep;
                oProps.pm_name = JobInfo.Fabric;
                oProps.scale_factor = JobInfo.ScaleFactor;
                oProps.art_size = "" + oProps.art_width.ToString("0.000") + " x " + oProps.art_height.ToString("0.000") + " in";

                //Set the values for the Job Object
                oJobUpdateObj.name = JobInfo.JobName;
                oJobUpdateObj.description = JobInfo.JobDescription;
                oJobUpdateObj.notes = "";
                oJobUpdateObj.order_id = OrderID;
                oJobUpdateObj.ship_date = JobInfo.ShipDate.ToString("yyyy-MM-ddT00:00:00-hh:mm");
                oJobUpdateObj.start_date = JobInfo.StartDate.ToString("yyyy-MM-ddT00:00:00-hh:mm");
                oJobUpdateObj.product_id = GetProductID(JobInfo.ProductName);
                oJobUpdateObj.external_id = "External ID";
                oJobUpdateObj.source = "API";
                oJobUpdateObj.attach_proofs = false;
                //oJobUpdateObj.proof_profile_id = GetProfileID(JobInfo.ProductName);
                oJobUpdateObj.proof_profile_id = GetProfileID(GetProofProfile(JobInfo.ProductName));
                oJobUpdateObj.notes = JobInfo.Customer_PO;
                oJobUpdateObj.assignments = oAssignments;
                oJobUpdateObj.props = oProps;

                //oJobUpdateObj.finishing_profile_id = iFinishingProfileID;
                //oJobUpdateObj.inherit_finishing_profile = bInheritFinishingProfile;
                //oJobUpdateObj.production_profile_id = iProductionProfileID;

                /*
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!FROM JOB!!!!!!!!!!!!!!!!!!!!!!!!!!");
                var json = new JavaScriptSerializer().Serialize(oJobUpdateObj);
                Console.WriteLine(json);
                */

                try
                {
                    //Add Header and Parameters
                    rrRequest.AddHeader("Authorization", sarrCredentials[0] + " " + sarrCredentials[1]);
                    rrRequest.AddJsonBody(oJobUpdateObj);

                    //Execute Request
                    IRestResponse<dynamic> rrResponse = rcClient.Execute<dynamic>(rrRequest);

                    /*
                    Console.WriteLine("Response: ");
                    Console.WriteLine(rrResponse.StatusCode);
                    Console.WriteLine(rrResponse.Content.ToString());
                    Console.WriteLine();
                     */

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadLine();
                }
                //Console.WriteLine();
            }
        }
        public static void DeleteJob(int toDelete)
         {
            string sURL = @"https://raps.mcraeimaging.com/api/v2/job/" + toDelete+"";
            RestClient rcClient = new RestClient(sURL);
            RestRequest rrRequest = new RestRequest(Method.DELETE);
            string[] sarrCredentials = Authorize();
            
            try
            {
                 //Add Header and Parameters
                 rrRequest.AddHeader("Authorization", sarrCredentials[0] + " " + sarrCredentials[1]);
                 //rrRequest.AddJsonBody(oJobObj);

                 //Execute Request
                 IRestResponse<dynamic> rrResponse = rcClient.Execute<dynamic>(rrRequest);
                 //Console.WriteLine(""+toDelete+" Job Deleted");
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message);
                 Console.ReadLine();
             }
        }
        public static void CreateUser(string companyName, string userName)
        {
            string sURL = @"https://raps.mcraeimaging.com/api/v2/user/create";
            RestClient rcClient = new RestClient(sURL);
            RestRequest rrRequest = new RestRequest(Method.POST);
            string[] sarrCredentials = Authorize();

            List<string> iAlerts = new List<string>();

            UserInfo newuser = new UserInfo();
            newuser.email = userName;
            newuser.client_id = GetCustomerID(companyName);
            newuser.name = userName;
            newuser.password = "112233";
            newuser.enabled = true;
            newuser.locale = "en";

            iAlerts.Add("alert.job.approve"); //Job Approved
            //iAlerts.Add("alert.job.checks_ok"); //Awaiting Approval
            //iAlerts.Add("alert.job.checks_ok_to_step_1");
            //iAlerts.Add("alert.job.upload_proof"); //Proof Uploaded for Job Sent only when proof is uploaded manually
            //iAlerts.Add("alert.job.undo_approve"); //Undo Approve Job
            //iAlerts.Add("alert.job.cancel");
            //iAlerts.Add("alert.job.create"); //Job Created
            //iAlerts.Add("alert.job.checks_error"); //File cannot be processed
            //iAlerts.Add("alert.job.undo_cancel");
            //iAlerts.Add("alert.job.place_on_hold"); //Preflight Errors(On Hold)
            //iAlerts.Add("alert.job.re_release_from_released"); //Job Sent to Production Again    Not applicable for No release workflow
            //iAlerts.Add("alert.job.release"); //Sent to Production  Not applicable for No release workflow
            //iAlerts.Add("alert.job.request_new_revision"); //New Revision requested
            //iAlerts.Add("alert.job.reject"); //Preflight Errors Not approved
            //iAlerts.Add("alert.job.undo_reject"); //Undo Cancel due to Preflight Error
            //iAlerts.Add("alert.job.request_rush"); //RUSH status requested
            //iAlerts.Add("alert.job.clear_rush"); //RUSH status cleared
            //iAlerts.Add("alert.job.undo_rush"); //RUSH status undone
            iAlerts.Add("alert.order.create"); //Order Created
            //iAlerts.Add("alert.comment.create"); //New Comment Posted
            //iAlerts.Add("alert.comment.delete"); //Comment Deleted
            //iAlerts.Add("alert.comment.edit"); //Comment Edited

            newuser.alertTypes = iAlerts;

            /*
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!FROM USER!!!!!!!!!!!!!!!!!!!!!!!!!!");
            var json = new JavaScriptSerializer().Serialize(newuser);
            Console.WriteLine(json);
            */

            try
            {
                //Add Header and Parameters
                rrRequest.AddHeader("Authorization", sarrCredentials[0] + " " + sarrCredentials[1]);
                rrRequest.AddJsonBody(newuser);

                //Execute Request
                IRestResponse<dynamic> rrResponse = rcClient.Execute<dynamic>(rrRequest);

                /*
                Console.WriteLine("Response: ");
                Console.WriteLine(rrResponse.StatusCode);
                Console.WriteLine(rrResponse.Content.ToString());
                Console.WriteLine();
                */

                /*
                if (rrResponse.StatusCode.ToString() == "OK") //new order.
                {
                }
                else if (rrResponse.StatusCode.ToString() == "BadRequest") //order exist.
                {
                    if (rrResponse.Content.ToString().Contains("The email has already been taken."))
                    {
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine("WARNING WARNING WARNING WARNING WARNING WARNING");
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine();
                        Console.WriteLine("CREATE USER ERROR: Email is already taken");
                    }
                    else if (rrResponse.Content.ToString().Contains("The selected client id is invalid."))
                    {
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine("WARNING WARNING WARNING WARNING WARNING WARNING");
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine();
                        Console.WriteLine("CREATE USER ERROR: Client Name Not Found."); 
                    }                    
                }
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
        public static void UpdateUser(string userName) //NOT USING THIS YET
        {
            string userID = userName;
            string sURL = @"https://raps.mcraeimaging.com/api/v2/user/"+userID+"";
            RestClient rcClient = new RestClient(sURL);
            RestRequest rrRequest = new RestRequest(Method.POST);
            string[] sarrCredentials = Authorize();

            List<string> iAlerts = new List<string>();

            UserInfo newuser = new UserInfo();
            newuser.email = userName;
            newuser.name = userName;
            newuser.password = "112233";
            newuser.enabled = true;
            newuser.locale = "en";

            iAlerts.Add("alert.job.approve"); //Job Approved
            //iAlerts.Add("alert.job.checks_ok"); //Awaiting Approval
            //iAlerts.Add("alert.job.checks_ok_to_step_1");
            //iAlerts.Add("alert.job.upload_proof"); //Proof Uploaded for Job Sent only when proof is uploaded manually
            //iAlerts.Add("alert.job.undo_approve"); //Undo Approve Job
            //iAlerts.Add("alert.job.cancel");
            //iAlerts.Add("alert.job.create"); //Job Created
            //iAlerts.Add("alert.job.checks_error"); //File cannot be processed
            //iAlerts.Add("alert.job.undo_cancel");
            //iAlerts.Add("alert.job.place_on_hold"); //Preflight Errors(On Hold)
            //iAlerts.Add("alert.job.re_release_from_released"); //Job Sent to Production Again    Not applicable for No release workflow
            //iAlerts.Add("alert.job.release"); //Sent to Production  Not applicable for No release workflow
            //iAlerts.Add("alert.job.request_new_revision"); //New Revision requested
            //iAlerts.Add("alert.job.reject"); //Preflight Errors Not approved
            //iAlerts.Add("alert.job.undo_reject"); //Undo Cancel due to Preflight Error
            //iAlerts.Add("alert.job.request_rush"); //RUSH status requested
            //iAlerts.Add("alert.job.clear_rush"); //RUSH status cleared
            //iAlerts.Add("alert.job.undo_rush"); //RUSH status undone
            iAlerts.Add("alert.order.create"); //Order Created
            //iAlerts.Add("alert.comment.create"); //New Comment Posted
            //iAlerts.Add("alert.comment.delete"); //Comment Deleted
            //iAlerts.Add("alert.comment.edit"); //Comment Edited

            newuser.alertTypes = iAlerts;

            /*
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!FROM USER!!!!!!!!!!!!!!!!!!!!!!!!!!");
            var json = new JavaScriptSerializer().Serialize(newuser);
            Console.WriteLine(json);
            */

            try
            {
                //Add Header and Parameters
                rrRequest.AddHeader("Authorization", sarrCredentials[0] + " " + sarrCredentials[1]);
                rrRequest.AddJsonBody(newuser);

                //Execute Request
                IRestResponse<dynamic> rrResponse = rcClient.Execute<dynamic>(rrRequest);

                /*
                Console.WriteLine("Response: ");
                Console.WriteLine(rrResponse.StatusCode);
                Console.WriteLine(rrResponse.Content.ToString());
                Console.WriteLine();
                */

                /*
                if (rrResponse.StatusCode.ToString() == "OK") //new order.
                {
                }
                else if (rrResponse.StatusCode.ToString() == "BadRequest") //order exist.
                {
                    if (rrResponse.Content.ToString().Contains("The email has already been taken."))
                    {
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine("WARNING WARNING WARNING WARNING WARNING WARNING");
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine();
                        Console.WriteLine("CREATE USER ERROR: Email is already taken");
                    }
                    else if (rrResponse.Content.ToString().Contains("The selected client id is invalid."))
                    {
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine("WARNING WARNING WARNING WARNING WARNING WARNING");
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Console.WriteLine();
                        Console.WriteLine("CREATE USER ERROR: Client Name Not Found."); 
                    }                    
                }
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public static void ProcessFileMakerXML(string Path)
        {
            Globals.xmlPath = Path;
            FileMakerData fmdData = new FileMakerData();
            int iOrderID;
            List<FileMakerJobInfo> fmjiJobs = new List<FileMakerJobInfo>();
            fmdData.Load(Path);
            fmjiJobs = fmdData.Jobs;

            //TO ADD USER TO ORDER--------------------------------------------------------------------------------------------------------------------------------//

            /*** TO ADD INSTANCE USERS TO THE ORDER ***/
            //Globals.iarrInstanceUserIDs.Add(100); //Tucanna Tech Support
            Globals.iarrInstanceUserIDs.Add(101); //Dave McQuiggin
            Globals.iarrInstanceUserIDs.Add(102); //Lin
            //Globals.iarrInstanceUserIDs.Add(105); //tBOT
            //Globals.iarrInstanceUserIDs.Add(107); //Lori Roy
            Globals.iarrInstanceUserIDs.Add(114); //Derrick
            //Globals.iarrInstanceUserIDs.Add(125); //Stella
            Globals.iarrInstanceUserIDs.Add(198); //Deen
            Globals.iarrInstanceUserIDs.Add(199); //Davent
            Globals.iarrInstanceUserIDs.Add(200); //Matthew
            Globals.iarrInstanceUserIDs.Add(GetUserID(fmdData.SalesRep));

            /*** TO ADD NON INSTANCE USERS TO THE ORDER ***/
            //if (fmdData.ClientName == "B&Co inc." || fmdData.ClientName == "Adfab"  || fmdData.ClientName == "Holland & Crosby Limited" || fmdData.ClientName == "Central Station" || fmdData.ClientName == "EurOptimum Display")
           if (fmdData.ClientName == "B&Co inc." || 
               fmdData.ClientName == "Adfab" || 
               fmdData.ClientName == "Test Company" || 
               fmdData.ClientName == "McRae Imaging (Testing & System Development)" ||
               fmdData.ClientName == "Metal Creations" ||
               fmdData.ClientName == "Holland & Crosby Limited" ||
               fmdData.ClientName == "Central Station" ||
               fmdData.ClientName == "EurOptimum Display" ||
               fmdData.ClientName == "Impact Imaging" ||
               fmdData.ClientName == "Archex Display" 
               )
           {
                foreach (string addUser in Globals.curUserEmail)
                {
                    CreateUser(fmdData.ClientName, addUser);
                    Globals.iarrNonInstanceUserIds.Add(GetUserID(addUser));
                }
           }
            //Console.WriteLine(String.Join("\n", Globals.iarrInstanceUserIDs));            

            //TO ADD USER TO ORDER--------------------------------------------------------------------------------------------------------------------------------//

            #region ---Displaying info
            //FOR DISPLAYING-----------------------------------------------------------------------------------//

            //Console.WriteLine("!!!!!!!!!!!!");
            //Console.WriteLine("Order#: " + fmdData.OrderName);
            //Console.WriteLine("OrderID: " + iOrderID);
            //Console.WriteLine(String.Join("\n", GetOrdersList()));
            //Console.WriteLine(String.Join("\n", GetJobList()));
            //Console.WriteLine(String.Join("\n", GetClientList()));
            //Console.WriteLine(String.Join("\n", GetProductList()));
            //Console.WriteLine(String.Join("\n", GetProfilesList()));
            //Console.WriteLine(String.Join("\n", GetAlertList()));
            //Console.WriteLine(String.Join("\n", GetUserList()));

            //FOR DISPLAYING-----------------------------------------------------------------------------------//
            #endregion

            //Create Order
            iOrderID = CreateOrder(Path);
            Console.WriteLine("Order#: " + fmdData.OrderName);
            Console.WriteLine();

            //Globals.orderExist = "ERROR"; //USE THIS FOR TESTING

            /*Create Job*/
            if (Globals.orderExist == "NO")
            {
                Console.WriteLine("Creating New Order.\n");
                /*
                Console.WriteLine("Order#: " + fmdData.OrderName);
                Console.WriteLine("OrderID: " + iOrderID);
                Console.WriteLine("# of Jobs in XML: " + fmjiJobs.Count());
                */
                foreach (FileMakerJobInfo fmjiJob in fmjiJobs)
                {
                    CreateJob(fmjiJob, iOrderID);
                }
            }

            
            else if (Globals.orderExist == "YES")
            {
                Console.WriteLine("Order Already Exist.");
                Console.WriteLine();
                Console.WriteLine("--- UPDATING ORDER.");
                UpdateOrder(Path, iOrderID);
                Console.WriteLine();
                GetTFLOWJobName(iOrderID); //Add the job names from tFlow

                #region ---testing for jobs
                
                // TESTING FOR JOBS 
                /*for (int i = 0; i < Globals.curjobNameXML.Count(); i++)
                {
                    Console.WriteLine("Job Name From FileMaker: " + Globals.curjobNameXML[i]);
                    //Console.WriteLine("JobID: " + GetJobID(iOrderID, Globals.curjobNameXML[i]));
                }
                //Console.WriteLine("# of Jobs in TFLOW: " + Globals.curjobNameTFLOW.Count());
                for (int i = 0; i < Globals.curjobNameTFLOW.Count(); i++)
                {
                    Console.WriteLine("Job Name From Tflow: " + Globals.curjobNameTFLOW[i]);
                    //Console.WriteLine("JobID: " + GetJobID(iOrderID, Globals.curjobNameTFLOW[i]));
                }
                //////COMAPRE JOBS BETWEEN XML AND TFLOW          
                for (int a = 0; a < Globals.curjobNameXML.Count(); a++)
                {
                    for (int b = 0; b < Globals.curjobNameTFLOW.Count(); b++)
                        if (Globals.curjobNameXML[a] == Globals.curjobNameTFLOW[b])
                        {
                            //Console.WriteLine("THE MATCHING # is: " + Globals.curjobNameXML[a]);
                        }
                }
                
                //Console.WriteLine("Order#: " + fmdData.OrderName);
                //Console.WriteLine("OrderID: " + iOrderID);
                //Console.WriteLine("# of Jobs in XML: " + Globals.curjobNameXML.Count());
                */
                #endregion

                var XMLNotTFLOW = Globals.curjobNameXML.Except(Globals.curjobNameTFLOW).ToList();
                var TFLOWNotXML = Globals.curjobNameTFLOW.Except(Globals.curjobNameXML).ToList();
                /*
                Console.WriteLine("XML");
                Console.WriteLine(String.Join("\n", Globals.curjobNameXML));
                Console.WriteLine("TFLOW");
                Console.WriteLine(String.Join("\n", Globals.curjobNameTFLOW));

                Console.WriteLine("XML NOT IN TFLOW");
                Console.WriteLine(String.Join("\n", XMLNotTFLOW));
                Console.WriteLine("TFLOW NOT IN XML");
                Console.WriteLine(String.Join("\n", TFLOWNotXML));

                Console.WriteLine("XML COUNT: " + XMLNotTFLOW.Count);
                Console.WriteLine("TFLOW COUNT: " +TFLOWNotXML.Count);
                */

                ////DELETE THE EXTRA ITEMS IN TFLOW
                if (TFLOWNotXML.Count > 0)
                {
                    //Console.WriteLine(String.Join("\n", TFLOWNotXML));
                    Console.WriteLine("--- DELETING JOBS.");
                    Console.WriteLine();
                    foreach (string toDelete in TFLOWNotXML)
                    {
                        DeleteJob(GetJobID(iOrderID, toDelete));
                    }
                }

                ////CREATE THE JOBS FROM XML THAT ARE NOT IN TFLOW
                if (XMLNotTFLOW.Count > 0)
                {
                    Console.WriteLine("--- CREATING NEW JOB ITEMS.");
                    //Console.WriteLine(String.Join("\n", XMLNotTFLOW));
                    Console.WriteLine();

                    foreach (string toCreate in XMLNotTFLOW)
                    {
                        string fixedName;
                        fixedName = toCreate.Remove(2, toCreate.Count() - 2);

                        if (toCreate.Contains("- SIDE A") || toCreate.Contains("- SIDE B"))
                        {
                            if (toCreate.Contains("- SIDE A"))
                            { Globals.SideAOnly = true; }
                            else if (toCreate.Contains("- SIDE B"))
                            { Globals.SideBOnly = true; }

                            foreach (FileMakerJobInfo fmjiJob in fmjiJobs)
                            {
                                if (fmjiJob.JobName == fixedName)
                                {
                                    CreateJob(fmjiJob, iOrderID);
                                    Globals.SideAOnly = false;
                                    Globals.SideBOnly = false;
                                }
                            }
                        }
                        else
                        {
                            foreach (FileMakerJobInfo fmjiJob in fmjiJobs)
                            {
                                if (fmjiJob.JobName == fixedName)
                                {
                                    CreateJob(fmjiJob, iOrderID);
                                }
                            }
                        }
                    }
                    Console.WriteLine("--- UPDATING JOB ITEMS.");
                    Console.WriteLine();
                    foreach (FileMakerJobInfo fmjiJob in fmjiJobs)
                    {
                        UpdateJob(fmjiJob, iOrderID);
                    }
                }

                ////AFTER DELETING UPDATE THE JOBS TO TFLOW    
                if (XMLNotTFLOW.Count == 0)
                {
                    Console.WriteLine("--- UPDATING JOB ITEMS.");
                    Console.WriteLine();
                    foreach (FileMakerJobInfo fmjiJob in fmjiJobs)
                    {
                        UpdateJob(fmjiJob, iOrderID);
                    }
                }
            }

            else if (Globals.orderExist == "ERROR")
            {
                Console.WriteLine("ERROR UPLOADING THE ORDER. PLEASE CHECK WITH SUPPORT.\n");
            }
            Console.WriteLine("PROCESS COMPLETED. PLEASE CLOSE THE WINDOW.");

        }

    }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




    #region FileMakerXML
    public class FileMakerJobInfo
    {
        public string ClientName { get; set; }
        public string JobName { get; set; }
        public string JobDescription { get; set; }
        public string ProductName { get; set; }
        public DateTime ShipDate { get; set; }
        public DateTime StartDate { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public int Qty { get; set; }
        public string SalesRep { get; set; }
        public string ProofProfileName { get; set; }
        public string ScaleFactor { get; set; }
        public string Fabric { get; set; }
        public int DoubleSided { get; set; }
        public string Customer_PO { get; set; }
    }
    public class FileMakerData
    {
        public string ClientName { get; set; }
        public string OrderName { get; set; }
        public string OrderDescription { get; set; }
        public string SalesRep { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ShipDate { get; set; }
        public List<FileMakerJobInfo> Jobs { get; set; }

        public void Load(string Path)
        {
            Globals.curjobNameXML.Clear();

            //Variables
            XmlDocument doc = new XmlDocument();
            doc.Load(Path);
            XmlNodeList xmlJobs = doc.GetElementsByTagName("command");

            if (xmlJobs.Count <= 0)
            {
                Console.WriteLine("ERROR:");
                Console.WriteLine("There are no Jobs in the specified XML file.");
                Console.ReadLine();
            }
            else
            {
                //Set the Order variables
                ClientName = xmlJobs[0].SelectSingleNode("client_name").InnerText;
                OrderName = xmlJobs[0].SelectSingleNode("order_name").InnerText;
                OrderDescription = xmlJobs[0].SelectSingleNode("order_description").InnerText;
                SalesRep = xmlJobs[0].SelectSingleNode("sales_rep").InnerText;
                var names = SalesRep.Split(' ');
                SalesRep = names[0]; //only take first name 
                ShipDate = DateTime.Parse(xmlJobs[0].SelectSingleNode("ship_date").InnerText);
                StartDate = DateTime.Parse(xmlJobs[0].SelectSingleNode("start_date").InnerText);
                
                //Get all the information for each LineItem/Job
                Jobs = new List<FileMakerJobInfo>();
                
                foreach (XmlNode xmlCurJob in xmlJobs)
                {                    
                    FileMakerJobInfo fmJob = new FileMakerJobInfo();
                    fmJob.ClientName = xmlCurJob.SelectSingleNode("client_name").InnerText;                   
                    fmJob.JobName = xmlCurJob.SelectSingleNode("job_name").InnerText;
                    fmJob.JobDescription = xmlCurJob.SelectSingleNode("product_description").InnerText;
                    fmJob.StartDate = DateTime.Parse(xmlCurJob.SelectSingleNode("start_date").InnerText);
                    fmJob.ShipDate = DateTime.Parse(xmlCurJob.SelectSingleNode("ship_date").InnerText);
                    fmJob.UserEmail = xmlCurJob.SelectSingleNode("user_email").InnerText;
                    fmJob.UserName = xmlCurJob.SelectSingleNode("user_name").InnerText;
                    fmJob.Width = float.Parse(xmlCurJob.SelectSingleNode("min_width_inches").InnerText);
                    fmJob.Height = float.Parse(xmlCurJob.SelectSingleNode("min_height_inches").InnerText);
                    fmJob.ProofProfileName = "custom_proof_McRae";
                    fmJob.ProductName = xmlCurJob.SelectSingleNode("product_name").InnerText;
                    fmJob.Qty = int.Parse(xmlCurJob.SelectSingleNode("quantity").InnerText);
                    fmJob.SalesRep = xmlCurJob.SelectSingleNode("sales_rep").InnerText;
                    fmJob.SalesRep = names[0]; //only take first name                   
                    fmJob.Fabric = xmlCurJob.SelectSingleNode("pm_name").InnerText;
                    fmJob.DoubleSided = int.Parse(xmlCurJob.SelectSingleNode("double_sided").InnerText);
                    fmJob.Customer_PO = xmlCurJob.SelectSingleNode("customer_po").InnerText;

                    //Store Customer's Email
                    Globals.curUserEmail.Clear();
                    var emails = fmJob.UserEmail.Split(';');
                    foreach (String eachEmail in emails)
                    {
                        Globals.curUserEmail.Add(eachEmail.Trim());
                    }
                    //Console.WriteLine(String.Join("\n", Globals.curUserEmail));

                    //Store Job Name Depening On The Product
                    if (Globals.checkJOBNUMBER == "YES")
                    {                        
                        if (fmJob.DoubleSided == 1)
                        {
                            string TempName = fmJob.JobName;
                            fmJob.JobName = TempName + " | " + fmJob.Width + "x" + fmJob.Height + " | " + fmJob.Fabric + " | " + fmJob.JobDescription + " - SIDE A";
                            Globals.curjobNameXML.Add(fmJob.JobName);
                            fmJob.JobName = TempName + " | " + fmJob.Width + "x" + fmJob.Height + " | " + fmJob.Fabric + " | " + fmJob.JobDescription + " - SIDE B"; 
                            Globals.curjobNameXML.Add(fmJob.JobName);
                            fmJob.JobName = TempName;
                        }
                        else
                        {
                            string TempName = fmJob.JobName;
                            fmJob.JobName = TempName + " | " + fmJob.Width + "x" + fmJob.Height + " | " + fmJob.Fabric + " | " + fmJob.JobDescription;
                            Globals.curjobNameXML.Add(fmJob.JobName);
                            fmJob.JobName = TempName;
                        }
                    }
                    //Console.WriteLine(String.Join("\n", Globals.curjobNameXML));


                    //Determine Scale Factor
                    if (fmJob.Height < 200 && fmJob.Width < 200)
                    {
                        fmJob.ScaleFactor = "Recommended scale at 100%";
                    }
                    else
                    {
                        fmJob.ScaleFactor = "Recommended scale at 10%";
                    }
                    
                    Jobs.Add(fmJob);

                    //Console.WriteLine("--------------------------------");
                    //Console.WriteLine(Jobs);
                    //Console.WriteLine(String.Join("\n", Jobs[0]));

                }

            }

        }

    }
    #endregion
}
