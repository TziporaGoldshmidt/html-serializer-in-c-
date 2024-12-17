using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;


namespace html_serializer
{
    internal class HtmlHelper
    {
        //make htmlHelper singletone
        private readonly static HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;

       public List<string> htmlTags = new List<string>();
       public List<string> htmlVoidTags = new List<string>();

        private HtmlHelper() 
        {
            string jsonContent;
            jsonContent= File.ReadAllText("files/htmlTags.json");
            htmlTags = JsonSerializer.Deserialize<List<string>>(jsonContent);
            jsonContent = File.ReadAllText("files/htmlvoidTags.json");
            htmlVoidTags = JsonSerializer.Deserialize<List<string>>(jsonContent);

        }
    }
}
