using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace html_serializer
{
    internal class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }

        public HtmlElement(string name) : this()
        {
            Name = name;
        }
        public HtmlElement()
        {
            Classes = new List<string>();
            Attributes = new Dictionary<string, string>();
            Children = new List<HtmlElement>();
        }

        //find all children of htmlHelement
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                HtmlElement current = queue.Dequeue();
                foreach (HtmlElement child in current.Children)
                {
                    queue.Enqueue(child);
                }
                yield return current;
            }
        }

        //finding all parents of htmlHelement
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement element = this;
            while (element != null)
            {
                yield return element;
                element = element.Parent;
            }
        }

        //fiding all htmlHelements that fit to a giving selector
        public static void htmlElementsBySelector(HtmlElement element, Selector selector, HashSet<HtmlElement> result)
        {
            //find all htmlHelementChildren
            IEnumerable<HtmlElement> children = element.Descendants();
            //find just children that fit the current selector
            children = children.Where(child =>child != null 
            && (string.IsNullOrEmpty(selector.Id) || child.Id.Equals(selector.Id))
            && (string.IsNullOrEmpty(selector.TagName) || child.Name.Equals(selector.TagName))
            && selector.Classes.All(item => child.Classes.Contains(item)));
            //if it the last selector add children to result list
            if (selector.Child == null)
            {
                result.UnionWith(children);
                return;
            }
            //continue checking the next selector
            foreach (HtmlElement child in children)
            {
                htmlElementsBySelector(child, selector.Child, result);
            }
        }
    }
}
