﻿using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace html_serializer
{
    internal class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Parent { get; set; }
        public Selector Child { get; set; }    

        public Selector()
        {
            Classes = new List<string>();
        }

        public static Selector convertToSelector(string query)
        {
            //split to selector levels
            var queryList = query.Split(' ');
            Selector root = new Selector(), current = root;
            foreach (var oneQuery in queryList)
            {
                // split the part by "." & "#"
                List<string> parts = new Regex(@"(?<=#|\.)(?=\S)|(?=\S)(?=#|\.)").Split(oneQuery).ToList();
                // remove all empty strings
                parts.RemoveAll(string.IsNullOrEmpty);
                // conect the "." and "#" with their string
                for (int i = 0; i < parts.Count; i++)
                {
                    if (i > 0 && (parts[i - 1] == "#" || parts[i - 1] == "."))
                    {
                        parts[i - 1] += parts[i];
                        parts.RemoveAt(i);
                        i--;
                    }
                }
                foreach (string part in parts)
                {
                    //update the selector according to the string
                    if (part.StartsWith("#"))
                    {
                        current.Id = part.Substring(part.IndexOf('#')+1);
                    }
                    else if (part.StartsWith("."))
                    {
                        current.Classes.Add(part.Substring(1));
                    }
                    else if (HtmlHelper.Instance.htmlTags.Contains(part))
                    {
                        current.TagName = part;
                    }
                }
                //create the next selector
                Selector newSelector = new Selector();
                newSelector.Parent = current;
                current.Child = newSelector;
                current = newSelector;              
            }
            //delete the last empty selector
            current.Parent.Child = null;
            return root;
        }
    }
}
