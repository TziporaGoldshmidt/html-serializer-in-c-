using html_serializer;
using System.Text.RegularExpressions;

var html = await Load("https://hebrewbooks.org/beis");
//replace all kinds of spaces with empty string
 var cleanHtml = Regex.Replace(html, @"(?<=<[^>]*?>)\s+|\s+(?=<)|[\r\n]+", "");

//. - all kinds of notes, * - more then one time, ? - no greedy
//split the html to lines without empty lines
var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 0);

HtmlElement root = new HtmlElement("root"), currentElement = root;
//build html tree
foreach (var line in htmlLines)
{
    var parts = line.Split(new[] { ' ' }, 2);
    var tagName = parts[0];
    // Check for end of HTML
    if (tagName == "html/")
    {
        break;
    }

    // Check for closing tag
    if (tagName.StartsWith("/"))
    {
        // Move up in the tree
        currentElement = currentElement.Parent; 
        continue;
    }
    //check if it new tag
    if (HtmlHelper.Instance.htmlTags.Contains(tagName))
    {
        // Create new HtmlElement
        HtmlElement newElement = new HtmlElement(tagName);
        newElement.Parent = currentElement;
        currentElement.Children.Add(newElement);
        string attributesString = "";
        // Process attributes
        if (parts.Length > 1)
        {
            attributesString = parts[1];
            var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(attributesString);

            foreach (Match attribute in attributes)
            {
                var attrName = attribute.Groups[1].Value;
                var attrValue = attribute.Groups[2].Value;

                if (attrName == "class")
                {
                    newElement.Classes = attrValue.Split(' ').ToList();
                }
                else if (attrName == "id")
                {
                    newElement.Id = attrValue;
                }
                else
                    newElement.Attributes.Add(attrName, attrValue);
            }
        }

        // Check for self-closing tags
        if (!(attributesString.EndsWith("/") || HtmlHelper.Instance.htmlVoidTags.Contains(tagName)))
        {
            // Move down in the tree
            currentElement = newElement;
        }
    }
    //if it not a tag - its a value
    else
    {
        currentElement.InnerHtml += line;
    } 
}

string queryString = "div.aspNetHidden input";
Selector selector = Selector.convertToSelector(queryString);
var resultList = new HashSet<HtmlElement>();
HtmlElement.htmlElementsBySelector(root, selector, resultList);
resultList.ToList().ForEach(result => { Console.WriteLine("id: "+result.Id+" name: "+result.Name+" content: "+result.InnerHtml); });
     


Console.ReadLine();


async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}