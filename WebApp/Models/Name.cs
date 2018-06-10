using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class Name
    {
        
        //type of log
        private string fullName;
        public string FullName { get { return fullName; } set { fullName = value; } }
        //message of log
        private int id;
        public int Id { get { return id; } set { id = value; } }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public Name(string fullName, int id)
        {
            this.fullName = fullName;
            this.id = id;
        }
    
    }
}