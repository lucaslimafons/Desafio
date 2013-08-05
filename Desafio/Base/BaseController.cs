using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Desafio.Models;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace Desafio.Base
{
    public class BaseController : Controller
    {
        public Data Data { get; set; }

        public Responses Responses { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
            if (Data == null || Data.suspects.Count.Equals(0))
            {
                StreamReader sr;
                try
                {
                    sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/data"), Encoding.UTF8);
                    string filedata = sr.ReadToEnd();

                    this.Data = JsonDeserialize<Data>(filedata);
                    sr.Close();

                    sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/response"), Encoding.UTF8);
                    string fileresponse = sr.ReadToEnd();
                    this.Responses = JsonDeserialize<Responses>(fileresponse);
                    sr.Close();
                }
                catch (IOException ex)
                {
                    Data = new Data { guns = new List<string>(), suspects = new List<string>(), locals = new List<string>() };
                    Responses = new Responses();
                }
            }
        }
        public T JsonDeserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }
    }
}