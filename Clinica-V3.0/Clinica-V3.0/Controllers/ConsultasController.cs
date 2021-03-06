﻿using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Clinica_V3._0.Models;
using System.Data.Entity.Core.Objects;
using Microsoft.AspNet.Identity;

namespace Clinica_V3._0.Controllers
{

    public class ConsultasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [CustomAuthorize(Roles = "Medico,Administrador,Secretaria")]
        // GET: Consultas
        public ActionResult Index(string stringPaciente, string stringMedico, string stringPlanoSaude)
        {
            var consultas = db.Consultas.Where(x => x.Comparecimento == false).Include(c => c.Paciente);
            var user = db.Users.Find(User.Identity.GetUserId());
            if (User.IsInRole("Medico"))
            {
                consultas = consultas.Where(s => s.Medico.Nome.Contains(user.Medico.Nome));
            }

                if (!String.IsNullOrEmpty(stringPaciente))
            {
                consultas = consultas.Where(s => s.Paciente.Nome.Contains(stringPaciente));
            }

            if (!String.IsNullOrEmpty(stringMedico))
            {
                consultas = consultas.Where(s => s.Medico.Nome.Contains(stringMedico));
            }

            if (!String.IsNullOrEmpty(stringPlanoSaude))
            {
                consultas = consultas.Where(s => s.PlanoDeSaude.Contains(stringPlanoSaude));
            }

            return View(consultas.ToList());
        }
        [CustomAuthorize(Roles = "Medico,Administrador,Secretaria")]
        // GET: Consultas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consulta consulta = db.Consultas.Find(id);
            if (consulta == null)
            {
                return HttpNotFound();
            }
            return View(consulta);
        }
        [CustomAuthorize(Roles = "Administrador,Secretaria")]
        // GET: Consultas/Create
        public ActionResult Create()
        {
            ViewBag.IDPaciente = new SelectList(db.Paciente, "IDPaciente", "Nome");
            ViewBag.IDMedico = new SelectList(db.Medico, "UserId", "Nome");
            return View();
        }

        // POST: Consultas/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [CustomAuthorize(Roles = "Administrador,Secretaria")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDConsulta,PlanoDeSaude,DataConsulta,IDPaciente,IDMedico")] Consulta consulta, string hora)
        {

            if (consulta.DataConsulta.Date < DateTime.Now.Date) {
                ModelState.AddModelError("DataConsulta", "A data da consulta deve ser maior ou igual a Hoje.");
            }

            if (hora == "")
            {
                ModelState.AddModelError("hora", "Insira uma hora válida.");
            }
            else
            {
                string[] array = hora.Split(':');
                DateTime novaData = new DateTime(consulta.DataConsulta.Year, consulta.DataConsulta.Month, consulta.DataConsulta.Day, int.Parse(array[0]), int.Parse(array[1]), 00);
                consulta.DataConsulta = novaData;
                var nomeMedico = db.Medico.Find(consulta.IDMedico).Nome;
                var consultaNoMesmoHorario = this.consultaMesmoHorario(novaData, nomeMedico);
                if (consultaNoMesmoHorario)
                {
                    ModelState.AddModelError("DataConsulta", "Já existe consulta neste mesmo Horário.");
                }
            }

            if (ModelState.IsValid)
            {
                db.Consultas.Add(consulta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDPaciente = new SelectList(db.Paciente, "IDPaciente", "Nome", consulta.IDPaciente);
            ViewBag.IDMedico = new SelectList(db.Medico, "UserId", "Nome", consulta.IDMedico);
            return View(consulta);
        }


        private bool consultaMesmoHorario(DateTime novaData, string nomeMedico)
        {
            foreach (Consulta cons in db.Consultas)
            {
                if (novaData >= cons.DataConsulta &&
                    novaData <= cons.DataConsulta.AddMinutes(30)
                    && cons.Medico.Nome.Equals(nomeMedico))
                {
                    return true;
                }
            }
            
            return false;

        }
        [CustomAuthorize(Roles = "Medico,Administrador,Secretaria")]
        // GET: Consultas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consulta consulta = db.Consultas.Find(id);
            if (consulta == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDPaciente = new SelectList(db.Paciente, "IDPaciente", "Nome", consulta.IDPaciente);
            ViewBag.IDMedico = new SelectList(db.Medico, "UserId", "Nome", consulta.IDMedico);
            return View(consulta);
        }
        [CustomAuthorize(Roles = "Medico,Administrador,Secretaria")]
        // POST: Consultas/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDConsulta,PlanoDeSaude,DataConsulta,IDPaciente,IDMedico,Comparecimento,Observacoes")] Consulta consulta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(consulta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDPaciente = new SelectList(db.Paciente, "IDPaciente", "Nome", consulta.IDPaciente);
            ViewBag.IDMedico = new SelectList(db.Medico, "UserId", "Nome", consulta.IDMedico);
            return View(consulta);
        }

        // GET: Consultas/Delete/5
        [CustomAuthorize(Roles = "Administrador,Secretaria")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consulta consulta = db.Consultas.Find(id);
            if (consulta == null)
            {
                return HttpNotFound();
            }
            return View(consulta);
        }
        [CustomAuthorize(Roles = "Administrador,Secretaria")]
        // POST: Consultas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Consulta consulta = db.Consultas.Find(id);
            db.Consultas.Remove(consulta);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult PacienteFilter(string term)
        {
            term = term.ToLower();
            var list = db.Consultas.Where(x => x.Paciente.Nome.ToLower().Contains(term)).Select(x => x.Paciente.Nome).Distinct();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MedicoFilter(string term)
        {
            term = term.ToLower();
            var list = db.Consultas.Where(x => x.Medico.Nome.ToLower().Contains(term)).Select(x => x.Medico.Nome).Distinct();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PlanoSaudeFilter(string term)
        {
            term = term.ToLower();
            var list = db.Consultas.Where(x => x.PlanoDeSaude.ToLower().Contains(term)).Select(x => x.PlanoDeSaude).Distinct();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
