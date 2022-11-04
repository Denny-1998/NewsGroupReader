using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NewsGroupReader.Model;
using NewsGroupReader.ViewModel;

namespace NewsGroupReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private NewsGroupList ngl;
        private ArticleList al;
        private Connection c;




        public MainWindow()
        {
            InitializeComponent();

        }




        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            string address = "news.dotsrc.org";
            int port = 119;
            string username = "testingstuffeasv@gmail.com";
            string password = "af6ba1";

            AuthorisationData auth = new AuthorisationData(address, port, username, password);


            c = new Connection(auth,ref tbStatus);

            bool isConnected = await c.Connect();

            if (isConnected)
            { 
                //send username and password
                await c.authorizeAsync();

                tbStatus.Text = "getting news group data...";
                //create a list for all newsgroups on the server
                ngl = new NewsGroupList(c.GetNewsGroups());

                ngl.GenerateUiList(CbChooseGroup);


                tbStatus.Text = "ok";

                
            }
        }


        private void cbChooseGroup_SelectionChanged (object sender, RoutedEventArgs e)
        {
            string selectedGroup = CbChooseGroup.SelectedItem.ToString();

            Trace.WriteLine(selectedGroup);

            List<Article> articles = c.GetArticles(selectedGroup);
            al = new ArticleList(articles);


            al.GenerateUiList(LvArticleList);

            tbStatus.Text = "ok";
        }
    }
}
