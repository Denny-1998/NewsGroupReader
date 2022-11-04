using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml.Linq;

namespace NewsGroupReader.Model
{
    public class Connection
    {
        TextBox tbStatus;


        private string Address, UserName, Password;
        private int Port;
        TcpClient tcp;
        NetworkStream stream;
        NntpReader reader;




        #region Constuctors
        /// <summary>
        /// constructor if no textbox for status codes is used
        /// </summary>
        /// <param name="auth">
        /// takes authentificationdata object
        /// </param>
        public Connection(AuthorisationData auth)
        {
            Address = auth.Address;
            UserName = auth.UserName;
            Password = auth.Password;
            Port = auth.Port;

            this.tbStatus = new TextBox();

        }

        /// <summary>
        /// constructor if ui implements a textbox for status codes 
        /// </summary>
        /// <param name="auth">
        /// takes authentification data object 
        /// </param>
        /// <param name="tbStatus">
        /// takes the status textbox by reference to post status codes 
        /// </param>
        public Connection(AuthorisationData auth, ref TextBox tbStatus)
        {
            Address = auth.Address;
            UserName = auth.UserName;
            Password = auth.Password;
            Port = auth.Port;

            this.tbStatus = tbStatus;
            
        }
        #endregion





        #region Main methods
        /// <summary>
        /// establish tcp connection and 
        /// check if the response is 200 (helper method isSuccessfull() used for that)
        /// </summary>
        /// <returns>
        /// returns boolean if connection is succcessfull
        /// </returns>
        public async Task<bool> Connect()
        {
            tbStatus.Text = "connecting...";

            tcp = new TcpClient();

            try
            {
                tcp.Connect(Address, Port);
                if (tcp.Connected)
                {
                    stream = tcp.GetStream();

                    reader = new NntpReader(stream, Encoding.UTF8, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured: \n\n " + ex.Message);
            }


            return isSuccessfull();
        }







        /// <summary>
        /// method to send authorizationdata 
        /// it takes the username and password given by the constructor and sends authinfo commands
        /// </summary>
        /// <returns></returns>
        public async Task authorizeAsync() 
        {
            

            string UserCommand = $"AUTHINFO USER {UserName}" +
                "\n";        //this piece of s**t cost me two days
            string PasswordCommand = $"AUTHINFO PASS {Password}\n";




            //send username 
            SendData(UserCommand);
            

            string UserDataResponse = Response();

            

            //if successfull, send password
            if (UserDataResponse.StartsWith("381"))
            {
               //send data
               SendData(PasswordCommand);


                string PasswordResponse = Response();
                


            }
            else
            {
                MessageBox.Show("error: \n\n" + UserDataResponse);
            }
            
        }


        /// <summary>
        /// list all available news groups on this server
        /// </summary>
        /// <returns>
        /// returns list of news groups 
        /// </returns>
        public List<string> GetNewsGroups()
        {
            string getNewsGroupsCommand = "LIST\n";

            SendData(getNewsGroupsCommand);

            List<string> newsGroups = MultiLineResponse(true);

            return newsGroups;
        }




        /// <summary>
        /// list last available articles in selected newsgroup 
        /// (all articles would be unpractical because of the number)
        /// </summary>
        /// <returns>
        /// returns a list of articles 
        /// </returns>
        public List<Article> GetArticles(string selectedGroup)
        {
            string selectGroupCommand = $"GROUP {selectedGroup}\n";

            Trace.WriteLine($"getting data for {selectedGroup}...");


            SendData(selectGroupCommand);
            string response = Response();


            //successfull?
            if (!response.StartsWith("211"))
            {
                MessageBox.Show("something went wrong\n\n" + response);
                return null;
            }



                    //211 n f l s group selected
                    //n = estimated number of articles in group,    1
                    //f = first article number in the group,        2
                    //l = last article number in the group,         3
                    //s = name of the group.                        4


            //split string to get all the information seperately
            string[] subStrings = response.Split(' ');


            //this should not be possible
            if (subStrings.Length != 5) Trace.WriteLine("what is going on???");


            //get number of last article in the list
            int lastArticle = Int32.Parse(subStrings[3]);
            int articlesToShow = Int32.Parse(subStrings[1]);

             
            //limit number of articles 
            if(articlesToShow >= 10) articlesToShow = 10;                   //still loads them really slowly
            
            
            //generate a list to store the articles
            List<Article> articles = new List<Article>();
            
            
            
            //get title and id for each article
            for (int i = lastArticle; i > lastArticle-articlesToShow ; i--)
            {
                Article a = GenerateArticle(i);
                articles.Add(a);
            }

            

            return articles;
        }

        #endregion








        #region HelperMethods

        /// <summary>
        /// encodes data and sends them into the network stream 
        /// </summary>
        /// <param name="data"></param>
        private async void SendData(string data)
        {
            //encode data
            byte[] byteData;
            byteData = Encoding.UTF8.GetBytes(data);

            try
            {
                // Define the cancellation token.
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken cancellationToken = source.Token;




                //send data
                Trace.WriteLine("sending data...");

                await stream.WriteAsync(byteData, 0, byteData.Length, cancellationToken);

            }
            catch (Exception ex)
            {
                MessageBox.Show("sending went wrong \n\n" + ex);
            }

        }


        /// <summary>
        /// just reads a response from the stream, writes it to the console and returns it as a string
        /// </summary>
        /// <returns></returns>
        private string Response()
        {
            string response = reader.ReadLine();

            tbStatus.Text = response;
            Trace.WriteLine(response);


            return response;
        }
        



        /// <summary>
        /// reads multiple lines from the networkstream and creates a list object 
        /// </summary>
        /// <param name="isNewsGroupList"> if the response is expected to be the list of news groups, the lasf few characters get removed </param>
        /// <returns> 
        /// returns a list of lines in the response
        /// </returns>
        private List<string> MultiLineResponse(bool isNewsGroupList)
        {
            List<string> response; 

            response = reader.ListAllLines();


            //if multiline response is newsgroup list, remove the last characters 
            if (isNewsGroupList)
            {
                for (int i = 0; i < response.Count; i++)
                {
                    response[i] = RemoveLastCars(response[i]);
                }
            }


            return response;
        }



        /// <summary>
        /// sends a request for an article
        /// </summary>
        /// <param name="ArticleNumber"></param>
        /// <returns>
        /// returns an article object with all needed information and content 
        /// </returns>
        private Article GenerateArticle(int ArticleNumber)
        {
            string ArticleCommand = $"BODY {ArticleNumber}\n";

            //request an article from the server
            SendData(ArticleCommand);
            List<string> response = MultiLineResponse(false);


            //successfull?
            if (!response[0].StartsWith("222"))
            {
                Trace.WriteLine("something went wrong \n" + response[0]);
                tbStatus.Text = response[0];
                return null;
            }



            //get the head (not the whole head but article information)
            string head = response[0];
            
            //split first line to save information
                        //220 n<a> article retrieved - head and body follow
                        //n = article number,
                        //<a> = message-id.
            string[] subStrings = head.Split(' ');
            string[] subStrings2 = subStrings[1].Split('<');

            //define values from article head
            string articleNumber = subStrings2[0];
            string messageID = subStrings2[1].Replace(">", "");
            


            //remove first line to seperate body from head 
            response.RemoveAt(0);

            //merge all lines into one string
            string body = "";

            foreach(string line in response)
            {
                body = $"{body}\n{line}";
            }

            
            //create article object 
            Article a = new Article(ArticleNumber, messageID, head, body);


            return a;
        }


        //check if server returned 200 successfull
        private Boolean isSuccessfull()
        {


            string firstResponse = Response();



            if (firstResponse.StartsWith("200"))
            {
                return true;
            }
            else
            {
                MessageBox.Show("Something went wrong: \n\n" + firstResponse);

                return false;
            }
        }


        //remove the last chars from a newsgroup to just display the name
        private string RemoveLastCars(string input)
        {
            
            int index = input.IndexOf(" ");
            if (index >= 0)
                input = input.Substring(0, index);

            return input;
        }


        #endregion





        public void Disconnect()
        {
            
                stream.Close();
                tcp.Close();
            
        }



    }
}
