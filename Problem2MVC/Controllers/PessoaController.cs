using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DC;
using Problem2MVC.Models;
using System.IO;
using System.Globalization;

namespace Problem2MVC.Controllers
{
    public class PessoaController : Controller
    {
        private Problem2MVCContext db = new Problem2MVCContext();

        //
        // GET: /Pessoa/

        public ActionResult Index()
        {
            TempData["Mensagem"] = null;
            if (LerPasta())
            {
                if (TempData["Mensagem"] != null)
                    ViewBag.Message = TempData["Mensagem"].ToString();
                return View(db.Pessoas.ToList());
            } 
            return RedirectToAction("Index", "Home", ViewBag);
        }

        #region Controlador de leitura do arquivo
        private bool LerPasta()
        {
            string[] lista = System.IO.Directory.GetFiles(Request.PhysicalApplicationPath + "\\App_Data\\UploadedFiles\\");
            if (lista.Length == 0)
                return true;
            string erroMessage = string.Empty;
            int countSucess = 0;
            int countErro = 0;
            for (int i = 0; i < lista.Length; i++)
            {
                erroMessage += LerArquivo(lista[i], ref countSucess, ref countErro);
            }
            TempData["Mensagem"] = string.Format("{0} Dados inserido(s). {1} Dados com erro.\n", countSucess, countErro);
            if (erroMessage != string.Empty)
            {
                TempData["Mensagem"] += erroMessage;
                return false;
            }
            return true;
        }

        private string LerArquivo(string arquivoNome, ref int countSucess, ref int countErro)
        {
            string erroMessage = string.Empty;
            StreamReader rd = new StreamReader(arquivoNome);
            int linhaIndex = 0;

            try
            {
                while (!rd.EndOfStream)
                {
                    string linha = rd.ReadLine();
                    linhaIndex++;
                    SalvarPessoa(linha, ref countErro, ref countSucess, ref erroMessage, linhaIndex);
                }
            }
            catch
            {
                erroMessage += string.Format("- Erro ao ler o arquivo \"{0}\"", arquivoNome.Replace(Request.PhysicalApplicationPath + "\\App_Data\\UploadedFiles\\", ""));
            }

            rd.Close();
            rd.Dispose();

            System.IO.File.Delete(arquivoNome);

            return erroMessage;
        }

        private void SalvarPessoa(string linha, ref int countErro, ref int countSucess, ref string erroMessage, int linhaIndex)
        {
            Pessoa pessoa = new Pessoa();
            bool sucesso = true;
            try
            {
                pessoa.Nome = linha.Substring(0, 20);
                pessoa.Email = linha.Substring(20, 20);
                if (!System.Text.RegularExpressions.Regex.IsMatch(pessoa.Email, ".+\\@.+\\..+"))
                {
                    erroMessage += string.Format("- Email inválido. ({0}) [Linha: {1}]\n", linha.Substring(20, 20), linhaIndex);
                    throw new Exception("Email inválido.");
                }
                try
                {
                    pessoa.DataDeNascimento = DateTime.ParseExact(linha.Substring(40, 8), "yyyyMMdd", CultureInfo.InvariantCulture);
                    if (pessoa.DataDeNascimento.CompareTo(DateTime.Today) == 1)
                    {
                        erroMessage += string.Format("- Data de nascimento errada. ({0}) [Linha: {1}]\n", linha.Substring(40, 8), linhaIndex);
                        throw new Exception("Data de nascimento errada.");
                    }
                }
                catch
                {
                    erroMessage += string.Format("- Data inválida. ({0}) [Linha: {1}]\n", linha.Substring(40, 8), linhaIndex);
                    throw new Exception("Data inválida.");
                }
                pessoa.Celular = linha.Substring(48, 15);
                pessoa.TelefoneResidencial = linha.Substring(63);
            }
            catch
            {
                countErro++;
                sucesso = false;
            }
            if (sucesso)
            {
                db.Pessoas.Add(pessoa);
                db.SaveChanges();
                countSucess++;
            }
        }
        #endregion

        //
        // GET: /Pessoa/Details/5

        public ActionResult Details(int id = 0)
        {
            Pessoa pessoa = db.Pessoas.Find(id);
            if (pessoa == null)
            {
                return HttpNotFound();
            }
            return View(pessoa);
        }

        //
        // GET: /Pessoa/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Pessoa/Create

        [HttpPost]
        public ActionResult Create(Pessoa pessoa)
        {
            if (ModelState.IsValid)
            {
                db.Pessoas.Add(pessoa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pessoa);
        }

        //
        // GET: /Pessoa/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Pessoa pessoa = db.Pessoas.Find(id);
            if (pessoa == null)
            {
                return HttpNotFound();
            }
            return View(pessoa);
        }

        //
        // POST: /Pessoa/Edit/5

        [HttpPost]
        public ActionResult Edit(Pessoa pessoa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pessoa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pessoa);
        }

        //
        // GET: /Pessoa/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Pessoa pessoa = db.Pessoas.Find(id);
            if (pessoa == null)
            {
                return HttpNotFound();
            }
            return View(pessoa);
        }

        //
        // POST: /Pessoa/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Pessoa pessoa = db.Pessoas.Find(id);
            db.Pessoas.Remove(pessoa);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}