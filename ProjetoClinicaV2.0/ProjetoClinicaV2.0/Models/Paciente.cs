﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetoClinicaV2._0.Models
{
    public class Paciente
    {
        [Key]
        public int IDPaciente { get; set; }
        [Display (Name = "Nome")]
        public string Nome { get; set; }
        [Display(Name = "Telefone")]
        public long Telefone { get; set; }
        [Display(Name = "Endereço")]
        public string Endereco { get; set; }
        [Display(Name = "Data de nascimento")]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }
        public List<Consulta> Consultas { get; set; }
    }
}