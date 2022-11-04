using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using NewsGroupReader.ViewModel;

namespace NewsGroupReader.Model
{
    public class Article
    {

        public int ArticleNumber { get; set; }

        public string Title { get; set; }

        string MessageID { get; set; }

        public string Head { get; set; }

        public string Body { get; set; }



        public Article(int ArticleNumber, string MessageID, string Head, string Body)
        {
            this.ArticleNumber = ArticleNumber;
            this.MessageID = MessageID;
            this.Title = MessageID; //because articles dont have titles but we need something to show in the list
            this.Head = Head;
            this.Body = Body;
        }



                //220 n<a> article retrieved - head and body follow
                //n = article number,
                //<a> = message-id.
                //412 no newsgroup has been selected
                //420 no current article has been selected
                //423 no such article number in this group
                //430 no such article found


    }
}
