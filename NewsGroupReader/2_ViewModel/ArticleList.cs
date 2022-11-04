using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using NewsGroupReader.Model;

namespace NewsGroupReader.ViewModel
{
    public class ArticleList
    {
        public List<Article> Items { get; set; }

        public ArticleList (List<Article> items)
        {
            Items = items;
        }

        /// <summary>
        /// takes a selector from the ui and adds all list items to it 
        /// </summary>
        /// <param name="uiElement">
        /// input for the Selector
        /// </param>
        /// <returns>
        /// returns the same Selector with items 
        /// </returns>
        public Selector GenerateUiList(Selector uiElement)
        {

            foreach (var s in Items)
            {
                uiElement.Items.Add(s.Title);
            }

            return uiElement;
        }
    }
}
