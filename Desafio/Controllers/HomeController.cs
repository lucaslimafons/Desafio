using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using Desafio.Models;
using Desafio.Base;
using System.Globalization;

namespace Desafio.Controllers
{
    public class HomeController : BaseController
    {
        public List<Response> tentativas = new List<Response>();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Lista()
        {
            return View(this.Data);
        }

        public ActionResult Find()
        {
            string suspects = "";
            this.Data.suspects.ForEach(c => { suspects = string.Concat((string.IsNullOrEmpty(suspects) ? suspects : suspects + ", "), RemoveAcentos(c)); });
            ViewBag.Suspects = suspects;

            string locals = "";
            this.Data.locals.ForEach(c => { locals = string.Concat((string.IsNullOrEmpty(locals) ? locals : locals + ", "), RemoveAcentos(c)); });
            ViewBag.Locals = locals;

            string guns = "";
            this.Data.guns.ForEach(c => { guns = string.Concat((string.IsNullOrEmpty(guns) ? guns : guns + ", "), RemoveAcentos(c)); });
            ViewBag.Guns = guns;

            return View();
        }

        public ActionResult Play()
        {
            return View();
        }

        #region 'Util'
        public string MontaGridResponses()
        {
            Response response = this.FindKiller(0, 0, 0);
            if (this.tentativas != null && this.tentativas.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<table class=\"table table-striped table-bordered\">");
                sb.AppendLine("<thead>");
                sb.AppendLine("<tr>");
                sb.AppendLine("<th class=\"palette palette-wet-asphalt\"> Suspeito </th>");
                sb.AppendLine("<th class=\"palette palette-wet-asphalt\"> Local </th>");
                sb.AppendLine("<th class=\"palette palette-wet-asphalt\"> Arma </th>");
                sb.AppendLine("</tr>");
                sb.AppendLine("</thead>");
                sb.AppendLine("<tbody>");
                foreach (Response resp in this.tentativas)
                {
                    if (resp.suspect.Equals(response.suspect) && resp.local.Equals(resp.local) && resp.gun.Equals(response.gun))
                    {
                        sb.AppendLine("<tr class=\"text-success\">");
                        sb.AppendLine("<td>");
                        sb.AppendLine(resp.suspect);
                        sb.AppendLine("</td>");
                        sb.AppendLine("<td>");
                        sb.AppendLine(resp.local);
                        sb.AppendLine("</td>");
                        sb.AppendLine("<td>");
                        sb.AppendLine(resp.gun);
                        sb.AppendLine("</td>");
                        sb.AppendLine("</tr>");
                    }
                    else
                    {
                        sb.AppendLine("<tr>");
                        sb.AppendLine("<td>");
                        sb.AppendLine(resp.suspect);
                        sb.AppendLine("</td>");
                        sb.AppendLine("<td>");
                        sb.AppendLine(resp.local);
                        sb.AppendLine("</td>");
                        sb.AppendLine("<td>");
                        sb.AppendLine(resp.gun);
                        sb.AppendLine("</td>");
                        sb.AppendLine("</tr>");
                    }
                }
                sb.AppendLine("<tr>");
                sb.AppendLine("<td></td>");
                sb.AppendLine("<td class=\"bold\">");
                sb.AppendLine("Total de Tentativas");
                sb.AppendLine("</td>");
                sb.AppendLine("<td class=\"bold\">");
                sb.AppendLine(this.tentativas.Count.ToString());
                sb.AppendLine("</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("</tbody>");
                sb.AppendLine("</table>");
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }            
        }

        public Response FindKiller(int suspect, int local, int gun)
        {
            if (suspect > this.Data.suspects.Count - 1)
                suspect = this.Data.suspects.Count - 1;

            if (local > this.Data.locals.Count - 1)
                local = this.Data.locals.Count - 1;

            if(gun > this.Data.guns.Count - 1)
                gun = this.Data.guns.Count - 1;

            Response response = new Response
            {
                suspect = this.Data.suspects[suspect],
                local = this.Data.locals[local],
                gun = this.Data.guns[gun]
            };

            int ret = WitnessResponse(response);

            tentativas.Add(response);

            if (ret.Equals(1))
                return FindKiller(++suspect, local, gun);
            else if (ret.Equals(2))
                return FindKiller(suspect, ++local, gun);
            else if (ret.Equals(3))
                return FindKiller(suspect, local, ++gun);
            else
                return this.Responses.responses.Where(c => c.suspect.Equals(this.Data.suspects[suspect])
                    && c.local.Equals(this.Data.locals[local])
                    && c.gun.Equals(this.Data.guns[gun])).FirstOrDefault();
        }

        public int WitnessResponse(Response response)
        {
            int ret = 0;
            this.Responses.responses.ForEach(resp => {
                if (!response.suspect.Equals(resp.suspect))
                    ret = 1;
                else if (!response.local.Equals(resp.local))
                    ret = 2;
                else if (!response.gun.Equals(resp.gun))
                    ret = 3;
                else
                    ret = 0;
            });
            return ret;
        }

        public JsonResult FindSuspect(string nome)
        {
            var suspects = this.Data.suspects.Where(c => c.Trim().ToUpper().Contains(nome.Trim().ToUpper())).Select(c => new { value = c, item = c });
            return Json(suspects, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FindLocal(string nome)
        {
            var locals = this.Data.locals.Where(c => RemoveAcentos(c).Trim().ToUpper().Contains(RemoveAcentos(nome).Trim().ToUpper())).Select(c => new { value = c, item = c });
            return Json(locals, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FindGun(string nome)
        {
            var guns = this.Data.guns.Where(c => RemoveAcentos(c).Trim().ToUpper().Contains(RemoveAcentos(nome).Trim().ToUpper())).Select(c => new { value = c, item = c });
            return Json(guns, JsonRequestBehavior.AllowGet);
        }

        public JsonResult VerificaTentativa(string suspect, string local, string gun)
        {
            int ret = 0;
            this.Responses.responses.ForEach(resp =>
            {
                if (!suspect.Equals(resp.suspect))
                    ret = 1;
                else if (!local.Equals(resp.local))
                    ret = 2;
                else if (!gun.Equals(resp.gun))
                    ret = 3;
                else
                    ret = 0;
            });
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        private string RemoveAcentos(string text)
        {
            try
            {
                text = text.Normalize(NormalizationForm.FormD);
                StringBuilder sb = new StringBuilder();

                foreach (char c in text.ToCharArray())
                {
                    if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                        sb.Append(c);
                }
                return sb.ToString();
            }
            catch { return ""; }
        }
        #endregion
    }
}